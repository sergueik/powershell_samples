using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

using Utils;

namespace Test
{

	[TestFixture]
	public class VirtualBoxVMInfoTest
	{

		private XDocument document = null;
		private string filename = @"C:\Users\kouzm\VirtualBox VMs\Xubuntu 22.04\Xubuntu 22.04.vbox";
		[SetUp]
		public void SetUp()
		{ 
			document = XDocument.Load(filename);
			/*
<?xml version="1.0"?>
<VirtualBox xmlns="http://www.virtualbox.org/" version="1.16-windows">
  <Machine uuid="{7e261a39-d356-4eb1-a8ed-75675b149241}" name="Xubuntu 22.04" OSType="Ubuntu_64" snapshotFolder="Snapshots" lastStateChange="2026-09-10T00:14:37Z">
   <StorageControllers>
      <StorageController name="IDE" type="PIIX4" PortCount="2" useHostIOCache="true" Bootable="true">
        <AttachedDevice passthrough="false" type="DVD" hotpluggable="false" port="1" device="0">
          <Image uuid="{ca34e2f3-e32a-45d3-a259-ff90719abe5c}"/>
        </AttachedDevice>
      </StorageController>
      <StorageController name="SATA" type="AHCI" PortCount="1" useHostIOCache="false" Bootable="true" IDE0MasterEmulationPort="0" IDE0SlaveEmulationPort="1" IDE1MasterEmulationPort="2" IDE1SlaveEmulationPort="3">
        <AttachedDevice type="HardDisk" hotpluggable="false" port="0" device="0">
          <Image uuid="{e31692be-ff5c-424f-818d-07a377758041}"/>
        </AttachedDevice>
      </StorageController>
    </StorageControllers>
  </Machine>
</VirtualBox>
*/
		}

		[Test]
		public void test1()
		{
			Assert.IsNotNull(document);
			
			foreach (XElement descendant in
			document.Descendants()) {
				string localName = (string)descendant.Name.LocalName.ToString();
				string type = (string)descendant.Attribute("type");
				// Console.WriteLine(String.Format("Decendant Name: {0} Type: {1}", localName, type));
				if ("StorageController".Equals(localName)) {
					var controller = descendant;

					// Console.WriteLine("x: " + controller.Elements().Count());
					// Console.WriteLine("x: " + controller.Elements().First().Name);

					foreach (XElement element1 in
             controller.Elements()) {
						var localName1 = (string)element1.Name.LocalName.ToString();
						if ("AttachedDevice".Equals(localName1)) {
							var device = element1;
					
							string deviceType = (string)device.Attribute("type");
							string port = (string)device.Attribute("port");
							string deviceNumber = (string)device.Attribute("device");

							foreach (XElement element2 in
             device.Elements()) {

								var localName2 = (string)element2.Name.LocalName.ToString();
								if ("Image".Equals(localName2)) {
									XElement image = element2;

									string uuid = image == null
            ? null
            : (string)image.Attribute("uuid");

									Console.WriteLine(
										"  Device: type={0}, port={1}, device={2}, uuid={3}",
										deviceType, port, deviceNumber, uuid);
									// Device: type=DVD, port=1, device=0, uuid={ca34e2f3-e32a-45d3-a259-ff90719abe5c}
									// Device: type=HardDisk, port=0, device=0, uuid={e31692be-ff5c-424f-818d-07a377758041}
								}
							}
						}
					}
				}
			}
		}
		
	}
	public class ImageInfo
	{
		public string deviceType { get; set; }
		public string port { get; set; }
		public string deviceNumber { get; set; }
		public string uuid { get; set; }
	}
}
