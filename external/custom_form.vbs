' origin: http://forum.script-coding.com/viewtopic.php?pid=120959#p120959
Option Explicit

' Base64-кодированный фоновый рисунок
Const BGI = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAWIAAAB2CAYAAADybJlDAAAACXBIWXMAAC4jAAAuIwF4pT92AAAAIGNIUk0AAHolAACAgwAA+f8AAIDpAAB1MAAA6mAAADqYAAAXb5JfxUYAAAUjSURBVHja7N05ztxGEIBRjqHciZ34Bn3/w9QNnPsE49QwtPwc9lLd9V4kAdKwyQE+Fjnb6/1+XwCs85tDACDEAEIMgBADCDEAQgwgxAAIMYAQAyDEAEIMgBADCDEAQgwgxACM980hgD28Xq/Td7Hd/PdxzHPri+FBiBPH9hPbBVqIQYh3D+/2YRZiEOIT47tVmIUYhLhCfFMHWYhBiKsFOF2UhRiEuHKAUwRZiEGIBXhxkIUYhFiAFwdZiEGIBXhxjIUYhDhLhCP5OocFWYhBiGeHLaxdiEGI54YsNljj0n0SYhDiEYGLhGtKG2MhBiHOFuCdXiDsEmMhhtoh7hW9SLKOLWMsxFA3xD3iVznA3Y6FEEPNED8NoAB3PC5CDPVCvDLCFT4gcvv4CDHUCvGTEFYKcMw8Vn48FOpYEeG28RQ87cRjIoYaE/GnMdxpCo7O2336eH9f1/WPEIMQr4jwyo8Zt4HbaqOOoRCDEGePcCzc9pQYCzGcHeJZER45ia6ewmP0MfViHZxrxwjHle9XltvoYyTEIMIZIvwkwDNvvfT6/0IMpInw0wn4SYTj4Xa63T4RYjANr4hwj1sQKybhIc+NEIMIz4xwr3vAvSI8eyo2EQNLAphhGo1Ej92EGESyW0AWhbD3fkaHbT86lkIMNc2KcO+3ox3500lCDPWm4ZkRXjnx31lDTDgeP3wcIYZaEZ712Bk+lBG7bFOIoZbYdBsz3g0SC/a/CTGw6lZAlok/xZqEGGrFsg163FW3AnqvY8lULMQg3pkj3JKsYyghhjrT8KhL66oRbr3WI8Qg5OUm0Gz7JMQgxhmDdcoJ4UvrEmKoF9AfPVbbdN+2n8qFGNg5fkfcGhFiIFP82mb72Ho8lhDD+dOrqdxEDCyc6mLAY54+lQsxkDrspvwBx06IQUDtj4kYGDA5xnf+Hpvv07GEGEzHd4LZO5qVXqALIQZ6xaxtuu602xNiYNV03A46FkIM7BuhpNPwVEIM50VvxddBjvqC+hKEGAQ9++PG6U+IEAOzp+M7wS7xHmghBnaajk3EgEA+nI4rT8NCDNy+hO4VQtPxL3xzCIAvTKWz3zOcYRr+a9ZaTcSA2wTf9/usDQkxcCfGMWk7pQgx1PGHUAoxsNafHUM7ajouGXkhBjKFc8RXbQoxUCLGgizEgCB33+60KwIhBgR5ceB9oAPYNWTHvLBnIoa9Js2el9UnRPmIfRViYNol+CZBnh53tyaAE+O/1ScAhRgQ5cWEGBgZwa9OlW3SeqLj2rsdF/eIQQxXif/9OSYdlx73lLuuVYhhL9kvs3sELiau9b9RXnbyEmIg6wln5kln6U84CTGcK/Ptibjx72ZHefpz4cU6YJfwx6Tt3NnPLicIIYY9L9vb5us/Icg/W8OtfRRiOH9KPfXL1mPiND706kCIgenhKRblX/JiHdS5vM/wpUGzPnq81Qt8Qgym1dNPWOmjLMRQayquHPq0URZiMBVXO3GYiIEtYnjy7Yxs+xZCDDXCOipAuwW7fXB8h0/y3r4GZDthZIrwz9bfeh0XIYYzIvfVKNyJx0kfBmmDjnuXbbk1AZzukwjHhG2FEINL/4xTZpUIm4hBjKfGdtXtjZZ4WyHEwOk+jfCSk4YQg6k402S5Y4Tb0+0IMTAyPpF0XWkiLMRgKj5lKn7yy8yx+pgIMYhxlel8dYRDiEGMT5uK23VAhIUYqDgFp4rwdV3X6/1+e0phA6/Xa7dJ9rT3NQ+7B/0vAAAA//8DAERsQ7O6796eAAAAAElFTkSuQmCC"

