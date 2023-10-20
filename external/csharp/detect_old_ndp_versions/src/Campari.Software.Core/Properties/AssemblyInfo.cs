// ---------------------------------------------------------------------------
// Campari Software
//
// AssemblyInfo.cs
//
// Provides a cental location for Assembly attributes. Assembly attributes 
// are values that provide information about an assembly. The attributes are
// divided into the following sets of information: 
//
//    * Assembly identity attributes. 
//    * Informational attributes. 
//    * Assembly manifest attributes. 
//    * Strong name attributes. 
//
// ---------------------------------------------------------------------------
// Copyright (C) 2006 Campari Software
// All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
// ---------------------------------------------------------------------------
/* The Code Project Open License (CPOL)

Preamble
This License governs Your use of the Work. This License is intended to allow
developers to use the Source Code and Executable Files provided as part of
the Work in any application in any form. 

The main points subject to the terms of the License are:

   * Source Code and Executable Files can be used in commercial applications; 
   * Source Code and Executable Files can be redistributed; and 
   * Source Code can be modified to create derivative works. 
   * No claim of suitability, guarantee, or any warranty whatsoever is provided. 
     The software is provided "as-is". 

This License is entered between You, the individual or other entity reading
or otherwise making use of the Work licensed pursuant to this License and 
the individual or other entity which offers the Work under the terms of this
License ("Author").

For full license details, see http://www.codeproject.com/info/cpol10.aspx
*******************************************************************************/

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;


#region Assembly identity attributes
[assembly: SatelliteContractVersionAttribute("1.0.0.0")]
#endregion

#region Informational attributes

// String value specifying the Win32 file version number. This normally defaults to the assembly version.
// [assembly: AssemblyFileVersion("")] 

// String value specifying version information that is not used by the runtime, such as a full product version number.
// [assembly: AssemblyInformationalVersionAttribute("")] 
// [assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = false)]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: NeutralResourcesLanguage("en-US")]
#endregion

#region Assembly manifest attributes

#if Debug
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

// String value specifying a default alias to be used by referencing assemblies. This value provides
// a friendly name when the name of the assembly itself is not friendly (such as a GUID value). This
// value can also be used as a short form of the full assembly name.
// [assembly: AssemblyDefaultAliasAttribute("")]

[assembly: AssemblyDescription("Provides core functionality.")]
[assembly: AssemblyTitle("Campari Software Common Library for .NET 2.0 Core")]

#endregion

#region Strong name attributes

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]

#endregion
