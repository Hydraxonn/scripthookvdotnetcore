﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ScriptHookVDotNetCore.Generator;

namespace ScriptHookVDotNet.Generator
{

    [Generator]
    public class EntryPointGenerator : SymbolPopulator, ISourceGenerator
    {

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {

                Populate(context);

                var scripts = GenerateScriptsRegister();

                if (string.IsNullOrEmpty(scripts)) return;

                var init = EntryPointExists("OnInit") ? "" : InitCode;
                var unload = EntryPointExists("OnOnload") ? "" : UnloadCode;
                var keyboard = EntryPointExists("OnKeyboard") ? "" : KeyboardCode;
                var tick = EntryPointExists("OnTick") ? "" : TickCode;
                string source = $@"// <auto-generated/>
{Globals}
using System.Runtime.InteropServices;
using GTA;

namespace SHVDN;

/// <summary>
/// Template for generating AOT entrypoints
/// </summary>
public static unsafe partial class EntryPoint
{{
    static void ModuleSetup()
    {{
        {scripts}
    }}

    {init}
    {unload}
    {keyboard}
    {tick}
}}
";
                context.AddSource("EntryPoint.g.cs", source);
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox(default, ex.ToString(), "ScriptHookVDotNet source generator error", default);
#endif
            }
        }
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type = default);
        bool EntryPointExists(string entry)
        {
            return AllMethods.SelectMany(x => x.GetAttributes()).Any(x =>
            $"{x.AttributeClass.ContainingNamespace}.{x.AttributeClass.Name}"
            == "System.Runtime.InteropServices.UnmanagedCallersOnlyAttribute"
            && x.NamedArguments.Any(x => x.Key == "EntryPoint" && x.Value.Value.ToString() == entry));
        }
        string GenerateScriptsRegister()
        {
            var source = "GTA.Script script;\n";
            var scripts = AllTypes.Where(x => x.TypeKind == TypeKind.Class && !x.IsAbstract && x.BaseType?.ToString() == "GTA.Script");
            foreach (var script in scripts)
            {
                if (script.GetAttributes().Any(x => x.AttributeClass.ToString() == "GTA.ScriptAttributes" && x.NamedArguments.Any(x => x.Key == "NoDefaultInstance" && ((bool)x.Value.Value)))) continue;
                var fullName = script.ToString();
                source += $"script = new {fullName}();";
                source += $"Core.RegisterScript(script);\n";
                source += $"GTA.Console.RegisterCommands(typeof({fullName}));\n";
                source += $"GTA.Console.RegisterCommands(typeof({fullName}), script);\n";
            }
            return source;
        }
        public void Initialize(GeneratorInitializationContext context)
        {
        }
        #region Codes

        const string InitCode = @"
    [UnmanagedCallersOnly(EntryPoint = ""OnInit"")]
    public static void OnInit(HMODULE module)
    {
        try
        {
            Core.CurrentModule = module;
            ModuleSetup();
        }
        catch (Exception ex)
        {
            MessageBoxA(default, ex.ToString(), ""Module initialization error"", MB_OK);
            throw; // Crash the process
        }
    }";
        const string UnloadCode = @"/// <summary>
    /// Called prior to module unload
    /// </summary>
    /// <param name=""module""></param>
    [UnmanagedCallersOnly(EntryPoint = ""OnUnload"")]
    public static void OnUnload(HMODULE module)
    {
        Core.CurrentModule = module;
        Core.OnUnload();
    }";
        const string KeyboardCode = @"[UnmanagedCallersOnly(EntryPoint = ""OnKeyboard"")]
    public static void OnKeyboard(DWORD key, ushort repeats, bool scanCode, bool isExtended, bool isWithAlt,
        bool wasDownBefore, bool isUpNow)
    {
        Core.DoKeyEvent(
            key,
            !isUpNow,
            (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0,
            (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0,
            isWithAlt);
    }";
        const string TickCode = @"
    [UnmanagedCallersOnly(EntryPoint = ""OnTick"")]
    public static void OnTick(LPVOID currentFiber)
    {
        Core.DoTick(currentFiber);
    }";
        const string Globals = $@"
global using System;
global using System.IO;
global using DWORD = System.Int32;
global using DWORD64 = System.Int64;
global using HANDLE = System.IntPtr;
global using LPVOID = System.IntPtr;
global using HINSTANCE = System.IntPtr;
global using HMODULE = System.IntPtr;
global using static SHVDN.Globals;
global using static SHVDN.PInvoke;";
        #endregion
    }
}
