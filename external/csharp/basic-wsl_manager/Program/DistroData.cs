using System;
using System.Linq;

namespace Program {
	public class DistroData {
		public string DistroImage { get; set; }
		public string DistroName { get; set; }
		public string DistroState { get; set; }
		public int DistroWslVersion { get; set; }
		private string guid = "{9d51c0ac-cf84-46ab-bbfb-b8d417429ea5}"; 
		// key in HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Lxss
		public string Guid { get {return guid;} set {guid = value; } }
	}
}
