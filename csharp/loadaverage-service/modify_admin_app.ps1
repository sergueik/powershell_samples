# get-help add-localgroupmember
# get-help remove-LocalGroupMember
# administrators is S-1-5-32-544
add-localgroupmember -sid S-1-5-32-544 -member app


get-localgroupmember -sid S-1-5-32-544| select-object  -expandproperty name | where-object {$_ -match 'app'}
remove-localgroupmember -sid S-1-5-32-544 -member app

