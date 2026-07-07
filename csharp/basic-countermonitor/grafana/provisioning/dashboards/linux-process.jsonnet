
local graph(title, expr, unit='short') = {
    title: title,
    type: 'timeseries',
    targets: [{
        expr: expr,
    }],
    fieldConfig: {
        defaults: {
            unit: unit,
        },
    },
};

[
    graph(
        "RSS Memory",
        "process_rss_mb{cmd=~'$cmd'}",
        "decbytes"
    ),

    graph(
        "Virtual Memory",
        "process_vsz_mb{cmd=~'$cmd'}",
        "decbytes"
    ),

    graph(
        "Major Faults",
        "process_majflt{cmd=~'$cmd'}"
    ),
]
