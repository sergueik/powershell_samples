using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTerminal
{
    abstract class Box
    {
        protected Application app;

        public int Width { get; set; }
        public int Height { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public string title;

        public ConsoleColor backgroundColor = ConsoleColor.DarkCyan;
        public ConsoleColor borderColor = ConsoleColor.White;

        public Box(Application app, string title)
        {
            this.app = app;
            this.title = title;

            X = 0;
            Y = 0;
            Width = 24;
            Height = 12;
        }

        public void drawBackground()
        {
            lock (app.locker)
            {
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = backgroundColor;

                for (int i = 0; i < Width; ++i)
                    for (int j = 0; j < Height; ++j)
                    {
                        SetCursorPosition(i, j);
                        Console.Write(" ");
                    }
            }
        }

        public void drawBorders()
        {
            lock (app.locker)
            {
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = borderColor;

                SetCursorPosition(1, 1);
                Console.Write("╔");
                SetCursorPosition(Width - 2, 1);
                Console.Write("╗");

                SetCursorPosition(1, Height - 2);
                Console.Write("╚");
                SetCursorPosition(Width - 2, Height - 2);
                Console.Write("╝");

                for (int i = 2; i < Width - 2; ++i)
                {
                    SetCursorPosition(i, 1);
                    Console.Write("═");
                    SetCursorPosition(i, Height - 2);
                    Console.Write("═");
                }

                for (int i = 2; i < Height - 2; ++i)
                {
                    SetCursorPosition(1, i);
                    Console.Write("║");
                    SetCursorPosition(Width - 2, i);
                    Console.Write("║");
                }
            }
        }

        public void drawTitle()
        { 
            lock(app.locker)
            {
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = borderColor;
                var text = " " + title + " ";
                SetCursorPosition(Width / 2 - text.Length/2, 1);
                Console.Write(text);
            }
        }

        virtual public void draw()
        {
            drawBackground();
            drawBorders();
            drawTitle();
        }

        protected void SetCursorPosition(int left, int top)
        {
            Console.SetCursorPosition(left+X, top+Y);
        }
    }
}
