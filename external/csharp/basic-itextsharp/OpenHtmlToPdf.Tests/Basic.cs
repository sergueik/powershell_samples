using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text;
public class Basic
{
	public static void Main(string[] args)
	{           
		Document document = new Document();
            
		string fileName = "test.pdf";
		string filePath = Path.Combine(@"c:\\temp", fileName);

		try {
               
			PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));        
               
			document.Open();
                
			iTextSharp.text.Font font = FontFactory.GetFont("c:/Windows/Fonts/Arial.ttf", "cp1251", BaseFont.EMBEDDED, 10);
                
			document.Add(new Paragraph("Привет мир", font));
		} catch (Exception ex) {
			Console.Error.WriteLine("Ошибка при сохранении в PDF: " + ex.Message);
		} finally {                
			document.Close();
		}

		Console.Error.WriteLine("Сохранено в PDF: " + filePath);
	}
}