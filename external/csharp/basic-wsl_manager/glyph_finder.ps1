# https://www.codeproject.com/Articles/29010/WinForm-ImageButton 
# https://www.codeproject.com/Tips/5164771/Faded-Dimmed-Button-Images


param(
  [switch]$pause
)


@( 'System.Drawing','System.Windows.Forms','System.Windows.Forms.VisualStyles') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

Add-Type -TypeDefinition @'

using System;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;

public class GlyphFinder
{
    public static void Main(string[] args)
    {
        string[] glyphs =
        {
//           "\uE174",
//            "\uE102",
//            "\uE103",
//            "\uE184",
//            "\uE179",
//            "\uE107",
            "\u2776"
        };

        using (InstalledFontCollection fonts =
               new InstalledFontCollection())
        {
            Console.WriteLine("Installed fonts: {0}", fonts.Families.Length);
            Console.WriteLine();

            foreach (string text in glyphs)
            {
                int codePoint = Char.ConvertToUtf32(text, 0);

                Console.WriteLine(
                    "U+{0:X4}  '{1}'",
                    codePoint,
                    text);

                foreach (FontFamily family in fonts.Families)
                {
                    if (family.IsStyleAvailable(FontStyle.Regular))
                    {
                        // GetCellAscent/GetEmSize etc. only tell us
                        // that the font is valid, not glyph existence.
                        //
                        // The practical test below renders the character
                        // to a bitmap and compares it with the font's
                        // missing-glyph representation.
                        if (AppearsToRenderGlyph(family, text))
                        {
                            Console.WriteLine(
                                "    {0}",
                                family.Name);
                        }
                    }
                }

                Console.WriteLine();
            }
        }
    }

    static bool AppearsToRenderGlyph(FontFamily family, string text)
    {
        using (Font font = new Font(family, 32f, FontStyle.Regular,
                                    GraphicsUnit.Point))
        using (Bitmap bitmap = new Bitmap(100, 100))
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);
            g.DrawString(text, font, Brushes.Black, 5, 5);

            // This is deliberately a heuristic.
            // It detects whether the rendered result contains
            // something other than an empty glyph area.
            int darkPixels = 0;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);

                    if (c.R < 128 && c.G < 128 && c.B < 128)
                        darkPixels++;
                }
            }
            Console.Error.WriteLine(String.Format("Pixels: {0}",darkPixels ));
            return darkPixels > 20;
        }
    }
}
'@ -ReferencedAssemblies 'System.Drawing.dll','System.Windows.Forms.dll'

$o = New-Object GlyphFinder
[GlyphFinder]::Main(@())
Start-Sleep -Millisecond 100
