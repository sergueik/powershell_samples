namespace WslManagerFramework.Models {
	public class AvailableDistro {
		private string name; 
		private string friendlyName; 
		private string description; 
		private bool isInstalled; 
		public string Name { get {return name;} set {name = value;} }
		public string FriendlyName { get {return friendlyName;} set {friendlyName = value;} }
		public string Description { get {return description;} set {description = value;} }
		public bool IsInstalled { get {return isInstalled;} set {isInstalled = value;} }
        
		public AvailableDistro(string name, string friendlyName, string description) {
			this.name = name;
			this.friendlyName = friendlyName;
			this.description = description;
			this.isInstalled = false;
		}
	}
}
