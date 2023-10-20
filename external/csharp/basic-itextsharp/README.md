### Info 

replica of [OpenHtmlToPdf ](https://github.com/vilppu/OpenHtmlToPdf) .NET library for rendering HTML documents to PDF format. 

OpenHtmlToPdf uses [WkHtmlToPdf](http://github.com/antialize/wkhtmltopdf) native Windows library for HTML to PDF rendering.

### Usage

* accept default settings
```csharp
	const string html =
		"<!DOCTYPE html>" +
		"<html>" +
		"<head><meta charset='UTF-8'><title>Title</title></head>" +
		"<body>Body text...</body>" +
		"</html>";

	var pdf = Pdf
		.From(html)
		.Content();
```

### Defining *fluent* settings
```c#
	const string html =
		"<!DOCTYPE html>" +
		"<html>" +
		"<head><meta charset='UTF-8'><title>Title</title></head>" +
		"<body>Body text...</body>" +
		"</html>";

	var pdf = Pdf
		.From(html)
		.OfSize(PaperSize.A4)
		.WithTitle("Title")
		.WithoutOutline()
		.WithMargins(1.25.Centimeters())
		.Portrait()
		.Comressed()
		.Content();
```
* settings directly
```c#

	const string html =
		"<!DOCTYPE html>" +
		"<html>" +
		"<head><meta charset='UTF-8'><title>Title</title></head>" +
		"<body>Body text...</body>" +
		"</html>";

	var pdf = Pdf
		.From(html)
		.WithGlobalSetting("orientation", "Landscape")
		.WithObjectSetting("web.defaultEncoding", "utf-8")
		.Content();
```
### See Also
* API [documentation](http://wkhtmltopdf.org/libwkhtmltox/pagesettings.html) of Settins
