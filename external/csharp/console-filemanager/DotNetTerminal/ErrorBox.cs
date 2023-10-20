using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTerminal
{
    class ErrorBox : Box
    {
        public bool Running { get; set; }

        public string Text { get; set; }

        public ErrorBox(Application app) : this(app, app.Width - 10)
        { }

        public ErrorBox(Application app, int width)
            : base(app, "Error")
        {
            Width = width;
            Height = 8;
            X = app.Width / 2 - Width / 2;
            Y = app.Height / 2 - Height / 2;

            backgroundColor = ConsoleColor.DarkRed;
            borderColor = ConsoleColor.White;
        }

        int calc_lines_of_text()
        {
            int lines = 0;
            var chars_in_line = Width - 6;
            foreach (var line_text in Text.Split('\n'))
                for (int i = 0; i < line_text.Length; i += chars_in_line)
                    drawLine(line_text.Substring(i, Math.Min(chars_in_line, line_text.Length - i)), lines++);
            return lines;
        }

        public void run(string text)
        {
            Console.CursorVisible = false;
            
            Text = text;

            Height = Math.Min(calc_lines_of_text() + 6, app.Height - 4);

            Running = true;

            draw();
            while (Running)
            {
                var key_info = app.readKey();
                var key = key_info.Key;

                if (key == ConsoleKey.Escape) break;

                switch (key)
                {
                    case ConsoleKey.Escape:
                        Running = false;
                        break;
                    case ConsoleKey.Enter:
                        Running = false;
                        break;
                }

            }
            app.DrawPanels();
            Console.CursorVisible = true;
        }

        void drawLine(string text, int top)
        {
            SetCursorPosition(Width / 2 - text.Length / 2, top + 2);
            Console.Write(text);
        }

        void drawText()
        {
            lock (app.locker)
            {
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = borderColor;

                var lines = Text.Split('\n');
                int line = 0;
                int chars_in_line = Width - 6;
                foreach (var line_text in lines)
                {
                    if (line > Height - 7) break;

                    for (int i = 0; i < line_text.Length; i += chars_in_line)
                        drawLine(line_text.Substring(i, Math.Min(chars_in_line, line_text.Length - i)), line++);
                }
            }
        }

        override public void draw()
        {
            base.draw();

            drawText();
        }
    }
}
