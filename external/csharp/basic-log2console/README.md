### Info
replica of
[Log2Console](https://github.com/Statyk7/log2console)
simple Log Viewer for development
offering a nice UI to display, filter and search log messages from different logging services: Log4Net, Log4j and NLog.

### Background
It is possible to use Log2Console with NLog.

NLog Target: Log2Console Receiver: UDP Receiver, localhost, port 7071

#### Background

__Serilog__ is often referred to as the *new logging framework* on the __.NET__ platform, while __NLog__
is considered the *old* one

#### Key Differences Between NLog and Serilog

Logging Approach: __Serilog__ is designed from the ground up for structured logging,
treating logs as objects rather than just text
messages
making machine querying log data by tools like Seq or Elasticsearch) easier.
__NLog__ is primarily plaintext-based, though it supports structured logging,
it can have degraded performance in that mode.

Configuration: __NLog__ conventionally uses extensive __XML__ configuration (`NLog.config`),
which allows changing log behavior without recompilation. __Serilog__ primarily uses __C#__ favourite code based
*fluent* configuration, often integrated directly into the subject `Program.cs` file,
which is popular for modern __.NET__ apps.

Performance: While both are high-performance, early benchmarks often showed __Serilog__ having better
throughput and lower latency than __NLog__, especially under high load and with extensive structured data.

Ecosystem & "Sinks": Both logging frameworks have wide support for output destinations of vasious streaming types:
 files, databases, console, network, cloud. __NLog__ calls these "Targets," while __Serilog__ calls them "Sinks".

 __Serilog__ has gained a reputation for having a slightly better, more modern plugin ecosystem ("enrichers") for adding context
 to logs (e.g., __HTTP request__ info).
Syntax: __Serilog__ often requires slightly more code for setup, while __NLog__ configuration can become
complex heavy, sometimes referred to as "XML hell," for very advanced scenarios.


#### Usage Examples & Scenarios

Choose Serilog if: You are starting a new .NET project, building microservices, using cloud log managers (e.g., SEQ, Datadog), or want powerful structured data querying.
Usage Example: Log.Information("Processed {OrderCount} orders in {Elapsed:000} ms", count, elapsed); (This makes OrderCount a searchable property).
Choose NLog if: You are working on legacy .NET Framework projects, prefer XML configuration over code-based configuration, or need extreme performance in file-writing scenarios.
Usage Example: Defines targets and rules in an NLog.config file to manage file rotation and archiving.


#### Synonyms/Related Terminology

Nlog                   | Serilog           | Meaning                                        |
---------------------- | ----------------- | -----------------------------------------------|
Targets           | Sinks            | The output destination, e.g., File, Database, Socket |
Layouts           | Output Templates | Formatting the output text                           |
Layout Renderers  | Enrichers        | Adding contextual data, like ThreadId,TraceId        |
### Note

For data traversal and transformation tasks, maintaining a stream-based model (`jq`-style) is often more efficient than converting *everything* ( e.g. inputs) into __object graphs__, especially when no persistent object semantics are required.


Stream-oriented workflows suffer:
log pipelines
file scanning
ETL-style transformations
remote scripting over SSH-like flows
tool chaining (grep/sed/awk/jq ecosystems)

In those cases:

object materialization = overhead
pipeline serialization/deserialization = hidden cost
implicit typing = unpredictability

With Seq
see [chm_inspector)](https://github.com/sergueik/powershell_samples/commit/a144add5f0af23a792eb7cefb4015bf58df50cdc#diff-96f3029223cfaf0c4b420dac2a58afa84c13b7d5378be256f30f80e3f8388503) adding old versions of to `Serilog.ElasticSerch` to [package.config]](https://github.com/sergueik/powershell_samples/blob/a144add5f0af23a792eb7cefb4015bf58df50cdc/csharp/chm_inspector/Test/packages.config) and `Serilog.Seq` to [package.config](https://github.com/sergueik/powershell_samples/blame/debug-oom/csharp/chm_inspector/Test/packages.config) 


NOTE: an interesting gap:

[1.5.1](https://www.nuget.org/packages/Serilog.Sinks.Seq/1.5.1) and
[7.0.0](https://www.nuget.org/packages/Serilog.Sinks.Seq/7.0.0) -  that is what one calls a full blown __semantic versioning__   - they seem to completely abandon .Net Framework. Note, if even the people famous for turning style into doctrine agree on this (the imfamous "Explicit is better than implicit"), the principle must be fundamental. Yet Serilog / Nuget insists `netstandard2.0` *(compatible with __.NET Framework 4.6.1__+)*


### See Also

  * NLog [tutorial](https://github.com/NLog/NLog/wiki/Tutorial)
  * NLog [Configuration File](https://github.com/NLog/NLog/wiki/Configuration-file)
  * [NLog.Windows.Forms](https://github.com/NLog/NLog.Windows.Forms) NLog targets specific for Windows.Forms / WinForms
  * [NLog vs log4net vs Serilog: Compare .NET Logging Frameworks](https://stackify.com/nlog-vs-log4net-vs-serilog) 
  * [Serilog vs Nlog](https://dev.to/thomasardal/serilog-vs-nlog-2ojf/comments)
  * [datalust/seqcli](https://github.com/datalust/seqcli) command-line client. Administer, log, ingest, search, from any OS.
  * [datalust/seq-api](https://github.com/datalust/seq-api) HTTP client for seq
  * [datalust/serilog-sinks-seq](https://github.com/datalust/serilog-sinks-seq) - embedding seq in Serlog sink
  * [datalust/seq-logging](https://github.com/datalust/seq-logging) - an js `seq_logger.js` `DefineLogger` class  potentially embeddable on graalvm (300 lines)
  * [datalust/nlog-targets-seq](https://github.com/datalust/nlog-targets-seq) __NLog__ __4.5+__ target that writes events to __Seq__
  * [datalust/seq-examples](https://github.com/datalust/seq-examples) - examples. Note: `/tree/main/client` - is lacking Java
  * [datalust/seq-forwarder](https://github.com/datalust/seq-forwarder) - local collection and reliable forwarding of log data to Seq, still in __c#__
  * [serilogj/serilogj](https://github.com/serilogj/serilogj) - A direct Java port of .NET's Serilog
  * []()
  * []()
  * []()




---

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
