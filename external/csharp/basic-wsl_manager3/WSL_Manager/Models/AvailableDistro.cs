namespace WslManagerFramework.Models
{
	public class AvailableDistro
	{
		public string Name { get; set; }
		public string FriendlyName { get; set; }
		public string Description { get; set; }
		public bool IsInstalled { get; set; }
        
		public AvailableDistro(string name, string friendlyName, string description)
		{
			Name = name;
			FriendlyName = friendlyName;
			Description = description;
			IsInstalled = false;
		}
	}
}