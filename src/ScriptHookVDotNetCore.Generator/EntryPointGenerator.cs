﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ScriptHookVDotNetCore.Generator;

namespace ScriptHookVDotNet.Generator
{

    [Generator]
    public class EntryPointGenerator : SymbolPopulator,ISourceGenerator
    {
        
        public void Execute(GeneratorExecutionContext context)
        {
            Populate(context);
            var init = EntryPointExists("OnInit") ? "" : InitCode;
            var unload = EntryPointExists("OnOnload") ? "" : UnloadCode;
            var keyboard = EntryPointExists("OnKeyboard") ? "" : KeyboardCode;
            string source = $@"// <auto-generated/>
{Globals}
using System.Runtime.InteropServices;

namespace SHVDN;

/// <summary>
/// Template for generating AOT entrypoints
/// </summary>
public static unsafe partial class EntryPoint
{{
    static void ModuleSetup()
    {{
        {GenerateScriptsRegister()}
    }}

    {init}

    {unload}

    {keyboard}
}}
";
            context.AddSource("EntryPoint.g.cs", source);
        }
        bool EntryPointExists(string entry)
        {
            return AllMethods.SelectMany(x => x.GetAttributes()).Any(x =>
            $"{x.AttributeClass.ContainingNamespace}.{x.AttributeClass.Name}"
            == "System.Runtime.InteropServices.UnmanagedCallersOnlyAttribute"
            && x.NamedArguments.Any(x => x.Key == "EntryPoint" && x.Value.Value.ToString() == entry));
        }
        string GenerateScriptsRegister()
        {
            var source = "";
            var scripts = AllTypes.Where(x => $"{x.BaseType.ContainingNamespace}.{x.BaseType.Name}" == "GTA.Script");
            foreach (var script in scripts)
            {
                source += $"Core.RequestScriptCreation(new {script.ContainingNamespace}.{script.Name}());\n";
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
            ModuleSetup();
            Core.CurrentModule = module;
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
        for (int i = 0; i < 20; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }";
        const string KeyboardCode = @"[UnmanagedCallersOnly(EntryPoint = ""OnKeyboard"")]
    public static void OnKeyboard(DWORD key, ushort repeats, bool scanCode, bool isExtended, bool isWithAlt,
        bool wasDownBefore, bool isUpNow)
    {
        _keyboardMessage(
            key,
            !isUpNow,
            (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0,
            (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0,
            isWithAlt);
    }

    static void _keyboardMessage(DWORD keycode, bool keydown, bool ctrl, bool shift, bool alt)
    {
        // Filter out invalid key codes
        if (keycode <= 0 || keycode >= 256)
            return;

        // Convert message into a key event
        var keys = (Keys)keycode;
        if (ctrl) keys |= Keys.Control;
        if (shift) keys |= Keys.Shift;
        if (alt) keys |= Keys.Alt;
        Core.DoKeyEvent(keys, keydown);
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