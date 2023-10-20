using System;
using System.Linq;
using System.Windows.Forms;

namespace RealTimeEventLogReader {
	public partial class LogDetails : Form {
		public LogDetails() {
			InitializeComponent();
		}

		public LogDetails(string source, string xmlDetails) {
			InitializeComponent();
			Text = source;
			var xml = System.Xml.Linq.XDocument.Parse(xmlDetails).ToString();
			rtbDetailXML.Text = xml;
		}

	}
}
