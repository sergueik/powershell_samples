using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using OpenHtmlToPdf.Tests.Helpers;

namespace OpenHtmlToPdf.Tests
{
    [TestFixture]
    public class HtmlToPdfConversion
    {
        private const string HtmlDocumentFormat = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Title</title></head><body>{0}</body></html>";
        private const int _210mmInPostScriptPoints = 595;
        private const int _297mmInPostScriptPoints = 842;

        [Test]
        public void Pdf_document_content()
        {
            const string expectedDocumentContent = "Expected document content";
            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var result = Pdf.From(html).Content();

            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(result));
        }

        [Test]
        public void Text_encoding()
        {
            const string expectedDocumentContent = "Строка";
            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var result = Pdf.From(html).EncodedWith("utf-8").Content();

            TextAssert.
            	AreEqual(expectedDocumentContent.ToLower(), PdfDocumentReader.ToText(result));
        }

        [Test]
        public void style()
        {
        	const string text = "text";
        	// NOTE : cannot use 
            string expectedDocumentContent = string.Format(
        		"{0}{1}", @"<style> 
body *{ padding: 0; margin: 0; }
</style>", text);
            
            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var result = Pdf.From(html).EncodedWith("utf-8").Content();

            TextAssert.
            	AreEqual(text, PdfDocumentReader.ToText(result));
        }
        
        [Test]
        public void Table_With_Style()
        {
        	string text = "A complex table";
        	var html = @"
        	<!doctype html>
<html>
<head>
<meta charset=""UTF-8"">
<style>
table{
  margin: 50px 0;
  text-align: left;
  border-collapse: separate;
  border: 1px solid #ddd;
  border-spacing: 10px;
  border-radius: 3px;
  background: #fdfdfd;
  font-size: 14px;
  width: auto;
}

td,th{
  border: 1px solid #ddd;
  padding: 5px;
  border-radius: 3px;
}
th{
  background: #E4E4E4;
}
caption{
  font-style: italic;
  text-align: right;
  color: #547901;
}

table, th, td {
  border: 1px solid black;
}
table {
  width: 100%;
  border-collapse: collapse;
}
</style>
</head>
<body>
<table>
  <caption>A complex table
</caption>
  <thead>
    <tr>
      <th colspan=""3"">Invoice #123456789
</th>
      <th>14 January 2025 
</th>
    </tr>
    <tr>
      <td colspan=""2""><strong>Pay to:
</strong><br/> Acme Billing Co.<br/> 123 Main St.<br/> Cityville, NA 12345 
</td>
      <td colspan=""2""><strong>Customer:
</strong><br/> John Smith<br/> 321 Willow Way<br/> Southeast Northwestershire, MA 54321 
</td>
    </tr>
  </thead>
  <tbody>
    <tr>
      <th>Name / Description
</th>
      <th>Qty.
</th>
      <th>@
</th>
      <th>Cost
</th>
    </tr>
    <tr>
      <td>Paperclips
</td>
      <td>1000
</td>
      <td>0.01
</td>
      <td>10.00
</td>
    </tr>
    <tr>
      <td>Staples (box)
</td>
      <td>100
</td>
      <td>1.00
</td>
      <td>100.00
</td>
    </tr>
  </tbody>
  <tfoot>
    <tr>
      <th colspan=""3"">Subtotal
</th>
      <td> 110.00
</td>
    </tr>
    <tr>
      <th colspan=""2"">Tax
</th>
      <td> 8% 
</td>
      <td>8.80
</td>
    </tr>
    <tr>
      <th colspan=""3"">Grand Total
</th>
      <td>$ 118.80
</td>
    </tr>
  </tfoot>
</table>
</body>
</html>
";
        		            var result = Pdf.From(html).EncodedWith("utf-8").Content();

            TextAssert.Contains(text, PdfDocumentReader.ToText(result));


        }
        [Test]
        public void HabraFont()
        {
        	string text = "Строка";
        	var html = @"
<!doctype html>
<html>
<head>
<meta charset=""UTF-8"">
<style>
@font-face {
  font-family: ""HabraFont"";
  src: url(file:///C:/windows/fonts/Montserrat-Medium.ttf); 
  -fs-pdf-font-embed: embed;
  -fs-pdf-font-encoding: Identity-H;
}

@page {
  margin: 0px;
  padding: 0px;
  size: A4 portrait;
}

@media print {
  .new_page {
    page-break-after: always;
  }
}

body *{
  padding: 0;
  margin: 0;
}

* {
  font-family: ""HabraFont"";
}
</style>
</head>
<body>
Строка
</body>
</html>

        	";
	            var result = Pdf.From(html).EncodedWith("utf-8").Content();

            TextAssert.Contains(text, PdfDocumentReader.ToText(result));

        }

        
        [Test]
        public void Document_title()
        {
            const string expectedTitle = "Expected title";
            const string expectedDocumentContent = "Expected document content";
            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var result = Pdf.From(html).WithTitle(expectedTitle).Content();

            Assert.AreEqual(expectedTitle, PdfDocumentReader.Title(result));
            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(result));
        }

        [Test]
        public void Page_size()
        {
            const string expectedDocumentContent = "Expected document content";
            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var result = Pdf.From(html).OfSize(PaperSize.A4).Content();

            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(result));
            Assert.AreEqual(_210mmInPostScriptPoints, PdfDocumentReader.WidthOfFirstPage(result));
            Assert.AreEqual(_297mmInPostScriptPoints, PdfDocumentReader.HeightOfFirstPage(result));
        }

