# origin http://forum.oszone.net/thread-342008-2.html
[Reflection.Assembly]::LoadWithPartialName('System.Drawing') | Out-Null

Start-Sleep -Milliseconds 250

$oRectangle = ([System.Windows.Forms.Screen]::AllScreens)[0].Bounds
$oBitmap    = [System.Drawing.Bitmap]::new($oRectangle.Width, $oRectangle.Height)
$oGraphics  = [System.Drawing.Graphics]::FromImage($oBitmap)
$oGraphics.CopyFromScreen($oRectangle.Location, [System.Drawing.Point]::Empty, $oRectangle.Size)
$oMemoryStream = [System.IO.MemoryStream]::new()
$oBitmap.Save($oMemoryStream, [System.Drawing.Imaging.ImageFormat]::Png)
$oMemoryStream.Seek(0, [System.IO.SeekOrigin]::Begin) | Out-Null

$oMailMessage = New-Object -TypeName 'Net.Mail.MailMessage'
$oMailMessage.From = 'sender_user'
$oMailMessage.To.Add('recipient_email') 
$oMailMessage.SubjectEncoding = [System.Text.Encoding]::Unicode 
$oMailMessage.Subject = "Screenshot at $(Get-Date)"
$oMailMessage.IsBodyHTML      = $true  
$oMailMessage.BodyEncoding    = [System.Text.Encoding]::Unicode
$oMailMessage.Body = "<h2>Hello World!</h2></br>Screenshot at $(Get-Date) here."

$oContentType = New-Object -TypeName 'Net.Mime.ContentType' -Property @{
    MediaType = "image/png"
    Name      = "Screenshot at $(Get-Date).png"
}
$oAttachment = New-Object -TypeName 'Net.Mail.Attachment' -ArgumentList $oMemoryStream, $oContentType
$oMailMessage.Attachments.Add($oAttachment)

$oSmtpClient = New-Object -TypeName 'Net.Mail.SmtpClient' -ArgumentList 'smtp.yandex.ru', 587 #465
$oSmtpClient.EnableSsl = $true
$oSmtpClient.Credentials = New-Object -TypeName 'System.Net.NetworkCredential' -ArgumentList 'sender_user', 'password'
$oSmtpClient.Send($oMailMessage)

$oMemoryStream.Close()
$oMemoryStream.Dispose()
$oGraphics.Dispose()
$oBitmap.Dispose()
$oAttachment.Dispose()
$oMailMessage.Dispose()
$oSmtpClient.Dispose()
