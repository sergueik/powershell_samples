
using Newtonsoft.Json;
using System.IO;
using System;

// See https://aka.ms/new-console-template for more information

namespace Converter {
	enum PanelType {
		Row,
		PieChart,
		Timeseries,
		Stat,
		BarGauge,
		Gauge,
		HeatMap,
        Table,
        Graph
	}

	static class Program {
		static void Main(string[] args) {
			var json = File.ReadAllText("./test.json");
			Root parsed = JsonConvert.DeserializeObject<Root>(json);
			// TODO: how does c# 6.x cope with
			// Error CS1729: 'Converter.JSONNETBuilder' does not contain a constructor that takes 0 arguments
			// var builder = new JSONNETBuilder();
			var builder = new JSONNETBuilder("");
			foreach (var panel in parsed.panels) {
				if (panel == null) {
					Console.WriteLine("null panel");
				} else {
					builder.AddPanel(panel);
				}
			}

			Console.WriteLine(builder.jsonNetString);
		}
	}

	class JSONNETBuilder {
		public string jsonNetString = "";

		public JSONNETBuilder(string dashboardTitle) {
			jsonNetString = @"
                local grafana = import 'grafonnet/grafana.libsonnet';

                local prometheus = grafana.prometheus;

                grafana.dashboard.new(
                  'World Server New',
                  schemaVersion=26,
                  editable=true,
                  refresh='5s',
                  time_from='now-1h',
                  time_to='now',
                  timepicker=grafana.timepicker.new(
                    refresh_intervals=['30s', '1m', '5m', '15m', '30m', '1h', '2h', '1d', '2d', '7d'],
                  ),
                  uid='arkadia-worldservers-new',
                  tags=[],
                )
             ";
		}

