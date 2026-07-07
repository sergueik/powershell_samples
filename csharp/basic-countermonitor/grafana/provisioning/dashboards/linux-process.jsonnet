local graph(title, expr, unit='short') = {
  type: 'timeseries',
  title: title,

  fieldConfig: {
    defaults: {
      unit: unit,
    },
  },

  targets: [{
    refId: 'A',
    expr: expr,
  }],
};

{
  uid: 'linux-process',
  title: 'Linux Process Metrics',
  timezone: 'browser',
  schemaVersion: 38,
  version: 1,
  refresh: '5s',

  panels: [
    graph(
      'RSS Memory',
      'process_rss_mb{cmd=~"$cmd"}',
      'decbytes'
    ),

    graph(
      'Virtual Memory',
      'process_vsz_mb{cmd=~"$cmd"}',
      'decbytes'
    ),

    graph(
      'Major Faults',
      'process_majflt{cmd=~"$cmd"}'
    ),
  ],
}
