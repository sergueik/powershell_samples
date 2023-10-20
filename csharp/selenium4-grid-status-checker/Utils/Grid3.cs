using System;

// Converted from Selenium 3.9.1 status JSON via json2csharp.com
namespace Utils {

	public class Grid3 {
		public float status { get; set; }
		public Value3 value { get; set; }
	}

	// NOTE: The namespace 'Utils' already contains a definition for 'Value'
	// Unlike NewtonSoft.Json.JsonConvert 
	// the System.Web.Script.Serialization.JavaScriptSerializer has no ability to rename properties
	// therefore cannot add NewtonSoft annotations
	// to make property names flexible
	public class Value3 {
		public bool ready { get; set; }
		public string message { get; set; }
		public Build build { get; set; }
		public Os os { get; set; }
		public Java java { get; set; }
	}

	public class Build {
		public string revision { get; set; }
		public DateTime time { get; set; }
		public string version { get; set; }
	}

	public class Java {
		public string version { get; set; }
	}

	public class Os {
		public string arch { get; set; }
		public string name { get; set; }
		public string version { get; set; }
	}
}
