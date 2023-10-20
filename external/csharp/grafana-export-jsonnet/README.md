### Info
this directory contains a replica of [grafana-json-to-jsonnet-converter](https://github.com/TimonPost/grafana-json-to-jsonnet-converter) utility to convert grafana exported JSON to Jsonnet. Converted to .Net Framework 4.5 and down from c# 6.x to c# 5.
the goal is to produce valid grafana JSON that can be fed to grafana to import the dashboard.

### USage
The grafana "grafonnet" jsonnet template library:
30 or so `*.libsonnet` [files](https://github.com/grafana/grafonnet-lib/tree/master/grafonnet), covering various data sources

often replicated in the pet projects according to the needs:

e.g. one can find
```sh

gauge.libsonnet
influxdb.libsonnet
prometheus.libsonnet
stat_panel.libsonnet
table.libsonnet
timeseries.libsonnet
```
### Jsonnet Syntax
consder snippet of jssonnet code:

```json
{
  /**
   * Creates an SQL target.
   */
  target(
    rawSql,
    format='time_series',
    alias=null,
  ):: {
    [if alias != null then 'alias']: alias,
    format: format,
    [if rawSql != null then 'rawSql']: rawSql,
    groupBy:
      if group_tags != null then
        [{ type: 'tag', params: [tag_name] } for tag_name in group_tags] +
        [{ type: 'fill', params: [fill] }]
  },
}

```
and the calling dashboard creation code snippet:

```json
dashboard.new(
  title='SQL heatmap',
  editable=true,
  graphTooltip='shared_crosshair',
  time_from='now - 3d',
  tags=['SQL-DB'])
.addRows([
  row.new(title=r.row_title)
  .addPanels(
   [
      heatmap.new(
        title=metric.name,
        yAxis_format=metric.format,
      )
      .addTarget(
        grafana.sql.target(
          query=|||
	    RAW SQL
	 |||
	 )
      )

+ {
    addMappings(mappings):: std.foldl(function(p, m) p.addMapping(m), mappings, self),
    }
```

this includes  function definition, hash concatenation, logic, comprehensions, dereferencing, scopes, here-docs, string concatenation, library functions etc.

* NOTE: the code snippet glimpse of jsonnet syntax  is not a valid program fragment it was composed from

  * [https://github.com/grafana/grafonnet-lib/blob/master/grafonnet/sql.libsonnet](https://github.com/grafana/grafonnet-lib/blob/master/grafonnet/sql.libsonnet#L12)
  * [https://github.com/grafana/grafonnet-lib/blob/master/grafonnet/influxdb.libsonnet#L87](https://github.com/grafana/grafonnet-lib/blob/master/grafonnet/influxdb.libsonnet#L87)
  * [https://github.com/andreyzx12/jsonnet-grafana/blob/main/libs/stat_panel.libsonnet#L107](https://github.com/andreyzx12/jsonnet-grafana/blob/main/libs/stat_panel.libsonnet#L107)
  * [https://github.com/d-kireichuk/jsonnet-grafonnet-samples/blob/master/grafonnet-lib/graph_panel.libsonnet#L191](https://github.com/d-kireichuk/jsonnet-grafonnet-samples/blob/master/grafonnet-lib/graph_panel.libsonnet#L191)
### Usage

install binary compiled for Windows or Linux from https://github.com/google/go-jsonnet/releases
```sh
wget https://github.com/google/go-jsonnet/releases/download/v0.19.1/go-jsonnet_0.19.1_Linux_x86_64.tar.gz
tar xzvf go-jsonnet_0.19.1_Linux_x86_64.tar.gz
sudo mv jsonnet* /usr/local/bin/
```

for Windows place in the "Tools" folder which probably exists on your development machine


Go to `https://play.grafana.org` and

and export one of the demo show off dashboards
e.g. `the https://play.grafana.org/d/Zb3f4veGk/2-stats?orgId=1`
`https://play.grafana.org/d/000000029/prometheus-demo-dashboard?orgId=1&refresh=5m`

Select `Export for sharing externally` when exporting the JSON. This wil give quite heavy JSON even for a most typical dashboards


```text
Unhandled Exception: Newtonsoft.Json.JsonSerializationException: 
Error converting value {null} to type 'System.Int32'. 
Path 'id', line 64, position 13. ---> 
System.InvalidCastException: Null object cannot be converted to a value type.
```

patch the JSON:
from
```json
  "id": null,
```
to
```json
  "id": 42,
```
to avoid null id.
* if seeing more serious errors like
```text
Newtonsoft.Json.JsonReaderException: 
Error reading string. Unexpected token: StartObject. 
Path 'panels[7].fieldConfig.overrides[0].properties[0].value', 
line 733, position 27.
```
there is a schema mismatch:
```json

 "properties": [
    {
      "id": "custom.fillOpacity",
      "value": 10
    },
```
and
```json

  "properties": [
    {
      "id": "color",
      "value": {
        "fixedColor": "#705DA0",
        "mode": "fixed"
      }
    }

```
usually this can be solved by trying a simpler dashboard for p.o.c.
* run the __Converter__ in console:
```cmd
cd bin\Debug
Converter.exe > test.jsonnet
```
this will generate the `test.jsonnet` source like one checked in.

* run jsonnet:
```cmd
jsonnet.exe -J ..\..\ test.jsonnet  > result.json
```

this will produce result.json which can be imported in Grafana. NOTE: there invalid syntax errors are quite possible:

```text
RUNTIME ERROR: function has no parameter datasource
        test.jsonnet:(6:17)-(30:14)
        During evaluation
```

![grafana dashboard](https://github.com/sergueik/powershell_ui_samples/blob/master/external/csharp/grafana-export-jsonnet/screenshots/capture-imported-dashboard.png)

### Patching the Process

replacing the "grafana dashboarg call" in the generated `jsonnet `file
```code
            .addPanel(
              gridPos={h: 9, w: 12, x: 0, y: 0},
              panel=grafana.gaugePanel.new(title='Panel 2',pluginVersion='7.3.4',thresholdsMode='absolute',),
              datasource=prometheus
              .addTarget(
                    prometheus.target(
                      expr ='',
                      hide='False',
                    )
                  )
            )

```
with
```code
            .addPanel(
              gridPos={h: 9, w: 12, x: 0, y: 0},
              panel=grafana.gaugePanel.new(title='Panel 2',pluginVersion='7.3.4',thresholdsMode='absolute', datasource=prometheus)
              .addTarget(
                    prometheus.target(
                      expr ='',
                      hide='False',
                    )
                  )
            )
```
leads the generated  gashboard to have datasource configured.
The `.addPanel` in the file is sent to `grafana.dashboard` and the `grafonnet\dashboard.libsonnet` does not list `datasource` as valid property of the
`dashboard` type.

NOTE: replacing reference:
```code
local prometheus = grafana.prometheus;
```

with real import:
```code
local prometheus = 'grafonnet/prometheus.libsonnet';
```

leads to new `manifestation` error
		

```text
RUNTIME ERROR: Unexpected type string, expected number
        test.jsonnet:25:21-38
        grafonnet\gauge_panel.libsonnet:83:18-24        thunk from <object <anoymous>>
        Array element 0
        Field "targets"
        Array element 0
        Field "panels"
        During manifestation
```
### Note


NOTE: due to usage of the constructs like:
```JSON
  "annotations": {
    "list": [
      {
        "builtIn": 1,
        "datasource": "...",
        "enable": true,
        "hide": true,
        "iconColor": "rgba(0, 211, 255, 1)",
        "name": "Annotations & Alerts",
        "type": "dashboard"
      }
    ]
  },

```

in grafana JSON export with anonymous entries, automatic code generation is somewhat extra laborous, default run creates classes like:
```java
public class Annotations {
  List<Object> list = new ArrayList<Object>();
   // Getter Methods
   // Setter Methods
}
```

the unsophisticated code generators e.g. the one on [JSON to Java Code Generator](https://www.site24x7.com/tools/json-to-java.html) do not have enough information to enforce the properties of `Annotations` pojo class. One have to deserialize array entry

```json
      {
        "builtIn": 1,
        "datasource": "-- Grafana --",
        "enable": true,
        "hide": true,
        "iconColor": "rgba(0, 211, 255, 1)",
        "name": "Annotations & Alerts",
        "type": "dashboard"
      }
```
explicitly and create custom named classes, e.g. for Java:
```java

public class AnnotationEntry {

	private float builtIn;
	private String datasource;
	private boolean enable;
	private boolean hide;
	private String iconColor;
	private String name;
	private String type;

	public float getBuiltIn() {
		return builtIn;
	}

	public String getDatasource() {
		return datasource;
	}

	public boolean getEnable() {
		return enable;
	}

	public boolean getHide() {
		return hide;
	}

	public String getIconColor() {
		return iconColor;
	}

	public String getName() {
		return name;
	}

	public String getType() {
		return type;
	}

	public void setBuiltIn(float data) {
		builtIn = data;
	}

	public void setDatasource(String data) {
		datasource = data;
	}

	public void setEnable(boolean data) {
		enable = data;
	}

	public void setHide(boolean hide) {
		this.hide = hide;
	}

	public void setIconColor(String data) {
		iconColor = data;
	}

	public void setName(String data) {
		name = data;
	}

	public void setType(String data) {
		type = data;
	}
}
```

then hand craft `Annotations` to contain `AnnotationEntry`:

```java
public class Annotations {
	List<AnnotationEntry> annotationEntries = new ArrayList<>();

	public List<AnnotationEntry> getAnnotationEntries() {
		return annotationEntries;
	}

	public void setAnnotationEntries(List<AnnotationEntry> data) {
		annotationEntries = data;
	}

}
```

serving the type as JSON still produced acceptable data for that fragment:
```json
{
  "annotationEntries": [
    {
      "builtIn": 1,
      "datasource": "grafana",
      "enable": true,
      "hide": false,
      "iconColor": "rgba(0, 211, 255, 1)",
      "name": "Annotations & Alerts",
      "type": "dashboard"
    }
  ]
}
```

The JSON to Code [convertor](https://json2csharp.com/code-converters/json-to-pojo) does a little better code generation
for both __Java POJO Objects__ and __.Net Classes__. One still need to inspect the generated code:
another technical challenge with the Grafana Dashboard dump JSON 
is the presence of the colliding or nearly colliding generic object field names e.g:
```json
"targets": [
  {
    "refId": "A",
    "target": "select metric",
    "type": "timeserie"
  }
]
```
vs.

```json
"targets": [
  {
    "alias": "",
    "datasource": {
      "type": "datasource",
      "uid": "grafana"
    },
    "metrics": {
      "id": "1",
      "type": "count"
    },
    "query": "",
    "refid": "x",
    "type": "timeserie"
  }
]
```
vs.
```json
"target": {
  "limit": 100,
  "matchAny":  false,
  "tags": [],
  "type": "dashboard"
}
```
### Runtime Errors
```text
Unhandled Exception: Newtonsoft.Json.JsonSerializationException: Error converting value "-- Grafana --" to type 'Converter.Datasource'. Path 'annotations.list[0].datasource', line 66, position 38. ---> System.ArgumentException: Could not cast or convert from System.String to Converter.Datasource.
```

workaround: remove
```text
        "datasource": "-- Grafana --",
```

```text
Unhandled Exception: Newtonsoft.Json.JsonSerializationException: Error converting value {null} to type 'System.Int32'. Path 'id', line 77, position 13. 
---> System.InvalidCastException: 
Null object cannot be converted to a value type.
```
workaround:
convert
```text
  "id": null,
```
to
```text
  "id": 0,
```

```text
Unhandled Exception: System.Exception: No such type exists: heatmap
   at Converter.JSONNETBuilder.ParsePanelType(String type) Program.cs:line 244
```
### See Also
  * binary release of [grafana/grafonnet-lib](https://github.com/grafana/grafonnet-lib) used by this tool to hold reerence types needed during converting
  * binary releases of [google/og-jsonnet](https://github.com/google/go-jsonnet/releases)
  * grafonnet API [docs](https://grafana.github.io/grafonnet-lib/api-docs/)
     * __how to configure Grafana as code__ [blog](https://grafana.com/blog/2020/02/26/how-to-configure-grafana-as-code/)
   * __Using Jsonnet to Package Together Dashboards, Alerts and Exporters__ [youtube video](https://www.youtube.com/watch?v=b7-DtFfsL6E)
   * __Grafana As Code __ [youtube video](https://ftp.heanet.ie/mirrors/fosdem-video/2020/UD2.120/grafana_as_code.mp4)
   * __Automating Grafana Dashboards with Jsonnet__ [youtube video](https://www.youtube.com/watch?v=zmsZq9Pfp1g)
   * [grafana-tools/autograf](https://github.com/grafana-tools/autograf/tree/8d3b0eff0c8a124a44bcb06500366e08b255b162) - NOTE, the project was removed from github in the commit [8d3b0eff0c8a124a44bcb06500366e08b255b162](https://github.com/grafana-tools/autograf) - they now have an "SDK"
   * [Python tool](https://github.com/jakubplichta/grafana-dashboard-builder) for building Grafana dashboards in YAML.
   * [collection](https://github.com/uber/grafana-dash-gen) of utility classes to construct and publish grafana graphs
   * [jsonnet](https://jsonnet.org) templating language
   * [poc](https://github.com/TimonPost/grafana-json-to-jsonnet-converter) tool convert grafana exported JSON to Jsonnet
   * __jsonnet__ syntax [youtube video](https://www.youtube.com/watch?v=i5PVp92tAmE)
   * why use __jsonnet__ [youtube video](https://www.youtube.com/watch?v=W8kFrUOpO7s)
   * __jsonnet__ usage with k8 configurations YAML [youtube video](https://www.youtube.com/watch?v=zpgp3yCmXok)
   * Dashboards as Code pet projects links from __gsonnet youtube video__

      + [grafana dash dash dash gen](https://github.com/uber/grafana-dash-gen)
      + [Python grafanalib library](https://github.com/weaveworks/grafanalib)
      + [produce grafana dashboards with YAML](https://github.com/jakubplichta/grafana-dashboard-builder)
      + [generate Grafana dashboards from configuration](https://github.com/Showmax/grafana-dashboards-generator)
      + [grafyaml](https://docs.openstack.org/infra/grafyaml) - takes descriptions of Grafana dashboards in YAML format, and uses them to produce JSON formatted output suitable for direct import into grafana
      + [manage Grafana v4.0 Dashboard via saltstack](https://docs.saltstack.com/en/latest/ref/states/all/salt.states.grafana4_dashboard.html)

  * [grafana/grizzly](https://github.com/grafana/grizzly) - utility for managing Jsonnet dashboards against the Grafana API (golang)
  * [satyanash/promql-jsonnet](https://github.com/satyanash/promql-jsonnet) - Jsonnet based DSL for writing PromQL queries. This is useful for automatically creating dashboards on grafana with prometheus
  * [d-kireichuk/jsonnet-grafonnet-samples](https://github.com/d-kireichuk/jsonnet-grafonnet-samples) - code samples for "Automating Grafana dashboards with Jsonnet"

  * [Continuous Deployment of Grafana Dashboards Using Jsonnet and Jenkins, Part 1](https://www.neteye-blog.com/2021/07/continuous-deployment-of-grafana-dashboards-using-jsonnet-and-jenkins-part-1/)
  * [Grafana Jsonnet Workflow](https://janakerman.co.uk/grafana-jsonnet-workflow/)
  * [How to configure Grafana as code](https://grafana.com/blog/2020/02/26/how-to-configure-grafana-as-code/)
  * [Grafana as code](https://medium.com/@tarantool/grafana-as-code-b642cac9ae75)
  * [generate automated Grafana metrics dashboards for MicroProfile apps](https://developers.redhat.com/blog/2020/07/10/generate-automated-grafana-metrics-dashboards-for-microprofile-apps#about_microprofile_metrics)
  * [dashboard as a code with Grafana](https://github.com/edsoncelio/dashboard-as-a-code) - demo about how to create/build/deploy dashboards to grafana using jsonnet and grafonnet
  * [parinapatel/migrateGrafanaDashboards](https://github.com/parinapatel/migrateGrafanaDashboards) - migrate Grafana json Dashboard to jsonnet
  * [nanorobocop/simple-grafonnet](https://github.com/nanorobocop/simple-grafonnet) - generate simple Grafana dashboard based on app /metrics endpoint
  * [docker image with jsonnet and grafonnet](https://github.com/zabrowarnyrafal/grafonnet) - a grafana jsonet library to generate dashboards
  * [andreyzx12/jsonnet-grafana](https://github.com/andreyzx12/jsonnet-grafana) - repository include template grafana dashboard by wich generating jsonnet
  * [overview of several tools](https://www.codeproject.com/Articles/1201466/Working-with-Newtonsoft-Json-in-Csharp-VB) for working with Newtonsoft.Json in C# & VB
   * [overiew of several tools](https://www.codeproject.com/Articles/5339651/Working-with-System-Text-Json-in-Csharp) for working with System.Text.Json in C#
   * [overiew of several tools](https://www.codeproject.com/Articles/5340376/Deserializing-Json-Streams-using-Newtonsoft-and-Sy) for deserializing Json Streams using Newtonsoft.Json & System.Text.Json

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
