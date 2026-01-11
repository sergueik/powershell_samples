using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;
namespace ServiceMaster
{
    public static class PluginCheck
    {
        /// <summary>
        /// 获取Plugin文件夹下所有的功能插件
        /// </summary>
        /// <returns></returns>
        public static IPluginMenu[] getAllPluginsMenu()
        {
            IPluginInfo[] plugins = getAllPluginsInfo();
            List<IPluginMenu> menus = new List<IPluginMenu>();
            foreach (IPluginInfo plugin in plugins)
            {
                if (plugin is IPluginMenu)
                    menus.Add((IPluginMenu)plugin);
            }
            return menus.ToArray();
        }
        /// <summary>
        /// 获取Plugin文件夹下所有的插件
        /// </summary>
        /// <returns></returns>
        public static IPluginInfo[] getAllPluginsInfo()
        {
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Plugin"))
            {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Plugin");
                List<IPluginInfo> dlls = new List<IPluginInfo>();
                foreach(string file in files)
                {
                    FileInfo info = new FileInfo(file);
                    if (info.Extension == ".dll")
                        dlls.Add(getPluginClass(info.Name, "PluginInfo"));
                }
                return dlls.ToArray();
            }
            else
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Plugin");
                return new IPluginInfo[0];
            }
        }
        /// <summary>
        /// 获取插件中的指定的类
        /// </summary>
        /// <param name="filename">插件DLL文件名，包括后缀名，不包括目录名，此处文件名需要与插件的类的命名空间一样</param>
        /// <param name="className">需要的类的名字，不需要命名空间</param>
        /// <returns></returns>
        public static IPluginInfo getPluginClass(string filename,string className)
        {
            Assembly ass = null;
            try
            {
                ass = Assembly.LoadFrom(@"plugin\" + filename);
            }
            catch (BadImageFormatException)
            {
                return null;
            }
            string nspace=System.IO.Path.GetFileNameWithoutExtension(filename);
            Type PluginClass = ass.GetType(nspace + "." + className);
            if (PluginClass==null)
                return null;
            Object o = Activator.CreateInstance(PluginClass);
            IPluginInfo info = (IPluginInfo)o;
            return info;
        }
        /// <summary>
        /// 获取插件类型
        /// </summary>
        /// <param name="plugin">插件对象</param>
        /// <returns></returns>
        public static string getPluginType(IPluginInfo plugin)
        {
            string type = string.Empty;
            if (plugin is IPluginMenu)
                type += "内容菜单插件";
            if (plugin is IPluginUI)
            {
                if (type != string.Empty)
                    type += ",";
                type += "外观插件";
            }
            if (plugin is IPluginSysInfo)
            {
                if (type != string.Empty)
                    type += ",";
                type += "系统信息插件";
            }
            if (type == string.Empty)
                type += "其他插件";
            return type;
        }
    }
}
