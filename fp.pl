#!/usr/bin/perl

#Copyright (c) 2024 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

# "Object-oriented"-style data processing in Perl 
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
