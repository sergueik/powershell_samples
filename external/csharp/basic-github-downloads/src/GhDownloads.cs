using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Win32;

class AssetStat
{
    public string Name;
    public long Downloads, Size;
}

class ReleaseStat
{
    public string Tag;
    public DateTime Published;
    public bool Prerelease;
    public long Downloads;
    public List<AssetStat> Assets = new List<AssetStat>();
}

class Referrer
{
    public string Name;
    public int Count, Uniques;
}

class DayCount
{
    public DateTime Day;
    public int Count, Uniques;
}

class RepoStat
{
    public string Name, Url, Description, Language;
    public DateTime Created, Pushed;
    public int Stars, Forks, Issues;
    public long Downloads;
    public long? NewDownloads, NewStars;   // null — проекта не было в прошлой проверке
    public long? WeekDownloads, WeekStars; // прирост с недельного снимка
    public List<KeyValuePair<DateTime, double>> Trend = new List<KeyValuePair<DateTime, double>>();
    public List<ReleaseStat> Releases = new List<ReleaseStat>();
    public bool HasTraffic;
    public int Views, UniqueViews, Clones, UniqueClones;
    public List<DayCount> ViewDays = new List<DayCount>(), CloneDays = new List<DayCount>();
    public List<Referrer> Referrers = new List<Referrer>();

    public override string ToString() { return Name; }
}

class AccountStat
{
    public string User;
    public int Followers;
    public bool HasToken, TrafficDenied;
    public List<RepoStat> Repos = new List<RepoStat>();
}

// Снимок счётчиков после проверки — по ним считаются прирост, графики и история
class Snapshot
{
    public string Time;
    public long Total;
    public long Stars = -1, Forks = -1, Followers = -1; // -1 — в старых файлах истории этих полей нет
    public Dictionary<string, long> Repos = new Dictionary<string, long>();
    public Dictionary<string, long> RepoStars = new Dictionary<string, long>();

    public DateTime When() { return DateTime.Parse(Time, null, DateTimeStyles.RoundtripKind); }
    public bool HasStars() { return Stars >= 0; }
}

class ShareRow
{
    public string Name;
    public long Downloads;
    public int Files;
}

class Series
{
    public string Name;
    public Color Color;
    public bool Right, Fill;
    public List<KeyValuePair<DateTime, double>> Points = new List<KeyValuePair<DateTime, double>>();
}

static class Theme
{
    public static readonly float S;
    static Theme() { using (var g = Graphics.FromHwnd(IntPtr.Zero)) S = g.DpiX / 96f; }
    public static int D(double v) { return (int)Math.Round(v * S); }

    public static readonly Color Side = Color.FromArgb(30, 42, 68);
    public static readonly Color SideActive = Color.FromArgb(46, 63, 99);
    public static readonly Color SideHover = Color.FromArgb(38, 52, 84);
    public static readonly Color SideText = Color.FromArgb(196, 206, 224);
    public static readonly Color SideMuted = Color.FromArgb(138, 153, 181);
    public static readonly Color Accent = Color.FromArgb(96, 156, 255);
    public static readonly Color Donate = Color.FromArgb(232, 131, 58);
    public static readonly Color DonateHover = Color.FromArgb(242, 152, 86);

    public static readonly Color Back = Color.FromArgb(244, 246, 251);
    public static readonly Color CardBorder = Color.FromArgb(226, 231, 240);
    public static readonly Color Grid = Color.FromArgb(235, 239, 246);
    public static readonly Color BarBack = Color.FromArgb(234, 239, 248);
    public static readonly Color RowHover = Color.FromArgb(246, 248, 253);
    public static readonly Color RowHead = Color.FromArgb(240, 243, 250);
    public static readonly Color RowHeadHover = Color.FromArgb(228, 235, 249);
    public static readonly Color Text = Color.FromArgb(31, 35, 40);
    public static readonly Color Muted = Color.FromArgb(101, 109, 118);
    public static readonly Color Good = Color.FromArgb(26, 127, 55);
    public static readonly Color Bad = Color.FromArgb(207, 34, 46);
    public static readonly Color Blue = Color.FromArgb(47, 111, 235);
    public static readonly Color Amber = Color.FromArgb(222, 132, 20);
    public static readonly Color Violet = Color.FromArgb(124, 92, 230);

    public static readonly Font Body = new Font("Segoe UI", 10f);
    public static readonly Font Small = new Font("Segoe UI", 8.75f);
    public static readonly Font Bold = new Font("Segoe UI Semibold", 10f);
    public static readonly Font SmallBold = new Font("Segoe UI Semibold", 9f);
    public static readonly Font Head = new Font("Segoe UI Semibold", 11.5f);
    public static readonly Font Big = new Font("Segoe UI Semibold", 20f);
    public static readonly Font Title = new Font("Segoe UI Semibold", 15f);
    public static readonly Font SideName = new Font("Segoe UI Semibold", 11f);
}

class Hit
{
    public Rectangle R;
    public Action Click;
    public string Tip;
}

// Панель, которую целиком рисует функция Render; запоминает кликабельные места и подсказки
class Canvas : Panel
{
    public Func<Painter, int> Render; // возвращает высоту содержимого
    public ToolTip Tips;
    readonly List<Hit> hits = new List<Hit>();
    Hit hover;
    bool stale = true;

    public Canvas()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Theme.Back;
    }

    public void AddHit(Hit h) { hits.Add(h); }
    public bool IsHover(Rectangle r) { return hover != null && hover.R == r; }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        hits.Clear();
        if (Render == null) return;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
        int h = Render(new Painter(this, g));
        g.ResetTransform();
        stale = false;
        if (AutoScroll && AutoScrollMinSize.Height != h) AutoScrollMinSize = new Size(0, h);
    }

    protected override void OnInvalidated(InvalidateEventArgs e) { base.OnInvalidated(e); stale = true; }

    // Области кликов собираются при отрисовке; если окно ещё не перерисовалось — собрать без вывода на экран
    Hit Find(Point client)
    {
        if (stale && Render != null)
            using (var bmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bmp))
            {
                hits.Clear();
                g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
                Render(new Painter(this, g));
                stale = false;
            }
        var pt = new Point(client.X - AutoScrollPosition.X, client.Y - AutoScrollPosition.Y);
        for (int i = hits.Count - 1; i >= 0; i--)
            if (hits[i].R.Contains(pt)) return hits[i];
        return null;
    }

    static bool Same(Hit a, Hit b) { return a == null ? b == null : b != null && a.R == b.R && a.Tip == b.Tip; }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        var h = Find(e.Location);
        if (Same(h, hover)) return;
        bool redraw = (h != null && h.Click != null) || (hover != null && hover.Click != null);
        hover = h;
        Cursor = h != null && h.Click != null ? Cursors.Hand : Cursors.Default;
        if (Tips != null)
        {
            if (h != null && h.Tip != null) Tips.Show(h.Tip, this, e.X + 14, e.Y + 22, 30000);
            else Tips.Hide(this);
        }
        if (redraw) Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        if (Tips != null) Tips.Hide(this);
        bool redraw = hover != null;
        hover = null;
        Cursor = Cursors.Default;
        if (redraw) Invalidate();
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        if (e.Button != MouseButtons.Left) return;
        var h = Find(e.Location);
        if (h != null && h.Click != null) h.Click();
    }

    protected override void OnScroll(ScrollEventArgs se) { base.OnScroll(se); Invalidate(); }
    protected override void OnMouseWheel(MouseEventArgs e) { base.OnMouseWheel(e); Invalidate(); }
}

class Painter
{
    public readonly Graphics G;
    public readonly int W, H;
    readonly Canvas canvas;
    const TextFormatFlags Line = TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter |
                                 TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.PreserveGraphicsClipping;
    const TextFormatFlags Wrap = TextFormatFlags.NoPrefix | TextFormatFlags.WordBreak | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.PreserveGraphicsClipping;

    public Painter(Canvas c, Graphics g)
    {
        canvas = c;
        G = g;
        W = c.ClientSize.Width;
        H = c.ClientSize.Height;
    }

    public void Text(string s, Font f, Color c, Rectangle r, TextFormatFlags extra) { TextRenderer.DrawText(G, s, f, r, c, Line | extra); }
    public void Text(string s, Font f, Color c, Rectangle r) { Text(s, f, c, r, TextFormatFlags.Default); }
    public void Text(string s, Font f, Color c, int x, int y, int w) { Text(s, f, c, new Rectangle(x, y, w, f.Height)); }
    public void Right(string s, Font f, Color c, Rectangle r) { Text(s, f, c, r, TextFormatFlags.Right); }
    public int Measure(string s, Font f) { return TextRenderer.MeasureText(G, s, f, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine).Width; }

    // Текст с переносом строк; возвращает высоту
    public int ParaHeight(string s, Font f, int w) { return TextRenderer.MeasureText(G, s, f, new Size(w, int.MaxValue), Wrap).Height; }

    public int Para(string s, Font f, Color c, int x, int y, int w)
    {
        var sz = TextRenderer.MeasureText(G, s, f, new Size(w, int.MaxValue), Wrap);
        TextRenderer.DrawText(G, s, f, new Rectangle(x, y, w, sz.Height), c, Wrap);
        return sz.Height;
    }

    public static GraphicsPath Round(Rectangle r, int rad)
    {
        var p = new GraphicsPath();
        int d = Math.Min(rad * 2, Math.Min(r.Width, r.Height));
        if (d <= 1) { p.AddRectangle(r); return p; }
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    public void Fill(Rectangle r, Color c, int radius)
    {
        using (var b = new SolidBrush(c))
        {
            if (radius <= 0) { G.FillRectangle(b, r); return; }
            using (var path = Round(r, radius)) G.FillPath(b, path);
        }
    }

    public void Card(Rectangle r)
    {
        using (var path = Round(new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1), Theme.D(10)))
        using (var b = new SolidBrush(Color.White))
        using (var pen = new Pen(Theme.CardBorder))
        {
            G.FillPath(b, path);
            G.DrawPath(pen, path);
        }
    }

    public void Bar(Rectangle r, double frac, Color c)
    {
        Fill(r, Theme.BarBack, r.Height / 2);
        if (frac <= 0) return;
        int w = Math.Max(r.Height, (int)Math.Round(r.Width * Math.Min(1, frac)));
        Fill(new Rectangle(r.X, r.Y, w, r.Height), c, r.Height / 2);
    }

    public void Link(Rectangle r, Action a, string tip) { canvas.AddHit(new Hit { R = r, Click = a, Tip = tip }); }
    public void Tip(Rectangle r, string tip) { canvas.AddHit(new Hit { R = r, Tip = tip }); }
    public bool Hover(Rectangle r) { return canvas.IsHover(r); }
}

// ListView без мерцания при своей отрисовке столбца «Тренд»
class SmoothList : ListView
{
    public SmoothList() { DoubleBuffered = true; }
}

// Язык интерфейса: у каждой надписи в коде есть пара «по-русски / по-английски»
static class Lang
{
    public static bool En;

    public static string T(string ru, string en) { return En ? en : ru; }

    // Для английского достаточно двух форм: one — ровно 1, many — остальные
    public static string Pl(long n, string ru1, string ru2, string ru5, string en1, string en2)
    {
        if (En) return Math.Abs(n) == 1 ? en1 : en2;
        n = Math.Abs(n) % 100;
        if (n >= 11 && n <= 14) return ru5;
        switch (n % 10)
        {
            case 1: return ru1;
            case 2: case 3: case 4: return ru2;
            default: return ru5;
        }
    }

    public static void Apply(bool en)
    {
        En = en;
        var ci = CultureInfo.GetCultureInfo(en ? "en-US" : "ru-RU");
        Thread.CurrentThread.CurrentCulture = ci;
        CultureInfo.DefaultThreadCurrentCulture = ci;
    }
}

class TokenForm : Form
{
    public readonly TextBox Box;

    public TokenForm(string current)
    {
        Text = Lang.T("Токен GitHub", "GitHub token");
        Font = new Font("Segoe UI", 10f);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = MinimizeBox = ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(540, 300);

        Controls.Add(new Label
        {
            Location = new Point(14, 12), Size = new Size(512, 150),
            Text = Lang.T("Токен не обязателен. С ним программа покажет просмотры и клонирования ваших проектов " +
                          "за последние 14 дней (GitHub открывает их только владельцу) и не упрётся в лимит 60 запросов в час.\n\n" +
                          "Как получить: откройте ссылку ниже → Repository access: All repositories → " +
                          "Permissions → Administration: Read-only → Generate token. Скопируйте токен и вставьте в поле.",
                          "A token is optional. With it the program shows views and clones of your projects " +
                          "for the last 14 days (GitHub shows them only to the owner) and won't hit the limit of 60 requests per hour.\n\n" +
                          "How to get one: open the link below → Repository access: All repositories → " +
                          "Permissions → Administration: Read-only → Generate token. Copy the token and paste it into the field.")
        });
        var link = new LinkLabel { Text = Lang.T("Создать токен на GitHub", "Create a token on GitHub"), Location = new Point(14, 166), AutoSize = true };
        link.LinkClicked += delegate { Open("https://github.com/settings/personal-access-tokens/new"); };
        Controls.Add(link);
        Box = new TextBox { Location = new Point(14, 196), Width = 512, UseSystemPasswordChar = true, Text = current ?? "" };
        Controls.Add(Box);
        Controls.Add(new Label { Location = new Point(14, 226), AutoSize = true, ForeColor = Theme.Muted, Text = Lang.T("Хранится зашифрованным, прочитать его может только ваша учётная запись Windows.", "Stored encrypted; only your Windows account can read it.") });

        var ok = new Button { Text = Lang.T("Сохранить", "Save"), DialogResult = DialogResult.OK, Location = new Point(250, 258), AutoSize = true };
        var del = new Button { Text = Lang.T("Удалить токен", "Delete token"), Location = new Point(14, 258), AutoSize = true };
        del.Click += delegate { Box.Text = ""; DialogResult = DialogResult.OK; };
        var cancel = new Button { Text = Lang.T("Отмена", "Cancel"), DialogResult = DialogResult.Cancel, Location = new Point(390, 258), AutoSize = true };
        Controls.AddRange(new Control[] { ok, del, cancel });
        AcceptButton = ok;
        CancelButton = cancel;
    }

