# https://www.codeproject.com/Articles/29010/WinForm-ImageButton 
# https://www.codeproject.com/Tips/5164771/Faded-Dimmed-Button-Images


param(
  [switch]$pause
)


@( 'System.Drawing','System.Windows.Forms','System.Windows.Forms.VisualStyles') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

Add-Type -TypeDefinition @'

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

public class GlyphFinder
{
    const int Size = 48;

    static Bitmap Render(Font font, char c)
    {
        Bitmap bitmap = new Bitmap(Size, Size, PixelFormat.Format32bppArgb);

        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);
            g.TextRenderingHint =
                System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            using (Brush brush = new SolidBrush(Color.Black))
            {
                SizeF s = g.MeasureString(c.ToString(), font);

                g.DrawString(
                    c.ToString(),
                    font,
                    brush,
                    (Size - s.Width) / 2,
                    (Size - s.Height) / 2);
            }
        }

        return bitmap;
    }

    static double Difference(Bitmap a, Bitmap b)
    {
        long difference = 0;
        long pixels = 0;

        for (int y = 0; y < a.Height; y++)
        {
            for (int x = 0; x < a.Width; x++)
            {
                Color ca = a.GetPixel(x, y);
                Color cb = b.GetPixel(x, y);

                difference += Math.Abs(ca.R - cb.R);
                difference += Math.Abs(ca.G - cb.G);
                difference += Math.Abs(ca.B - cb.B);

                pixels += 3;
            }
        }

        return (double)difference / pixels;
    }

    static bool HasGlyph(Font font, char character)
    {
        // Deliberately bizarre PUA character.
        // It should produce the font's missing-glyph representation.
        char probe = '\uFBFF';

        using (Bitmap requested = Render(font, character))
        using (Bitmap missing = Render(font, probe))
        {
            double difference = Difference(requested, missing);

            Console.WriteLine(
                "U+{0:X4}: difference = {1:F4}",
                (int)character,
                difference);

            // Threshold is empirical; 0.01 is deliberately conservative.
            return difference > 0.01;
        }
    }

    public static void Main(string[] args)
    {
        string[] fonts =
        {
            "Segoe UI",
            "Segoe Fluent Icons",
            "Segoe UI Symbol"
        };

        char[] characters =
        {
            '\uE174',
            '\uE102',
            '\uE103',
            '\uE184',
            '\uE179',
            '\uE107'
        };

        foreach (string fontName in fonts)
        {
            Console.WriteLine();
            Console.WriteLine("=== {0} ===", fontName);

            using (Font font = new Font(fontName, 19))
            {
                foreach (char c in characters)
                {
                    Console.WriteLine(
                        "  U+{0:X4} -> {1}",
                        (int)c,
                        HasGlyph(font, c));
                }
            }
        }
    }
}
'@ -ReferencedAssemblies 'System.Drawing.dll','System.Windows.Forms.dll'

$o = New-Object GlyphFinder
[GlyphFinder]::Main(@())
Start-Sleep -Millisecond 100
