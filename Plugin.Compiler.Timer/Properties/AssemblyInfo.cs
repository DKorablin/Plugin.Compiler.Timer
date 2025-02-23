using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("d7a9a565-db2b-458e-bbb4-4ab1e8bb33bf")]
[assembly: System.CLSCompliant(true)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=120")]
#else

[assembly: AssemblyDescription("Create runtime method and invoke it in specific interval")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2019-2025")]
#endif