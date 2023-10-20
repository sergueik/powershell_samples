// ---------------------------------------------------------------------------
// Campari Software
//
// SolutionInfo.cs
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
// Copyright (C) 2006-2007 Campari Software
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

using System.Reflection;


#region Assembly identity attributes

[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("2.0.*")]

#endregion

#region Informational attributes

[assembly: AssemblyCompany("Campari Software")]
[assembly: AssemblyCopyright("Copyright © 2006-2007 Campari Software. All rights reserved.")]
[assembly: AssemblyTrademark(@"")]
[assembly: AssemblyProduct("Campari Software Common Library for .NET 2.0")]
#endregion

#region Assembly manifest attributes
// Do not use in a globally included SolutionInfo.cs file. The attributes in this
// section apply at an assembly level and should be used in the project specific
// AssemblyInfo.cs files.
#endregion

#region Strong name attributes
// Do not use in a globally included SolutionInfo.cs file. The attributes in this
// section apply at an assembly level and should be used in the project specific
// AssemblyInfo.cs files.
#endregion
