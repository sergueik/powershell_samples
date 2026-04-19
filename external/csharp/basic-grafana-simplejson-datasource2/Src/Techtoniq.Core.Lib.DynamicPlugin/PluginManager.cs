using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Techtoniq.Core.Lib.DynamicPlugin
{
    public static class PluginManager
    {
        /// <summary>
        /// PluginLoadContext provides a ring-fenced scope in to which an assembly can be loaded. This isolates all the dependant assemblies from any other assembly in the app domain, 
        /// enabling multiple dyanmically-loaded assemblies to use different versions of the same depdendant assembly.
        /// </summary>
        private class PluginLoadContext : AssemblyLoadContext
        {
            private AssemblyDependencyResolver _resolver;

            public PluginLoadContext(string pluginPath)
            {
                _resolver = new AssemblyDependencyResolver(pluginPath);
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
                if (assemblyPath != null)
                {
                    return LoadFromAssemblyPath(assemblyPath);
                }

                return null;
            }

            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
                if (libraryPath != null)
                {
                    return LoadUnmanagedDllFromPath(libraryPath);
                }

                return IntPtr.Zero;
            }
        }


        private static Assembly LoadAssembly(string assemblyPath)
        {
            string assemblyLocation = Path.GetFullPath(assemblyPath);
            PluginLoadContext loadContext = new PluginLoadContext(assemblyLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(assemblyLocation)));
        }

        public static void LoadPlugins(string pluginsPath)
        {
            var files = Directory.GetFiles(pluginsPath);
            foreach (var file in files)
            {
                var lowercaseFile = Path.GetFileName(file).ToLower();
                if (Path.GetExtension(lowercaseFile) == ".dll")
                {
                    LoadAssembly(file);
                }
            }
        }
    }
}
