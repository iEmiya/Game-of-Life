﻿using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Retail")]
#endif
[assembly: AssemblyCompany("Just Think®")]
[assembly: AssemblyProduct("Game of Life")]
[assembly: AssemblyCopyright("© 2011 Just Think®. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.0.0")]

[assembly: ComVisible(false)]

[assembly: NeutralResourcesLanguage("en")]
