#Copyright (c) 2025 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the 'Software'), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.
# see also: https://qna.habr.com/q/1398934
$shell = New-Object -com shell.application
$rb = $shell.Namespace(10)
($rb.items() | select-object -first 1).Path
C:\$Recycle.Bin\S-1-5-21-3826591462-1902725790-3394240593-1001\$RXMK2R1

new-item "a.txt"
 $rb.CopyHere("a.txt", 0)

($rb.items()).count
# 2
new-item "a2.txt"
$rb.CopyHere("a2.txt", 0)
($rb.items()).count

# 3
# origin: http://forum.oszone.net/thread-355724.html
# (New-Object -ComObject Shell.Application).NameSpace(0x0a).Items() | ? {$($_.ExtendedProperty('{9B174B33-40FF-11D2-A27E-00C04FC30871} 2'))+'\'+$($_.Name) -like $file} | Remove-Item