        [Test]
        public void Portrait()
        {
            const string expectedDocumentContent = "Expected document content";

            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var result = Pdf.From(html).Portrait().Content();

            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(result));
            Assert.AreEqual(_210mmInPostScriptPoints, PdfDocumentReader.WidthOfFirstPage(result));
            Assert.AreEqual(_297mmInPostScriptPoints, PdfDocumentReader.HeightOfFirstPage(result));
        }

        [Test]
        public void Landscape()
        {
            const string expectedDocumentContent = "Expected document content";

            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var result = Pdf.From(html).Landscape().Content();

            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(result));
            Assert.AreEqual(_297mmInPostScriptPoints, PdfDocumentReader.WidthOfFirstPage(result));
            Assert.AreEqual(_210mmInPostScriptPoints, PdfDocumentReader.HeightOfFirstPage(result));
        }

        [Test]
        public void Margins()
        {
            const string expectedDocumentContent = "Expected document content";

            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var result = Pdf.From(html).WithMargins(1.25.Centimeters()).Content();

            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(result));
        }

        [Test]
        public void With_outline()
        {
            const string expectedDocumentContent = "Expected document content";

            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var result = Pdf.From(html).WithOutline().Content();

            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(result));
        }

        [Test]
        public void Without_outline()
        {
            const string expectedDocumentContent = "Expected document content";

            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var result = Pdf.From(html).WithoutOutline().Content();

            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(result));
        }

        [Test]
        public void Compressed()
        {
            const string expectedDocumentContent = "Expected document content";

            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var result = Pdf.From(html).Comressed().Content();

            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(result));
        }

        [Test]
        public void Is_directory_agnostic()
        {
            const string expectedDocumentContent = "Expected document content";
            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            Directory.SetCurrentDirectory(@"c:\");
            var result = Pdf.From(html).Content();

            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(result));
        }

        [Test]
        public void Convert_multiple_documents_concurrently()
        {
            const string expectedDocumentContent = "Expected document content";
            const int documentCount = 10;
            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);
            var tasks = new List<Task<byte[]>>();

            for (var i = 0; i < documentCount; i++)
                tasks.Add(Task.Run(() => Pdf.From(html).Content()));

            Task.WaitAll(tasks.OfType<Task>().ToArray());

            foreach (var task in tasks)
                TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(task.Result));
        }

        [Test]
        public void Convert_multiple_documents_sequently()
        {
            const string expectedDocumentContent = "Expected document content";
            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            var first = Pdf.From(html).Content();
            var second = Pdf.From(html).Content();
            var third = Pdf.From(html).Content();

            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(first));
            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(second));
            TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(third));
        }

        [Test]
        [Ignore("there are actually leftover temp files")]
        public void No_temporary_files_are_left_behind()
        {
            const string expectedDocumentContent = "Expected document content";
            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);

            Pdf.From(html).Content();

            Assert.AreEqual(0, Directory.GetFiles(Path.Combine(Path.GetTempPath(), "OpenHtmlToPdf"), "*.pdf").Count());
        }

        [Test]
        public void Convert_massive_number_of_documents()
        {
            const string expectedDocumentContent = "Expected document content";
            const int documentCount = 100;
            var html = string.Format(HtmlDocumentFormat, expectedDocumentContent);
            var tasks = new List<Task<byte[]>>();

            for (var i = 0; i < documentCount; i++)
                tasks.Add(Task.Run(() => Pdf.From(html).Content()));

            Task.WaitAll(tasks.OfType<Task>().ToArray());

            foreach (var task in tasks)
                TextAssert.AreEqual(expectedDocumentContent, PdfDocumentReader.ToText(task.Result));
        }
    }
}