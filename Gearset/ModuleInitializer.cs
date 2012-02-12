using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Gearset
{
    /// <summary>
    /// This class provides a method that will be run at module initialization
    /// because we have a PostBuild Task that injects this method
    /// into the Module static constructor. This is needed to be able to load 
    /// a dll (specifically the LicenseManager dlls) from the Resource stream.
    /// http://tech.einaregilsson.com/2009/12/16/module-initializers-in-csharp/
    /// http://blogs.msdn.com/b/microsoft_press/archive/2010/02/03/jeffrey-richter-excerpt-2-from-clr-via-c-third-edition.aspx?PageIndex=2#comments
    /// </summary>
    internal class ModuleInitializer
    {
        internal static void Run()  
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve); 
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            String resourceName = "Gearset." + new AssemblyName(args.Name).Name + ".dll";

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }
}
