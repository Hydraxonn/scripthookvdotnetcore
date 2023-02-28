﻿using GTA;
using GTA.UI;
using McMaster.NETCore.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace SHVDN
{
    unsafe struct RuntimeConfig
    {
        public IntPtr TickPtr;
        fixed ulong Reserved[31];
    }

    /// <summary>
    /// The CoreCLR plugin loader implementations
    /// </summary>
    public static unsafe partial class Core
    {
        #region Main assembly only

        static readonly Dictionary<string, Loader> _loaders = new(StringComparer.OrdinalIgnoreCase);

        static readonly ConcurrentQueue<Action> _taskQueue = new();

        // Function that gets called from main thread by the c++ native host during startup
        static int CLR_EntryPoint(IntPtr pConfig, int cbArg)
        {
            var config = (RuntimeConfig*)pConfig;
            Debug.Assert(cbArg == sizeof(RuntimeConfig));
            // Set tick handler
            *(delegate* unmanaged<void>*)(&config->TickPtr) = &CLR_DoTick;

            // Register keyboard handler
            IntPtr kbHandler;
            *(delegate* unmanaged<DWORD, ushort, BYTE, BOOL, BOOL, BOOL, BOOL, void>*)(&kbHandler) = &KeyboardHandler;
            KeyboardHandlerRegister(kbHandler);


            // Set up main TLS and ThreadId
            OnInit(default);

            // Load base script
            RegisterScript(new BaseScript());

            return 0;
        }

        [UnmanagedCallersOnly]
        static void KeyboardHandler(DWORD key, ushort repeats, BYTE scanCode, BOOL isExtended, BOOL isWithAlt, BOOL wasDownBefore, BOOL isUpNow)
        {
            try
            {
                var control = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;
                var shift = (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0;
                var alt = isWithAlt != 0;
                var down = isUpNow == 0;
                DoKeyEvent(key, down, control, shift, alt);
                foreach (var l in _loaders)
                {
                    l.Value.DoKeyEvent(key, down, control, shift, alt);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Keyboard event error: " + ex.ToString());
            }
        }

        [UnmanagedCallersOnly]
        static void CLR_DoTick()
        {
            while (_taskQueue.TryDequeue(out var task))
            {
                try
                {
                    task();
                }
                catch (Exception ex)
                {
                    Logger.Error("Error executing queued task: " + ex.ToString());
                }
            }

            DoTick(default);
            lock (_loaders)
            {
                foreach (var loader in _loaders)
                {
                    loader.Value.DoTick(default);
                }
            }
        }
        internal static void CLR_Load(string dir)
        {
            lock (_loaders)
            {
                if (_loaders.ContainsKey(dir))
                    throw new ArgumentException("Script directory has already been loaded", nameof(dir));

                dir = Path.GetFullPath(dir);
                Logger.Debug($"Loading scripts from {dir}");
                var loader = new Loader(dir);
                _loaders.Add(dir, loader);
                loader.DoInit(default);
            }
        }
        internal static void CLR_Unload(string dir)
        {
            lock (_loaders)
            {
                if (_loaders.TryGetValue(dir, out var loader))
                {
                    Logger.Info($"Unloading scripts in {dir}");
                    loader.Dispose();
                    _loaders.Remove(dir);
                    Logger.Info($"Unloaded scripts in {dir}");
                }
            }
        }
        internal static void CLR_UnloadAll()
        {
            foreach (var dir in _loaders.Keys)
            {
                try
                {
                    CLR_Unload(dir);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to unload scripts in directory {dir}");
                    Logger.Error(ex.ToString());
                }
            }

        }
        internal static void CLR_ReloadAll()
        {
            static bool canLoadFromThisDir(string dir)
            {
                dir = Path.GetFullPath(dir);
                return !_loaders.ContainsKey(dir) && Directory.GetFiles(dir, "*.dll").Any(IsManagedAssembly);
            }

            CLR_UnloadAll();

            List<string> toLoad = new();
            var scriptsRoot = Path.GetFullPath("CoreScripts");
            if (canLoadFromThisDir(scriptsRoot))
                toLoad.Add(scriptsRoot);

            foreach (var dir in Directory.GetDirectories(scriptsRoot))
            {
                if (canLoadFromThisDir(dir))
                    toLoad.Add(dir);
            }

            foreach (var l in toLoad)
            {
                try
                {
                    CLR_Load(l);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load scripts from directory: {l}");
                    Logger.Error(ex.ToString());
                }
            }
            Debug.Assert(CurrentDirectory == null);
        }

        /// <summary>
        /// Helper method to determine whether a file is a managed assembly
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsManagedAssembly(string fileName)
        {
            try
            {

                using Stream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using BinaryReader binaryReader = new(fileStream);
                if (fileStream.Length < 64)
                {
                    return false;
                }

                //PE Header starts @ 0x3C (60). Its a 4 byte header.
                fileStream.Position = 0x3C;
                uint peHeaderPointer = binaryReader.ReadUInt32();
                if (peHeaderPointer == 0)
                {
                    peHeaderPointer = 0x80;
                }

                // Ensure there is at least enough room for the following structures:
                //     24 byte PE Signature & Header
                //     28 byte Standard Fields         (24 bytes for PE32+)
                //     68 byte NT Fields               (88 bytes for PE32+)
                // >= 128 byte Data Dictionary Table
                if (peHeaderPointer > fileStream.Length - 256)
                {
                    return false;
                }

                // Check the PE signature.  Should equal 'PE\0\0'.
                fileStream.Position = peHeaderPointer;
                uint peHeaderSignature = binaryReader.ReadUInt32();
                if (peHeaderSignature != 0x00004550)
                {
                    return false;
                }

                // skip over the PEHeader fields
                fileStream.Position += 20;

                const ushort PE32 = 0x10b;
                const ushort PE32Plus = 0x20b;

                // Read PE magic number from Standard Fields to determine format.
                var peFormat = binaryReader.ReadUInt16();
                if (peFormat != PE32 && peFormat != PE32Plus)
                {
                    return false;
                }

                // Read the 15th Data Dictionary RVA field which contains the CLI header RVA.
                // When this is non-zero then the file contains CLI data otherwise not.
                ushort dataDictionaryStart = (ushort)(peHeaderPointer + (peFormat == PE32 ? 232 : 248));
                fileStream.Position = dataDictionaryStart;

                uint cliHeaderRva = binaryReader.ReadUInt32();
                if (cliHeaderRva == 0)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        class Loader : PluginLoader
        {
            public delegate void KeyEventDelegate(DWORD key, bool down, bool ctrl, bool shift, bool alt);
            public Assembly ApiAssembly { get; }
            public Action<IntPtr> DoInit { get; }
            public Action<IntPtr> DoUnload { get; }
            public Action<IntPtr> DoTick { get; }
            public KeyEventDelegate DoKeyEvent { get; }
            public Loader(string folder) : base(GetConfig(folder))
            {
                var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

                ApiAssembly = LoadDefaultAssembly();
                var coreType = ApiAssembly.GetType(typeof(Core).FullName);

                var initMethod = getMethod(nameof(Core.OnInit));
                Debug.Assert(initMethod != null);

                var unloadMethod = getMethod(nameof(Core.OnUnload));
                Debug.Assert(unloadMethod != null);

                var tickMethod = getMethod(nameof(Core.DoTick));
                Debug.Assert(tickMethod != null);

                var keyEventMethod = getMethod(nameof(Core.DoKeyEvent));
                Debug.Assert(keyEventMethod != null);

                // Caching with delegates
                DoInit = (Action<IntPtr>)Delegate.CreateDelegate(typeof(Action<IntPtr>), initMethod);
                DoUnload = (Action<IntPtr>)Delegate.CreateDelegate(typeof(Action<IntPtr>), unloadMethod);
                DoTick = (Action<IntPtr>)Delegate.CreateDelegate(typeof(Action<IntPtr>), tickMethod);
                DoKeyEvent = (KeyEventDelegate)Delegate.CreateDelegate(typeof(KeyEventDelegate), keyEventMethod);

                List<Assembly> scriptAssemblies = new();
                foreach (var asmPath in Directory.GetFiles(folder, "*.dll").Where(IsManagedAssembly))
                {
                    // Skip loading of api assembly
                    if (Path.GetFileNameWithoutExtension(asmPath) == typeof(Core).Assembly.GetName().Name)
                        continue;

                    try
                    {
                        Logger.Debug("Loading assembly: " + asmPath);
                        scriptAssemblies.Add(LoadAssemblyFromPath(asmPath));
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                }

                setProp(nameof(CurrentDirectory), folder);
                setProp(nameof(ScriptAssemblies), scriptAssemblies.ToArray());
                setProp(nameof(MainAssembly), typeof(Core).Assembly);
                void setProp(string name, object value)
                {
                    var prop = coreType.GetProperty(name, flags);
                    Debug.Assert(prop != null, $"Property {name} not found");
                    prop.SetValue(null, value);
                }
                MethodInfo getMethod(string name)
                    => coreType.GetMethod(name, flags);
            }

            protected override void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    DoUnload(default);
                    _disposed = true;
                }

                // Call base class implementation.
                base.Dispose(disposing);
            }
            static PluginConfig GetConfig(string folderPath)
            {
                var files = Directory.GetFiles(folderPath, "*.dll");
                if (!files.Any(IsManagedAssembly))
                    throw new FileNotFoundException("No managed assemblies were found in this folder");

                var mainAssemblyPath = Path.Combine(folderPath, typeof(Core).Assembly.GetName().Name + ".dll"); // ScriptHookVDotNetCore.dll
                mainAssemblyPath = Path.GetFullPath(mainAssemblyPath);

                bool alwaysCopy = false;
#if DEBUG
                alwaysCopy = true;
#endif
                // Copy the API assembly if there's none
                if (!File.Exists(mainAssemblyPath) || alwaysCopy)
                    File.Copy(typeof(Core).Assembly.Location, mainAssemblyPath, true);

                // Don't use shared type to allow different version of API assembly to be loaded at the same time
                return new(mainAssemblyPath)
                {
                    EnableHotReload = false,
                    IsUnloadable = true,
                    LoadInMemory = true,
                    PreferSharedTypes = false
                };
            }
        }

        // These methods can be invoke by reflection API too control script load/unload
        private static void RequestLoad(string dir)
        {
            _taskQueue.Enqueue(() => CLR_Load(dir));
        }
        private static void RequestUnload(string dir)
        {
            _taskQueue.Enqueue(() => CLR_Unload(dir));
        }
        private static string[] ListScriptDirectories()
        {
            lock (_loaders)
            {
                return _loaders.Keys.ToArray();
            }
        }

        #endregion

        #region Set up by main assembly

        /// <summary>
        /// All assemblies loaded in to current context
        /// </summary>
        public static Assembly[] ScriptAssemblies { get; private set; }

        /// <summary>
        /// The directory used by this load context
        /// </summary>
        public static string CurrentDirectory { get; private set; }

        /// <summary>
        /// The main assembly that creates and loads the current <see cref="AssemblyLoadContext"/>
        /// </summary>
        public static Assembly MainAssembly { get; private set; }

        #endregion


        static void FindAndRegisterAllScripts()
        {
            Debug.Assert(CurrentDirectory != null);
            Debug.Assert(ScriptAssemblies != null);
            Debug.Assert(MainAssembly != null);

            foreach (var asm in ScriptAssemblies)
            {
                Logger.Debug($"Loading scripts in {asm}");
                try
                {
                    foreach (var scriptType in asm.GetTypes().Where(x => x.IsAssignableTo(typeof(Script)) && !x.IsAbstract))
                    {
                        try
                        {
                            var script = (Script)Activator.CreateInstance(scriptType);
                            RegisterScript(script);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Failed to register script {scriptType}");
                            Logger.Error(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }
        }

        public static class RuntimeController
        {
            static readonly Type MainCoreType;
            static RuntimeController()
            {
                if (MainAssembly == null)
                    throw new InvalidOperationException("Can't use runtime controller in main assembly");

                MainCoreType = MainAssembly.GetType(typeof(Core).FullName);
            }
            static object Invoke(string methodName, params object[] args)
                => MainCoreType.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, args);
            public static void RequestUnload(string dir) => Invoke(nameof(Core.RequestUnload), dir);
            public static void RequestLoad(string dir) => Invoke(nameof(Core.RequestLoad), dir);
            public static string[] ListScriptDirectories() => (string[])Invoke(nameof(Core.ListScriptDirectories));
        }
    }
}