using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DotNetTerminal
{
    class Panel
    {
        Application app;

        public int X { get; set; }
        public int Y { get; set; }

        public string Name { get; set; }

        string[] drives;
        int selected_drive;

        ConsoleColor backgroundColor = ConsoleColor.Blue;
        ConsoleColor borderColor = ConsoleColor.Cyan;
        ConsoleColor foregroundColor = ConsoleColor.Cyan;

        bool visible;

        public bool Visible { get{return visible;} set {
            visible = value;
            if (!visible)
            {
                watcher.EnableRaisingEvents = false;
            }
            else
            {
                watcher.Path = currentDirectory;
                watcher.EnableRaisingEvents = true;
            }
        } }

        public bool Focused { get; set; }

        int tmp = 0;

        string currentDirectory;

        public string directory { get { return currentDirectory; } }

        public int Width { get { return app.Width / 2;  } }
        public int Height { get { return app.Height - 2; } } // 1 for command line, 1 for tips

        List<FileSystemInfo> files;
        public int selectedIndex = 0;
        int showStartIndex = 0;

        Dictionary<string, int> positions;

        FileSystemWatcher watcher;

        public Panel(Application app, string currentDirectory)
        {
            this.app = app;
            X = 0;
            Y = 0;

            visible = false;

            positions = new Dictionary<string, int>();

            this.currentDirectory = currentDirectory;

            watcher = new FileSystemWatcher();
            watcher.Changed += new FileSystemEventHandler(OnFileEvent);
            watcher.Renamed += new RenamedEventHandler(OnRenameEvent);
            watcher.Deleted += new FileSystemEventHandler(OnFileEvent);
            watcher.EnableRaisingEvents = false;

            watcher.IncludeSubdirectories = false;

            Focused = false;

            this.currentDirectory = currentDirectory;
            reloadFiles();
        }

        public void OnFileEvent(object source, FileSystemEventArgs fsea)
        {
            reloadFiles();
            if (visible) changeDirectory(currentDirectory);
        }

        public void OnRenameEvent(object source, RenamedEventArgs rea)
        {
            reloadFiles();
            if (visible) changeDirectory(currentDirectory);
        }

        public void sortFiles()
        {
            int n = files.Count;
            for (int i=0;i<n;++i)
                for (int j=i+1;j<n;++j)
                    if ((IsDirectory(files[j]) && !IsDirectory(files[i]))||(IsDirectory(files[j]) && IsDirectory(files[i]) && (files[i].Name.CompareTo(files[j].Name) > 0)))
                    {
                        FileSystemInfo tmp = files[i];
                        files[i] = files[j];
                        files[j] = tmp;
                    }
        }

        public bool IsDirectory(object info) 
        {
            bool ch = info.GetType() == typeof(DirectoryInfo);
            if (ch) return true;
            if (info.GetType() == typeof(FileInfo))
                return ((FileInfo)info).Attributes.HasFlag(FileAttributes.Directory);
            if (info.GetType() == typeof(FileSystemInfo))
                return ((FileSystemInfo)info).Attributes.HasFlag(FileAttributes.Directory);
            return false;
        }

        public void updateSelected()
        {
            drawSelectedFileInfo();
            drawFile(selectedIndex);
        }

        public void updateSelected(int index)
        {
            if (index >= files.Count) index = files.Count- 1;
            if (index < 0 ) index = 0;
            int old_selected = selectedIndex;
            selectedIndex = index;

            if (selectedIndex >= showStartIndex + 36)
            {
                showStartIndex = selectedIndex - 36 + 1;
                draw();
            }
            if (selectedIndex < showStartIndex)
            {
                showStartIndex = selectedIndex;
                draw();
            }

            drawSelectedFileInfo();
            drawFile(selectedIndex);
            drawFile(old_selected);
        }

        public void selectNextFile()
        {
            updateSelected(selectedIndex + 1);
        }

        public void selectPrevFile()
        {
            updateSelected(selectedIndex - 1);
        }

        public void selectLeft()
        {
            updateSelected(selectedIndex - 18);
        }

        public void selectRight()
        {
            updateSelected(selectedIndex + 18);
        }

        public void draw()
        {
            if (!Visible)
            {
                clear();
                return;
            }
            // fillBackground();
            drawBorders();
            drawHeaders();

            drawSelectedFileInfo();
            drawDirectoryInfo();

            drawCurrentDirectory();
        }

        public void drawSelectedFileInfo()
        {
            if (!visible) return;
            if (!Focused)
            {
                Console.BackgroundColor = backgroundColor;
                SetCursorPosition(1, Height - 2);
                for (int i = 1; i < Width - 1; ++i) Console.Write(" ");
                return;
            }
            if (selectedIndex == -1) return;
            if (selectedIndex >= files.Count) return;

            lock (app.locker)
            {
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = foregroundColor;

                SetCursorPosition(1, Height - 2);
                for (int i = 1; i < Width - 1; ++i) Console.Write(" ");

                var info = new FileInfo(files[selectedIndex].FullName);

                SetCursorPosition(1, Height - 2);
                Console.Write(info.Name.Substring(0, Math.Min(info.Name.Length, 20)));

                SetCursorPosition(Width - 16, Height - 2);
                Console.Write(info.CreationTime.ToString(" dd.MM.yy HH:mm"));

                if (IsDirectory(info))
                {
                    string text = " Folder ";

                    SetCursorPosition(Width - 15 - text.Length, Height - 2);
                    Console.Write(text);
                }
                else
                {
                    string file_size = formatSize(info.Length);

                    SetCursorPosition(Width - 16 - file_size.Length, Height - 2);
                    Console.Write(file_size);
                }
            }
        }

        public void drawFile(int index)
        {
            if (!visible) return;
            if (index == -1 || index >= files.Count) return;
            int max_count = 18; // in column

            lock (app.locker)
            {
                if (index == selectedIndex && Focused)
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                }
                else
                {
                    Console.BackgroundColor = backgroundColor;
                }

                Console.ForegroundColor = foregroundColor;

                var info = files[index];

                if (IsDirectory(info))
                    Console.ForegroundColor = ConsoleColor.White;

                if (info.Attributes.HasFlag(FileAttributes.Hidden) || info.Attributes.HasFlag(FileAttributes.System))
                {
                    if (index == selectedIndex && Focused)
                        Console.ForegroundColor = ConsoleColor.Gray;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                }

                string name = info.Name;

                if (new DirectoryInfo(currentDirectory).Parent != null && index == 0) name = "..";

                int max_width = 19;
                int left = 0, top = 0;

                if (index - showStartIndex >= max_count)
                {
                    max_width = 18;
                    left = Width / 2 + 1;
                    top = index - max_count + 2 - showStartIndex;
                }
                else
                {
                    left = 1;
                    top = index + 2 - showStartIndex;
                }

                SetCursorPosition(left, top);
                if (name.Length > max_width) name = name.Substring(0, max_width) + "}";
                while (name.Length < max_width) name = name + " ";
                Console.Write(name);
            }
        }

        public bool drawFileBackground(int index)
        {
            if (!visible) return false;
            int max_count = 18; // in column
            if (index > max_count * 2) return false;

            lock (app.locker)
            {
                Console.BackgroundColor = backgroundColor;
             
                int max_width = 19;
                int left = 0, top = 0;

                if (index >= max_count)
                {
                    max_width = 18;
                    left = Width / 2 + 1;
                    top = index - max_count + 2;
                }
                else
                {
                    left = 1;
                    top = index + 2;
                }

                SetCursorPosition(left, top);
                var name = "";
                while (name.Length < max_width) name += " ";
                Console.Write(name);

            }
            return true;
        }

        public void drawCurrentDirectory()
        {
            int max_count = 18;
            for (int i = showStartIndex; i < Math.Min(files.Count, showStartIndex + max_count * 2); ++i)
                if (i < Math.Min(files.Count, showStartIndex + max_count * 2) && i >= showStartIndex)
                {
                    drawFile(i);
                }
            for (int i = Math.Min(files.Count, showStartIndex + max_count * 2); i < max_count * 2; ++i)
                drawFileBackground(i - showStartIndex);
        }

        void drawDirectoryInfo()
        {
            lock (app.locker)
            {

                DirectoryInfo info = new DirectoryInfo(currentDirectory);

                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = ConsoleColor.Black;

                string name = " " + info.Name + " ";

                if (name.Length > Width - 2) name = name.Substring(0, Width - 2);

                int nleft = Math.Max(Width / 2 - name.Length / 2, 0);
                int ntop = 0;
                SetCursorPosition(nleft, ntop);
                Console.Write(name);

                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = borderColor;

                for (int i = 1; i < nleft; ++i)
                {
                    SetCursorPosition(i, 0);
                    Console.Write("═");
                }
                for (int i = nleft + name.Length; i < Width - 1; ++i)
                {
                    SetCursorPosition(i, 0);
                    Console.Write("═");
                }

                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = foregroundColor;

                int fileCount = 0;
                long size = 0;
                foreach (var fileInfo in info.GetFiles())
                {
                    fileCount++;
                    size += fileInfo.Length;
                }

                string file_size = formatSize(size);

                string files_info = " " + file_size + " in " + fileCount + " files ";

                if (files_info.Length > Width - 2) files_info = files_info.Substring(0, Width - 2);

                int left = Math.Max(Width / 2 - files_info.Length / 2, 0);
                int top = Height - 1;
                SetCursorPosition(left, top);
                Console.Write(files_info);

                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = borderColor;

                for (int i = 1; i < left; ++i)
                {
                    SetCursorPosition(i, Height - 1);
                    Console.Write("═");
                }
                for (int i = left + files_info.Length; i < Width - 1; ++i)
                {
                    SetCursorPosition(i, Height - 1);
                    Console.Write("═");
                }
            }
        }

        void drawHeaders()
        {
            lock (app.locker)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                SetCursorPosition(1, 1);
                Console.Write("        Name       ");
                SetCursorPosition(Width / 2 + 1, 1);
                Console.Write("       Name       ");
            }
        }

        void fillBackground()
        {
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = backgroundColor;
            for (int i = 0; i < Height; ++i)
            {
                SetCursorPosition(0, i);
                for (int j = 0; j < Width; ++j) Console.Write(" ");
            }
        }

        void drawBorders()
        {
            lock (app.locker)
            {

                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = borderColor;

                SetCursorPosition(0, 0);
                Console.Write("╔");
                SetCursorPosition(0, Height - 1);
                Console.Write("╚");
                for (int i = 1; i < Width - 1; ++i)
                {
                    SetCursorPosition(i, 0);
                    Console.Write("═");
                    SetCursorPosition(i, Height - 1);
                    Console.Write("═");
                }
                SetCursorPosition(Width - 1, 0);
                Console.Write("╗");
                SetCursorPosition(Width - 1, Height - 1);
                Console.Write("╝");
                for (int i = 1; i < Height - 1; ++i)
                {
                    SetCursorPosition(0, i);
                    Console.Write("║");
                    SetCursorPosition(Width - 1, i);
                    Console.Write("║");
                }

                for (int i = 1; i < Height - 3; ++i)
                {
                    SetCursorPosition(Width / 2, i);
                    Console.Write("│");
                }

                for (int i = 1; i < Width - 1; ++i)
                {
                    SetCursorPosition(i, Height - 3);
                    Console.Write("─");
                }
            }
        }

        public void reloadFiles()
        {
            DirectoryInfo currentDirectoryInfo = new DirectoryInfo(currentDirectory);
            files = currentDirectoryInfo.GetFileSystemInfos().ToList();
            sortFiles();
            var info = new DirectoryInfo(currentDirectoryInfo.FullName);

            if (info.Parent != null)
            {
                files.Insert(0, info.Parent);
            }
        }

        public void changeDirectory(string directory_name)
        {
            if (!IsDirectory(new FileInfo(directory_name))) return;

            if (currentDirectory != null)
                positions[currentDirectory] = selectedIndex;

            currentDirectory = directory_name;
            DirectoryInfo currentDirectoryInfo = new DirectoryInfo(currentDirectory);

            reloadFiles();

            watcher.Path = currentDirectoryInfo.FullName;

            if (positions.ContainsKey(currentDirectory))
            {
                showStartIndex = 0;
                updateSelected(positions[currentDirectory]);
            }
            else
            {
                showStartIndex = 0;
                updateSelected(0);
            }

            drawCurrentDirectory();
            drawDirectoryInfo();

            // draw();
        }

        public void clear()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            string text = "";
            for (int j = 0; j < Width; ++j) text += " ";
            for (int i = 0; i < Height; ++i)
            {
                SetCursorPosition(0, i);
                Console.Write(text);
            }
        }

        public void Action()
        {
            var file_info = files[selectedIndex];

            if (IsDirectory(file_info))
            {
                changeDirectory(file_info.FullName);
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(file_info.FullName);
            }
            catch (Exception ex) {
                app.ShowError(ex.Message);
            }

        }

        public void OpenEdit()
        {
            var file_info = files[selectedIndex];

            try
            {
                System.Diagnostics.Process.Start("notepad", file_info.FullName);
            }
            catch (Exception ex) {
                app.ShowError(ex.Message);
            }
        }

        string formatSize(long size)
        {
            string rr = "B";
            if (size / (1024 * 1024 * 1024) >= 1)
            {
                size /= 1024 * 1024 * 1024;
                rr = "GB";
            }
            else if (size / (1024 * 1024) >= 1)
            {
                size /= 1024 * 1024;
                rr = "MB";
            }
            else if (size > 1024 - 1)
            {
                size /= 1024;
                rr = "KB";
            }
            return size + rr;
        }

        public List<FileSystemInfo> AllFiles { get { return files; } }

        void SetCursorPosition(int left, int top)
        {
            Console.SetCursorPosition(left + X, top + Y);
        }

        public void SelectDrive()
        {
            Console.CursorVisible = false;
            drives = Environment.GetLogicalDrives();
            selected_drive = 0;
            
            DrawSelectDriveBox();

            drawDrives();

            while (true)
            {
                var key_info = app.readKey();
                var key = key_info.Key;

                if (key == ConsoleKey.UpArrow)
                {
                    selected_drive = Math.Max(selected_drive - 1, 0);
                    drawDrives();
                }
                if (key == ConsoleKey.DownArrow)
                {
                    selected_drive = Math.Min(selected_drive + 1, drives.Length - 1);
                    drawDrives();
                }

                if (key == ConsoleKey.Enter)
                {
                    Visible = true;
                    if (app.currentPanel == null)
                    {
                        app.currentPanel = this;
                        Focused = true;
                    }
                    changeDirectory(drives[selected_drive]);
                    draw();
                    break;
                }

                if (key == ConsoleKey.Escape)
                {
                    draw();
                    break;
                }
            }

            Console.CursorVisible = true;
        }

        void drawDrives()
        {
            lock (app.locker)
            {

                int w = 34;
                int h = drives.Length + 4;

                for (int i = 0; i < drives.Length; ++i)
                {
                    if (i == selected_drive)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    SetCursorPosition(6, i + Height / 2 - h / 2 + 2);
                    for (int j = 0; j < w - 6; ++j) Console.Write(" ");
                    SetCursorPosition(6, i + Height / 2 - h / 2 + 2);
                    Console.Write(drives[i]);

                    DriveInfo info = new DriveInfo(drives[i]);

                    string total_size = "";
                    string free_space = "";
                    string name = "";
                    int left = 0;
                    
                    try {
                        name = info.DriveType.ToString();
                        Console.Write("  " + name); left += name.Length;
                        free_space = formatSize(info.AvailableFreeSpace);
                        total_size = formatSize(info.TotalSize);
                        for (int qwe = name.Length; qwe < 9; ++qwe) Console.Write(" ");
                        Console.Write(total_size);
                        for (int qwe = name.Length; qwe < 8; ++qwe) Console.Write(" ");
                        Console.Write(free_space);
                    }
                    catch (Exception ex) { 
                        var drive_info = "Not ready";
                        for (int qwe = left; qwe < 9; ++qwe) Console.Write(" ");
                        Console.Write(drive_info);
                    }

                }
            }
        }

        void DrawSelectDriveBox()
        {
            lock (app.locker)
            {

                int w = 34;
                int h = drives.Length + 4;

                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = ConsoleColor.White;

                for (int i = 0; i < w; ++i)
                    for (int j = 0; j < h; ++j)
                    {
                        SetCursorPosition(3 + i, Height / 2 - h / 2 + j);
                        Console.Write(" ");
                    }

                for (int i = 0; i < w; ++i)
                {
                    SetCursorPosition(3 + i, Height / 2 - h / 2 + 0);
                    Console.Write(" ");
                    SetCursorPosition(3 + i, Height / 2 - h / 2 + h - 1);
                    Console.Write(" ");
                }

                SetCursorPosition(3, Height / 2 - h / 2 + 1);
                Console.Write(" ╔");
                SetCursorPosition(3 + w - 2, Height / 2 - h / 2 + 1);
                Console.Write("╗ ");

                SetCursorPosition(3, Height / 2 - h / 2 + h - 2);
                Console.Write(" ╚");
                SetCursorPosition(3 + w - 2, Height / 2 - h / 2 + h - 2);
                Console.Write("╝ ");

                for (int i = 2; i < w - 2; ++i)
                {
                    SetCursorPosition(3 + i, Height / 2 - h / 2 + 1);
                    Console.Write("═");
                    SetCursorPosition(3 + i, Height / 2 - h / 2 + h - 2);
                    Console.Write("═");
                }

                for (int i = 2; i < h - 2; ++i)
                {
                    SetCursorPosition(4, Height / 2 - h / 2 + i);
                    Console.Write("║");
                    SetCursorPosition(3 + w - 2, Height / 2 - h / 2 + i);
                    Console.Write("║");
                }

            }
        }
    }
}
