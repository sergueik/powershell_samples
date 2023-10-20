/*
 * Created by SharpDevelop.
 */
using System;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Text;
using System.Xml;

public class LogReader
{
	private ArrayList arrList = new ArrayList();
	private string logName = string.Empty;
	private string query = string.Empty;
	public LogReader(string logName, String query, ArrayList arrList)
	{
		this.logName = logName;
		this.query = query;
		this.arrList = arrList;
	}
	public void Collect()
	{
		var eventsQuery = new EventLogQuery(logName, PathType.LogName, query) { ReverseDirection = true };
		var logReader = new EventLogReader(eventsQuery);
		for (EventRecord eventdetail = logReader.ReadEvent(); eventdetail != null; eventdetail = logReader.ReadEvent()) {
			var record = new EventLogRecord(eventdetail);
			PostDetail(record);

		}
	}
	private void PostDetail(params EventLogRecord[] records)
	{
		foreach (EventLogRecord record in records) {
			if (record != null) {
				Console.WriteLine(record.EventData);
				string xml = record.DetailsXML;
				var doc = new XmlDocument();
				doc.LoadXml(xml);
				// https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlnamespacemanager?view=netframework-4.0
				var xmlNamespaceManager = new XmlNamespaceManager(doc.NameTable);
				xmlNamespaceManager.AddNamespace("t", doc.DocumentElement.NamespaceURI);
				foreach (string attr in attrList) {
					// https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlnodelist?view=netframework-4.0
					XmlNodeList dataNodes = doc.SelectNodes(String.Format(@"//t:Data[@Name=""{0}""]", attr), xmlNamespaceManager);
					var count = dataNodes.Count;
					Console.WriteLine(String.Format("Found {0} Data nodes", count));
					for (int index = 0; index < count; index++) {
						Console.WriteLine(String.Format("Data node text: {0}", dataNodes.Item(index).InnerText));
					}
				}
			}
		}
	}

}
public enum LogLevel
{
        
	Error = 1,
	Warning = 2,
	Information = 4,
	SuccessAudit = 8,
	FailureAudit = 16
}

class EventLogRecord: ICloneable
{
	private string levelString;
	// TODO: find way to suppress
	// Add-Type : cgh24oab.0.cs(45) :
	// Warning as Error: The field 'EventLogRecord.level' is never used
	//private LogLevel level;
	private DateTime timestamp;
	private string source;
	private int eventID;
	private int taskCategory;
	private string detailsXML;
	private string logName;
	private string eventData;

	public string LevelString {
		get { return levelString; }
		set { levelString = value; }
	}

	//public LogLevel Level
	//{
	//    get
	//    {
	//        return level;
	//    }

	//    set
	//    {
	//        level = value;
	//    }
	//}

	public DateTime Timestamp {
		get { return timestamp; }
		set { timestamp = value; }
	}

	public string Source {
		get { return source; }
		set { source = value; }
	}

	public int EventID {
		get { return eventID; }
		set { eventID = value; }
	}

	public int TaskCategory {
		get { return taskCategory; }
		set { taskCategory = value; }
	}

	public string DetailsXML {
		get { return detailsXML; }
		set { detailsXML = value; }
	}

	public string LogName {
		get { return logName; }
		set { logName = value; }
	}

	public string EventData {
		get { return eventData; }
		set { eventData = value; }
	}

	public EventLogRecord(EventRecord eventdetail)
	{
		eventID = eventdetail.Id;
		detailsXML = eventdetail.ToXml();
		taskCategory = eventdetail.Task.Value;
		timestamp = eventdetail.TimeCreated.Value;
		source = eventdetail.ProviderName;
		//level = (LogLevel)eventdetail.Level.Value;
		levelString = eventdetail.LevelDisplayName;
		logName = eventdetail.LogName;
		eventData = GetEventData(eventdetail);
	}

	private string GetEventData(EventRecord eventdetail)
	{
		StringBuilder eventData = new StringBuilder();
		foreach (EventProperty prop in eventdetail.Properties) {
			if (prop.Value is byte[]) {
				eventData.Append(prop.Value.ToString() + "\n");
			} else {
				eventData.Append(prop.Value.ToString() + "\n");
			}

                
		}

		return eventData.ToString();
	}

	public object Clone()
	{
		return this.MemberwiseClone();
	}
}
