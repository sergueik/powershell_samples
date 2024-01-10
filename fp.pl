#!/usr/bin/perl

use strict;

# Origin: TPL
my $funcs = {
    'x' => sub { sprintf 'xxx %s', shift },
    'y' => sub { my $value = shift; sprintf 'yyy %d', $value },
};
my $fallback = sub { sprintf 'default %s', shift };
sub fallback_method {
  sprintf 'default %s', shift 
}
my $data = { 'x' => 10, 'y' => 20 };
map { print $_ . ': ' . $funcs->{$_}->( $data->{$_} ) . $/ } keys %$data;
$data->{'z'} = 100;
my $k;
my $v;

foreach $k ( keys %$data ) {
    if ( exists $$funcs{$k} ) {
        $v = $funcs->{$k}->( $data->{$k} );
    }
    else {
        $v = $fallback->( $data->{$k} );
    }
    print $v, $/;
}

foreach my $key ( keys %$data ) {
    my $arg = $data->{$key};
    my $val =
      ( exists $$funcs{$key} )
      ? $funcs->{$key}->($arg)
      : $fallback->($arg);

    print $val, $/;
}
