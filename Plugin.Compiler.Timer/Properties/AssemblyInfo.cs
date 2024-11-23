using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("d7a9a565-db2b-458e-bbb4-4ab1e8bb33bf")]
[assembly: System.CLSCompliant(true)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=120")]
#else

[assembly: AssemblyTitle("Plugin.Compiler.Timer")]
[assembly: AssemblyDescription("Create runtime method and invoke it in specific interval")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyProduct("Plugin.CompilerTimer")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2019")]
#endif