    public static void Open(string url)
    {
        try { Process.Start(url); } catch { }
    }
}

// Полоса с подсказкой под страницами; части текста между ** рисуются жирным
class NoticeBar : Control
{
    string text = "";

    public NoticeBar()
    {
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    public string Note
    {
        get { return text; }
        set { text = value ?? ""; Invalidate(); }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(BackColor);
        using (var pen = new Pen(Color.FromArgb(246, 222, 186)))
            e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
        var parts = text.Split(new[] { "**" }, StringSplitOptions.None);
        const TextFormatFlags fl = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
        var widths = new int[parts.Length];
        int total = 0;
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length == 0) continue;
            widths[i] = TextRenderer.MeasureText(e.Graphics, parts[i], i % 2 == 1 ? Theme.Bold : Theme.Body, new Size(int.MaxValue, Height), fl).Width;
            total += widths[i];
        }
        int x = Math.Max(Padding.Left, (Width - total) / 2);
        for (int i = 0; i < parts.Length && x < Width; i++)
        {
            if (parts[i].Length == 0) continue;
            bool bold = i % 2 == 1;
            var r = new Rectangle(x, 0, Math.Min(widths[i], Width - Padding.Right - x), Height);
            TextRenderer.DrawText(e.Graphics, parts[i], bold ? Theme.Bold : Theme.Body, r, bold ? Color.FromArgb(176, 86, 14) : Theme.Text, fl | TextFormatFlags.EndEllipsis);
            x += widths[i];
        }
    }
}

class MainForm : Form
{
    const string RegKey = @"Software\GitHubDownloads";
    const string DefaultUser = "OlegChe77";
    const string DonateUrl = "https://dalink.to/olegche";
    static readonly string[] PageNamesRu = { "Обзор", "Проекты", "Релизы и файлы", "Трафик", "История" };
    static readonly string[] PageNamesEn = { "Overview", "Projects", "Releases & files", "Traffic", "History" };
    static readonly string[] ColNamesRu = { "Проект", "Скачиваний", "Новых", "Тренд", "Звёзд", "Форков", "Открытых задач", "Язык", "Релизов", "Последний релиз", "Просмотров" };
    static readonly string[] ColNamesEn = { "Project", "Downloads", "New", "Trend", "Stars", "Forks", "Open issues", "Language", "Releases", "Latest release", "Views" };
    static readonly int[] ColWidths = { 160, 106, 62, 92, 58, 62, 112, 84, 68, 142, 96 };
    const int TrendCol = 3;

    static string[] PageNames { get { return Lang.En ? PageNamesEn : PageNamesRu; } }
    static string[] ColNames { get { return Lang.En ? ColNamesEn : ColNamesRu; } }
    static string T(string ru, string en) { return Lang.T(ru, en); }
    static string Pl(long n, string ru1, string ru2, string ru5, string en1, string en2) { return Lang.Pl(n, ru1, ru2, ru5, en1, en2); }

    static string TipDown { get { return T("Сколько раз всего скачали файлы из раздела Releases во всех проектах.\nКнопка «Code → Download ZIP» и git clone сюда не входят — GitHub их не считает.",
        "How many times files from the Releases section were downloaded across all projects.\n\"Code → Download ZIP\" and git clone are not included — GitHub doesn't count them."); } }
    static string TipWeek { get { return T("Сколько скачиваний прибавилось за последние 7 дней.\nСчитается по прошлым проверкам: если программу запускали реже, период будет длиннее — он указан под числом.",
        "How many downloads were added over the last 7 days.\nCalculated from earlier checks: if the program was run less often, the period is longer — it is shown under the number."); } }
    static string TipStars { get { return T("Сколько людей отметили проекты звёздочкой (Star) — это «нравится» на GitHub.\nПрирост считается по прошлым проверкам этой программы.",
        "How many people starred your projects (Star) — GitHub's \"like\".\nGrowth is calculated from this program's earlier checks."); } }
    static string TipFollowers { get { return T("Сколько людей подписаны на аккаунт (Follow).", "How many people follow the account (Follow)."); } }

    static int rateRemaining = -1, rateLimit = -1;

    readonly ToolTip tips = new ToolTip { AutoPopDelay = 30000, InitialDelay = 300 };
    readonly Font infoFont = new Font("Segoe UI", 10f), infoBold = new Font("Segoe UI Semibold", 10.5f), infoTitle = new Font("Segoe UI Semibold", 13f);
    Canvas side, overview, releases, traffic, history;
    Control[] pages;
    Panel header;
    Label titleLbl, accLbl, status;
    NoticeBar notice;
    TextBox userBox;
    Button refreshBtn;
    ComboBox relCombo;
    Label relLbl;
    string[] lastError; // текст последней ошибки: по-русски и по-английски
    SmoothList list;
    RichTextBox info;
    SplitContainer split;
    bool splitSet, loading, loadFailed, tokenSet;
    int page;

    AccountStat data;
    DateTime loadedAt;
    List<Snapshot> hist = new List<Snapshot>();
    Snapshot curSnap, prevSnap, weekSnap, weekStarSnap;
    DateTime chartFrom;
    int sortCol = 1;
    bool sortDesc = true;

    static int D(double v) { return Theme.D(v); }

    public MainForm()
    {
        string savedLang = Reg("Lang");
        Lang.Apply(savedLang != null ? savedLang == "en" : CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "ru");
        Text = T("Скачивания на GitHub", "GitHub Downloads");
        Font = new Font("Segoe UI", 10f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1180, 800);
        MinimumSize = new Size(900, 620);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.Back;
        try
        {
            using (Stream ico = typeof(MainForm).Assembly.GetManifestResourceStream("app.ico"))
                Icon = new Icon(ico);
        }
        catch
        {
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
        }

        side = new Canvas { Dock = DockStyle.Left, Width = D(140), BackColor = Theme.Side, Tips = tips, Render = RenderSide };

        // Шапка: заголовок раздела, аккаунт и «Обновить»
        header = new Panel { Dock = DockStyle.Top, Height = D(60) };
        titleLbl = new Label { AutoSize = true, UseMnemonic = false, Font = Theme.Title, ForeColor = Theme.Text, Location = new Point(D(18), D(14)) };
        accLbl = new Label { AutoSize = true, Text = T("Аккаунт GitHub:", "GitHub account:"), ForeColor = Theme.Muted };
        userBox = new TextBox { Width = D(170), Text = Reg("User") ?? DefaultUser };
        refreshBtn = new Button { Text = T("Обновить", "Refresh"), AutoSize = true, FlatStyle = FlatStyle.Flat, BackColor = Theme.Blue, ForeColor = Color.White, Cursor = Cursors.Hand };
        refreshBtn.FlatAppearance.BorderSize = 0;
        refreshBtn.Click += delegate { LoadStats(); };
        tips.SetToolTip(refreshBtn, T("Загрузить свежие данные с GitHub (F5)", "Load fresh data from GitHub (F5)"));
        header.Controls.AddRange(new Control[] { titleLbl, accLbl, userBox, refreshBtn });
        header.Resize += delegate { LayoutHeader(); };
        AcceptButton = refreshBtn;

        status = new Label { Dock = DockStyle.Bottom, Height = D(28), Padding = new Padding(D(18), 0, D(18), 0), TextAlign = ContentAlignment.MiddleLeft, ForeColor = Theme.Muted };

        notice = new NoticeBar { Dock = DockStyle.Top, Height = D(38), Padding = new Padding(D(20), 0, D(20), 0), BackColor = Color.FromArgb(255, 247, 233) };
        UpdateNotice();

        overview = NewCanvas(RenderOverview);
        traffic = NewCanvas(RenderTraffic);
        history = NewCanvas(RenderHistory);

        // «Релизы и файлы»: выбор проекта и рисованная страница
        var relPage = new Panel { Dock = DockStyle.Fill };
        var relTop = new Panel { Dock = DockStyle.Top, Height = D(38) };
        relLbl = new Label { AutoSize = true, Text = T("Проект:", "Project:"), ForeColor = Theme.Muted, Location = new Point(D(20), D(9)) };
        relCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = D(280), Location = new Point(D(84), D(5)) };
        relCombo.SelectedIndexChanged += delegate { releases.AutoScrollPosition = Point.Empty; releases.Invalidate(); };
        relTop.Controls.AddRange(new Control[] { relLbl, relCombo });
        releases = NewCanvas(RenderReleases);
        relPage.Controls.Add(releases);
        relPage.Controls.Add(relTop);

        // «Проекты»: таблица и подробности
        list = new SmoothList { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, HideSelection = false, MultiSelect = false, OwnerDraw = true, BorderStyle = BorderStyle.None };
        list.ColumnClick += (s, e) =>
        {
            if (e.Column == sortCol) sortDesc = !sortDesc;
            else { sortCol = e.Column; sortDesc = e.Column != 0 && e.Column != 7; }
            FillList();
        };
        list.SelectedIndexChanged += delegate { if (list.SelectedItems.Count > 0) ShowDetails((RepoStat)list.SelectedItems[0].Tag); };
        list.ItemActivate += delegate { if (list.SelectedItems.Count > 0) TokenForm.Open(((RepoStat)list.SelectedItems[0].Tag).Url); };
        list.DrawColumnHeader += (s, e) => { e.DrawDefault = true; };
        list.DrawItem += (s, e) => { };
        list.DrawSubItem += DrawListCell;
        var listBox = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        listBox.Controls.Add(list);

