#!/usr/bin/perl

use strict;
my $csv = {
    key1 => { key => 'key1', value => 1, other        => 10 },
    key2 => { key => 'key2', value => 2, 'extra data' => undef, other => 20 },
    key4 => { key => 'key4', value => 2, 'extra data' => 42, other => 40 },
    key3 => { key => 'key3', value => 3, 'extra data' => 'x', other => 30 }
};
use vars qw|$key $row|;
my $columns = [ 'key', 'value', 'extra data', 'other' ];
print STDERR join( ',', ( map { "\"$_\"" } 'key', @$columns ) ) . "\n";
print STDERR join "\n", map {
    $key = $_;
    $row = $csv->{$_};
    join(
        ',', $key,    # alternatively include the "key" in $columns
        map { defined( $$row{$_} ) ? $row->{$_} : '' } @$columns
      )
} keys %$csv;
print STDERR "\n";

# Perl shortcut  (not typed)
print STDERR join "\n", map {
    $key = $_;
    $row = $csv->{$_};
    join( ',', $key, map { $row->{$_} || '' } @$columns )
} keys %$csv;
print STDERR "\n";
