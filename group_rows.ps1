# origin: https://qna.habr.com/q/1233372
$csv = Import-Csv ....
foreach ($group in ($csv | Group-Object -Property ComputerName)) {
  $computerName = $group.Name
  $data = $group.Group # Оригинальные объекты.  Тут всегда массив, даже если из одного элемента
  #....
}