        info = new RichTextBox { Dock = DockStyle.Fill, ReadOnly = true, BorderStyle = BorderStyle.None, BackColor = Color.White, DetectUrls = false };
        var infoBox = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(D(14), D(10), D(8), D(8)) };
        infoBox.Controls.Add(info);

        split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterWidth = D(10), BackColor = Theme.Back };
        split.Panel1.Controls.Add(listBox);
        split.Panel2.Controls.Add(infoBox);
        var projPage = new Panel { Dock = DockStyle.Fill, Padding = new Padding(D(20), D(4), D(20), D(6)) };
        projPage.Controls.Add(split);

        var host = new Panel { Dock = DockStyle.Fill };
        pages = new Control[] { overview, projPage, relPage, traffic, history };
        foreach (var pg in pages) host.Controls.Add(pg);

        var main = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Back };
        main.Controls.Add(host);
        main.Controls.Add(notice);
        main.Controls.Add(header);
        main.Controls.Add(status);

        Controls.Add(main);
        Controls.Add(side);

        UpdateTokenButton();
        SelectPage(0);
        Load += delegate { LayoutHeader(); };
        Shown += delegate { userBox.Select(userBox.TextLength, 0); LoadStats(); };
    }

    Canvas NewCanvas(Func<Painter, int> render)
    {
        return new Canvas { Dock = DockStyle.Fill, AutoScroll = true, Tips = tips, Render = render };
    }

    void LayoutHeader()
    {
        int r = header.Width - D(20);
        refreshBtn.Left = r - refreshBtn.Width;
        userBox.Left = refreshBtn.Left - D(8) - userBox.Width;
        accLbl.Left = userBox.Left - D(6) - accLbl.Width;
        int mid = header.Height / 2 + D(1);
        refreshBtn.Top = mid - refreshBtn.Height / 2;
        userBox.Top = mid - userBox.Height / 2;
        accLbl.Top = mid - accLbl.Height / 2;
        titleLbl.Top = mid - titleLbl.Height / 2;
    }

    void SelectPage(int i)
    {
        page = i;
        for (int j = 0; j < pages.Length; j++) pages[j].Visible = j == i;
        titleLbl.Text = PageNames[i];
        side.Invalidate();
        if (i == 1 && !splitSet && split.Height > D(200))
        {
            split.SplitterDistance = split.Height * 55 / 100;
            splitSet = true;
        }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.F5) { LoadStats(); return true; }
        for (int i = 0; i < PageNames.Length; i++)
            if (keyData == (Keys.Control | (Keys.D1 + i))) { SelectPage(i); return true; }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    void UpdateTokenButton()
    {
        tokenSet = !string.IsNullOrEmpty(LoadToken());
        side.Invalidate();
        UpdateNotice();
    }

    void UpdateNotice()
    {
        if (notice == null) return;
        notice.Note = T("Внимание: GitHub даёт посмотреть статистику только **60 раз в час**." + (tokenSet ? " С токеном — больше." : ""),
                        "Heads up: GitHub lets you view statistics only **60 times per hour**." + (tokenSet ? " With a token — more." : ""));
    }

    // Переключение языка: перерисовываем всё, что хранит готовый текст
    void SwitchLanguage()
    {
        Lang.Apply(!Lang.En);
        SetReg("Lang", Lang.En ? "en" : "ru");
        Text = T("Скачивания на GitHub", "GitHub Downloads");
        accLbl.Text = T("Аккаунт GitHub:", "GitHub account:");
        refreshBtn.Text = T("Обновить", "Refresh");
        tips.SetToolTip(refreshBtn, T("Загрузить свежие данные с GitHub (F5)", "Load fresh data from GitHub (F5)"));
        relLbl.Text = T("Проект:", "Project:");
        UpdateNotice();
        LayoutHeader();
        SelectPage(page);
        if (data != null)
        {
            FillList();
            FillReleaseCombo();
            if (list.SelectedItems.Count > 0) ShowDetails((RepoStat)list.SelectedItems[0].Tag);
        }
        UpdateStatus();
        InvalidatePages();
    }

    // Строка состояния собирается из состояния программы, чтобы её можно было пересобрать на другом языке
    void UpdateStatus()
    {
        if (loading)
        {
            status.ForeColor = Theme.Muted;
            status.Text = T("Загружаю данные с GitHub…", "Loading data from GitHub…");
        }
        else if (lastError != null)
        {
            status.ForeColor = Theme.Bad;
            status.Text = Lang.En ? lastError[1] : lastError[0];
        }
        else if (data != null)
        {
            int withReleases = data.Repos.Count(r => r.Releases.Count > 0);
            string rate = rateRemaining >= 0 ? string.Format(T(" · запросов к GitHub осталось {0} из {1} в час", " · GitHub requests left: {0} of {1} per hour"), rateRemaining, rateLimit) : "";
            status.ForeColor = Theme.Muted;
            status.Text = string.Format(T("{0} {1}, с релизами {2}{3}", "{0} {1}, with releases: {2}{3}"), data.Repos.Count,
                Pl(data.Repos.Count, "проект", "проекта", "проектов", "project", "projects"), withReleases, rate);
        }
        else status.Text = "";
    }

    void EditToken()
    {
        if (loading) return;
        using (var f = new TokenForm(LoadStoredToken()))
        {
            if (f.ShowDialog(this) != DialogResult.OK) return;
            SaveToken(f.Box.Text.Trim());
        }
        UpdateTokenButton();
        LoadStats();
    }

    void LoadStats()
    {
        string user = userBox.Text.Trim();
        if (user.Length == 0 || loading) return;
        SetReg("User", user);
        loading = true;
        refreshBtn.Enabled = false;
        UpdateStatus();
        InvalidatePages();
        string token = LoadToken();
        var th = new Thread(delegate()
        {
            AccountStat acc = null;
            string[] error = null;
            try { acc = Fetch(user, token); }
            catch (Exception ex) { error = Describe(ex); }
            BeginInvoke((MethodInvoker)delegate { ShowStats(acc, error); });
        });
        th.IsBackground = true;
        th.Start();
    }

    void InvalidatePages()
    {
        side.Invalidate();
        overview.Invalidate();
        releases.Invalidate();
        traffic.Invalidate();
        history.Invalidate();
    }

    void ShowStats(AccountStat acc, string[] error)
    {
        loading = false;
        refreshBtn.Enabled = true;
        loadFailed = error != null;
        lastError = error;
        if (error != null) { UpdateStatus(); InvalidatePages(); return; }
        data = acc;

        DateTime now = DateTime.Now;
        loadedAt = now;
        var cur = new Snapshot
        {
            Time = now.ToString("o"),
            Total = acc.Repos.Sum(r => r.Downloads),
            Stars = acc.Repos.Sum(r => (long)r.Stars),
            Forks = acc.Repos.Sum(r => (long)r.Forks),
            Followers = acc.Followers
        };
        foreach (var r in acc.Repos)
        {
            cur.Repos[r.Name] = r.Downloads;
            cur.RepoStars[r.Name] = r.Stars;
        }

        var h = LoadHistory(acc.User);
        h.RemoveAll(s => s.When() < now.AddDays(-400));
        h.Sort((a, b) => a.When().CompareTo(b.When()));
        prevSnap = h.Count > 0 ? h[h.Count - 1] : null;
        // Неделя: последний снимок старше 7 дней, иначе самый ранний
        weekSnap = WeekBase(h, now, s => true);
        weekStarSnap = WeekBase(h, now, s => s.HasStars());
        foreach (var r in acc.Repos)
        {
            r.NewDownloads = Delta(prevSnap == null ? null : prevSnap.Repos, r.Name, r.Downloads);
            r.NewStars = Delta(prevSnap == null ? null : prevSnap.RepoStars, r.Name, r.Stars);
            r.WeekDownloads = Delta(weekSnap == null ? null : weekSnap.Repos, r.Name, r.Downloads);
            r.WeekStars = Delta(weekStarSnap == null ? null : weekStarSnap.RepoStars, r.Name, r.Stars);
        }
        if (prevSnap == null || Changed(prevSnap, cur)) h.Add(cur);
        SaveHistory(acc.User, h);
        hist = h;
        curSnap = cur;

        // Окно графиков: 3 месяца или с первой проверки
        chartFrom = now.AddDays(-90);
        if (h.Count > 0 && h[0].When() > chartFrom) chartFrom = h[0].When();
        if ((now - chartFrom).TotalHours < 24) chartFrom = now.AddDays(-1);
        foreach (var r in acc.Repos)
        {
            string name = r.Name;
            r.Trend = HistSeries(s => { long v; return s.Repos.TryGetValue(name, out v) ? v : -1; }, r.Downloads, now.AddDays(-90));
        }

        FillList();
        if (list.Items.Count > 0 && list.SelectedItems.Count == 0) list.Items[0].Selected = true;
        FillReleaseCombo();

        UpdateStatus();
        InvalidatePages();
    }

    static Snapshot WeekBase(List<Snapshot> h, DateTime now, Func<Snapshot, bool> ok)
    {
        var c = h.Where(ok).ToList();
        return c.LastOrDefault(s => s.When() <= now.AddDays(-7)) ?? c.FirstOrDefault();
    }

    static long? Delta(Dictionary<string, long> old, string name, long now)
    {
        long v;
        if (old == null || !old.TryGetValue(name, out v)) return null;
        return now - v;
    }

    static bool Changed(Snapshot a, Snapshot b)
    {
        return a.Total != b.Total || a.Stars != b.Stars || a.Forks != b.Forks || a.Followers != b.Followers ||
               !a.Repos.OrderBy(p => p.Key).SequenceEqual(b.Repos.OrderBy(p => p.Key)) ||
               !a.RepoStars.OrderBy(p => p.Key).SequenceEqual(b.RepoStars.OrderBy(p => p.Key));
    }

    // Точки для графика из истории: значение < 0 — в снимке нет данных
    List<KeyValuePair<DateTime, double>> HistSeries(Func<Snapshot, double> sel, double current, DateTime from)
    {
        var pts = new List<KeyValuePair<DateTime, double>>();
        double before = -1;
        foreach (var s in hist)
        {
            double v = sel(s);
            if (v < 0) continue;
            DateTime t = s.When();
            if (t < from) { before = v; continue; }
            pts.Add(new KeyValuePair<DateTime, double>(t, v));
        }
        if (before >= 0) pts.Insert(0, new KeyValuePair<DateTime, double>(from, before));
        if (pts.Count == 0 || pts[pts.Count - 1].Key < loadedAt) pts.Add(new KeyValuePair<DateTime, double>(loadedAt, current));
        return pts;
    }

    // ——— Боковое меню ———

    int RenderSide(Painter p)
    {
        int x = D(16), w = p.W - D(28);
        p.Text(data != null ? data.User : userBox.Text.Trim(), Theme.SideName, Color.White, x, D(18), w);
        string upd = loading ? T("загрузка…", "loading…") : loadFailed ? T("ошибка загрузки", "load failed") : data == null ? "" :
            T("обновлено ", "updated ") + loadedAt.ToString(loadedAt.Date == DateTime.Today ? "HH:mm" : "dd.MM HH:mm");
        p.Text(upd, Theme.Small, loadFailed && !loading ? Theme.DonateHover : Theme.SideMuted, x, D(42), w);

        int y = D(84), ih = D(38);
        for (int i = 0; i < PageNames.Length; i++)
        {
            var r = new Rectangle(0, y + i * ih, p.W, ih);
            bool act = i == page;
            if (act) p.Fill(r, Theme.SideActive, 0);
            else if (p.Hover(r)) p.Fill(r, Theme.SideHover, 0);
            if (act) p.Fill(new Rectangle(0, r.Y, D(3), r.Height), Theme.Accent, 0);
            p.Text(PageNames[i], act ? Theme.Bold : Theme.Body, act ? Color.White : Theme.SideText, new Rectangle(x, r.Y, p.W - x - D(4), r.Height));
            int k = i;
            p.Link(r, delegate { SelectPage(k); }, null);
        }

        var donate = new Rectangle(D(14), p.H - D(16) - D(26), p.W - D(28), D(26));
        p.Fill(donate, p.Hover(donate) ? Theme.DonateHover : Theme.Donate, D(13));
        p.Text(T("Помощь проекту", "Support the project"), Theme.SmallBold, Color.White, donate, TextFormatFlags.HorizontalCenter);
        p.Link(donate, delegate { TokenForm.Open(DonateUrl); }, T("Поддержать автора программы — откроется страница в браузере", "Support the author — opens a page in your browser"));

        var tok = new Rectangle(0, donate.Y - D(12) - D(34), p.W, D(34));
        if (p.Hover(tok)) p.Fill(tok, Theme.SideHover, 0);
        p.Text(tokenSet ? T("Токен ✓", "Token ✓") : T("Токен…", "Token…"), Theme.Body, Theme.SideText, new Rectangle(x, tok.Y, p.W - x - D(4), tok.Height));
        p.Link(tok, EditToken, T("Необязательно. С токеном видно просмотры и клонирования ваших проектов\nи снимается лимит 60 запросов к GitHub в час.",
                                 "Optional. With a token you can see views and clones of your projects\nand the limit of 60 GitHub requests per hour is lifted."));

        var lng = new Rectangle(0, tok.Y - D(34), p.W, D(34));
        if (p.Hover(lng)) p.Fill(lng, Theme.SideHover, 0);
        p.Text(Lang.En ? "Русский" : "English", Theme.Body, Theme.SideText, new Rectangle(x, lng.Y, p.W - x - D(4), lng.Height));
        p.Link(lng, SwitchLanguage, "Switch language / Сменить язык");
        return 0;
    }

    // ——— Общие детали страниц ———

    int Placeholder(Painter p)
    {
        string s = loading ? T("Загружаю данные с GitHub…", "Loading data from GitHub…") : T("Данных нет. Проверьте имя аккаунта и нажмите «Обновить».", "No data. Check the account name and press \"Refresh\".");
        p.Text(s, Theme.Body, Theme.Muted, new Rectangle(0, D(40), p.W, D(30)), TextFormatFlags.HorizontalCenter);
        return 0;
    }

    static List<Rectangle> Grid(int width, int y, int n, int h, int minW)
    {
        int pad = D(20), gap = D(14);
        int per = Math.Max(1, Math.Min(n, (width - 2 * pad + gap) / (minW + gap)));
        int w = (width - 2 * pad - (per - 1) * gap) / per;
        var list = new List<Rectangle>();
        for (int i = 0; i < n; i++) list.Add(new Rectangle(pad + (i % per) * (w + gap), y + (i / per) * (h + gap), w, h));
        return list;
    }

    static int BottomOf(List<Rectangle> rs) { return rs.Max(r => r.Bottom); }

    static void Tile(Painter p, Rectangle r, string caption, string value, Color vc, string hint, Color hc, string tip)
    {
        p.Card(r);
        int x = r.X + D(16), w = r.Width - D(28);
        p.Text(caption, Theme.Body, Theme.Muted, x, r.Y + D(12), w);
        p.Text(value, Theme.Big, vc, x, r.Y + D(32), w);
        p.Text(hint, Theme.Small, hc, x, r.Bottom - D(28), w);
        if (tip != null) p.Tip(r, tip);
    }

    // Заголовок карточки; возвращает y начала содержимого
    static int Head(Painter p, Rectangle r, string title, string sub, int reserve)
    {
        int x = r.X + D(16), w = r.Width - D(32) - reserve;
        p.Text(title, Theme.Head, Theme.Text, x, r.Y + D(12), w);
        if (!string.IsNullOrEmpty(sub))
        {
            int tw = p.Measure(title, Theme.Head);
            p.Text(sub, Theme.Small, Theme.Muted, new Rectangle(x + tw, r.Y + D(13), w - tw, Theme.Head.Height));
        }
        return r.Y + D(46);
    }

    static int Legend(Painter p, int right, int y, List<Series> ss)
    {
        int x = right;
        for (int i = ss.Count - 1; i >= 0; i--)
        {
            int tw = p.Measure(ss[i].Name, Theme.Small);
            x -= tw;
            p.Text(ss[i].Name, Theme.Small, Theme.Muted, new Rectangle(x, y, tw, Theme.Head.Height));
            x -= D(12);
            p.Fill(new Rectangle(x, y + Theme.Head.Height / 2 - D(4), D(10), D(10)), ss[i].Color, D(3));
            x -= D(14);
        }
        return right - x;
    }

    // Подбор «круглого» шага оси: n делений, шаг целый
    static void NiceAxis(double min, double max, int n, out double lo, out double step)
    {
        if (max - min < 1) { min = Math.Max(0, min - n / 2); max = min + n; }
        double raw = (max - min) / n;
        double mag = Math.Pow(10, Math.Floor(Math.Log10(raw)));
        foreach (double k in new[] { 1, 2, 2.5, 5, 10, 20, 25, 50, 100 })
        {
            step = Math.Max(1, Math.Round(k * mag, 6));
            lo = Math.Floor(min / step) * step;
            if (lo + n * step >= max - 1e-9) return;
        }
        step = Math.Ceiling(raw);
        lo = Math.Floor(min);
    }

    static void Chart(Painter p, Rectangle area, DateTime x0, DateTime x1, List<Series> ss, string axisFmt, string tipFmt)
    {
        var g = p.G;
        var left = ss.Where(s => !s.Right && s.Points.Count > 0).ToList();
        var right = ss.Where(s => s.Right && s.Points.Count > 0).ToList();
        int lw = D(60), rw = right.Count > 0 ? D(48) : D(12), bh = D(24);
        var plot = new Rectangle(area.X + lw, area.Y + D(8), area.Width - lw - rw, area.Height - D(8) - bh);
        if (plot.Width < 60 || plot.Height < 40) return;
        const int n = 4;
        double llo = 0, lstep = 1, rlo = 0, rstep = 1;
        if (left.Count > 0) NiceAxis(left.Min(s => s.Points.Min(q => q.Value)), left.Max(s => s.Points.Max(q => q.Value)), n, out llo, out lstep);
        if (right.Count > 0) NiceAxis(right.Min(s => s.Points.Min(q => q.Value)), right.Max(s => s.Points.Max(q => q.Value)), n, out rlo, out rstep);

        using (var grid = new Pen(Theme.Grid))
            for (int i = 0; i <= n; i++)
            {
                int yy = plot.Bottom - i * plot.Height / n;
                g.DrawLine(grid, plot.X, yy, plot.Right, yy);
                if (left.Count > 0)
                    p.Text((llo + i * lstep).ToString("N0"), Theme.Small, left[0].Color, new Rectangle(area.X, yy - D(10), lw - D(8), D(20)), TextFormatFlags.Right);
                if (right.Count > 0)
                    p.Text((rlo + i * rstep).ToString("N0"), Theme.Small, right[0].Color, new Rectangle(plot.Right + D(6), yy - D(10), rw - D(6), D(20)));
            }

        double span = Math.Max(1, (x1 - x0).TotalSeconds);
        for (int i = 0; i <= 4; i++)
        {
            DateTime t = x0.AddSeconds(span * i / 4);
            int xx = plot.X + i * plot.Width / 4, lab = D(100);
            Rectangle lr = i == 0 ? new Rectangle(xx, plot.Bottom + D(4), lab, D(18))
                         : i == 4 ? new Rectangle(xx - lab, plot.Bottom + D(4), lab, D(18))
                         : new Rectangle(xx - lab / 2, plot.Bottom + D(4), lab, D(18));
            p.Text(t.ToString(axisFmt), Theme.Small, Theme.Muted, lr, i == 0 ? TextFormatFlags.Left : i == 4 ? TextFormatFlags.Right : TextFormatFlags.HorizontalCenter);
        }

        foreach (var s in ss)
        {
            if (s.Points.Count == 0) continue;
            double lo = s.Right ? rlo : llo, st = s.Right ? rstep : lstep;
            var pts = s.Points.Select(q => new PointF(
                plot.X + (float)((q.Key - x0).TotalSeconds / span * plot.Width),
                plot.Bottom - (float)((q.Value - lo) / (n * st) * plot.Height))).ToArray();
            if (pts.Length >= 2)
            {
                if (s.Fill)
                {
                    var poly = new List<PointF>(pts);
                    poly.Add(new PointF(pts[pts.Length - 1].X, plot.Bottom));
                    poly.Add(new PointF(pts[0].X, plot.Bottom));
                    using (var b = new SolidBrush(Color.FromArgb(28, s.Color))) g.FillPolygon(b, poly.ToArray());
                }
                using (var pen = new Pen(s.Color, D(2.2)) { LineJoin = LineJoin.Round }) g.DrawLines(pen, pts);
            }
            bool marks = pts.Length <= 40;
            for (int i = 0; i < pts.Length; i++)
            {
                if (marks || i == pts.Length - 1)
                    using (var b = new SolidBrush(s.Color)) g.FillEllipse(b, pts[i].X - D(3), pts[i].Y - D(3), D(6), D(6));
                if (pts.Length <= 200)
                    p.Tip(new Rectangle((int)pts[i].X - D(6), (int)pts[i].Y - D(6), D(12), D(12)),
                        s.Points[i].Key.ToString(tipFmt) + " — " + s.Name.ToLowerInvariant() + ": " + s.Points[i].Value.ToString("N0"));
            }
        }
    }

    static string Signed(long d) { return d > 0 ? "+" + d.ToString("N0") : d.ToString("N0"); }

    static string Pct(long part, long total)
    {
        if (total <= 0) return "";
        double v = 100.0 * part / total;
        return v > 0 && v < 1 ? "<1%" : v.ToString("0") + "%";
    }

    string Since(Snapshot s)
    {
        return s.When() <= loadedAt.AddDays(-7) ? T("с ", "since ") + s.When().ToString("dd.MM") : T("с ", "since ") + s.When().ToString("dd.MM HH:mm") + T(", история короче", ", shorter history");
    }

    bool FullWeek(Snapshot s) { return s.When() <= loadedAt.AddDays(-7); }

    static string Ago(DateTime d)
    {
        int days = (int)(DateTime.Today - d.Date).TotalDays;
        if (days <= 0) return T("сегодня", "today");
        if (days == 1) return T("вчера", "yesterday");
        if (days < 31) return days + " " + Pl(days, "день", "дня", "дней", "day", "days") + T(" назад", " ago");
        if (days < 365) return (days / 30) + T(" мес. назад", " mo. ago");
        int y = days / 365;
        return y + " " + Pl(y, "год", "года", "лет", "year", "years") + T(" назад", " ago");
    }

    // ——— Обзор ———

    int RenderOverview(Painter p)
    {
        if (data == null) return Placeholder(p);
        int pad = D(20), gap = D(14), y = D(4);
        long total = curSnap.Total;
        int withRel = data.Repos.Count(r => r.Releases.Count > 0);

        var t = Grid(p.W, y, 4, D(104), D(170));
        long? nw = prevSnap == null ? (long?)null : total - prevSnap.Total;
        if (nw > 0) Tile(p, t[0], T("Скачиваний", "Downloads"), total.ToString("N0"), Theme.Text, "+" + nw.Value.ToString("N0") + T(" с прошлой проверки", " since last check"), Theme.Good, TipDown);
        else Tile(p, t[0], T("Скачиваний", "Downloads"), total.ToString("N0"), Theme.Text, string.Format(T("файлы релизов, {0} {1}", "release files, {0} {1}"), withRel, Pl(withRel, "проект", "проекта", "проектов", "project", "projects")), Theme.Muted, TipDown);

        if (weekSnap == null) Tile(p, t[1], T("За неделю", "This week"), "—", Theme.Muted, T("после следующей проверки", "after the next check"), Theme.Muted, TipWeek);
        else
        {
            long d = total - weekSnap.Total;
            Tile(p, t[1], T("За неделю", "This week"), Signed(d), d > 0 ? Theme.Good : Theme.Text, Since(weekSnap), Theme.Muted, TipWeek);
        }

        string forks = T("форков ", "forks ") + curSnap.Forks.ToString("N0");
        if (weekStarSnap == null) Tile(p, t[2], T("Звёзд", "Stars"), curSnap.Stars.ToString("N0"), Theme.Text, forks, Theme.Muted, TipStars);
        else
        {
            long d = curSnap.Stars - weekStarSnap.Stars;
            string per = FullWeek(weekStarSnap) ? T(" за неделю", " this week") : T(" с ", " since ") + weekStarSnap.When().ToString("dd.MM");
            Tile(p, t[2], T("Звёзд", "Stars"), curSnap.Stars.ToString("N0"), Theme.Text, (d != 0 ? Signed(d) + per : T("без изменений", "no change") + per) + " · " + forks, d > 0 ? Theme.Good : Theme.Muted, TipStars);
        }

        if (weekStarSnap == null || weekStarSnap.Followers < 0) Tile(p, t[3], T("Подписчиков", "Followers"), data.Followers.ToString("N0"), Theme.Text, T("подписаны на аккаунт", "follow the account"), Theme.Muted, TipFollowers);
        else
        {
            long d = data.Followers - weekStarSnap.Followers;
            string per = FullWeek(weekStarSnap) ? T(" за неделю", " this week") : T(" с ", " since ") + weekStarSnap.When().ToString("dd.MM");
            Tile(p, t[3], T("Подписчиков", "Followers"), data.Followers.ToString("N0"), Theme.Text, d != 0 ? Signed(d) + per : T("без изменений", "no change") + per, d > 0 ? Theme.Good : Theme.Muted, TipFollowers);
        }
        y = BottomOf(t) + gap;

        // График роста
        var ss = new List<Series>
        {
            new Series { Name = T("Скачивания", "Downloads"), Color = Theme.Blue, Fill = true, Points = HistSeries(s => s.Total, curSnap.Total, chartFrom) },
            new Series { Name = T("Звёзды", "Stars"), Color = Theme.Amber, Right = true, Points = HistSeries(s => s.Stars, curSnap.Stars, chartFrom) }
        };
        var card = new Rectangle(pad, y, p.W - 2 * pad, D(310));
        p.Card(card);
        int lg = Legend(p, card.Right - D(16), card.Y + D(12), ss);
        Head(p, card, T("Рост за 3 месяца", "Growth over 3 months"), hist.Count > 0 && hist[0].When() > loadedAt.AddDays(-90)
            ? T("   по проверкам программы с ", "   from the program's checks since ") + hist[0].When().ToString("dd.MM.yyyy") : T("   по проверкам программы", "   from the program's checks"), lg + D(12));
        var area = new Rectangle(card.X + D(10), card.Y + D(48), card.Width - D(24), card.Height - D(60));
        if (ss[0].Points.Count < 2)
            p.Text(T("График появится после следующей проверки: программа запоминает счётчики при каждом обновлении.", "The chart will appear after the next check: the program saves the counters on every refresh."), Theme.Body, Theme.Muted, area, TextFormatFlags.HorizontalCenter);
        else
            Chart(p, area, chartFrom, loadedAt, ss, (loadedAt - chartFrom).TotalDays < 3 ? "dd.MM HH:mm" : "dd.MM", "dd.MM.yyyy HH:mm");
        y = card.Bottom + gap;

        int rh = D(32), rows = 5;
        var two = Grid(p.W, y, 2, D(48) + rows * rh + D(10), D(330));
        GrowthCard(p, two[0], rh, rows);
        FreshCard(p, two[1], rh, rows);
        return BottomOf(two) + pad;
    }

    void GrowthCard(Painter p, Rectangle r, int rh, int rows)
    {
        p.Card(r);
        int y = Head(p, r, T("Быстрее всех растут", "Fastest growing"), weekSnap == null ? null : "   " + (FullWeek(weekSnap) ? T("за неделю", "this week") : T("с ", "since ") + weekSnap.When().ToString("dd.MM HH:mm")), 0);
        var grow = data.Repos.Select(x => new { R = x, Dl = x.WeekDownloads ?? 0, St = x.WeekStars ?? 0 })
            .Where(x => x.Dl > 0 || x.St > 0).OrderByDescending(x => x.Dl).ThenByDescending(x => x.St).Take(rows).ToList();
        int x0 = r.X + D(16), iw = r.Width - D(32);
        if (grow.Count == 0)
        {
            p.Para(weekSnap == null ? T("Прирост появится после следующей проверки.", "Growth will appear after the next check.") : T("За этот период новых скачиваний и звёзд нет.", "No new downloads or stars in this period."), Theme.Body, Theme.Muted, x0, y + D(4), iw);
            return;
        }
        long max = Math.Max(1, grow.Max(x => x.Dl));
        foreach (var g in grow)
        {
            var row = new Rectangle(r.X + D(6), y, r.Width - D(12), rh);
            if (p.Hover(row)) p.Fill(row, Theme.RowHover, D(6));
            p.Text(g.R.Name, Theme.Bold, Theme.Text, new Rectangle(x0, y, iw * 40 / 100, rh));
            p.Bar(new Rectangle(x0 + iw * 42 / 100, y + rh / 2 - D(4), iw * 26 / 100, D(8)), (double)g.Dl / max, Theme.Blue);
            p.Right(g.Dl > 0 ? "+" + g.Dl.ToString("N0") + T(" скач.", " dl.") : "", Theme.Body, Theme.Good, new Rectangle(x0 + iw * 68 / 100, y, iw * 20 / 100, rh));
            p.Right(g.St > 0 ? "+" + g.St + " ★" : "", Theme.Body, Theme.Amber, new Rectangle(x0 + iw * 88 / 100, y, iw * 12 / 100, rh));
            string url = g.R.Url;
            p.Link(row, delegate { TokenForm.Open(url); }, T("Открыть проект на GitHub", "Open the project on GitHub"));
            y += rh;
        }
    }

    void FreshCard(Painter p, Rectangle r, int rh, int rows)
    {
        p.Card(r);
        int y = Head(p, r, T("Свежие релизы", "Latest releases"), null, 0);
        var fresh = (from x in data.Repos from rel in x.Releases orderby rel.Published descending select new { R = x, Rel = rel }).Take(rows).ToList();
        int x0 = r.X + D(16), iw = r.Width - D(32);
        if (fresh.Count == 0) { p.Para(T("Релизов пока нет.", "No releases yet."), Theme.Body, Theme.Muted, x0, y + D(4), iw); return; }
        foreach (var f in fresh)
        {
            var row = new Rectangle(r.X + D(6), y, r.Width - D(12), rh);
            if (p.Hover(row)) p.Fill(row, Theme.RowHover, D(6));
            int nameW = Math.Min(p.Measure(f.R.Name, Theme.Bold), iw * 38 / 100);
            p.Text(f.R.Name, Theme.Bold, Theme.Text, new Rectangle(x0, y, nameW, rh));
            p.Text(f.Rel.Tag + (f.Rel.Prerelease ? T(" · пре-релиз", " · pre-release") : ""), Theme.Body, Theme.Muted, new Rectangle(x0 + nameW, y, iw * 58 / 100 - nameW, rh));
            p.Right(Ago(f.Rel.Published), Theme.Small, Theme.Muted, new Rectangle(x0 + iw * 58 / 100, y, iw * 22 / 100, rh));
            p.Right(f.Rel.Downloads.ToString("N0") + T(" скач.", " dl."), Theme.Body, Theme.Text, new Rectangle(x0 + iw * 80 / 100, y, iw * 20 / 100, rh));
            string url = f.R.Url + "/releases/tag/" + Uri.EscapeDataString(f.Rel.Tag);
            p.Link(row, delegate { TokenForm.Open(url); }, string.Format(T("{0} {1} — опубликован {2:dd.MM.yyyy}. Открыть релиз на GitHub", "{0} {1} — published {2:dd.MM.yyyy}. Open the release on GitHub"), f.R.Name, f.Rel.Tag, f.Rel.Published));
            y += rh;
        }
    }

    // ——— Проекты ———

    void FillList()
    {
        if (data == null) return;
        bool traffic = data.Repos.Any(r => r.HasTraffic);
        int cols = traffic ? ColNames.Length : ColNames.Length - 1;
        if (sortCol >= cols) { sortCol = 1; sortDesc = true; }
        if (list.Columns.Count != cols)
        {
            list.Columns.Clear();
            for (int i = 0; i < cols; i++)
                list.Columns.Add(ColNames[i], D(ColWidths[i]), i == 0 || i == 7 || i == 9 || i == TrendCol ? HorizontalAlignment.Left : HorizontalAlignment.Right);
        }
        for (int i = 0; i < cols; i++)
            list.Columns[i].Text = ColNames[i] + (i == sortCol ? (sortDesc ? " ▼" : " ▲") : "");

        var repos = new List<RepoStat>(data.Repos);
        repos.Sort((a, b) =>
        {
            int c = Key(a, sortCol).CompareTo(Key(b, sortCol));
            if (sortDesc) c = -c;
            if (c == 0) c = b.Downloads.CompareTo(a.Downloads);
            if (c == 0) c = b.Stars.CompareTo(a.Stars);
            return c != 0 ? c : string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        });

        string selected = list.SelectedItems.Count > 0 ? ((RepoStat)list.SelectedItems[0].Tag).Name : null;
        list.BeginUpdate();
        list.Items.Clear();
        foreach (var r in repos)
        {
            bool rel = r.Releases.Count > 0;
            var it = new ListViewItem(r.Name) { UseItemStyleForSubItems = false, Tag = r };
            it.SubItems.Add(rel ? r.Downloads.ToString("N0") : "—");
            var nw = it.SubItems.Add(!rel || r.NewDownloads == null ? "—" : Signed(r.NewDownloads.Value));
            if (r.NewDownloads > 0) nw.ForeColor = Theme.Good;
            it.SubItems.Add("");
            it.SubItems.Add(r.Stars.ToString("N0"));
            it.SubItems.Add(r.Forks.ToString("N0"));
            it.SubItems.Add(r.Issues.ToString("N0"));
            it.SubItems.Add(r.Language ?? "—");
            it.SubItems.Add(r.Releases.Count.ToString());
            it.SubItems.Add(rel ? r.Releases[0].Tag + " · " + r.Releases[0].Published.ToString("dd.MM.yyyy") : T("нет релизов", "no releases"));
            if (traffic) it.SubItems.Add(r.HasTraffic ? r.Views.ToString("N0") : "—");
            if (!rel)
                foreach (ListViewItem.ListViewSubItem s in it.SubItems) if (s != nw) s.ForeColor = Theme.Muted;
            list.Items.Add(it);
            if (r.Name == selected) it.Selected = true;
        }
        list.EndUpdate();
    }

    static IComparable Key(RepoStat r, int col)
    {
        switch (col)
        {
            case 0: return r.Name.ToLowerInvariant();
            case 1: return r.Downloads;
            case 2: return r.NewDownloads ?? -1L;
            case 3: return r.WeekDownloads ?? -1L;
            case 4: return r.Stars;
            case 5: return r.Forks;
            case 6: return r.Issues;
            case 7: return (r.Language ?? "").ToLowerInvariant();
            case 8: return r.Releases.Count;
            case 9: return r.Releases.Count > 0 ? r.Releases[0].Published : DateTime.MinValue;
            default: return r.Views;
        }
    }

    // Столбец «Тренд» — мини-график скачиваний за 3 месяца по истории проверок
    void DrawListCell(object sender, DrawListViewSubItemEventArgs e)
    {
        if (e.ColumnIndex != TrendCol) { e.DrawDefault = true; return; }
        var r = (RepoStat)e.Item.Tag;
        bool sel = e.Item.Selected, focus = sel && list.Focused;
        using (var b = new SolidBrush(focus ? SystemColors.Highlight : sel ? SystemColors.Control : list.BackColor)) e.Graphics.FillRectangle(b, e.Bounds);
        var box = Rectangle.Inflate(e.Bounds, -D(8), -D(5));
        var pts = r.Trend;
        if (r.Releases.Count == 0 || pts.Count < 2)
        {
            TextRenderer.DrawText(e.Graphics, "—", list.Font, box, focus ? Color.White : Theme.Muted, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            return;
        }
        double min = pts.Min(q => q.Value), max = pts.Max(q => q.Value);
        double t0 = pts[0].Key.Ticks, span = Math.Max(1, pts[pts.Count - 1].Key.Ticks - t0);
        var arr = pts.Select(q => new PointF(
            box.X + (float)((q.Key.Ticks - t0) / span * box.Width),
            max > min ? box.Bottom - (float)((q.Value - min) / (max - min) * box.Height) : box.Y + box.Height / 2f)).ToArray();
        var old = e.Graphics.SmoothingMode;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        Color c = focus ? Color.White : max > min ? Theme.Good : Theme.Muted;
        using (var pen = new Pen(c, D(1.6)) { LineJoin = LineJoin.Round }) e.Graphics.DrawLines(pen, arr);
        using (var b = new SolidBrush(c)) e.Graphics.FillEllipse(b, arr[arr.Length - 1].X - D(2), arr[arr.Length - 1].Y - D(2), D(4), D(4));
        e.Graphics.SmoothingMode = old;
    }

    void ShowDetails(RepoStat r)
    {
        info.Clear();
        Line(r.Name, infoTitle, Theme.Text);
        if (!string.IsNullOrEmpty(r.Description)) Line(r.Description, infoFont, Theme.Muted);
        Line("", infoFont, Theme.Text);
        Line(string.Format(T("Язык: {0} · создан {1:dd.MM.yyyy} · последнее изменение кода {2:dd.MM.yyyy}", "Language: {0} · created {1:dd.MM.yyyy} · last code change {2:dd.MM.yyyy}"), r.Language ?? "—", r.Created, r.Pushed), infoFont, Theme.Text);
        string starGrowth = "";
        if (r.NewStars != null && r.NewStars != 0) starGrowth += T(", с прошлой проверки ", ", since last check ") + Signed(r.NewStars.Value);
        if (r.WeekStars != null && r.WeekStars != 0) starGrowth += ", " + (weekStarSnap != null && FullWeek(weekStarSnap) ? T("за неделю ", "this week ") : T("с ", "since ") + weekStarSnap.When().ToString("dd.MM") + " ") + Signed(r.WeekStars.Value);
        Line(string.Format(T("Звёзд: {0:N0}{1} · форков: {2:N0} · открытых задач (Issues и запросы на слияние): {3:N0}", "Stars: {0:N0}{1} · forks: {2:N0} · open issues (Issues and pull requests): {3:N0}"), r.Stars, starGrowth, r.Forks, r.Issues), infoFont, Theme.Text);

        Line("", infoFont, Theme.Text);
        Line(T("Скачивания файлов из релизов", "Downloads of release files"), infoBold, Theme.Text);
        if (r.Releases.Count == 0)
            Line(T("В проекте нет релизов — скачивать нечего, поэтому счётчика нет.", "The project has no releases — nothing to download, so there is no counter."), infoFont, Theme.Muted);
        else
        {
            Line(string.Format(T("Всего: {0:N0} в {1} {2}", "Total: {0:N0} in {1} {2}"), r.Downloads, r.Releases.Count, Pl(r.Releases.Count, "релизе", "релизах", "релизах", "release", "releases")), infoFont, Theme.Text);
            if (r.NewDownloads != null)
                Line(T("С прошлой проверки: ", "Since last check: ") + Signed(r.NewDownloads.Value), infoFont, r.NewDownloads > 0 ? Theme.Good : Theme.Text);
            if (r.WeekDownloads != null && weekSnap != null)
                Line((FullWeek(weekSnap) ? T("За неделю: ", "This week: ") : T("С ", "Since ") + weekSnap.When().ToString("dd.MM HH:mm") + ": ") + Signed(r.WeekDownloads.Value), infoFont, r.WeekDownloads > 0 ? Theme.Good : Theme.Text);
            DateTime first = r.Releases.Min(x => x.Published);
            double days = Math.Max(1, (DateTime.Now - first).TotalDays);
            Line(string.Format(T("В среднем: {0:N1} в день с первого релиза ({1:dd.MM.yyyy})", "Average: {0:N1} per day since the first release ({1:dd.MM.yyyy})"), r.Downloads / days, first), infoFont, Theme.Text);
            var latest = r.Releases[0];
            long ld = (long)Math.Ceiling(Math.Max(1, (DateTime.Now - latest.Published).TotalDays));
            Line(string.Format(T("Последний релиз {0}: {1:N0} за {2:N0} {3}", "Latest release {0}: {1:N0} in {2:N0} {3}"), latest.Tag, latest.Downloads, ld, Pl(ld, "день", "дня", "дней", "day", "days")), infoFont, Theme.Text);
            var top = r.Releases.SelectMany(x => x.Assets.Select(a => new { x.Tag, a })).OrderByDescending(x => x.a.Downloads).FirstOrDefault();
            if (top != null && top.a.Downloads > 0)
                Line(string.Format(T("Чаще всего качают: {0} ({1}) — {2:N0}", "Most downloaded: {0} ({1}) — {2:N0}"), top.a.Name, top.Tag, top.a.Downloads), infoFont, Theme.Text);
            Line(T("Все файлы по релизам — в разделе «Релизы и файлы».", "All files by release are in the \"Releases & files\" section."), infoFont, Theme.Muted);
        }

        Line("", infoFont, Theme.Text);
        Line(T("Посещаемость за 14 дней", "Traffic for 14 days"), infoBold, Theme.Text);
        if (r.HasTraffic)
        {
            Line(string.Format(T("Просмотров страницы: {0:N0}, разных посетителей: {1:N0}", "Page views: {0:N0}, unique visitors: {1:N0}"), r.Views, r.UniqueViews), infoFont, Theme.Text);
            Line(string.Format(T("Клонирований (git clone): {0:N0}, разных: {1:N0}", "Clones (git clone): {0:N0}, unique: {1:N0}"), r.Clones, r.UniqueClones), infoFont, Theme.Text);
            if (r.Referrers.Count > 0)
            {
                Line(T("Откуда приходят:", "Where visitors come from:"), infoFont, Theme.Text);
                foreach (var s in r.Referrers) Line(string.Format(T("   {0} — {1:N0} просмотров, {2:N0} человек", "   {0} — {1:N0} views, {2:N0} people"), s.Name, s.Count, s.Uniques), infoFont, Theme.Text);
            }
        }
        else if (data.TrafficDenied)
            Line(T("Токен есть, но GitHub не дал доступ. Нужно право Administration: Read-only для этого проекта.", "A token is set, but GitHub denied access. The Administration: Read-only permission is needed for this project."), infoFont, Theme.Muted);
        else
            Line(T("GitHub показывает это только владельцу. Добавьте токен кнопкой «Токен…» в меню слева.", "GitHub shows this only to the owner. Add a token with the \"Token…\" button in the left menu."), infoFont, Theme.Muted);
        info.SelectionStart = 0;
        info.ScrollToCaret();
    }

    void Line(string text, Font font, Color color)
    {
        info.SelectionStart = info.TextLength;
        info.SelectionFont = font;
        info.SelectionColor = color;
        info.AppendText(text + "\n");
    }

    // ——— Релизы и файлы ———

    void FillReleaseCombo()
    {
        var was = relCombo.SelectedItem as RepoStat;
        relCombo.BeginUpdate();
        relCombo.Items.Clear();
        relCombo.Items.Add(T("Все проекты", "All projects"));
        foreach (var r in data.Repos.Where(x => x.Releases.Count > 0).OrderByDescending(x => x.Downloads)) relCombo.Items.Add(r);
        int idx = 0;
        if (was != null)
            for (int i = 1; i < relCombo.Items.Count; i++)
                if (((RepoStat)relCombo.Items[i]).Name == was.Name) idx = i;
        relCombo.SelectedIndex = idx;
        relCombo.EndUpdate();
    }

    static string Ext(string name)
    {
        string n = name.ToLowerInvariant();
        foreach (var e in new[] { ".tar.gz", ".tar.xz", ".tar.bz2", ".tar.zst" })
            if (n.EndsWith(e)) return e;
        int dot = n.LastIndexOf('.');
        return dot <= 0 || dot == n.Length - 1 ? T("без расширения", "no extension") : n.Substring(dot);
    }

    static string Platform(string name)
    {
        string n = name.ToLowerInvariant();
        if (n.Contains("arm64") || n.Contains("aarch64")) return "arm64";
        if (n.Contains("x86_64") || n.Contains("x64") || n.Contains("amd64") || n.Contains("win64")) return "x64";
        if (n.Contains("armv7") || n.Contains("armhf") || n.Contains("arm32")) return "arm32";
        if (n.Contains("x86") || n.Contains("i386") || n.Contains("i686") || n.Contains("win32") || n.Contains("ia32")) return "x86";
        return T("не указана", "not specified");
    }

    int RenderReleases(Painter p)
    {
        if (data == null) return Placeholder(p);
        int pad = D(20), gap = D(14), y = D(6);
        var only = relCombo.SelectedItem as RepoStat;
        var repos = only != null ? new List<RepoStat> { only } : data.Repos.Where(r => r.Releases.Count > 0).OrderByDescending(r => r.Downloads).ToList();
        if (repos.Count == 0)
        {
            var c = new Rectangle(pad, y, p.W - 2 * pad, D(100));
            p.Card(c);
            int cy = Head(p, c, T("Релизов нет", "No releases"), null, 0);
            p.Para(T("Ни в одном проекте нет релизов (раздел Releases на GitHub), поэтому считать скачивания не из чего.", "None of the projects has releases (the Releases section on GitHub), so there are no downloads to count."), Theme.Body, Theme.Muted, c.X + D(16), cy, c.Width - D(32));
            return c.Bottom + pad;
        }
        var files = (from r in repos from rel in r.Releases from a in rel.Assets select new { R = r, Rel = rel, A = a }).ToList();
        long total = files.Sum(f => f.A.Downloads);
        long bytes = files.Sum(f => f.A.Downloads * f.A.Size);
        int relCount = repos.Sum(r => r.Releases.Count);

        var t = Grid(p.W, y, 3, D(104), D(170));
        Tile(p, t[0], T("Скачиваний файлов", "File downloads"), total.ToString("N0"), Theme.Text,
            only != null ? string.Format(T("в {0} {1}", "in {0} {1}"), relCount, Pl(relCount, "релизе", "релизах", "релизах", "release", "releases"))
                         : string.Format("{0} {1}, {2} {3}", repos.Count, Pl(repos.Count, "проект", "проекта", "проектов", "project", "projects"), relCount, Pl(relCount, "релиз", "релиза", "релизов", "release", "releases")),
            Theme.Muted, TipDown);
        var top = files.OrderByDescending(f => f.A.Downloads).FirstOrDefault();
        Tile(p, t[1], T("Файлов", "Files"), files.Count.ToString("N0"), Theme.Text, top != null && top.A.Downloads > 0 ? T("чаще всех: ", "most downloaded: ") + top.A.Name : T("в релизах", "in releases"), Theme.Muted,
            top != null && top.A.Downloads > 0 ? string.Format(T("Сколько файлов выложено в релизах.\nЧаще всех качают {0} ({1} {2}) — {3:N0}.", "How many files are published in releases.\nMost downloaded: {0} ({1} {2}) — {3:N0}."), top.A.Name, top.R.Name, top.Rel.Tag, top.A.Downloads) : T("Сколько файлов выложено в релизах.", "How many files are published in releases."));
        Tile(p, t[2], T("Отдано трафика", "Traffic served"), FormatSize(bytes), Theme.Text, T("скачивания × размер файла", "downloads × file size"), Theme.Muted,
            T("Сколько данных GitHub отдал при скачиваниях: для каждого файла число скачиваний умножено на его размер.\nЕсли файл в релизе заменяли, берётся нынешний размер.",
              "How much data GitHub served for downloads: for each file, the number of downloads multiplied by its size.\nIf a file in a release was replaced, its current size is used."));
        y = BottomOf(t) + gap;

        var byExt = files.GroupBy(f => Ext(f.A.Name)).Select(g => new ShareRow { Name = g.Key, Downloads = g.Sum(f => f.A.Downloads), Files = g.Count() })
            .OrderByDescending(s => s.Downloads).ThenBy(s => s.Name).ToList();
        var byPlat = files.GroupBy(f => Platform(f.A.Name)).Select(g => new ShareRow { Name = g.Key, Downloads = g.Sum(f => f.A.Downloads), Files = g.Count() })
            .OrderByDescending(s => s.Downloads).ThenBy(s => s.Name).ToList();
        int rows = Math.Min(8, Math.Max(1, Math.Max(byExt.Count, byPlat.Count))), rh = D(30);
        var two = Grid(p.W, y, 2, D(48) + rows * rh + D(10), D(300));
        ShareCard(p, two[0], T("По типам файлов", "By file type"), byExt, total, Theme.Blue, rows);
        ShareCard(p, two[1], T("По платформам", "By platform"), byPlat, total, Theme.Violet, rows);
        y = BottomOf(two) + gap;

        // Файлы по релизам
        var groups = (from r in repos from rel in r.Releases select new { R = r, Rel = rel }).ToList();
        int hh = D(38), fh = D(28);
        var card = new Rectangle(pad, y, p.W - 2 * pad, D(46) + D(26) + groups.Sum(g => hh + Math.Max(1, g.Rel.Assets.Count) * fh) + D(10));
        p.Card(card);
        Head(p, card, T("Файлы по релизам", "Files by release"), T("   полоска — доля от всех скачиваний проекта", "   the bar is the share of all the project's downloads"), 0);
        int x0 = card.X + D(16), iw = card.Width - D(32);
        int wName = iw * 34 / 100, cBar = x0 + wName + D(12), wBar = iw * 20 / 100;
        int cDl = x0 + iw * 56 / 100, wDl = iw * 12 / 100;
        int cPct = cDl + wDl, wPct = iw * 8 / 100;
        int cSize = cPct + wPct, wSize = iw * 11 / 100;
        int cTr = cSize + wSize, wTr = x0 + iw - cTr;
        int yy = card.Y + D(46);
        p.Text(T("Файл", "File"), Theme.Small, Theme.Muted, new Rectangle(x0, yy, wName, D(22)));
        p.Text(T("Доля", "Share"), Theme.Small, Theme.Muted, new Rectangle(cBar, yy, wBar, D(22)));
        p.Right(T("Скачиваний", "Downloads"), Theme.Small, Theme.Muted, new Rectangle(cDl, yy, wDl, D(22)));
        p.Right("%", Theme.Small, Theme.Muted, new Rectangle(cPct, yy, wPct, D(22)));
        p.Right(T("Размер", "Size"), Theme.Small, Theme.Muted, new Rectangle(cSize, yy, wSize, D(22)));
        p.Right(T("Трафик", "Traffic"), Theme.Small, Theme.Muted, new Rectangle(cTr, yy, wTr, D(22)));
        p.Tip(new Rectangle(cTr, yy, wTr, D(22)), T("Скачивания × размер файла", "Downloads × file size"));
        yy += D(26);
        foreach (var gr in groups)
        {
            var hr = new Rectangle(card.X + D(8), yy + D(4), card.Width - D(16), hh - D(8));
            p.Fill(hr, p.Hover(hr) ? Theme.RowHeadHover : Theme.RowHead, D(6));
            string tag = (only == null ? gr.R.Name + "  ·  " : "") + gr.Rel.Tag;
            int tw = Math.Min(p.Measure(tag, Theme.Bold), iw / 2);
            p.Text(tag, Theme.Bold, Theme.Text, new Rectangle(x0, hr.Y, tw, hr.Height));
            p.Text(string.Format("  ·  {0:dd.MM.yyyy}  ·  {1:N0} {2}{3}", gr.Rel.Published, gr.Rel.Downloads, Pl(gr.Rel.Downloads, "скачивание", "скачивания", "скачиваний", "download", "downloads"), gr.Rel.Prerelease ? T("  ·  пре-релиз", "  ·  pre-release") : ""),
                Theme.Body, Theme.Muted, new Rectangle(x0 + tw, hr.Y, iw - tw, hr.Height));
            string url = gr.R.Url + "/releases/tag/" + Uri.EscapeDataString(gr.Rel.Tag);
            p.Link(hr, delegate { TokenForm.Open(url); }, T("Открыть релиз на GitHub", "Open the release on GitHub"));
            yy += hh;
            if (gr.Rel.Assets.Count == 0)
            {
                p.Text(T("(файлов нет)", "(no files)"), Theme.Body, Theme.Muted, new Rectangle(x0, yy, wName, fh));
                yy += fh;
                continue;
            }
            foreach (var a in gr.Rel.Assets)
            {
                var fr = new Rectangle(card.X + D(8), yy, card.Width - D(16), fh);
                if (p.Hover(fr)) p.Fill(fr, Theme.RowHover, D(4));
                double share = gr.R.Downloads > 0 ? (double)a.Downloads / gr.R.Downloads : 0;
                p.Text(a.Name, Theme.Body, Theme.Text, new Rectangle(x0, yy, wName, fh));
                p.Bar(new Rectangle(cBar, yy + fh / 2 - D(4), wBar, D(8)), share, Theme.Blue);
                p.Right(a.Downloads.ToString("N0"), Theme.Body, Theme.Text, new Rectangle(cDl, yy, wDl, fh));
                p.Right(Pct(a.Downloads, gr.R.Downloads), Theme.Small, Theme.Muted, new Rectangle(cPct, yy, wPct, fh));
                p.Right(FormatSize(a.Size), Theme.Small, Theme.Muted, new Rectangle(cSize, yy, wSize, fh));
                p.Right(FormatSize(a.Downloads * a.Size), Theme.Body, Theme.Text, new Rectangle(cTr, yy, wTr, fh));
                p.Link(fr, delegate { TokenForm.Open(url); }, a.Name + T(" — открыть релиз на GitHub", " — open the release on GitHub"));
                yy += fh;
            }
        }
        return card.Bottom + pad;
    }

    static void ShareCard(Painter p, Rectangle r, string title, List<ShareRow> rows, long total, Color color, int max)
    {
        p.Card(r);
        int y = Head(p, r, title, null, 0), rh = D(30);
        int x0 = r.X + D(16), iw = r.Width - D(32);
        if (total == 0) { p.Para(T("Скачиваний пока нет.", "No downloads yet."), Theme.Body, Theme.Muted, x0, y + D(4), iw); return; }
        foreach (var s in rows.Take(max))
        {
            p.Text(s.Name, Theme.Body, Theme.Text, new Rectangle(x0, y, iw * 28 / 100, rh));
            p.Bar(new Rectangle(x0 + iw * 29 / 100, y + rh / 2 - D(4), iw * 40 / 100, D(8)), (double)s.Downloads / total, color);
            p.Right(s.Downloads.ToString("N0"), Theme.Body, Theme.Text, new Rectangle(x0 + iw * 70 / 100, y, iw * 18 / 100, rh));
            p.Right(Pct(s.Downloads, total), Theme.Small, Theme.Muted, new Rectangle(x0 + iw * 88 / 100, y, iw * 12 / 100, rh));
            p.Tip(new Rectangle(x0, y, iw, rh), string.Format("{0}: {1} {2}, {3:N0} {4}", s.Name, s.Files, Pl(s.Files, "файл", "файла", "файлов", "file", "files"), s.Downloads, Pl(s.Downloads, "скачивание", "скачивания", "скачиваний", "download", "downloads")));
            y += rh;
        }
    }

    // ——— Трафик ———

    int RenderTraffic(Painter p)
    {
        if (data == null) return Placeholder(p);
        int pad = D(20), gap = D(14), y = D(4);
        if (!data.Repos.Any(r => r.HasTraffic))
        {
            string msg = data.TrafficDenied
                ? T("Токен есть, но GitHub не дал доступ к посещаемости. Нужен fine-grained токен с правом Administration: Read-only для ваших проектов.", "A token is set, but GitHub denied access to traffic. A fine-grained token with the Administration: Read-only permission for your projects is needed.")
                : T("Просмотры страниц и клонирования GitHub показывает только владельцу проекта. Добавьте токен — программа покажет их за последние 14 дней.", "GitHub shows page views and clones only to the project owner. Add a token and the program will show them for the last 14 days.");
            int mh = p.ParaHeight(msg, Theme.Body, p.W - 2 * pad - D(32));
            var c = new Rectangle(pad, y, p.W - 2 * pad, D(46) + mh + D(10) + D(30) + D(18));
            p.Card(c);
            int cy = Head(p, c, data.TrafficDenied ? T("У токена нет прав на трафик", "The token has no traffic permission") : T("Нужен токен", "A token is needed"), null, 0);
            cy += p.Para(msg, Theme.Body, Theme.Muted, c.X + D(16), cy, c.Width - D(32)) + D(10);
            var btn = new Rectangle(c.X + D(16), cy, D(170), D(30));
            p.Fill(btn, p.Hover(btn) ? Color.FromArgb(70, 130, 245) : Theme.Blue, D(6));
            p.Text(data.TrafficDenied ? T("Заменить токен…", "Replace token…") : T("Добавить токен…", "Add token…"), Theme.Bold, Color.White, btn, TextFormatFlags.HorizontalCenter);
            p.Link(btn, EditToken, null);
            return c.Bottom + pad;
        }

        var tr = data.Repos.Where(r => r.HasTraffic).OrderByDescending(r => r.Views).ThenByDescending(r => r.Clones).ToList();
        var t = Grid(p.W, y, 3, D(104), D(170));
        Tile(p, t[0], T("Просмотров", "Views"), tr.Sum(r => r.Views).ToString("N0"), Theme.Text, string.Format(T("за 14 дней, посетителей {0:N0}", "in 14 days, visitors {0:N0}"), tr.Sum(r => r.UniqueViews)), Theme.Muted,
            T("Сколько раз открывали страницы проектов на GitHub за последние 14 дней\nи сколько было разных посетителей.", "How many times project pages on GitHub were opened in the last 14 days\nand how many unique visitors there were."));
        Tile(p, t[1], T("Клонирований", "Clones"), tr.Sum(r => r.Clones).ToString("N0"), Theme.Text, string.Format(T("за 14 дней, разных {0:N0}", "in 14 days, unique {0:N0}"), tr.Sum(r => r.UniqueClones)), Theme.Muted,
            T("Сколько раз проекты скачали через git clone за последние 14 дней.", "How many times projects were fetched with git clone in the last 14 days."));
        var best = tr[0];
        Tile(p, t[2], T("Больше всех смотрят", "Most viewed"), best.Views > 0 ? best.Name : "—", Theme.Text, best.Views > 0 ? string.Format("{0:N0} {1}", best.Views, Pl(best.Views, "просмотр", "просмотра", "просмотров", "view", "views")) : T("просмотров не было", "no views"), Theme.Muted, null);
        y = BottomOf(t) + gap;

        // По дням: GitHub не присылает дни без посещений — там нули
        DateTime last = DateTime.UtcNow.Date, first = last.AddDays(-13);
        var vs = new Series { Name = T("Просмотры", "Views"), Color = Theme.Blue, Fill = true };
        var cs = new Series { Name = T("Клонирования", "Clones"), Color = Theme.Amber, Right = true };
        for (DateTime d = first; d <= last; d = d.AddDays(1))
        {
            DateTime day = d;
            vs.Points.Add(new KeyValuePair<DateTime, double>(day, tr.Sum(r => r.ViewDays.Where(x => x.Day == day).Sum(x => x.Count))));
            cs.Points.Add(new KeyValuePair<DateTime, double>(day, tr.Sum(r => r.CloneDays.Where(x => x.Day == day).Sum(x => x.Count))));
        }
        var ss = new List<Series> { vs, cs };
        var card = new Rectangle(pad, y, p.W - 2 * pad, D(280));
        p.Card(card);
        int lg = Legend(p, card.Right - D(16), card.Y + D(12), ss);
        Head(p, card, T("По дням", "By day"), T("   последние 14 дней, все проекты", "   last 14 days, all projects"), lg + D(12));
        Chart(p, new Rectangle(card.X + D(10), card.Y + D(48), card.Width - D(24), card.Height - D(60)), first, last, ss, "dd.MM", "dd.MM.yyyy");
        y = card.Bottom + gap;

        var refs = tr.SelectMany(r => r.Referrers).GroupBy(x => x.Name)
            .Select(g => new Referrer { Name = g.Key, Count = g.Sum(x => x.Count), Uniques = g.Sum(x => x.Uniques) })
            .OrderByDescending(x => x.Count).Take(10).ToList();
        int rh = D(30), rows = Math.Max(1, Math.Max(tr.Count, refs.Count));
        var two = Grid(p.W, y, 2, D(48) + D(24) + rows * rh + D(10), D(330));

        // По проектам
        var pc = two[0];
        p.Card(pc);
        int py = Head(p, pc, T("По проектам", "By project"), null, 0);
        int x0 = pc.X + D(16), iw = pc.Width - D(32);
        p.Right(T("Просмотров / людей", "Views / people"), Theme.Small, Theme.Muted, new Rectangle(x0 + iw * 44 / 100, py, iw * 38 / 100, D(20)));
        p.Right(T("Клонов", "Clones"), Theme.Small, Theme.Muted, new Rectangle(x0 + iw * 82 / 100, py, iw * 18 / 100, D(20)));
        py += D(24);
        int maxV = Math.Max(1, tr.Max(r => r.Views));
        foreach (var r in tr)
        {
            var row = new Rectangle(pc.X + D(6), py, pc.Width - D(12), rh);
            if (p.Hover(row)) p.Fill(row, Theme.RowHover, D(6));
            p.Text(r.Name, Theme.Body, Theme.Text, new Rectangle(x0, py, iw * 32 / 100, rh));
            p.Bar(new Rectangle(x0 + iw * 33 / 100, py + rh / 2 - D(4), iw * 24 / 100, D(8)), (double)r.Views / maxV, Theme.Blue);
            p.Right(string.Format("{0:N0} / {1:N0}", r.Views, r.UniqueViews), Theme.Body, Theme.Text, new Rectangle(x0 + iw * 58 / 100, py, iw * 24 / 100, rh));
            p.Right(string.Format("{0:N0}", r.Clones), Theme.Body, Theme.Muted, new Rectangle(x0 + iw * 82 / 100, py, iw * 18 / 100, rh));
            string url = r.Url + "/graphs/traffic";
            p.Link(row, delegate { TokenForm.Open(url); }, string.Format(T("{0}: клонирований {1:N0}, разных {2:N0}. Открыть график посещаемости на GitHub", "{0}: clones {1:N0}, unique {2:N0}. Open the traffic graph on GitHub"), r.Name, r.Clones, r.UniqueClones));
            py += rh;
        }

        // Откуда приходят
        var rc = two[1];
        p.Card(rc);
        int ry = Head(p, rc, T("Откуда приходят", "Where visitors come from"), T("   топ сайтов за 14 дней", "   top sites for 14 days"), 0);
        x0 = rc.X + D(16);
        iw = rc.Width - D(32);
        if (refs.Count == 0) p.Para(T("Переходов с других сайтов не было.", "No visits from other sites."), Theme.Body, Theme.Muted, x0, ry + D(4), iw);
        else
        {
            p.Right(T("Просмотров / людей", "Views / people"), Theme.Small, Theme.Muted, new Rectangle(x0 + iw * 66 / 100, ry, iw * 34 / 100, D(20)));
            ry += D(24);
            int maxR = Math.Max(1, refs.Max(x => x.Count));
            foreach (var x in refs)
            {
                p.Text(x.Name, Theme.Body, Theme.Text, new Rectangle(x0, ry, iw * 36 / 100, rh));
                p.Bar(new Rectangle(x0 + iw * 37 / 100, ry + rh / 2 - D(4), iw * 30 / 100, D(8)), (double)x.Count / maxR, Theme.Violet);
                p.Right(string.Format("{0:N0} / {1:N0}", x.Count, x.Uniques), Theme.Body, Theme.Text, new Rectangle(x0 + iw * 68 / 100, ry, iw * 32 / 100, rh));
                ry += rh;
            }
        }
        return BottomOf(two) + pad;
    }

    // ——— История ———

    Snapshot At(DateTime t, Func<Snapshot, bool> ok) { return hist.LastOrDefault(s => ok(s) && s.When() <= t); }

    // Прирост за эту и прошлую неделю; null — истории не хватает
    void Weeks(Func<Snapshot, long> sel, Func<Snapshot, bool> ok, out long? thisW, out long? lastW, out bool partial)
    {
        var a7 = At(loadedAt.AddDays(-7), ok);
        var a14 = At(loadedAt.AddDays(-14), ok);
        var b = a7 ?? hist.FirstOrDefault(ok);
        partial = a7 == null;
        thisW = b != null && b != curSnap ? sel(curSnap) - sel(b) : (long?)null;
        lastW = a7 != null && a14 != null ? sel(a7) - sel(a14) : (long?)null;
    }

    int RenderHistory(Painter p)
    {
        if (data == null || hist.Count == 0) return Placeholder(p);
        int pad = D(20), gap = D(14), y = D(4);
        var first = hist[0];
        Func<Snapshot, bool> all = s => true;

        var t = Grid(p.W, y, 4, D(104), D(170));
        if (weekSnap == null || (loadedAt - weekSnap.When()).TotalDays < 1)
            Tile(p, t[0], T("В день за неделю", "Per day, last week"), "—", Theme.Muted, T("нужна история хотя бы за сутки", "needs at least a day of history"), Theme.Muted, null);
        else
        {
            double days = (loadedAt - weekSnap.When()).TotalDays;
            Tile(p, t[0], T("В день за неделю", "Per day, last week"), ((curSnap.Total - weekSnap.Total) / days).ToString("N1"), Theme.Text, T("скачиваний, ", "downloads, ") + Since(weekSnap), Theme.Muted,
                T("Сколько скачиваний в среднем прибавлялось за сутки на последней неделе.", "How many downloads were added per day on average over the last week."));
        }
        double allDays = (loadedAt - first.When()).TotalDays;
        if (allDays < 1) Tile(p, t[1], T("В день за всё время", "Per day, all time"), "—", Theme.Muted, T("нужна история хотя бы за сутки", "needs at least a day of history"), Theme.Muted, null);
        else Tile(p, t[1], T("В день за всё время", "Per day, all time"), ((curSnap.Total - first.Total) / allDays).ToString("N1"), Theme.Text, T("скачиваний, с ", "downloads, since ") + first.When().ToString("dd.MM.yyyy"), Theme.Muted,
            T("Сколько скачиваний в среднем прибавлялось за сутки с первой проверки в этой программе.", "How many downloads were added per day on average since the first check in this program."));

        long? tw, lw;
        bool partial;
        Weeks(s => s.Total, all, out tw, out lw, out partial);
        if (tw == null) Tile(p, t[2], T("Эта неделя", "This week"), "—", Theme.Muted, T("после следующей проверки", "after the next check"), Theme.Muted, null);
        else
        {
            string hint = lw == null ? T("прошлой недели нет в истории", "last week is not in the history") : T("прошлая: ", "last week: ") + Signed(lw.Value) + (lw > 0 ? string.Format(" ({0}{1:0}%)", tw >= lw ? "+" : "", 100.0 * (tw.Value - lw.Value) / lw.Value) : "");
            Tile(p, t[2], T("Эта неделя", "This week"), Signed(tw.Value), lw != null && tw > lw ? Theme.Good : lw != null && tw < lw ? Theme.Bad : Theme.Text, hint, Theme.Muted,
                T("Сколько скачиваний прибавилось за 7 дней и сравнение с семью днями до этого.", "How many downloads were added over 7 days, compared with the 7 days before."));
        }
        Tile(p, t[3], T("Проверок в истории", "Checks in history"), hist.Count.ToString("N0"), Theme.Text, T("с ", "since ") + first.When().ToString("dd.MM.yyyy") + T(", хранятся 400 дней", ", kept for 400 days"), Theme.Muted,
            T("Снимок счётчиков сохраняется после проверки, только если что-то изменилось.", "A snapshot of the counters is saved after a check only if something changed."));
        y = BottomOf(t) + gap;

        // Неделя к неделе
        var metrics = new[]
        {
            new { Name = T("Скачивания", "Downloads"), Sel = (Func<Snapshot, long>)(s => s.Total), Ok = all },
            new { Name = T("Звёзды", "Stars"), Sel = (Func<Snapshot, long>)(s => s.Stars), Ok = (Func<Snapshot, bool>)(s => s.HasStars()) },
            new { Name = T("Форки", "Forks"), Sel = (Func<Snapshot, long>)(s => s.Forks), Ok = (Func<Snapshot, bool>)(s => s.HasStars()) },
            new { Name = T("Подписчики", "Followers"), Sel = (Func<Snapshot, long>)(s => s.Followers), Ok = (Func<Snapshot, bool>)(s => s.Followers >= 0) }
        };
        int rh = D(30);
        var wc = new Rectangle(pad, y, p.W - 2 * pad, D(46) + D(24) + metrics.Length * rh + D(34));
        p.Card(wc);
        int wy = Head(p, wc, T("Неделя к неделе", "Week over week"), T("   прирост за последние 7 дней и за 7 дней до них", "   growth over the last 7 days and the 7 days before"), 0);
        int x0 = wc.X + D(16), iw = wc.Width - D(32);
        int[] cx = { x0, x0 + iw * 30 / 100, x0 + iw * 52 / 100, x0 + iw * 74 / 100, x0 + iw };
        string[] heads = { "", T("Эта неделя", "This week"), T("Прошлая неделя", "Last week"), T("Разница", "Difference") };
        for (int i = 1; i < 4; i++) p.Right(heads[i], Theme.Small, Theme.Muted, new Rectangle(cx[i], wy, cx[i + 1] - cx[i], D(20)));
        wy += D(24);
        bool anyPartial = false, anyMissing = false;
        foreach (var m in metrics)
        {
            Weeks(m.Sel, m.Ok, out tw, out lw, out partial);
            if (partial && tw != null) anyPartial = true;
            if (tw == null || lw == null) anyMissing = true;
            p.Text(m.Name, Theme.Body, Theme.Text, new Rectangle(cx[0], wy, cx[1] - cx[0], rh));
            p.Right(tw == null ? "—" : Signed(tw.Value) + (partial ? "*" : ""), Theme.Body, tw > 0 ? Theme.Good : Theme.Text, new Rectangle(cx[1], wy, cx[2] - cx[1], rh));
            p.Right(lw == null ? "—" : Signed(lw.Value), Theme.Body, Theme.Text, new Rectangle(cx[2], wy, cx[3] - cx[2], rh));
            string diff = tw == null || lw == null ? "—" : Signed(tw.Value - lw.Value);
            p.Right(diff, Theme.Body, tw != null && lw != null ? (tw > lw ? Theme.Good : tw < lw ? Theme.Bad : Theme.Muted) : Theme.Muted, new Rectangle(cx[3], wy, cx[4] - cx[3], rh));
            wy += rh;
        }
        if (anyPartial || anyMissing)
            p.Text(anyPartial ? T("* история короче недели — прирост с первой проверки. «Прошлая неделя» появится, когда истории будет 2 недели.", "* the history is shorter than a week — growth since the first check. \"Last week\" will appear when there are 2 weeks of history.")
                              : T("«—» — для этого периода ещё нет снимков (звёзды, форки и подписчики пишутся в историю с этой версии программы).", "\"—\" — there are no snapshots for this period yet (stars, forks and followers are recorded in the history starting with this version of the program)."),
                Theme.Small, Theme.Muted, new Rectangle(x0, wy + D(4), iw, D(20)));
        y = wc.Bottom + gap;

        // Снимки
        const int maxRows = 60;
        var shown = Enumerable.Range(0, hist.Count).Reverse().Take(maxRows).ToList();
        var sc = new Rectangle(pad, y, p.W - 2 * pad, D(46) + D(24) + shown.Count * D(28) + D(10) + (hist.Count > maxRows ? D(24) : 0));
        p.Card(sc);
        int sy = Head(p, sc, T("Снимки", "Snapshots"), T("   сохраняются после проверки, если счётчики изменились", "   saved after a check if the counters changed"), 0);
        x0 = sc.X + D(16);
        iw = sc.Width - D(32);
        int[] col = { 0, 20, 34, 45, 56, 67, 76, 87, 100 };
        string[] names = Lang.En
            ? new[] { "Date and time", "Downloads", "Growth", "Per day", "Stars", "Growth", "Forks", "Followers" }
            : new[] { "Дата и время", "Скачиваний", "Прирост", "В день", "Звёзд", "Прирост", "Форков", "Подписчиков" };
        for (int i = 0; i < names.Length; i++)
        {
            var cr = new Rectangle(x0 + iw * col[i] / 100, sy, iw * (col[i + 1] - col[i]) / 100, D(20));
            if (i == 0) p.Text(names[i], Theme.Small, Theme.Muted, cr); else p.Right(names[i], Theme.Small, Theme.Muted, cr);
        }
        p.Tip(new Rectangle(x0 + iw * col[3] / 100, sy, iw * (col[4] - col[3]) / 100, D(20)), T("Прирост скачиваний, делённый на число суток с предыдущего снимка (не меньше одних суток)", "Download growth divided by the number of days since the previous snapshot (at least one day)"));
        sy += D(24);
        int fh = D(28);
        foreach (int i in shown)
        {
            var s = hist[i];
            var o = i > 0 ? hist[i - 1] : null;
            var cells = new string[8];
            var colors = new Color[8];
            for (int k = 0; k < 8; k++) colors[k] = Theme.Text;
            cells[0] = s.When().ToString("dd.MM.yyyy HH:mm");
            cells[1] = s.Total.ToString("N0");
            if (o != null)
            {
                long d = s.Total - o.Total;
                cells[2] = Signed(d);
                colors[2] = d > 0 ? Theme.Good : Theme.Muted;
                cells[3] = (d / Math.Max(1, (s.When() - o.When()).TotalDays)).ToString("N1");
                colors[3] = Theme.Muted;
            }
            else { cells[2] = cells[3] = "—"; colors[2] = colors[3] = Theme.Muted; }
            cells[4] = s.HasStars() ? s.Stars.ToString("N0") : "—";
            if (s.HasStars() && o != null && o.HasStars() && s.Stars != o.Stars)
            {
                cells[5] = Signed(s.Stars - o.Stars);
                colors[5] = s.Stars > o.Stars ? Theme.Good : Theme.Bad;
            }
            else { cells[5] = s.HasStars() && o != null && o.HasStars() ? "0" : "—"; colors[5] = Theme.Muted; }
            cells[6] = s.Forks >= 0 ? s.Forks.ToString("N0") + (o != null && o.Forks >= 0 && s.Forks != o.Forks ? " (" + Signed(s.Forks - o.Forks) + ")" : "") : "—";
            cells[7] = s.Followers >= 0 ? s.Followers.ToString("N0") + (o != null && o.Followers >= 0 && s.Followers != o.Followers ? " (" + Signed(s.Followers - o.Followers) + ")" : "") : "—";
            if (s.Forks < 0) colors[6] = Theme.Muted;
            if (s.Followers < 0) colors[7] = Theme.Muted;
            for (int k = 0; k < 8; k++)
            {
                var cr = new Rectangle(x0 + iw * col[k] / 100, sy, iw * (col[k + 1] - col[k]) / 100, fh);
                if (k == 0) p.Text(cells[k], Theme.Body, colors[k], cr); else p.Right(cells[k], Theme.Body, colors[k], cr);
            }
            sy += fh;
        }
        if (hist.Count > maxRows)
            p.Text(string.Format(T("Показаны последние {0} из {1}.", "Showing the latest {0} of {1}."), maxRows, hist.Count), Theme.Small, Theme.Muted, new Rectangle(x0, sy + D(2), iw, D(20)));
        return sc.Bottom + pad;
    }

    static string FormatSize(long bytes)
    {
        if (bytes >= 1L << 30) return (bytes / 1073741824.0).ToString("0.0") + T(" ГБ", " GB");
        if (bytes >= 1 << 20) return (bytes / 1048576.0).ToString("0.0") + T(" МБ", " MB");
        if (bytes >= 1 << 10) return (bytes / 1024.0).ToString("0") + T(" КБ", " KB");
        return bytes + T(" Б", " B");
    }

    // ——— GitHub ———

    static AccountStat Fetch(string user, string token)
    {
        var acc = new AccountStat { HasToken = !string.IsNullOrEmpty(token) };
        var u = GetObject("https://api.github.com/users/" + Uri.EscapeDataString(user), token);
        acc.User = (string)u["login"];
        acc.Followers = Convert.ToInt32(u["followers"]);
        foreach (var repo in GetAll("https://api.github.com/users/" + Uri.EscapeDataString(acc.User) + "/repos?type=owner&per_page=100", token))
        {
            if (true.Equals(repo["fork"])) continue;
            var r = new RepoStat
            {
                Name = (string)repo["name"],
                Url = (string)repo["html_url"],
                Description = repo["description"] as string,
                Language = repo["language"] as string,
                Created = Date(repo["created_at"]),
                Pushed = Date(repo["pushed_at"]),
                Stars = Convert.ToInt32(repo["stargazers_count"]),
                Forks = Convert.ToInt32(repo["forks_count"]),
                Issues = Convert.ToInt32(repo["open_issues_count"])
            };
            string api = "https://api.github.com/repos/" + Uri.EscapeDataString(acc.User) + "/" + Uri.EscapeDataString(r.Name);
            foreach (var rel in GetAll(api + "/releases?per_page=100", token))
            {
                if (true.Equals(rel["draft"])) continue;
                var rs = new ReleaseStat
                {
                    Tag = (string)rel["tag_name"],
                    Published = Date(rel["published_at"] ?? rel["created_at"]),
                    Prerelease = true.Equals(rel["prerelease"])
                };
                foreach (Dictionary<string, object> a in (object[])rel["assets"])
                {
                    var s = new AssetStat { Name = (string)a["name"], Downloads = Convert.ToInt64(a["download_count"]), Size = Convert.ToInt64(a["size"]) };
                    rs.Downloads += s.Downloads;
                    rs.Assets.Add(s);
                }
                rs.Assets.Sort((x, y) => y.Downloads.CompareTo(x.Downloads));
                r.Downloads += rs.Downloads;
                r.Releases.Add(rs);
            }
            r.Releases.Sort((x, y) => y.Published.CompareTo(x.Published));
            if (acc.HasToken)
            {
                try { LoadTraffic(r, api, token); }
                catch (WebException ex)
                {
                    // Посещаемость видна только с правом Administration на проект
                    var resp = ex.Response as HttpWebResponse;
                    if (resp == null || (resp.StatusCode != HttpStatusCode.Forbidden && resp.StatusCode != HttpStatusCode.NotFound)) throw;
                }
            }
            acc.Repos.Add(r);
        }
        acc.TrafficDenied = acc.HasToken && !acc.Repos.Any(r => r.HasTraffic);
        return acc;
    }

    static void LoadTraffic(RepoStat r, string api, string token)
    {
        var v = GetObject(api + "/traffic/views", token);
        var c = GetObject(api + "/traffic/clones", token);
        r.Views = Convert.ToInt32(v["count"]);
        r.UniqueViews = Convert.ToInt32(v["uniques"]);
        r.Clones = Convert.ToInt32(c["count"]);
        r.UniqueClones = Convert.ToInt32(c["uniques"]);
        r.ViewDays = Days(v, "views");
        r.CloneDays = Days(c, "clones");
        foreach (Dictionary<string, object> x in (object[])Json().DeserializeObject(Get(api + "/traffic/popular/referrers", token)))
            r.Referrers.Add(new Referrer { Name = (string)x["referrer"], Count = Convert.ToInt32(x["count"]), Uniques = Convert.ToInt32(x["uniques"]) });
        r.HasTraffic = true;
    }

    // Посуточные счётчики из ответа traffic/views или traffic/clones; дата — по UTC, как у GitHub
    static List<DayCount> Days(Dictionary<string, object> o, string key)
    {
        var list = new List<DayCount>();
        object arr;
        if (!o.TryGetValue(key, out arr) || !(arr is object[])) return list;
        foreach (Dictionary<string, object> x in (object[])arr)
        {
            DateTime d;
            if (!DateTime.TryParse(x["timestamp"] as string, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out d)) continue;
            list.Add(new DayCount { Day = d.Date, Count = Convert.ToInt32(x["count"]), Uniques = Convert.ToInt32(x["uniques"]) });
        }
        return list;
    }

    static DateTime Date(object o)
    {
        var s = o as string;
        DateTime d;
        return s != null && DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out d)
            ? d.ToLocalTime() : DateTime.MinValue;
    }

    static JavaScriptSerializer Json() { return new JavaScriptSerializer { MaxJsonLength = int.MaxValue }; }

    static Dictionary<string, object> GetObject(string url, string token)
    {
        return (Dictionary<string, object>)Json().DeserializeObject(Get(url, token));
    }

    static IEnumerable<Dictionary<string, object>> GetAll(string url, string token)
    {
        for (int page = 1; ; page++)
        {
            var items = (object[])Json().DeserializeObject(Get(url + "&page=" + page, token));
            foreach (Dictionary<string, object> item in items) yield return item;
            if (items.Length < 100) yield break;
        }
    }

    static string Get(string url, string token)
    {
        using (var wc = new WebClient())
        {
            wc.Encoding = Encoding.UTF8;
            wc.Headers[HttpRequestHeader.UserAgent] = "GhDownloads";
            wc.Headers[HttpRequestHeader.Accept] = "application/vnd.github+json";
            if (!string.IsNullOrEmpty(token)) wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + token;
            string s = wc.DownloadString(url);
            int n;
            if (int.TryParse(wc.ResponseHeaders["X-RateLimit-Remaining"], out n)) rateRemaining = n;
            if (int.TryParse(wc.ResponseHeaders["X-RateLimit-Limit"], out n)) rateLimit = n;
            return s;
        }
    }

    // Текст ошибки сразу на обоих языках: ответ GitHub после потока недоступен, а язык можно переключить в любой момент
    static string[] Describe(Exception ex)
    {
        var we = ex as WebException;
        if (we != null)
        {
            var resp = we.Response as HttpWebResponse;
            if (resp == null) return new[] { "Нет связи с GitHub: " + we.Message, "Cannot reach GitHub: " + we.Message };
            if (resp.StatusCode == HttpStatusCode.NotFound) return new[] { "Такой аккаунт на GitHub не найден.", "No such account on GitHub." };
            if (resp.StatusCode == HttpStatusCode.Unauthorized)
                return new[] { "GitHub не принял токен (устарел или удалён). Замените или удалите его кнопкой «Токен…» в меню слева.",
                               "GitHub rejected the token (expired or deleted). Replace or delete it with the \"Token…\" button in the left menu." };
            if (resp.StatusCode == HttpStatusCode.Forbidden || (int)resp.StatusCode == 429)
            {
                string reset = resp.Headers["X-RateLimit-Reset"];
                long sec;
                if (reset != null && long.TryParse(reset, out sec))
                {
                    DateTime at = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(sec).ToLocalTime();
                    return new[] { string.Format("GitHub ограничивает число запросов (60 в час без токена). Попробуйте после {0:HH:mm}.", at),
                                   string.Format("GitHub limits the number of requests (60 per hour without a token). Try again after {0:HH:mm}.", at) };
                }
                return new[] { "GitHub ограничивает число запросов (60 в час без токена). Попробуйте позже.",
                               "GitHub limits the number of requests (60 per hour without a token). Try again later." };
            }
            string code = (int)resp.StatusCode + " " + resp.StatusDescription;
            return new[] { "Ошибка GitHub: " + code, "GitHub error: " + code };
        }
        return new[] { "Ошибка: " + ex.Message, "Error: " + ex.Message };
    }

    // ——— Хранение ———

    static string HistoryPath(string user)
    {
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GitHubDownloads");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, "history-" + user.ToLowerInvariant() + ".json");
    }

    static List<Snapshot> LoadHistory(string user)
    {
        try
        {
            string p = HistoryPath(user);
            if (File.Exists(p))
            {
                var h = Json().Deserialize<List<Snapshot>>(File.ReadAllText(p, Encoding.UTF8)) ?? new List<Snapshot>();
                foreach (var s in h)
                {
                    if (s.Repos == null) s.Repos = new Dictionary<string, long>();
                    if (s.RepoStars == null) s.RepoStars = new Dictionary<string, long>();
                }
                return h;
            }
        }
        catch { }
        return new List<Snapshot>();
    }

    static void SaveHistory(string user, List<Snapshot> hist)
    {
        try { File.WriteAllText(HistoryPath(user), Json().Serialize(hist), Encoding.UTF8); } catch { }
    }

    static string Reg(string name)
    {
        try
        {
            using (var k = Registry.CurrentUser.OpenSubKey(RegKey))
            {
                var v = k == null ? null : k.GetValue(name) as string;
                return string.IsNullOrEmpty(v) ? null : v;
            }
        }
        catch { return null; }
    }

    static void SetReg(string name, string value)
    {
        try
        {
            using (var k = Registry.CurrentUser.CreateSubKey(RegKey))
            {
                if (value == null) k.DeleteValue(name, false);
                else k.SetValue(name, value);
            }
        }
        catch { }
    }

    static string LoadStoredToken()
    {
        string s = Reg("Token");
        if (s == null) return null;
        try { return Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(s), null, DataProtectionScope.CurrentUser)); }
        catch { return null; }
    }

    // Сохранённый токен важнее переменной GITHUB_TOKEN
    static string LoadToken()
    {
        return LoadStoredToken() ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN");
    }

    static void SaveToken(string token)
    {
        if (token.Length == 0) { SetReg("Token", null); return; }
        SetReg("Token", Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(token), null, DataProtectionScope.CurrentUser)));
    }

    [STAThread]
    static void Main()
    {
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // TLS 1.2
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