Dim aItems, i

' Массив, содержащий пункты для списка
aItems = Array("Пункт A", "Пункт B", "Пункт C", "Пункт D", "Пункт E")

' Создание обертки HTA окна
With New clsSmallWrapperForm
	' Настройка окна
	.ShowInTaskbar = "yes"
	.Title = "Тест формы HTA"
	.BackgroundImage = BGI
	.Width = 354
	.Height = 118
	.Visible = False
	' Создание окна
	.Create
	' Назанчение обработчиков
	Set .Handlers = New clsSmallWrapperHandlers
	' Добавление списка
	With .AddElement("ListBox1", "SELECT")
		.size = 6
		.multiple = True
		.style.left = "15px"
		.style.top = "10px"
		.style.width = "250px"
	End With
	.AppendTo "Form"
	' Добавление пунктов в список
	For i = 0 To UBound(aItems)
		.AddElement , "OPTION"
		.AddText aItems(i)
		.AppendTo "ListBox1"
	Next
	' Добавление кнопки OK
	With .AddElement("Button1", "INPUT")
		.type = "button"
		.value = "OK"
		.style.left = "285px"
		.style.top = "10px"
		.style.width = "50px"
		.style.height = "20px"
	End With
	.AppendTo "Form"
	' Добавление кнопки Отмена
	With .AddElement("Button2", "INPUT")
		.type = "button"
		.value = "Отмена"
		.style.left = "285px"
		.style.top = "40px"
		.style.width = "50px"
		.style.height = "20px"
	End With
	.AppendTo "Form"
	' Добавление надписи
	With .AddElement("Label1", "SPAN")
		.style.left = "15px"
		.style.top = "98px"
		.style.width = "350px"
	End With
	.AddText "Выберите пункты"
	.AppendTo "Form"
	' Показать окно
	.Visible = True
	' Ожидание закрытия окна или выбора пунктов пользователем
	Do While .ChkDoc And Not .Handlers.Selected
		WScript.Sleep 100
	Loop
	' Получение результатов из массива .Handlers.SelectedItems
	If .Handlers.Selected Then
		MsgBox "Выбрано " & (UBound(.Handlers.SelectedItems) + 1) & " пункт(ов)" & vbCrLf & Join(.Handlers.SelectedItems, vbCrLf)
	Else
		MsgBox "Окно закрыто"
	End If
	' Остальная часть кода ...
	
End With

Class clsSmallWrapperHandlers
	
	' Класс обработчиков реализует обработку событий
	' Отредактируйте код для обеспечения требуемого поведения
	' Сохраняйте общепринятые для VB имена обработчиков: Public Sub <ID элемента>_<Название события>()
	
	Public oswForm ' обязательное свойство
	
	Public Selected
	Public SelectedItems
	
	Private Sub Class_Initialize()
		Selected = False
		SelectedItems = Array()
	End Sub
	
	Public Sub ListBox1_Click()
		Dim vItem
		With CreateObject("Scripting.Dictionary")
			For Each vItem In oswForm.Window.ListBox1.childNodes
				If vItem.Selected Then .Item(vItem.innerText) = ""
			Next
			SelectedItems = .Keys()
		End With
		oswForm.Window.Label1.style.color = "buttontext"
		oswForm.Window.Label1.innerText = (UBound(SelectedItems) + 1) & " выбрано"
	End Sub
	
	Public Sub Button1_Click()
		Selected = UBound(SelectedItems) >= 0
		If Selected Then
			oswForm.Window.close
		Else
			oswForm.Window.Label1.style.color = "darkred"
			oswForm.Window.Label1.innerText = "Выберите хотя бы 1 пункт"
		End If
	End Sub
	
	Public Sub Button2_Click()
		oswForm.Window.close
	End Sub
	
