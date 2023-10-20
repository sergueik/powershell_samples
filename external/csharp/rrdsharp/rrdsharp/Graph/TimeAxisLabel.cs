// ============================================================
//  RRDSharp: Managed implementation of RRDTool for .NET/Mono
// ============================================================
//
// Project Info:  http://sourceforge.net/projects/rrdsharp/
// Project Lead:  Julio David Quintana (david@quintana.org)
//
// Distributed under terms of the LGPL:
//
// This library is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this
// library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330,
// Boston, MA 02111-1307, USA.

using System;

namespace RrdSharp.Graph
{
	internal class TimeAxisLabel : Comment
	{
	
		internal TimeAxisLabel( string text )
		{
			this.text 	= text;
			lfToken		= Comment.TKN_ACF; 
			base.ParseComment();
			
			// If there's no line end, add centered-line end
			if ( !base.CompleteLine ) 
			{
				oList.Add( "" );
				oList.Add( Comment.TKN_ACF );
			
				oList.Add( "" );
				oList.Add( Comment.TKN_ALF );
				
				this.lineCount += 2;
			}
		}
	}

}