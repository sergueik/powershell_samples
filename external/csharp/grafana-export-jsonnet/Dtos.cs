using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter {
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class Target2 {
		public int limit { get; set; }
		public bool matchAny { get; set; }
		public List<object> tags { get; set; }
		public string type { get; set; }
	}

	public class List {
		public int builtIn { get; set; }
		public Datasource datasource { get; set; }
		public bool enable { get; set; }
		public bool hide { get; set; }
		public string iconColor { get; set; }
		public string name { get; set; }
		public Target target { get; set; }
		public string type { get; set; }
	}

	public class Annotations {
		public List<List> list { get; set; }
	}

	public class GridPos {
		public int h { get; set; }
		public int w { get; set; }
		public int x { get; set; }
		public int y { get; set; }
	}

	public class Step {
		public string color { get; set; }
		public float? value { get; set; }
	}

	public class Thresholds {
		public string mode { get; set; }
		public List<Step> steps { get; set; }
	}

	public class Color {
		public string mode { get; set; }
	}

	public class HideFrom {
		public bool legend { get; set; }
		public bool tooltip { get; set; }
		public bool viz { get; set; }
	}

	public class ScaleDistribution {
		public string type { get; set; }
	}

	public class Stacking {
		public string group { get; set; }
		public string mode { get; set; }
	}

	public class ThresholdsStyle {
		public string mode { get; set; }
	}

	public class Custom {
		public HideFrom hideFrom { get; set; }
		public string axisLabel { get; set; }
		public string axisPlacement { get; set; }
		public int? barAlignment { get; set; }
		public string drawStyle { get; set; }
		public int? fillOpacity { get; set; }
		public string gradientMode { get; set; }
		public string lineInterpolation { get; set; }
		public int? lineWidth { get; set; }
		public int? pointSize { get; set; }
		public ScaleDistribution scaleDistribution { get; set; }
		public string showPoints { get; set; }
		public bool? spanNulls { get; set; }
		public Stacking stacking { get; set; }
		public ThresholdsStyle thresholdsStyle { get; set; }
	}

	public class Defaults {
		public List<object> mappings { get; set; }
		public Thresholds thresholds { get; set; }
		public string unit { get; set; }
		public Color color { get; set; }
		public Custom custom { get; set; }
	}

	public class Matcher {
		public string id { get; set; }
		public string options { get; set; }
	}

	public class Property {
		public string id { get; set; }
		public string value { get; set; }
	}

	public class Override {
		public Matcher matcher { get; set; }
		public List<Property> properties { get; set; }
	}

	public class FieldConfig {
		public Defaults defaults { get; set; }
		public List<Override> overrides { get; set; }
	}

	public class ReduceOptions {
		public List<string> calcs { get; set; }
		public string fields { get; set; }
		public bool values { get; set; }
	}

	public class Legend {
		public string displayMode { get; set; }
		public string placement { get; set; }
		public List<object> calcs { get; set; }
		public List<object> values { get; set; }
	}

	public class Tooltip {
		public string mode { get; set; }
	}

	public class Options {
		public string orientation { get; set; }
		public ReduceOptions reduceOptions { get; set; }
		public bool showThresholdLabels { get; set; }
		public bool showThresholdMarkers { get; set; }
		public Legend legend { get; set; }
		public string pieType { get; set; }
		public Tooltip tooltip { get; set; }
		public string colorMode { get; set; }
		public string graphMode { get; set; }
		public string justifyMode { get; set; }
		public string textMode { get; set; }
		public List<string> displayLabels { get; set; }
		public string displayMode { get; set; }
		public bool? showUnfilled { get; set; }
	}

	public class Datasource {
		public string type { get; set; }
		public string uid { get; set; }
	}

	public class Target {
		public Datasource datasource { get; set; }
		public bool exemplar { get; set; }
		public string expr { get; set; }
		public bool hide { get; set; }
		public string interval { get; set; }
		public string legendFormat { get; set; }
		public string refId { get; set; }
		public string format { get; set; }
		public bool? instant { get; set; }
		public int? intervalFactor { get; set; }
	}

	public class Panel {
		public bool collapsed { get; set; }
		public GridPos gridPos { get; set; }
		public int id { get; set; }
		public List<object> panels { get; set; }
		public string title { get; set; }
		public string type { get; set; }
		public string description { get; set; }
		public FieldConfig fieldConfig { get; set; }
		public string interval { get; set; }
		public Options options { get; set; }
		public string pluginVersion { get; set; }
		public List<Target> targets { get; set; }
	}

	public class Templating {
		public List<object> list { get; set; }
	}

	public class Time {
		public string from { get; set; }
		public string to { get; set; }
	}

	public class Timepicker {
		public List<string> refresh_intervals { get; set; }
	}

	public class Root {
		public Annotations annotations { get; set; }
		public bool editable { get; set; }
		public int fiscalYearStartMonth { get; set; }
		public int graphTooltip { get; set; }
		// TODO: make nulls allowed
		public int id { get; set; }
		public List<object> links { get; set; }
		public bool liveNow { get; set; }
		public List<Panel> panels { get; set; }
		public string refresh { get; set; }
		public int schemaVersion { get; set; }
		public string style { get; set; }
		public List<object> tags { get; set; }
		public Templating templating { get; set; }
		public Time time { get; set; }
		public Timepicker timepicker { get; set; }
		public string timezone { get; set; }
		public string title { get; set; }
		public string uid { get; set; }
		public int version { get; set; }
		public string weekStart { get; set; }
	}
}
