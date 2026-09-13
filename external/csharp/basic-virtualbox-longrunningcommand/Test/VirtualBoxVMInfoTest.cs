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

namespace Test {

	[TestFixture]
	public class VirtualBoxVMInfoTest {

		private XDocument document = null;
		private string filename = @"C:\Users\kouzm\VirtualBox VMs\Xubuntu 22.04\Xubuntu 22.04.vbox";

		[SetUp]
		public void SetUp() {
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
		public void test1() {
			var devices = new List<DeviceInfo>();
			Assert.IsNotNull(document);

			foreach (XElement descendant in document.Descendants()) {
				if ("StorageController".Equals(descendant.Name.LocalName)) {
					var controller = descendant;

					var controllerName = controller.Attribute("name").Value;
					var controllerType = controller.Attribute("type").Value;
					foreach (XElement element1 in controller.Elements()) {
						if ("AttachedDevice".Equals(element1.Name.LocalName)) {
							var device = element1;
					
							var deviceType = device.Attribute("type").Value;
							var port = device.Attribute("port").Value;
							var deviceNumber = device.Attribute("device").Value;

							foreach (XElement element2 in device.Elements()) {
								if ("Image".Equals(element2.Name.LocalName)) {
									XElement image = element2;

									var uuid = image == null ? null : image.Attribute("uuid").Value;

									Console.WriteLine("Device: controller={0}, controllerType={1}, deviceType={2}, port={3}, deviceNumber={4}, uuid={5}", controllerName, controllerType, deviceType, port, deviceNumber, uuid);
									var deviceInfo = new DeviceInfo();
									deviceInfo.controller = controllerName;
									deviceInfo.controllerType = controllerType;
									deviceInfo.deviceType = deviceType;
									deviceInfo.deviceNumber = deviceNumber;
									deviceInfo.port = port;
									deviceInfo.uuid = uuid;
									if("HardDisk".Equals(deviceType))
										devices.Add(deviceInfo);
								}
							}
						}
					}
				}
			}
			Assert.IsNotEmpty(devices);
		}

		[Test]
		public void test2() {
			Assert.IsNotNull(document);
			var devices = new List<DeviceInfo>();
			XElement root = document.Root;
			var defaultNamespace = root.GetDefaultNamespace();
			XElement machine = root.Element(defaultNamespace + "Machine");

			XElement controllers = machine.Element(defaultNamespace + "StorageControllers");

			foreach (XElement controller in controllers.Elements()) {

				if ("StorageController".Equals(controller.Name.LocalName)) {

					var controllerName = controller.Attribute("name").Value;
					var controllerType = controller.Attribute("type").Value;

					foreach (XElement device in controller.Elements()) {

						if ("AttachedDevice".Equals(device.Name.LocalName)) {

							var deviceType = device.Attribute("type").Value;
							var port = device.Attribute("port").Value;
							var deviceNumber = device.Attribute("device").Value;

							XElement image = device.Elements().FirstOrDefault((XElement xElement) => "Image".Equals(xElement.Name.LocalName));

							var uuid = image == null ? null : image.Attribute("uuid").Value;

							Console.WriteLine("Device: controller={0}, controllerType={1}, deviceType={2}, port={3}, deviceNumber={4}, uuid={5}", controllerName, controllerType, deviceType, port, deviceNumber, uuid);
							var deviceInfo = new DeviceInfo();
							deviceInfo.controller = controllerName;
							deviceInfo.controllerType = controllerType;
							deviceInfo.deviceType = deviceType;
							deviceInfo.deviceNumber = deviceNumber;
							deviceInfo.port = port;
							deviceInfo.uuid = uuid;
							if("HardDisk".Equals(deviceType))
								devices.Add(deviceInfo);
						}
					}
				}
			}
			Assert.IsNotEmpty(devices);
		}

		[Test]
		public void test3() {
			var devices = new List<DeviceInfo>();
			XElement root = document.Root;
			Assert.IsNotNull(document);
            XNamespace defaultNameSpace = root.GetDefaultNamespace();

			XElement machine = root.Element(defaultNameSpace + "Machine");
			XElement controllers = machine.Element(defaultNameSpace + "StorageControllers");

			foreach (XElement controller in controllers.Elements(defaultNameSpace + "StorageController")) {

				var controllerName = controller.Attribute("name").Value;
				var controllerType = controller.Attribute("type").Value;

				foreach (XElement device in controller.Elements(defaultNameSpace + "AttachedDevice")) {

					var deviceType = device.Attribute("type").Value;
					var port = device.Attribute("port").Value;
					var deviceNumber = device.Attribute("device").Value;

					XElement image = device.Element(defaultNameSpace + "Image");

					var uuid =
						image == null ? null : image.Attribute("uuid").Value;

					Console.WriteLine("Device: controller={0}, controllerType={1}, deviceType={2}, port={3}, deviceNumber={4}, uuid={5}", controllerName, controllerType, deviceType, port, deviceNumber, uuid);
					var deviceInfo = new DeviceInfo();
					deviceInfo.controller = controllerName;
					deviceInfo.controllerType = controllerType;
					deviceInfo.deviceType = deviceType;
					deviceInfo.deviceNumber = deviceNumber;
					deviceInfo.port = port;
					deviceInfo.uuid = uuid;
					if("HardDisk".Equals(deviceType))
						devices.Add(deviceInfo);
				}
			}
			Assert.IsNotEmpty(devices);
		}
	}

	public class ImageInfo {
		public string uuid { get; set; }
	}

	public class DeviceInfo {
		public string deviceType { get; set; }
		public string port { get; set; }
		public string controller { get; set; }
		public string controllerType { get; set; }
		public string deviceNumber { get; set; }
		public string uuid { get; set; }
	}
}
