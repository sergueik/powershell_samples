
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
            .addPanel(
              gridPos={h: 9, w: 12, x: 12, y: 0},
              panel=grafana.gaugePanel.new(title='Panel 1',pluginVersion='7.3.4',thresholdsMode='absolute',datasource=prometheus)
              .addTarget(
                    prometheus.target(
                      expr ='',
                      hide='False',
                    )
                  )
            )
            .addPanel(
              gridPos={h: 9, w: 12, x: 0, y: 9},
              panel=grafana.gaugePanel.new(title='Panel Title',pluginVersion='7.3.4',thresholdsMode='absolute', datasource=prometheus)
              .addTarget(
                    prometheus.target(
                      expr ='',
                      hide='False',
                    )
                  )
            )
            .addPanel(
              gridPos={h: 9, w: 12, x: 12, y: 9},
              panel=grafana.gaugePanel.new(title='Panel Title',pluginVersion='7.3.4',thresholdsMode='absolute',datasource=prometheus)
              .addTarget(
                    prometheus.target(
                      expr ='',
                      hide='False',
                    )
                  )
            )
            .addPanel(
              gridPos={h: 9, w: 12, x: 0, y: 18},
              panel=grafana.gaugePanel.new(title='Panel Title',pluginVersion='7.3.4',thresholdsMode='absolute',datasource=prometheus)
              
              .addTarget(
                    prometheus.target(
                      expr ='',
                      hide='False',
                    )
                  )
            )
