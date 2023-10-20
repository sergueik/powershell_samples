#Copyright (c) 2021 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

param(
  [Parameter(Mandatory = $true,Position = 1)]
  [String]$datafile =  'data.json',
  # select series by "target"
  [String]$target = 'gaps'
  # TODO: debug . : A parameter with the name 'Verbose' was defined multiple times for thecommand.
  # [switch] $verbose
)
$verbose = $true
Add-type -TypeDefinition  @"

#pragma warning disable 0628

// http://stackoverflow.com/questions/10622674/chart-creating-dynamically-in-net-c-sharp
// http://www.emoticode.net/c-sharp/datavisualization-charting-chart-simple-example.html
// https://social.msdn.microsoft.com/Forums/vstudio/en-US/fb6fef67-3427-4b6a-a24b-d768b7986153/create-a-waterfall-chart-with-ms-chart-control-need-help?forum=MSWinWebChart
// http://www.codeproject.com/Articles/168056/Windows-Charting-Application
// https://help.syncfusion.com/windowsforms/chart/chart-types
// https://msdn.microsoft.com/en-us/library/system.windows.forms.datavisualization.charting.chart%28v=vs.110%29.aspx

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Util {
	public class ChartForm : Form {
		private IContainer components = null;
		// 'System.Collections.ArrayList' is non-generic type
		private ArrayList column1 = new ArrayList();
		private ArrayList column2 = new ArrayList();
		public Chart ch = new Chart();
		private ChartArea chartArea1 = new ChartArea();
		private Legend legend1 = new Legend();

		public void Add(float item1, float item2) {
			column1.Add(item1);
			column2.Add(item2);
		}

		public ChartForm() {
			InitializeComponent();
		}

		public void Form1_Load(object sender, EventArgs e) {
			ch.Series.Clear();
			// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.datavisualization.charting.series?view=netframework-4.0
			var series1 = new Series {
				Name = "Series1",
				Color = Color.Green,
				IsVisibleInLegend = false,
				IsXValueIndexed = false,
				ChartType = SeriesChartType.Line
			};

			ch.Series.Add(series1);

			for (int i = 0; i < column1.Count; i++) {
				series1.Points.AddXY(column1[i],column2[i] );
			}

			var series2 = new Series {
				Name = "Series2",
				Color = Color.Green,
				IsVisibleInLegend = false,
				IsXValueIndexed = false,
				// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.datavisualization.charting.series.isxvalueindexed?view=netframework-4.8
				ChartType = SeriesChartType.Point
			};

			ch.Series.Add(series2);

			for (int i = 0; i < column1.Count; i++) {
				series2.Points.AddXY(column1[i],column2[i] );
			}
			ch.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
			ch.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
			ch.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
			ch.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
			// System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;

			// TODO: hoow to enforce grid step, interval ?

			ch.Invalidate();
		}

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			components = new Container();

			((System.ComponentModel.ISupportInitialize)(ch)).BeginInit();
			SuspendLayout();

			chartArea1.Name = "ChartArea1";
			ch.ChartAreas.Add(chartArea1);
			ch.Dock = DockStyle.Fill;
			legend1.Name = "Legend1";
			ch.Legends.Add(legend1);
			ch.Location = new Point(0, 50);
			ch.Name = "ch";
			ch.Size = new Size(800, 600);
			ch.TabIndex = 0;
			ch.Text = "ch";
			//
			// Form1
			//
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			//ClientSize = new Size(284, 262);
			// Size = new Size(1200, 800);
			Controls.Add(ch);
			Name = "Form1";
			Text = "Chart";
			// want to pass the data
			// Load += new System.EventHandler(Form1_Load);
			((System.ComponentModel.ISupportInitialize)(ch)).EndInit();
			ResumeLayout(false);
		}

	}
}
"@ -ReferencedAssemblies 'System.Windows.Forms.DataVisualization.dll','System.Windows.Forms.DataVisualization.Design.dll','System.Windows.Forms.dll','System.Windows.Forms.dll','System.Drawing.dll','System.Data.dll','System.Xml.dll','System.ComponentModel.dll'

@( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

$o = new-object -TypeName 'Util.ChartForm'
$o.Size = new-object System.Drawing.Size(797, 267);

$o.ResumeLayout($false)
$o.PerformLayout()

$o.Add_Shown({ $o.Activate() })
$o.KeyPreview = $True

# origin: http://blogs.msdn.com/b/timid/archive/2013/03/05/converting-pscustomobject-to-from-hashtables.aspx
# https://www.powershellgallery.com/packages/ArcAdminTools/0.1.9/Content/Public%5CConvertTo-HashtableFromPsCustomObject.ps1
function ConvertTo-HashtableFromPsCustomObject {
  param(
    [Parameter(
      Position = 0,
      Mandatory = $true,
      ValueFromPipeline = $true,
      ValueFromPipelineByPropertyName = $true
    )] [object[]]$psCustomObject
  );

  process {
    foreach ($myPsObject in $psCustomObject) {
      $output = @{};
      $myPsObject | Get-Member -MemberType *Property | % {
        $output.($_.Name) = $myPsObject.($_.Name);
      }
      $output;
    }
  }
}


<#
# sqlite3 load_average.db "select load_average,status from load_average;"
import-csv -path (resolve-path $datafile) -header x,y | foreach-object {
  $o.Add($_.x, $_.y )
}
#>


# NOTE: Powerhsll's stock cmdlet "convertfrom-json" produces PsCustomObject type which is very uncomfortable to work with
$x = get-content -path (resolve-path -path $datafile) | convertfrom-json
$y = ConvertTo-HashtableFromPsCustomObject -psCustomObject $x
$z = $y |where-object { $_.target -eq $target }
$w = $z['datapoints']

# rebase the ticks
$t0 = 1.0 * $w[0][1]
$data = @()

0..($w.count-1) | foreach-object {
  $cnt = $_
  $u =  $w[$cnt]
  $m =  $u[0]
  
  $t = (1.0* $u[1] - $t0 )/ 1000.0
  if ($verbose ){
    write-output ('Adjusting timestamp : {0}, {1} / {2}' -f $u[1], $t0, $t)
  }
  if ($verbose ){
    write-output ('Preparing data: {0}, {1} / {2}' -f $t, $m, $t0)
  }
  # NOTE: the following flattens the data!
  <#
    $row = ($t, $m)
    $data += $row
  #>
  $row = @{'t' = $t; 'm' = $m; }
  $data += $row
}


$data | foreach-object {
  if ($verbose) {
    write-output ('Drawing {0}, {1}' -f $_.t, $_.m)
  }
  $o.Add($_.t, $_.m)
}

$o.Form1_Load($null, $null)
[void]$o.ShowDialog()

$o.Dispose()

