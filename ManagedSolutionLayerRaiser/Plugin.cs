using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Managed Solution Layer Raiser"),
        ExportMetadata("Description", "Quickly, easily and safely raise your organization-owned managed solutions to the top of the stack of managed layers to resolve environmental inconsistencies and make your application behave as you intended without losing data"),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "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"),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Cyan"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class Plugin : PluginBase, IPayPalPlugin
    {
        public string DonationDescription => "Thank you for using the Managed Solution Layer Raiser XrmToolBox plugin. If you have found it helpful, please consider making a donation";

        public string EmailAccount => "info@xrmsolutionsuk.com";

        public override IXrmToolBoxPluginControl GetControl()
        {
            return new PluginControl();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public Plugin()
        {
            // If you have external assemblies that you need to load, uncomment the following to 
            // hook into the event that will fire when an Assembly fails to resolve
            // AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named Sample.XrmToolBox.MyPlugin 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}