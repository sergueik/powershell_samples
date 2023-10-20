using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DotNetTerminal
{
    class Application
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Panel leftPanel { get; set; }
        public Panel rightPanel { get; set; }

        public Panel currentPanel;

        public object locker = new object();

        string current_directory;

        string command;

        YesNoBox exit_menu;
        MakeFolderBox mkdir_menu;
        AboutBox about_box;
        ErrorBox large_error_box;
        ErrorBox error_box;

        public bool Running { get; set; }

        public char[] chars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '_', '\\', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', ',', '-', '=', '+', ' '};

        public Application() 
        {
            Init();
        }

        void Init()
        {
            Width = 80;
            Height = 25;

            Console.SetWindowSize(Width, Height);

            leftPanel = new Panel(this, @"C:\temp"); leftPanel.Name = "l";
            rightPanel = new Panel(this, @"C:\"); rightPanel.Name = "r";

            command = "";

            rightPanel.X = Width / 2;

            exit_menu = new YesNoBox(this, "Quit", "Do you want to Quit DNT?");

            exit_menu.YesPressed = delegate(ConsoleKeyInfo info) { Environment.Exit(0); };

            mkdir_menu = new MakeFolderBox(this);

            about_box = new AboutBox(this);

            large_error_box = new ErrorBox(this);

            error_box = new ErrorBox(this, 40);
        }

        public ConsoleKeyInfo readKey() {
            return Console.ReadKey(true);
        }

        public void ShowError(string s)
        {
            error_box.run(s);
        }

        void run()
        {
            Running = true;

            leftPanel.Visible = true;
            rightPanel.Visible = true;

            currentPanel = leftPanel;
            currentPanel.Focused = true;
            current_directory = currentPanel.directory;

            draw();

            currentPanel.updateSelected(0);

            while (true)
            {
                // Scroll up
                Console.SetCursorPosition(0, 0);

                write_cmd();

                ConsoleKeyInfo key_info = readKey();
                ConsoleKey key = key_info.Key;

                if (chars.Contains(key_info.KeyChar))
                {
                    command += key_info.KeyChar;
                    write_cmd();
                    continue;
                }

                if (key == ConsoleKey.Backspace && command.Length > 0)
                {
                    command = command.Substring(0, command.Length - 1);
                    write_cmd();
                    continue;
                }

                switch (key)
                {
                    case ConsoleKey.Enter:
                        if (command.Length > 0)
                        {
                            action();
                            write_cmd();
                            continue;
                        }
                        break;
                    case ConsoleKey.F10:
                        exit_menu.run();
                        break;
                    case ConsoleKey.F1:
                        if (key_info.Modifiers == ConsoleModifiers.Control) { TogglePanel(leftPanel); continue; }
                        leftPanel.SelectDrive();
                        break;
                    case ConsoleKey.F2:
                        if (key_info.Modifiers == ConsoleModifiers.Control) { TogglePanel(rightPanel); continue; }
                        rightPanel.SelectDrive();
                        break;
                }

                if (null == currentPanel)
                    currentPanel = leftPanel.Visible ? leftPanel : (rightPanel.Visible ? rightPanel : null);

                if (!leftPanel.Visible && !rightPanel.Visible) currentPanel = null;
                if (null == currentPanel) continue;

                switch (key)
                {
                    case ConsoleKey.F3:
                        currentPanel.OpenEdit();
                        break;
                    case ConsoleKey.F4:
                        currentPanel.OpenEdit();
                        break;
                    case ConsoleKey.F5: error_box.run("Not Implemented"); break;
                    case ConsoleKey.F6: error_box.run("Not Implemented"); break;
                    case ConsoleKey.F7: if (currentPanel != null) mkdir_menu.run(currentPanel.directory); break;
                    case ConsoleKey.F8: error_box.run("Not Implemented"); break;
                    case ConsoleKey.F9: about_box.run(); break;
                    case ConsoleKey.UpArrow: currentPanel.selectPrevFile(); break;
                    case ConsoleKey.DownArrow: currentPanel.selectNextFile(); break;
                    case ConsoleKey.LeftArrow: currentPanel.selectLeft(); break;
                    case ConsoleKey.RightArrow: currentPanel.selectRight(); break;
                    case ConsoleKey.PageUp: currentPanel.selectLeft(); break;
                    case ConsoleKey.PageDown: currentPanel.selectRight(); break;
                    case ConsoleKey.Home: currentPanel.updateSelected(0); break;
                    case ConsoleKey.End: currentPanel.updateSelected(currentPanel.AllFiles.Count - 1); break;
                    case ConsoleKey.Tab: ChangePanel(); break;
                    case ConsoleKey.Enter:
                        currentPanel.Action();
                        current_directory = currentPanel.directory;
                        write_cmd();
                        command = "";
                        break;
                }
            }
        }

        void action()
        {
            var cdir = current_directory;
            command = command.Trim();
            if (cdir[cdir.Length - 1] != '\\') cdir += '\\';
            var cmd = cdir + command;
            try
            {
                if (File.Exists(cmd))
                    System.Diagnostics.Process.Start(cmd);
                else
                    if (command == "exit") Environment.Exit(0);
                    else
                        System.Diagnostics.Process.Start(command);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            command = "";
        }

        public void write_cmd()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            var cdir = current_directory;
            if (current_directory.Length > 12)
                cdir = current_directory.Substring(0, 3) + "..." + current_directory.Substring(current_directory.Length - 7, 7);

            var text = cdir + ">" + command;
            int max_length = 80 - 1;
            if (text.Length > max_length)
                text = text.Substring(text.Length - max_length, max_length);
            Console.SetCursorPosition(0, Height - 2);
            Console.Write(text);

            for (int i = text.Length; i < Width; ++i) Console.Write(" ");

            Console.SetCursorPosition(text.Length, Height - 2);
        }

        void ChangePanel()
        {
            if (currentPanel == leftPanel || currentPanel == rightPanel)
            {
                if (!otherPanel(currentPanel).Visible) return;

                currentPanel.Focused = false;
                currentPanel.updateSelected();

                currentPanel = otherPanel(currentPanel);

                currentPanel.Focused = true;
                currentPanel.updateSelected();

                current_directory = currentPanel.directory;
            }
        }

        Panel otherPanel(Panel p)
        {
            return p == leftPanel ? rightPanel : leftPanel;
        }

        void TogglePanel(Panel panel)
        {
            if (panel.Visible)
            {
                if (currentPanel == panel && otherPanel(panel).Visible)
                {
                    currentPanel = otherPanel(panel);
                    currentPanel.Focused = true;
                    currentPanel.updateSelected();
                }
                else
                    currentPanel = null;

                panel.Visible = false;
                panel.Focused = false;
                panel.clear();
            }
            else
            {
                if (currentPanel != otherPanel(panel))
                {
                    currentPanel = panel;
                    currentPanel.Focused = true;
                }
                panel.Visible = true;
                panel.draw();
            } 
        }

        void DestroyAll()
        {

            leftPanel = null;
            rightPanel = null;

            currentPanel = null;

            current_directory = null;

            command = null;

            exit_menu = null;
            mkdir_menu = null;
            about_box = null;
            large_error_box = null;
            error_box = null;
        }

        public void secureRun()
        {
            while (true)
            {
                try
                {
                    DestroyAll();
                    Init();
                    run();
                }
                catch (Exception ex)
                {
                    Running = false;
                    large_error_box.run(ex.ToString());
                }
            }
        }

        public void DrawPanels()
        {
            if (!Running) return;
            leftPanel.draw();
            leftPanel.updateSelected();

            rightPanel.draw();
            rightPanel.updateSelected();
        }

        void drawFooterMenu(string key, string text)
        {
            if (!Running) return;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(key);

            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(text);
        }

        void drawFooter()
        {
            if (!Running) return;
            Console.SetCursorPosition(0, Height - 1);

            drawFooterMenu("1", "Left  ");
            drawFooterMenu(" 2", "Right ");
            drawFooterMenu(" 3", "View  ");
            drawFooterMenu(" 4", "Edit  ");
            drawFooterMenu(" 5", "Copy  ");
            drawFooterMenu(" 6", "Move  ");
            drawFooterMenu(" 7", "MkDir ");
            drawFooterMenu(" 8", "Find  ");
            drawFooterMenu(" 9", "About ");
            drawFooterMenu(" 10", "Quit ");
        }

        void draw()
        {
            if (!Running) return;
            drawFooter();
            DrawPanels();
        }

        int log_lines = 0;

        public void log(string s)
        {
            var bg = Console.BackgroundColor;
            var fg = Console.ForegroundColor;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, Height + log_lines);
            Console.Write(s);
            log_lines++;
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
        }

    }
}