		public void AddPanel(Panel panel) {
			string gridpos = "{" + string.Format("h: {0}, w: {1}, x: {2}, y: {3}", panel.gridPos.h, panel.gridPos.w, panel.gridPos.x, panel.gridPos.y) + "}";

			PanelType panelType = ParsePanelType(panel.type);
			// NOTE: datasource is hardcoded
			jsonNetString += String.Format(@"
            .addPanel(
              gridPos={0},
              panel={1}
            ", gridpos, CreatePanel(panel, panelType));

			if (panel.targets != null) {
				foreach (var target in panel.targets) {
					jsonNetString += String.Format(@"  {0}", AddTarget(target));
				}
			}

			jsonNetString += ")";

		}

		public string AddTarget(Target target) {
			var fields = AddField("", "format", target.format);
			fields = AddField(fields, "legendFormat", target.legendFormat);
			fields = AddField(fields, "intervalFactor", target.intervalFactor);
			fields = AddField(fields, "hide", target.hide);
			fields = AddField(fields, "instant", target.instant);
			fields = AddField(fields, "interval", target.interval);


			return String.Format(@".addTarget(
                    prometheus.target(
                      expr ='{0}',
                      {1}
                    )
                  )
            ", target.expr, fields);
		}

		private string CreatePanel(Panel panel, PanelType panelType) {
			Console.Error.WriteLine("Create panel type {0}", panelType);
			switch (panelType) {
				case PanelType.Row:
					return String.Format("{0}", RowPanel(panel));
				case PanelType.PieChart:
					return String.Format("{0}", PiePanel(panel));
				case PanelType.Timeseries:
					return String.Format("{0}", TimeseriesPanel(panel));
				case PanelType.Stat:
					return  String.Format("{0}", StatsPanel(panel));
				case PanelType.BarGauge:
					return  String.Format("{0}", BarGauge(panel));
				case PanelType.Gauge:
					return  String.Format("{0}", Gauge(panel));
				case PanelType.HeatMap:
					return  String.Format("{0}", HeatMap(panel));
				case PanelType.Table:
					return  String.Format("{0}", Table(panel));
				case PanelType.Graph:
					return  String.Format("{0}", Graph(panel));
				default:
					throw new Exception("No such panel can be created");
			}
		}

		private string RowPanel(Panel panel) {
			
			var fields = AddField("", "title", panel.title);
			fields = AddField(fields, "datasource", "prometheus");
			fields = AddField(fields, "description", panel.description);
			fields = AddField(fields, "collapse", panel.collapsed ? "true" : "false");
			// https://stackoverflow.com/questions/2361857/what-does-mean-in-c-sharp

			return String.Format("grafana.row.new({0})", fields);
		}


		private string TimeseriesPanel(Panel panel) {
			var config = panel.fieldConfig.defaults;
			var custom = config.custom;
			var color = config.color;

			var fields = AddField("", "title", panel.title);
			fields = AddField(fields, "datasource", "prometheus");
			fields = AddField(fields, "description", panel.description);

			// NOTE: need to fill fields
			return String.Format(@"grafana.heatmapPanel.new({0})", fields);
		}

		private string HeatMap(Panel panel) {
			var config = panel.fieldConfig.defaults;

			// NOTE: looks like builder pattern but is not
			var fields = AddField("", "title", panel.title);
			fields = AddField(fields, "datasource", "prometheus");
			fields = AddField(fields, "description", panel.description);
			fields = AddField(fields, "pluginVersion", panel.pluginVersion);
			if  (config!= null) {
			   fields = AddField(fields, "xAxis_show", config.unit);
			   fields = AddField(fields, "thresholdsMode", config.thresholds.mode);
			}
			return String.Format(@"grafana.heatmapPanel.new({0})", fields);
            
		}

		private string Table(Panel panel) {
			var config = panel.fieldConfig.defaults;

			// NOTE: looks like builder pattern but is not
			var fields = AddField("", "title", panel.title);
			fields = AddField(fields, "datasource", "prometheus");
			fields = AddField(fields, "description", panel.description);
			fields = AddField(fields, "pluginVersion", panel.pluginVersion);
			if  (config!= null) {
			   fields = AddField(fields, "xAxis_show", config.unit);
			   fields = AddField(fields, "thresholdsMode", config.thresholds.mode);
			}
			return String.Format(@"grafana.table.new({0})", fields);
		}

		private string Graph(Panel panel) {
			var config = panel.fieldConfig.defaults;

			// NOTE: looks like builder pattern but is not
			var fields = AddField("", "title", panel.title);
			fields = AddField(fields, "datasource", "prometheus");
			fields = AddField(fields, "description", panel.description);
			fields = AddField(fields, "pluginVersion", panel.pluginVersion);
			if  (config!= null) {
			   fields = AddField(fields, "xAxis_show", config.unit);
			   fields = AddField(fields, "thresholdsMode", config.thresholds.mode);
			}
			return String.Format(@"graphPanel.new({0})", fields);
		}


		private string Gauge(Panel panel) {
			var config = panel.fieldConfig.defaults;

			// NOTE: looks like builder pattern but is not
			var fields = AddField("", "title", panel.title);
			fields = AddField(fields, "datasource", "prometheus");
			fields = AddField(fields, "description", panel.description);
			fields = AddField(fields, "pluginVersion", panel.pluginVersion);
			fields = AddField(fields, "unit", config.unit);
			fields = AddField(fields, "thresholdsMode", config.thresholds.mode);

			return String.Format(@"grafana.gaugePanel.new({0})", fields);
            
		}

		private string BarGauge(Panel panel) {
			var config = panel.fieldConfig.defaults;

			var fields = AddField("", "title", panel.title);
			fields = AddField(fields, "datasource", "prometheus");
			fields = AddField(fields, "description", panel.description);
			fields = AddField(fields, "unit", config.unit);

			// TODO: why it was set to string literal here?
			// return @$"grafana.barGaugePanel.new({fields})";
			return String.Format(@"grafana.barGaugePanel.new({0})", fields);
            
		}

		private string PiePanel(Panel panel) {
			var options = panel.options;
			var config = panel.fieldConfig.defaults;

			var fields = AddField("", "title", panel.title);
			fields = AddField(fields, "datasource", "prometheus");
			fields = AddField(fields, "description", panel.description);
			fields = AddField(fields, "pieType", options.pieType);

			return String.Format(@"grafana.pieChartPanel.new({0})", fields);
		}

		private string StatsPanel(Panel panel) {
			var options = panel.options;
			var config = panel.fieldConfig.defaults;

			var fields = AddField("", "title", panel.title);
			// NOTE: this adds atasource='prometheus',
			// but we need datasource=prometheus
			fields = AddField(fields, "datasource", "prometheus");
			fields = AddField(fields, "description", panel.description);
			fields = AddField(fields, "colorMode", options.colorMode);
			fields = AddField(fields, "justifyMode", options.justifyMode);
			fields = AddField(fields, "orientation", options.orientation);
			fields = AddField(fields, "pluginVersion", panel.pluginVersion);
			fields = AddField(fields, "unit", config.unit);
			fields = AddField(fields, "thresholdsMode", config.thresholds.mode);
			return String.Format(@"grafana.statPanel.new({0})", fields);
		}

		// TODO:  what does the compiler error CS0453 mean:
		// Error CS0453: The type 'string' must be a non-nullable value type in order to use it as parameter 'T' in the generic type or method 'System.Nullable<T>'
		// private string AddField(string json, string name, string? field)
		private string AddField(string json, string name, string field) {
			if (!String.IsNullOrEmpty(field)) {
				// TODO: why it was set to string literal here?
				// return json += @$"{name}='{field}',";
				return json += String.Format(@"{0}='{1}',", name, field);
                
			}

			return json;
		}

		// TODO:  what does the compiler error CS0453 mean:
		// Error CS0453: The type 'dynamic' must be a non-nullable value type in order to use it as parameter 'T' in the generic type or method 'System.Nullable<T>'
		// private string AddField(string json, string name, dynamic? field)
		private string AddField(string json, string name, dynamic field) {
			if (field != null) {
				return json += String.Format(@"{0}='{1}',", name, field);                
			}
			return json;
		}


		private PanelType ParsePanelType(string type) {
			Console.Error.WriteLine(String.Format("adding {0}" , type));
			switch (type) {
				case "row":
					return PanelType.Row;
				case "piechart":
					return PanelType.PieChart;
				case "timeseries":
					return PanelType.Timeseries;
				case "stat":
					return PanelType.Stat;
				case "bargauge":
					return PanelType.BarGauge;
				case "gauge":
					return PanelType.Gauge;
				case "heatmap":
					return PanelType.HeatMap;
				case "table":
					return PanelType.Table;
				case "graph":
					return PanelType.Graph;
				default:
					throw new Exception(String.Format("No such type exists: {0}", type));
			}
		}
	}
}
