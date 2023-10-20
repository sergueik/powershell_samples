// ---------------------------------------------------------------------------
// Campari Software
//
// WindowsFoundationLibrary.cs
//
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

namespace Campari.Software
{
    #region enum WindowsFoundationLibrary
    /// <summary>
    /// Specifies the .NET 3.0 Windows Foundation Library
    /// </summary>
    public enum WindowsFoundationLibrary
    {
        /// <summary>
        /// Windows Communication Foundation
        /// </summary>
        WCF,

        /// <summary>
        /// Windows Presentation Foundation
        /// </summary>
        WPF,

        /// <summary>
        /// Windows Workflow Foundation
        /// </summary>
        WF,

        /// <summary>
        /// Windows CardSpace
        /// </summary>
        CardSpace,
    }
    #endregion
}
