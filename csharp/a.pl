$t =<<EOF;

a


b


c


d
e
f
EOF
$t =~ s/(?<=\S)(\n\n\n)/A/gm;
print $/,$t;
