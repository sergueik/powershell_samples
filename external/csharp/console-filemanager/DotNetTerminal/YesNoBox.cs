using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTerminal
{
    public delegate void BoxKeyPressedEvent(ConsoleKeyInfo key_info);

    class YesNoBox : Box
    {
        public string text;

        public YesNo current;

        public bool Running { get; set; }

        public BoxKeyPressedEvent YesPressed { get; set; }
        public BoxKeyPressedEvent NoPressed { get; set; }

        public YesNoBox(Application app, string title, string text) : base(app, title)
        {
            this.text = text;

            Width = 30;
            Height = 7;
            X = app.Width / 2 - Width / 2;
            Y = app.Height / 2 - Height / 2;

            backgroundColor = ConsoleColor.Gray;
            borderColor = ConsoleColor.Black;
        }

        public void run()
        {
            Console.CursorVisible = false;

            current = YesNo.YES;
            Running = true;

            draw();
            while (Running)
            {
                var key_info = app.readKey();
                var key = key_info.Key;

                if (key == ConsoleKey.Escape) break;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        current = YesNo.YES;
                        drawYesNo();
                        break;
                    case ConsoleKey.RightArrow:
                        current = YesNo.NO;
                        drawYesNo();
                        break;
                    case ConsoleKey.Enter:
                        Running = false;
                        action(key_info);
                        break;
                }

            }
            app.DrawPanels();
            Console.CursorVisible = true;
        }

        void action(ConsoleKeyInfo info)
        {
            if (current == YesNo.YES && YesPressed != null) YesPressed(info);
            if (current == YesNo.NO && NoPressed != null) NoPressed(info);
        }

        void drawText()
        { 
            lock(app.locker){
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = borderColor;

                SetCursorPosition(Width / 2 - text.Length / 2, 2);
                Console.Write(text);

                for (int i = 2; i < Width - 2; ++i)
                {
                    SetCursorPosition(i, 3);
                    Console.Write("─");
                }
            }
        }

        void drawYesNo()
        {
            var yes = "{ Yes }";
            var no = "[ No ]";
            lock (app.locker)
            {

                SetCursorPosition(Width / 2 - (yes.Length+no.Length + 1) / 2, Height - 3);

                if (current == YesNo.YES)
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                else
                    Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = borderColor;
                Console.Write(yes);

                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = borderColor;
                Console.Write(" ");

                if (current == YesNo.NO)
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                else
                    Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = borderColor;
                Console.Write(no);
            }
        }

        override public void draw()
        {
            base.draw();

            drawText();

            drawYesNo();
        }
            
    }

    enum YesNo {YES, NO}
}
