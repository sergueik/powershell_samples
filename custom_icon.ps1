# https://onlineimagetools.com/convert-image-to-base64
# online image ico to base64 converter 
# https://icons.iconarchive.com/icons/scafer31000/bubble-circle-2/16/Message-icon.png

  @( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

[void][System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms')
[void][System.Reflection.Assembly]::LoadWithPartialName('System.Drawing')

$f = New-Object System.Windows.Forms.Form
$f.Size = New-Object System.Drawing.Size(160,130)
$f.Text = 'dummy'
$f.AutoSize = $false
$f.MaximizeBox = $false
$f.MinimizeBox = $true
$f.BackColor = '#FEFEFE' # '#c08888'
$f.ShowIcon = $true 
$f.SizeGripStyle = [System.Windows.Forms.SizeGripStyle]::Hide
$f.FormBorderStyle = [System.Windows.Forms.FormBorderStyle]::Fixed3D
$f.WindowState = 'Normal'
$f.StartPosition = 'CenterScreen' 
$f.Opacity = 1.0
$f.TopMost = $false

$f.Font = 'Microsoft Sans Serif,10'
$f.MaximizeBox = $false
#  neutral
$iconBase64 = 'iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAC+UlEQVR42nVTW0hUURRd59x5OflitKFifKQflTpWpmJjidqDMqyP6OVHUB8WalFC9BEkFBVESaGW+FFQaBRUEKVBNMSYY9YUouhUSg5jmelgcnOa173ndueKlmULDhw2e6+z1t77EPwF08Z95uRtB8oN6WuLKaXJIIAkwfV9oNc69KCuafhZc++f+WTmEmFM1GafbqlNSTMfTtAJlMkxiVMp1RBCSqLLr2HDH/saHWfKqn2jrsAsgc6YoLVcsbXmJMYW+zkthnwqzIdUXRAs4Efv6JS181hhie+bK6AQ5Na2NxTmZFaMiDpMBGdFIUo9LWBK+E2yUMMQzbx43fPhWtcxSyUxFuwxbzx1sztGR+ngFIWGA/amUmwxUcRpp8nG/RLahhnufWIIyd5WRIXg5iXWeenQKpJR87SudFNBld2jgooCly0cMg0UhPxr4d04w4lOEaKsKjc2gKe2rnqSe5vvW5+gTrOPq7BjKcHJNRz+h7Cd829FPHFJyIsLoOMr+onlTsBrNkh6h4dDTR5FaQrFRYcIg46AyQV8UEJiFEGvR8JZWd3DQYZzrxksxhDs39Q/Sc6toDfDIOgd42pUraYoX0kx/ENSeqFMUPYcoQoTAUtjCK53M+WsWxzCy1GZYFkD31eQSNNsI1osigQe7eQQrSXzWpiUm7n9vogxL1BoEvDCLfaTJUfb6rYW5VdZv0QoSRkLgbrNHJJi55IMTEg4/kxEv0fpBjYlCXj8vKOe6LN2mbOONHfrmJ86eb2yWRc2ELgnAZ8QTgWcsv83I1B6EkaOMYDPPg1zNu5fpTwTf9TWUJSXVdE3BniCOnDyOEU2/yRMC4JINhDYXr275rmaX6kQ0BiT1lDZ3rpueXyxhw+hn4+eb4jIivNCr9fA/n7MOtFQUMIm3YFZo2GSqLKW2qT0vMOpep4KjCgqiLxRYUUaFYGTj2SfnV2NP1rKqsPFc37jDDTm3WZN9sFyVVJ+MVVHJIdjLORzCW67Nei40RTsuTvnO/8Cl+E5Sa72joAAAAAASUVORK5CYII=' 
# thumb
$iconBase64 = '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'
# wrench 
$iconBase64 = 'iVBORw0KGgoAAAANSUhEUgAAAD8AAABBCAYAAABmd3xuAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAUISURBVHhe7VltcxNVFPbP+QKIKKAItLS8VBheHWR8GTqjDgj1AzBUHVCcQSrOCO0HLTgqfuITH5RhwAGl1pLdvJGwSdskbbK7SR7PububtMvNDG13Y7e5Z+aZbdLkJs+5z3nuOZvn0MGhyHdqKPKdGoq8LGKbnl9RML464zJrhiIvC9kCUYYi7wtFXhayBaIMRd4XirwsZAtEGYq8LxR5WcgWiDLaT/7NF1y8iNhmAl8bzxFk7wkJ7SW/ZRX0HesRf38v4gP9SF84i+TgSegfHUH87e3Qtq11E9CeJLSVvLbjNRhXvkE5qcOczMOeKcEqTMPM51B4cA/pi19AP7ANWteatqigvTvfvQbp08dQ1h+hZlZQr1absC3YpSKm/7iF5IeHEdu6OvQEhE9+bi1TjWs9ryB95hhmx/9GzbZRr9VQr9cd0N8100Tx7m0kjh6gBKx6er0AES55Iq1RjWvdLzcTwNeu1Uif+hizj8YpAVaTvAtWxdStm9D6Xm++LwSER55dnGo8f+M6MpfOQ+tdR4b3kvM/VsD2V50ETPxDkicFEGkOkYBaFfb0FBKnj1PiqP79aweE8Mizs/cfROVxCuaTDDJDX0LfvclJAO8mH3PsAedOwSKi8+TPu18pI//bT9DeekO+fgAIjzwRSw2egD2ZEztrkbsbw0PQ9mwm4pQA3v2d65H97mvY5PhMnqORAMtCaewB9CN98vUDQGjkuc5Tn38Ke2rSIUPkzEyaEvAt9L1bBLJD51FJJcjtHdnPA3kBewKrR7Z+EAiRPO382U9o5/PivV4CLEqGMXoF2auXYBpZUd9zg18nrkR+ZmIM+gf7pOsHgdBr3sw+nr+jdKbbxQJhGjU+33213oBlonDnd2gHe+TrB4DwyHNNk8GV/ryDGtWvR8oL/2MvvOe55rnhiR/e2TRJ2ecsAeGRZ1CTItw8Z7Te4VagcqhSx5e/NiL8oXFMBohwybu7b4wO07lNxuee588M9gjyhdzoVcSp5+dSClIB4ZJnULOjH+pF/pcfxXEn+nhWwbMqgV7HHpGjBGp9G51jMqAEhE+exlONhhR931ZkaGqbGX8o5Mw9vEgAqYE9QTx2jzyOuVd+XZUMMvfrNafpYQVIP2thaAN5F7RbPNTE392DFM3xkzdvoHj/Lgr3bsP4+Qdkvr+IcjzmmKOrCo7G1fWAHHlAfH93ICbYPvIM/rLc1tIX5xsXPN9zj8/zu5j2Bk+irE1QAkgFvON+8DFJ3SB7gL6/a8km2F7yfoidc8HK2LUBaeoKedgRCZD5Aj1nGU9IAcOilJZSAv8veT84GaQIVkAlnXTG3RYJYAUY10fEaSLUtIgSWF7kGawAGn8TA/2o0CzQqgS4OxR9gDf5LUIBy488gxNACuDZoBz7t2GCHPOSIBQw5fQB7AGsANl6LbA8yTO8EvhsALPCBJ++4+MkwDHB7OULiJFpLkT+y5c8gxWwayOZ4ICrAIkJ8gkwU4IxcpnIr1tB5BmeAgZPCBMULbKXALrWymUUafpL9h9aQbKfC1ZAz1oywaOUgISY+Ii9uNFZenhf/AiymHt90SDP4AT0Oo3Q7NhfsPKG2PHGLe4FyN1DdMgz3BJIHn9PzAnxd/rcLm/hxBnRIs/gBNCgJH7XW+KIGz3yAUKR94UiLwvZAlGGIu8LRV4WsgWiDEXeF4q8LGQLRBmKvC9aku+EUOQ7NRT5To0OJg/8B2e5AyQxI1LwAAAAAElFTkSuQmCC'
# facepalm
$iconBytes = [Convert]::FromBase64String($iconBase64)
$stream = New-Object IO.MemoryStream($iconBytes, 0, $iconBytes.Length)
$stream.Write($iconBytes, 0, $iconBytes.Length);
$iconImage = [System.Drawing.Image]::FromStream($stream, $true)
$f.Icon = [System.Drawing.Icon]::FromHandle((New-Object System.Drawing.Bitmap -Argument $stream).GetHIcon())

[void]$f.ShowDialog()
$f.Dispose()
