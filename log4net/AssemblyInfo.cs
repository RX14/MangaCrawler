#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System.Reflection;
using System.Runtime.CompilerServices;

#if (!SSCLI)
//
// log4net makes use of static methods which cannot be made com visible
//
[assembly: System.Runtime.InteropServices.ComVisible(false)]
#endif

//
// log4net is CLS compliant
//
[assembly: System.CLSCompliant(true)]

#if (!NETCF)
//
// If log4net is strongly named it still allows partially trusted callers
//
[assembly: System.Security.AllowPartiallyTrustedCallers]
#endif

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//

#if (CLI_1_0)
[assembly: AssemblyTitle("Apache log4net for CLI 1.0 Compatible Frameworks")]
#elif (NET_1_0)
[assembly: AssemblyTitle("Apache log4net for .NET Framework 1.0")]
#elif (NET_1_1)
[assembly: AssemblyTitle("Apache log4net for .NET Framework 1.1")]
#elif (NET_4_0)
#if CLIENT_PROFILE
[assembly: AssemblyTitle("Apache log4net for .NET Framework 4.0 Client Profile")]
#else
[assembly: AssemblyTitle("Apache log4net for .NET Framework 4.0")]
#endif // Client Profile
#elif (NET_2_0)
#if CLIENT_PROFILE
[assembly: AssemblyTitle("Apache log4net for .NET Framework 3.5 Client Profile")]
#else
[assembly: AssemblyTitle("Apache log4net for .NET Framework 2.0")]
#endif // Client Profile
#elif (NETCF_1_0)
[assembly: AssemblyTitle("Apache log4net for .NET Compact Framework 1.0")]
#elif (NETCF_2_0)
[assembly: AssemblyTitle("Apache log4net for .NET Compact Framework 2.0")]
#elif (MONO_1_0)
[assembly: AssemblyTitle("Apache log4net for Mono 1.0")]
#elif (MONO_2_0)
[assembly: AssemblyTitle("Apache log4net for Mono 2.0")]
#elif (SSCLI_1_0)
[assembly: AssemblyTitle("Apache log4net for Shared Source CLI 1.0")]
#elif (CLI_1_0)
[assembly: AssemblyTitle("Apache log4net for CLI Compatible Frameworks")]
#elif (NET)
[assembly: AssemblyTitle("Apache log4net for .NET Framework")]
#elif (NETCF)
[assembly: AssemblyTitle("Apache log4net for .NET Compact Framework")]
#elif (MONO)
[assembly: AssemblyTitle("Apache log4net for Mono")]
#elif (SSCLI)
[assembly: AssemblyTitle("Apache log4net for Shared Source CLI")]
#else
[assembly: AssemblyTitle("Apache log4net")]
#endif

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Retail")]
#endif

[assembly: AssemblyProduct("log4net")]
[assembly: AssemblyDefaultAlias("log4net")]
[assembly: AssemblyCulture("")]

