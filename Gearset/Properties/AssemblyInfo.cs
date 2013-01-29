using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if WINDOWS
using System.Windows;
using System.Windows.Markup;
#endif

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Gearset")]
[assembly: AssemblyProduct("Gearset")]

[assembly: AssemblyDescription("Gearset, XNA Development toolkit")]
[assembly: AssemblyCompany("Complot Games (The Complot)")]
[assembly: AssemblyCopyright("Copyright © 2011 Complot Games, Inc. All Rights Reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

#if WINDOWS
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]
#endif

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("d34c4ece-6e8b-48d2-8243-94b07bba9503")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//

//#if WINDOWS
////[assembly: AssemblyVersion("1.2.*")]
//#else
//// Wire-up here the version number of the Windows build so all platforms have the same build number.
//[assembly: AssemblyVersion("1.2.4277.155")]
//#endif
[assembly: AssemblyVersion("2.0.1")]

// Adding this attribute will force the Obfuscator to stop obfuscating internals
// which ends up showing too much code to customers. WARNING.
//[assembly: InternalsVisibleTo("Inspector")]