End Class

Class clsSmallWrapperForm
	
	' Служебный класс для функциональности HTA окна
	' Не подлежит изменению
	
	' Аттрибуты тэга HTA
	Public Border ' thick | dialog | none | thin
	Public BorderStyle ' normal | complex | raised | static | sunken
	Public Caption ' yes | no
	Public ContextMenu ' yes | no
	Public Icon ' path
	Public InnerBorder ' yes | no
	Public MinimizeButton ' yes | no
	Public MaximizeButton ' yes | no
	Public Scroll ' yes | no | auto
	Public Selection ' yes | no
	Public ShowInTaskbar ' yes | no
	Public SysMenu ' yes | no
	Public WindowState ' normal | minimize | maximize
	
	' Свойства формы
	Public Title
	Public BackgroundImage
	Public Width
	Public Height
	Public Left
	Public Top
	Public Self
	
	Dim oWnd
	Dim oDoc
	Dim bVisible
	Dim oswHandlers
	Dim oLastCreated
	
	Private Sub Class_Initialize()
		Set Self = Me
		Set oswHandlers = Nothing
		Border = "thin"
		ContextMenu = "no"
		InnerBorder = "no"
		MaximizeButton = "no"
		Scroll = "no"
		Selection = "no"
	End Sub
	
	Private Sub Class_Terminate()
		On Error Resume Next
		oWnd.Close
	End Sub
	
	Public Sub Create()
		Dim sName, sAttrs, sSignature, oShellWnd, oProc
		sAttrs = ""
		For Each sName In Array("Border", "Caption", "ContextMenu", "MaximizeButton", "Scroll", "Selection", "ShowInTaskbar", "Icon", "InnerBorder", "BorderStyle", "SysMenu", "WindowState", "MinimizeButton")
			If Eval(sName) <> "" Then sAttrs = sAttrs & " " & sName & "=" & Eval(sName)
		Next
		If Len(sAttrs) >= 240 Then Err.Raise 450, "<HTA:APPLICATION" & sAttrs & " />"
		sSignature = Mid(Replace(CreateObject("Scriptlet.TypeLib").Guid, "-", ""), 2, 16)
		Set oProc = CreateObject("WScript.Shell").Exec("mshta ""about:<script>moveTo(-32000,-32000);document.title='*'</script><hta:application" & sAttrs & " /><object id='s' classid='clsid:8856F961-340A-11D0-A96B-00C04FD705A2'><param name=RegisterAsBrowser value=1></object><script>s.putProperty('" & sSignature & "',document.parentWindow);</script>""")
		Do
			If oProc.Status > 0 Then Err.Raise 507, "mshta.exe"
			For Each oShellWnd In CreateObject("Shell.Application").Windows
				On Error Resume Next
				Set oWnd = oShellWnd.GetProperty(sSignature)
				If Err.Number = 0 Then
					On Error Goto 0
					With oWnd
						Set oDoc = .document
						With .document
							.open
							.close
							.title = Title
							.getElementsByTagName("head")(0).appendChild .createElement("style")
							.styleSheets(0).cssText = "* {font:8pt tahoma;position:absolute;}"
							.getElementsByTagName("body")(0).id = "Form"
						End With
						.Form.style.background = "buttonface"
						If BackgroundImage <> "" Then
							.Form.style.backgroundRepeat = "no-repeat"
							.Form.style.backgroundImage = "url(" & BackgroundImage & ")"
						End If
						If IsEmpty(Width) Then Width = .Form.offsetWidth
						If IsEmpty(Height) Then Height = .Form.offsetHeight
						.resizeTo .screen.availWidth, .screen.availHeight
						.resizeTo Width + .screen.availWidth - .Form.offsetWidth, Height + .screen.availHeight - .Form.offsetHeight
						If IsEmpty(Left) Then Left = CInt((.screen.availWidth - Width) / 2)
						If IsEmpty(Top) Then Top = CInt((.screen.availHeight - Height) / 2)
						bVisible = IsEmpty(bVisible) Or bVisible
						Visible = bVisible
						.execScript "var smallWrapperThunks = (function(){" &_
							"var thunks,elements={};return {" &_
								"parseHandlers:function(h){" &_
									"thunks=h;for(var key in thunks){var p=key.toLowerCase().split('_');if(p.length==2){elements[p[0]]=elements[p[0]]||{};elements[p[0]][p[1]]=key;}}}," &_
								"forwardEvents:function(e){" &_
									"if(elements[e.id.toLowerCase()]){for(var key in e){if(key.search('on')==0){var q=elements[e.id.toLowerCase()][key.slice(2)];if(q){eval(e.id+'.'+key+'=function(){thunks.'+q+'()}')}}}}}}})()"
						If Not oswHandlers Is Nothing Then
							.smallWrapperThunks.parseHandlers oswHandlers
							.smallWrapperThunks.forwardEvents .Form
						End If
					End With
					Exit Sub
				End If
				On Error Goto 0
			Next
			WScript.Sleep 100
		Loop
	End Sub
	
	Public Property Get Handlers()
		Set Handlers = oswHandlers
	End Property
	
	Public Property Set Handlers(oHandlers)
		Dim oElement
		If Not oswHandlers Is Nothing Then Set oswHandlers.oswForm = Nothing
		Set oswHandlers = oHandlers
		Set oswHandlers.oswForm = Me
		If ChkDoc Then
			oWnd.smallWrapperThunks.parseHandlers oswHandlers
			For Each oElement In oDoc.all
				If oElement.id <> "" Then oWnd.smallWrapperThunks.forwardEvents oElement
			Next
		End If
	End Property
	
	Public Sub ForwardEvents(oElement)
		If ChkDoc Then oWnd.smallWrapperThunks.forwardEvents oElement
	End Sub
	
	Public Function AddElement(sId, sTagName)
		Set oLastCreated = oDoc.createElement(sTagName)
		If VarType(sId) <> vbError Then
			If Not(IsNull(sId) Or IsEmpty(sId)) Then oLastCreated.id = sId
		End If
		oLastCreated.style.position = "absolute"
		Set AddElement = oLastCreated
	End Function
	
	Public Function AppendTo(vNode)
		If Not IsObject(vNode) Then Set vNode = oDoc.getElementById(vNode)
		vNode.appendChild oLastCreated
		ForwardEvents oLastCreated
		Set AppendTo = oLastCreated
	End Function
	
	Public Function AddText(sText)
		oLastCreated.appendChild oDoc.createTextNode(sText)
	End Function
	
	Public Property Get Window()
		Set Window = oWnd
	End Property
	
	Public Property Get Document()
		Set Document = oDoc
	End Property
	
	Public Property Get Visible()
		Visible = bVisible
	End Property
	
	Public Property Let Visible(bWindowVisible)
		bVisible = bWindowVisible
		If ChkDoc Then
			If bVisible Then
				oWnd.moveTo Left, Top
			Else
				oWnd.moveTo -32000, -32000
			End If
		End If
	End Property
	
	Public Function ChkDoc()
		On Error Resume Next
		ChkDoc = CBool(TypeName(oDoc) = "HTMLDocument")
	End Function
	
End Class