﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Elektronik.UI.Localization;
using Debug = UnityEngine.Debug;

namespace Elektronik.PluginsSystem.UnitySide
{
    public static class PluginsLoader
    {
        public static readonly Lazy<List<IElektronikPlugin>> Plugins = new Lazy<List<IElektronikPlugin>>(LoadPlugins);
        public static readonly List<IElektronikPlugin> ActivePlugins = new List<IElektronikPlugin>();

        public static void EnablePlugin(IElektronikPlugin plugin)
        {
            if (!ActivePlugins.Contains(plugin)) ActivePlugins.Add(plugin);
        }

        public static void DisablePlugin(IElektronikPlugin plugin)
        {
            if (ActivePlugins.Contains(plugin)) ActivePlugins.Remove(plugin);
        }

        private static List<IElektronikPlugin> LoadPlugins()
        {
            var res = new List<IElektronikPlugin>();
            try
            {
                var currentDir = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) ?? "";
                var pluginsDir = Path.Combine(currentDir, @"Plugins");
                var dlls = Directory.GetDirectories(pluginsDir)
                        .Select(d => Path.Combine(d, "libraries"))
                        .Where(Directory.Exists)
                        .SelectMany(d => Directory.GetFiles(d, "*.dll"));
                foreach (var file in dlls)
                {
                    try
                    {
                        res.AddRange(Assembly.LoadFrom(file)
                                             .GetTypes()
                                             .Where(p => typeof(IElektronikPlugin).IsAssignableFrom(p) && p.IsClass &&
                                                            !p.IsAbstract)
                                             .Select(InstantiatePlugin<IElektronikPlugin>)
                                             .Where(p => p != null));
                        TextLocalizationExtender.ImportTranslations(Path.Combine(Path.GetDirectoryName(file)!,
                                                                        @"../data/translations.csv"));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Plugin load error: {file}, {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"PluginsLoader initialized with error: {e.Message}");
            }

            SetupContextMenu(Environment.GetCommandLineArgs()[0],
                             res.OfType<IDataSourcePluginOffline>().SelectMany(p => p.SupportedExtensions));
            return res;
        }

        private static void SetupContextMenu(string elektronikExe, IEnumerable<string> extensions)
        {
#if UNITY_STANDALONE_WIN
            try
            {
                var setter = new Process
                {
                    StartInfo =
                    {
                        FileName = Path.Combine(Path.GetDirectoryName(elektronikExe)!,
                                                @"Plugins\ContextMenuSetter\ContextMenuSetter.exe"),
                        Arguments = $"\"{elektronikExe}\" {string.Join(" ", extensions)}"
                    }
                };
                setter.Start();
            }
            catch (Exception e)
            {
                Debug.LogError($"Context menu setter error: {e.Message}");
            }
#endif
        }

        private static T InstantiatePlugin<T>(Type t) where T : class
        {
            try
            {
                return (T) Activator.CreateInstance(t);
            }
            catch (Exception e)
            {
                Debug.LogError($"Plugin initialisation error. {t} - {e.Message}");
            }

            return null;
        }
    }
}