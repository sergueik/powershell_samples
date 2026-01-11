namespace ServiceMaster
{
    partial class MainFrom
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("运行中");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("暂停");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("停止");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("运行状态", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("自动");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("手动");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("已禁用");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("启动类型", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7});
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("可以停止");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("无法停止");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("可以暂停");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("无法暂停");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("可以启动");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("服务可控性", new System.Windows.Forms.TreeNode[] {
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13});
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("独立进程服务");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("共享进程服务");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("交互进程服务");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("服务进程", new System.Windows.Forms.TreeNode[] {
            treeNode15,
            treeNode16,
            treeNode17});
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("本地系统");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("本地服务");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("网络服务");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("登录类型", new System.Windows.Forms.TreeNode[] {
            treeNode19,
            treeNode20,
            treeNode21});
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("搜索结果");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrom));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CurrentSvcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.按启动类型ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DemandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DisabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OwnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InteractiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.按服务登陆类型ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localSvcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NTsvcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localSysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.QuickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MasterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputBatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputCommonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sQLServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.KAVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iISToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SvcLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.插件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OutputBatchFile = new System.Windows.Forms.ToolStripMenuItem();
            this.OutputConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.sortTreeView = new System.Windows.Forms.TreeView();
            this.label7 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.ServiceDetailTable = new System.Windows.Forms.TableLayoutPanel();
            this.label24 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.lblServiceName = new System.Windows.Forms.Label();
            this.lblDisplayName = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblRunMode = new System.Windows.Forms.Label();
            this.tbxSysDescription = new System.Windows.Forms.TextBox();
            this.tbxComment = new System.Windows.Forms.TextBox();
            this.lblSuggest = new System.Windows.Forms.Label();
            this.lblSuggestAdv = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.btnDemand = new System.Windows.Forms.Button();
            this.btnAuto = new System.Windows.Forms.Button();
            this.btnDisable = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblService = new System.Windows.Forms.Label();
            this.lbxRunning = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbxDepandentSvc = new System.Windows.Forms.ListBox();
            this.lbxSvcDependOn = new System.Windows.Forms.ListBox();
            this.tbxSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.youhua = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.ServiceDetailTable.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ServiceToolStripMenuItem,
            this.ModeToolStripMenuItem,
            this.ConfigToolStripMenuItem,
            this.BatToolStripMenuItem,
            this.SvcLogToolStripMenuItem,
            this.HelpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(717, 25);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ServiceToolStripMenuItem
            // 
            this.ServiceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CurrentSvcToolStripMenuItem,
            this.outputListToolStripMenuItem,
            this.按启动类型ToolStripMenuItem,
            this.ProcessToolStripMenuItem,
            this.按服务登陆类型ToolStripMenuItem});
            this.ServiceToolStripMenuItem.Name = "ServiceToolStripMenuItem";
            this.ServiceToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.ServiceToolStripMenuItem.Text = "服务";
            // 
            // CurrentSvcToolStripMenuItem
            // 
            this.CurrentSvcToolStripMenuItem.Name = "CurrentSvcToolStripMenuItem";
            this.CurrentSvcToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.CurrentSvcToolStripMenuItem.Text = "查看当前服务";
            this.CurrentSvcToolStripMenuItem.Click += new System.EventHandler(this.CurrentSvcToolStripMenuItem_Click);
            // 
            // outputListToolStripMenuItem
            // 
            this.outputListToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartToolStripMenuItem,
            this.PauseToolStripMenuItem,
            this.StopToolStripMenuItem});
            this.outputListToolStripMenuItem.Name = "outputListToolStripMenuItem";
            this.outputListToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.outputListToolStripMenuItem.Text = "按运行状态";
            // 
            // StartToolStripMenuItem
            // 
            this.StartToolStripMenuItem.Name = "StartToolStripMenuItem";
            this.StartToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.StartToolStripMenuItem.Text = "启动";
            this.StartToolStripMenuItem.Click += new System.EventHandler(this.StartToolStripMenuItem_Click);
            // 
            // PauseToolStripMenuItem
            // 
            this.PauseToolStripMenuItem.Name = "PauseToolStripMenuItem";
            this.PauseToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.PauseToolStripMenuItem.Text = "暂停";
            this.PauseToolStripMenuItem.Click += new System.EventHandler(this.PauseToolStripMenuItem_Click);
            // 
            // StopToolStripMenuItem
            // 
            this.StopToolStripMenuItem.Name = "StopToolStripMenuItem";
            this.StopToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.StopToolStripMenuItem.Text = "停止";
            this.StopToolStripMenuItem.Click += new System.EventHandler(this.StopToolStripMenuItem_Click);
            // 
            // 按启动类型ToolStripMenuItem
            // 
            this.按启动类型ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AutoToolStripMenuItem,
            this.DemandToolStripMenuItem,
            this.DisabledToolStripMenuItem});
            this.按启动类型ToolStripMenuItem.Name = "按启动类型ToolStripMenuItem";
            this.按启动类型ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.按启动类型ToolStripMenuItem.Text = "按启动类型";
            // 
            // AutoToolStripMenuItem
            // 
            this.AutoToolStripMenuItem.Name = "AutoToolStripMenuItem";
            this.AutoToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.AutoToolStripMenuItem.Text = "自动";
            this.AutoToolStripMenuItem.Click += new System.EventHandler(this.AutoToolStripMenuItem_Click);
            // 
            // DemandToolStripMenuItem
            // 
            this.DemandToolStripMenuItem.Name = "DemandToolStripMenuItem";
            this.DemandToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.DemandToolStripMenuItem.Text = "手动";
            this.DemandToolStripMenuItem.Click += new System.EventHandler(this.DemandToolStripMenuItem_Click);
            // 
            // DisabledToolStripMenuItem
            // 
            this.DisabledToolStripMenuItem.Name = "DisabledToolStripMenuItem";
            this.DisabledToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.DisabledToolStripMenuItem.Text = "禁用";
            this.DisabledToolStripMenuItem.Click += new System.EventHandler(this.DisabledToolStripMenuItem_Click);
            // 
            // ProcessToolStripMenuItem
            // 
            this.ProcessToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShareToolStripMenuItem,
            this.OwnToolStripMenuItem,
            this.InteractiveToolStripMenuItem});
            this.ProcessToolStripMenuItem.Name = "ProcessToolStripMenuItem";
            this.ProcessToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.ProcessToolStripMenuItem.Text = "按服务进程类型";
            // 
            // ShareToolStripMenuItem
            // 
            this.ShareToolStripMenuItem.Name = "ShareToolStripMenuItem";
            this.ShareToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.ShareToolStripMenuItem.Text = "共享进程服务";
            this.ShareToolStripMenuItem.Click += new System.EventHandler(this.ShareToolStripMenuItem_Click);
            // 
            // OwnToolStripMenuItem
            // 
            this.OwnToolStripMenuItem.Name = "OwnToolStripMenuItem";
            this.OwnToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.OwnToolStripMenuItem.Text = "独占进程服务";
            this.OwnToolStripMenuItem.Click += new System.EventHandler(this.OwnToolStripMenuItem_Click);
            // 
            // InteractiveToolStripMenuItem
            // 
            this.InteractiveToolStripMenuItem.Name = "InteractiveToolStripMenuItem";
            this.InteractiveToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.InteractiveToolStripMenuItem.Text = "交互服务";
            this.InteractiveToolStripMenuItem.Click += new System.EventHandler(this.InteractiveToolStripMenuItem_Click);
            // 
            // 按服务登陆类型ToolStripMenuItem
            // 
            this.按服务登陆类型ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.localSvcToolStripMenuItem,
            this.NTsvcToolStripMenuItem,
            this.localSysToolStripMenuItem});
            this.按服务登陆类型ToolStripMenuItem.Name = "按服务登陆类型ToolStripMenuItem";
            this.按服务登陆类型ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.按服务登陆类型ToolStripMenuItem.Text = "按服务登陆类型";
            // 
            // localSvcToolStripMenuItem
            // 
            this.localSvcToolStripMenuItem.Name = "localSvcToolStripMenuItem";
            this.localSvcToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.localSvcToolStripMenuItem.Text = "本地服务";
            this.localSvcToolStripMenuItem.Click += new System.EventHandler(this.localSvcToolStripMenuItem_Click);
            // 
            // NTsvcToolStripMenuItem
            // 
            this.NTsvcToolStripMenuItem.Name = "NTsvcToolStripMenuItem";
            this.NTsvcToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.NTsvcToolStripMenuItem.Text = "网络服务";
            this.NTsvcToolStripMenuItem.Click += new System.EventHandler(this.NTsvcToolStripMenuItem_Click);
            // 
            // localSysToolStripMenuItem
            // 
            this.localSysToolStripMenuItem.Name = "localSysToolStripMenuItem";
            this.localSysToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.localSysToolStripMenuItem.Text = "本地系统";
            this.localSysToolStripMenuItem.Click += new System.EventHandler(this.localSysToolStripMenuItem_Click);
            // 
            // ModeToolStripMenuItem
            // 
            this.ModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.QuickToolStripMenuItem,
            this.GuideToolStripMenuItem,
            this.MasterToolStripMenuItem});
            this.ModeToolStripMenuItem.Name = "ModeToolStripMenuItem";
            this.ModeToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.ModeToolStripMenuItem.Text = "模式";
            this.ModeToolStripMenuItem.Click += new System.EventHandler(this.ModeToolStripMenuItem_Click);
            // 
            // QuickToolStripMenuItem
            // 
            this.QuickToolStripMenuItem.Name = "QuickToolStripMenuItem";
            this.QuickToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.QuickToolStripMenuItem.Text = "快速优化模式";
            this.QuickToolStripMenuItem.Click += new System.EventHandler(this.QuickToolStripMenuItem_Click);
            // 
            // GuideToolStripMenuItem
            // 
            this.GuideToolStripMenuItem.Name = "GuideToolStripMenuItem";
            this.GuideToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.GuideToolStripMenuItem.Text = "向导模式";
            this.GuideToolStripMenuItem.Click += new System.EventHandler(this.GuideToolStripMenuItem_Click);
            // 
            // MasterToolStripMenuItem
            // 
            this.MasterToolStripMenuItem.Name = "MasterToolStripMenuItem";
            this.MasterToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.MasterToolStripMenuItem.Text = "深度优化模式";
            this.MasterToolStripMenuItem.Click += new System.EventHandler(this.MasterToolStripMenuItem_Click);
            // 
            // ConfigToolStripMenuItem
            // 
            this.ConfigToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.outputConfigToolStripMenuItem,
            this.inputConfigToolStripMenuItem});
            this.ConfigToolStripMenuItem.Name = "ConfigToolStripMenuItem";
            this.ConfigToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.ConfigToolStripMenuItem.Text = "工具";
            // 
            // outputConfigToolStripMenuItem
            // 
            this.outputConfigToolStripMenuItem.Name = "outputConfigToolStripMenuItem";
            this.outputConfigToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.outputConfigToolStripMenuItem.Text = "导出配置";
            this.outputConfigToolStripMenuItem.Click += new System.EventHandler(this.outputConfigToolStripMenuItem_Click);
            // 
            // inputConfigToolStripMenuItem
            // 
            this.inputConfigToolStripMenuItem.Name = "inputConfigToolStripMenuItem";
            this.inputConfigToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.inputConfigToolStripMenuItem.Text = "导入配置";
            this.inputConfigToolStripMenuItem.Click += new System.EventHandler(this.inputConfigToolStripMenuItem_Click);
            // 
            // BatToolStripMenuItem
            // 
            this.BatToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.outputBatToolStripMenuItem,
            this.outputCommonToolStripMenuItem});
            this.BatToolStripMenuItem.Name = "BatToolStripMenuItem";
            this.BatToolStripMenuItem.Size = new System.Drawing.Size(56, 21);
            this.BatToolStripMenuItem.Text = "批处理";
            // 
            // outputBatToolStripMenuItem
            // 
            this.outputBatToolStripMenuItem.Name = "outputBatToolStripMenuItem";
            this.outputBatToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.outputBatToolStripMenuItem.Text = "导出批处理";
            this.outputBatToolStripMenuItem.Click += new System.EventHandler(this.outputBatToolStripMenuItem_Click);
            // 
            // outputCommonToolStripMenuItem
            // 
            this.outputCommonToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sQLServerToolStripMenuItem,
            this.KAVToolStripMenuItem,
            this.vMToolStripMenuItem,
            this.iISToolStripMenuItem,
            this.UpdateToolStripMenuItem});
            this.outputCommonToolStripMenuItem.Name = "outputCommonToolStripMenuItem";
            this.outputCommonToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.outputCommonToolStripMenuItem.Text = "导出常用批处理";
            // 
            // sQLServerToolStripMenuItem
            // 
            this.sQLServerToolStripMenuItem.Name = "sQLServerToolStripMenuItem";
            this.sQLServerToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.sQLServerToolStripMenuItem.Tag = "SQL";
            this.sQLServerToolStripMenuItem.Text = "SQL Server";
            this.sQLServerToolStripMenuItem.Click += new System.EventHandler(this.sQLServerToolStripMenuItem_Click);
            // 
            // KAVToolStripMenuItem
            // 
            this.KAVToolStripMenuItem.Name = "KAVToolStripMenuItem";
            this.KAVToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.KAVToolStripMenuItem.Tag = "KAV";
            this.KAVToolStripMenuItem.Text = "卡巴斯基";
            this.KAVToolStripMenuItem.Click += new System.EventHandler(this.sQLServerToolStripMenuItem_Click);
            // 
            // vMToolStripMenuItem
            // 
            this.vMToolStripMenuItem.Name = "vMToolStripMenuItem";
            this.vMToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.vMToolStripMenuItem.Tag = "VMWARE";
            this.vMToolStripMenuItem.Text = "VMware";
            this.vMToolStripMenuItem.Click += new System.EventHandler(this.sQLServerToolStripMenuItem_Click);
            // 
            // iISToolStripMenuItem
            // 
            this.iISToolStripMenuItem.Name = "iISToolStripMenuItem";
            this.iISToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.iISToolStripMenuItem.Tag = "IIS";
            this.iISToolStripMenuItem.Text = "IIS";
            this.iISToolStripMenuItem.Click += new System.EventHandler(this.sQLServerToolStripMenuItem_Click);
            // 
            // UpdateToolStripMenuItem
            // 
            this.UpdateToolStripMenuItem.Name = "UpdateToolStripMenuItem";
            this.UpdateToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.UpdateToolStripMenuItem.Tag = "Update";
            this.UpdateToolStripMenuItem.Text = "自动升级";
            this.UpdateToolStripMenuItem.Click += new System.EventHandler(this.sQLServerToolStripMenuItem_Click);
            // 
            // SvcLogToolStripMenuItem
            // 
            this.SvcLogToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewLogToolStripMenuItem,
            this.ClearLogToolStripMenuItem});
            this.SvcLogToolStripMenuItem.Name = "SvcLogToolStripMenuItem";
            this.SvcLogToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.SvcLogToolStripMenuItem.Text = "日志";
            // 
            // ViewLogToolStripMenuItem
            // 
            this.ViewLogToolStripMenuItem.Name = "ViewLogToolStripMenuItem";
            this.ViewLogToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ViewLogToolStripMenuItem.Text = "查看日志";
            this.ViewLogToolStripMenuItem.Click += new System.EventHandler(this.ViewLogToolStripMenuItem_Click);
            // 
            // ClearLogToolStripMenuItem
            // 
            this.ClearLogToolStripMenuItem.Name = "ClearLogToolStripMenuItem";
            this.ClearLogToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ClearLogToolStripMenuItem.Text = "清除日志";
            this.ClearLogToolStripMenuItem.Click += new System.EventHandler(this.ClearLogToolStripMenuItem_Click);
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutToolStripMenuItem,
            this.插件ToolStripMenuItem});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.HelpToolStripMenuItem.Text = "帮助";
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.AboutToolStripMenuItem.Text = "关于";
            this.AboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // 插件ToolStripMenuItem -> PluginToolStripMenuItem
            // 
            this.PluginToolStripMenuItem.Name = "插件ToolStripMenuItem";
            this.PluginToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.PluginToolStripMenuItem.Text = "插件";
            this.PluginToolStripMenuItem.Click += new System.EventHandler(this.PluginToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OutputBatchFile,
            this.OutputConfig});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(137, 48);
            // 
            // OutputBatchFile
            // 
            this.OutputBatchFile.Name = "OutputBatchFile";
            this.OutputBatchFile.Size = new System.Drawing.Size(136, 22);
            this.OutputBatchFile.Text = "导出批处理";
            this.OutputBatchFile.Click += new System.EventHandler(this.OutputBatchFile_Click);
            // 
            // OutputConfig
            // 
            this.OutputConfig.Name = "OutputConfig";
            this.OutputConfig.Size = new System.Drawing.Size(136, 22);
            this.OutputConfig.Text = "导出配置";
            this.OutputConfig.Click += new System.EventHandler(this.OutputConfig_Click);
            // 
            // sortTreeView
            // 
            this.sortTreeView.Location = new System.Drawing.Point(0, 22);
            this.sortTreeView.Name = "sortTreeView";
            treeNode1.Name = "Running";
            treeNode1.Tag = "4";
            treeNode1.Text = "运行中";
            treeNode2.Name = "Paused";
            treeNode2.Tag = "7";
            treeNode2.Text = "暂停";
            treeNode3.Name = "Stopped";
            treeNode3.Tag = "1";
            treeNode3.Text = "停止";
            treeNode4.Name = "ByStatus";
            treeNode4.Text = "运行状态";
            treeNode5.Name = "Auto";
            treeNode5.Tag = "2";
            treeNode5.Text = "自动";
            treeNode6.Name = "Demand";
            treeNode6.Tag = "3";
            treeNode6.Text = "手动";
            treeNode7.Name = "Disable";
            treeNode7.Tag = "4";
            treeNode7.Text = "已禁用";
            treeNode8.Name = "StartType";
            treeNode8.Text = "启动类型";
            treeNode9.Name = "canStop";
            treeNode9.Tag = "canStop";
            treeNode9.Text = "可以停止";
            treeNode10.Name = "cannotStop";
            treeNode10.Tag = "cannotStop";
            treeNode10.Text = "无法停止";
            treeNode11.Name = "canPause";
            treeNode11.Tag = "canPause";
            treeNode11.Text = "可以暂停";
            treeNode12.Name = "cannotPause";
            treeNode12.Tag = "cannotPause";
            treeNode12.Text = "无法暂停";
            treeNode13.Name = "canStart";
            treeNode13.Tag = "canStart";
            treeNode13.Text = "可以启动";
            treeNode14.Name = "CanControl";
            treeNode14.Text = "服务可控性";
            treeNode15.Name = "Own";
            treeNode15.Tag = "Own";
            treeNode15.Text = "独立进程服务";
            treeNode16.Name = "Share";
            treeNode16.Tag = "Share";
            treeNode16.Text = "共享进程服务";
            treeNode17.Name = "InteractiveSvc";
            treeNode17.Tag = "Interactive";
            treeNode17.Text = "交互进程服务";
            treeNode18.Name = "ServiceProcess";
            treeNode18.Text = "服务进程";
            treeNode19.Name = "LocalSystem";
            treeNode19.Tag = "LocalSystem";
            treeNode19.Text = "本地系统";
            treeNode20.Name = "LocalService";
            treeNode20.Tag = "LocalService";
            treeNode20.Text = "本地服务";
            treeNode21.Name = "NetworkService";
            treeNode21.Tag = "NetworkService";
            treeNode21.Text = "网络服务";
            treeNode22.Name = "LogonType";
            treeNode22.Text = "登录类型";
            treeNode23.Name = "SearchResult";
            treeNode23.Text = "搜索结果";
            this.sortTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode8,
            treeNode14,
            treeNode18,
            treeNode22,
            treeNode23});
            this.sortTreeView.Size = new System.Drawing.Size(144, 446);
            this.sortTreeView.TabIndex = 9;
            this.sortTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.sortTreeView_AfterSelect);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 10;
            this.label7.Text = "服务分类";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.sortTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel4);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(717, 471);
            this.splitContainer1.SplitterDistance = 144;
            this.splitContainer1.TabIndex = 11;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.ServiceDetailTable);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(321, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(248, 285);
            this.panel4.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 19;
            this.label2.Text = "服务属性";
            // 
            // ServiceDetailTable
            // 
            this.ServiceDetailTable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Outset;
            this.ServiceDetailTable.ColumnCount = 2;
            this.ServiceDetailTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.83983F));
            this.ServiceDetailTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.16017F));
            this.ServiceDetailTable.Controls.Add(this.label24, 0, 2);
            this.ServiceDetailTable.Controls.Add(this.label26, 0, 1);
            this.ServiceDetailTable.Controls.Add(this.label27, 0, 0);
            this.ServiceDetailTable.Controls.Add(this.label29, 0, 3);
            this.ServiceDetailTable.Controls.Add(this.label31, 0, 5);
            this.ServiceDetailTable.Controls.Add(this.label32, 0, 7);
            this.ServiceDetailTable.Controls.Add(this.label33, 0, 8);
            this.ServiceDetailTable.Controls.Add(this.label34, 0, 6);
            this.ServiceDetailTable.Controls.Add(this.label35, 0, 4);
            this.ServiceDetailTable.Controls.Add(this.lblServiceName, 1, 0);
            this.ServiceDetailTable.Controls.Add(this.lblDisplayName, 1, 1);
            this.ServiceDetailTable.Controls.Add(this.lblStatus, 1, 2);
            this.ServiceDetailTable.Controls.Add(this.lblRunMode, 1, 3);
            this.ServiceDetailTable.Controls.Add(this.tbxSysDescription, 1, 4);
            this.ServiceDetailTable.Controls.Add(this.tbxComment, 1, 5);
            this.ServiceDetailTable.Controls.Add(this.lblSuggest, 1, 7);
            this.ServiceDetailTable.Controls.Add(this.lblSuggestAdv, 1, 8);
            this.ServiceDetailTable.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ServiceDetailTable.Location = new System.Drawing.Point(0, 22);
            this.ServiceDetailTable.Name = "ServiceDetailTable";
            this.ServiceDetailTable.RowCount = 9;
            this.ServiceDetailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.ServiceDetailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.ServiceDetailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.ServiceDetailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.ServiceDetailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.ServiceDetailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.ServiceDetailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.ServiceDetailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.ServiceDetailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.ServiceDetailTable.Size = new System.Drawing.Size(248, 263);
            this.ServiceDetailTable.TabIndex = 18;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label24.Location = new System.Drawing.Point(5, 42);
            this.label24.Name = "label24";
            this.label24.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.label24.Size = new System.Drawing.Size(58, 18);
            this.label24.TabIndex = 4;
            this.label24.Text = "当前状态";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label26.Location = new System.Drawing.Point(5, 22);
            this.label26.Name = "label26";
            this.label26.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.label26.Size = new System.Drawing.Size(58, 18);
            this.label26.TabIndex = 1;
            this.label26.Text = "显示名称";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label27.Location = new System.Drawing.Point(5, 2);
            this.label27.Name = "label27";
            this.label27.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.label27.Size = new System.Drawing.Size(58, 18);
            this.label27.TabIndex = 0;
            this.label27.Text = "服务名称";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label29.Location = new System.Drawing.Point(5, 62);
            this.label29.Name = "label29";
            this.label29.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.label29.Size = new System.Drawing.Size(58, 18);
            this.label29.TabIndex = 7;
            this.label29.Text = "启动模式";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label31.Location = new System.Drawing.Point(5, 151);
            this.label31.Margin = new System.Windows.Forms.Padding(3);
            this.label31.Name = "label31";
            this.label31.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.label31.Size = new System.Drawing.Size(58, 46);
            this.label31.TabIndex = 8;
            this.label31.Text = "专家点评";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label32.Location = new System.Drawing.Point(5, 222);
            this.label32.Name = "label32";
            this.label32.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.label32.Size = new System.Drawing.Size(58, 18);
            this.label32.TabIndex = 12;
            this.label32.Text = "普通用户";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label33.Location = new System.Drawing.Point(5, 242);
            this.label33.Name = "label33";
            this.label33.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.label33.Size = new System.Drawing.Size(58, 19);
            this.label33.TabIndex = 13;
            this.label33.Text = "高级用户";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(5, 202);
            this.label34.Name = "label34";
            this.label34.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.label34.Size = new System.Drawing.Size(55, 16);
            this.label34.TabIndex = 14;
            this.label34.Text = "推荐配置";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label35.Location = new System.Drawing.Point(5, 85);
            this.label35.Margin = new System.Windows.Forms.Padding(3);
            this.label35.Name = "label35";
            this.label35.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.label35.Size = new System.Drawing.Size(58, 58);
            this.label35.TabIndex = 16;
            this.label35.Text = "系统描述";
            // 
            // lblServiceName
            // 
            this.lblServiceName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblServiceName.Location = new System.Drawing.Point(71, 2);
            this.lblServiceName.Name = "lblServiceName";
            this.lblServiceName.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.lblServiceName.Size = new System.Drawing.Size(172, 18);
            this.lblServiceName.TabIndex = 0;
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.AutoSize = true;
            this.lblDisplayName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDisplayName.Location = new System.Drawing.Point(71, 22);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.lblDisplayName.Size = new System.Drawing.Size(172, 18);
            this.lblDisplayName.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Location = new System.Drawing.Point(71, 42);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.lblStatus.Size = new System.Drawing.Size(172, 18);
            this.lblStatus.TabIndex = 0;
            // 
            // lblRunMode
            // 
            this.lblRunMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRunMode.Location = new System.Drawing.Point(71, 62);
            this.lblRunMode.Name = "lblRunMode";
            this.lblRunMode.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.lblRunMode.Size = new System.Drawing.Size(172, 18);
            this.lblRunMode.TabIndex = 0;
            // 
            // tbxSysDescription
            // 
            this.tbxSysDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxSysDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxSysDescription.Location = new System.Drawing.Point(71, 85);
            this.tbxSysDescription.Multiline = true;
            this.tbxSysDescription.Name = "tbxSysDescription";
            this.tbxSysDescription.ReadOnly = true;
            this.tbxSysDescription.Size = new System.Drawing.Size(172, 58);
            this.tbxSysDescription.TabIndex = 15;
            // 
            // tbxComment
            // 
            this.tbxComment.BackColor = System.Drawing.SystemColors.Control;
            this.tbxComment.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbxComment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxComment.Location = new System.Drawing.Point(71, 151);
            this.tbxComment.Multiline = true;
            this.tbxComment.Name = "tbxComment";
            this.tbxComment.Size = new System.Drawing.Size(172, 46);
            this.tbxComment.TabIndex = 0;
            // 
            // lblSuggest
            // 
            this.lblSuggest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSuggest.Location = new System.Drawing.Point(71, 222);
            this.lblSuggest.Name = "lblSuggest";
            this.lblSuggest.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.lblSuggest.Size = new System.Drawing.Size(172, 18);
            this.lblSuggest.TabIndex = 0;
            // 
            // lblSuggestAdv
            // 
            this.lblSuggestAdv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSuggestAdv.Location = new System.Drawing.Point(71, 242);
            this.lblSuggestAdv.Name = "lblSuggestAdv";
            this.lblSuggestAdv.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.lblSuggestAdv.Size = new System.Drawing.Size(172, 19);
            this.lblSuggestAdv.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(321, 285);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(248, 91);
            this.panel1.TabIndex = 21;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(251, 19);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(77, 12);
            this.label12.TabIndex = 14;
            this.label12.Text = "服务详细信息";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.ItemSize = new System.Drawing.Size(60, 17);
            this.tabControl1.Location = new System.Drawing.Point(0, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(248, 85);
            this.tabControl1.TabIndex = 13;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.btnPause);
            this.tabPage1.Controls.Add(this.btnStop);
            this.tabPage1.Controls.Add(this.btnStart);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(240, 60);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "更改服务状态";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(168, 47);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(37, 12);
            this.label13.TabIndex = 10;
            this.label13.Text = "停  止";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(88, 47);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(37, 12);
            this.label14.TabIndex = 10;
            this.label14.Text = "启  动";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 47);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(37, 12);
            this.label15.TabIndex = 10;
            this.label15.Text = "暂  停";
            // 
            // btnPause
            // 
            this.btnPause.AutoSize = true;
            this.btnPause.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPause.BackgroundImage")));
            this.btnPause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
            this.btnPause.Location = new System.Drawing.Point(14, 6);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(38, 38);
            this.btnPause.TabIndex = 7;
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStop
            // 
            this.btnStop.AutoSize = true;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.Location = new System.Drawing.Point(170, 6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(38, 38);
            this.btnStop.TabIndex = 8;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.AutoSize = true;
            this.btnStart.Image = ((System.Drawing.Image)(resources.GetObject("btnStart.Image")));
            this.btnStart.Location = new System.Drawing.Point(90, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(38, 38);
            this.btnStart.TabIndex = 9;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.label19);
            this.tabPage2.Controls.Add(this.label20);
            this.tabPage2.Controls.Add(this.btnDemand);
            this.tabPage2.Controls.Add(this.btnAuto);
            this.tabPage2.Controls.Add(this.btnDisable);
            this.tabPage2.Location = new System.Drawing.Point(4, 21);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(240, 60);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "更改启动方式";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(167, 47);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(37, 12);
            this.label16.TabIndex = 3;
            this.label16.Text = "手  动";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(88, 47);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(37, 12);
            this.label19.TabIndex = 2;
            this.label19.Text = "自  动";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(12, 47);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(37, 12);
            this.label20.TabIndex = 1;
            this.label20.Text = "禁  用";
            // 
            // btnDemand
            // 
            this.btnDemand.AutoSize = true;
            this.btnDemand.Image = ((System.Drawing.Image)(resources.GetObject("btnDemand.Image")));
            this.btnDemand.Location = new System.Drawing.Point(170, 6);
            this.btnDemand.Name = "btnDemand";
            this.btnDemand.Size = new System.Drawing.Size(38, 38);
            this.btnDemand.TabIndex = 0;
            this.btnDemand.UseVisualStyleBackColor = true;
            this.btnDemand.Click += new System.EventHandler(this.btnDemand_Click);
            // 
            // btnAuto
            // 
            this.btnAuto.AutoSize = true;
            this.btnAuto.Image = ((System.Drawing.Image)(resources.GetObject("btnAuto.Image")));
            this.btnAuto.Location = new System.Drawing.Point(90, 6);
            this.btnAuto.Name = "btnAuto";
            this.btnAuto.Size = new System.Drawing.Size(38, 38);
            this.btnAuto.TabIndex = 0;
            this.btnAuto.UseVisualStyleBackColor = true;
            this.btnAuto.Click += new System.EventHandler(this.btnAuto_Click);
            // 
            // btnDisable
            // 
            this.btnDisable.AutoSize = true;
            this.btnDisable.Image = ((System.Drawing.Image)(resources.GetObject("btnDisable.Image")));
            this.btnDisable.Location = new System.Drawing.Point(14, 6);
            this.btnDisable.Name = "btnDisable";
            this.btnDisable.Size = new System.Drawing.Size(38, 38);
            this.btnDisable.TabIndex = 0;
            this.btnDisable.UseVisualStyleBackColor = true;
            this.btnDisable.Click += new System.EventHandler(this.btnDisable_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblCount);
            this.panel3.Controls.Add(this.lblService);
            this.panel3.Controls.Add(this.lbxRunning);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(321, 376);
            this.panel3.TabIndex = 20;
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(250, 7);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(65, 12);
            this.lblCount.TabIndex = 21;
            this.lblCount.Text = "共计0000项";
            // 
            // lblService
            // 
            this.lblService.AutoSize = true;
            this.lblService.Location = new System.Drawing.Point(3, 7);
            this.lblService.Name = "lblService";
            this.lblService.Size = new System.Drawing.Size(101, 12);
            this.lblService.TabIndex = 20;
            this.lblService.Text = "当前运行中的服务";
            // 
            // lbxRunning
            // 
            this.lbxRunning.ContextMenuStrip = this.contextMenuStrip1;
            this.lbxRunning.FormattingEnabled = true;
            this.lbxRunning.ItemHeight = 12;
            this.lbxRunning.Location = new System.Drawing.Point(0, 22);
            this.lbxRunning.Name = "lbxRunning";
            this.lbxRunning.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxRunning.Size = new System.Drawing.Size(315, 352);
            this.lbxRunning.TabIndex = 19;
            this.lbxRunning.SelectedIndexChanged += new System.EventHandler(this.lbxRunning_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.lbxDepandentSvc);
            this.panel2.Controls.Add(this.lbxSvcDependOn);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 376);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(569, 95);
            this.panel2.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(295, 2);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(113, 12);
            this.label8.TabIndex = 19;
            this.label8.Text = "依存于此服务的服务";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "此服务依存的服务";
            // 
            // lbxDepandentSvc
            // 
            this.lbxDepandentSvc.DisplayMember = "DisplayName";
            this.lbxDepandentSvc.FormattingEnabled = true;
            this.lbxDepandentSvc.ItemHeight = 12;
            this.lbxDepandentSvc.Location = new System.Drawing.Point(287, 17);
            this.lbxDepandentSvc.Name = "lbxDepandentSvc";
            this.lbxDepandentSvc.Size = new System.Drawing.Size(282, 76);
            this.lbxDepandentSvc.TabIndex = 17;
            // 
            // lbxSvcDependOn
            // 
            this.lbxSvcDependOn.DisplayMember = "DisplayName";
            this.lbxSvcDependOn.FormattingEnabled = true;
            this.lbxSvcDependOn.ItemHeight = 12;
            this.lbxSvcDependOn.Location = new System.Drawing.Point(0, 17);
            this.lbxSvcDependOn.Name = "lbxSvcDependOn";
            this.lbxSvcDependOn.Size = new System.Drawing.Size(281, 76);
            this.lbxSvcDependOn.TabIndex = 16;
            // 
            // tbxSearch
            // 
            this.tbxSearch.Location = new System.Drawing.Point(574, 3);
            this.tbxSearch.Name = "tbxSearch";
            this.tbxSearch.Size = new System.Drawing.Size(100, 20);
            this.tbxSearch.TabIndex = 12;
            this.tbxSearch.Text = "输入搜索文本";
            this.tbxSearch.Click += new System.EventHandler(this.tbxSearchText_Click);
            this.tbxSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxSearchText_KeyPress);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(680, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(34, 21);
            this.btnSearch.TabIndex = 13;
            this.btnSearch.Text = "Go";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "当前状态";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(4, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(185, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "当前状态";
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(0, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(100, 23);
            this.label18.TabIndex = 0;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(0, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 23);
            this.label11.TabIndex = 0;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(0, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(100, 23);
            this.label17.TabIndex = 0;
            // 
            // youhua
            // 
            this.youhua.Location = new System.Drawing.Point(0, 0);
            this.youhua.Name = "youhua";
            this.youhua.Size = new System.Drawing.Size(100, 23);
            this.youhua.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(0, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 23);
            this.label10.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(0, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 23);
            this.label9.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 23);
            this.label6.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 0;
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox3.Location = new System.Drawing.Point(57, 109);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(139, 14);
            this.textBox3.TabIndex = 15;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label36.Location = new System.Drawing.Point(57, 43);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(139, 20);
            this.label36.TabIndex = 3;
            this.label36.Text = "DisplayName";
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label37.Location = new System.Drawing.Point(4, 24);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(9, 1);
            this.label37.TabIndex = 4;
            this.label37.Text = "当前状态";
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(57, 109);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(139, 14);
            this.textBox1.TabIndex = 15;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label21.Location = new System.Drawing.Point(57, 43);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(139, 20);
            this.label21.TabIndex = 3;
            this.label21.Text = "DisplayName";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label22.Location = new System.Drawing.Point(4, 60);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(59, 17);
            this.label22.TabIndex = 4;
            this.label22.Text = "当前状态";
            // 
            // MainFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 498);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.tbxSearch);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainFrom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "系统服务优化专家";
            this.Load += new System.EventHandler(this.Welcome_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ServiceDetailTable.ResumeLayout(false);
            this.ServiceDetailTable.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SvcLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem QuickToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem GuideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ClearLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MasterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CurrentSvcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outputListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outputConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem BatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outputBatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outputCommonToolStripMenuItem;
        private System.Windows.Forms.TreeView sortTreeView;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem StartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 按启动类型ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AutoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DemandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DisabledToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ProcessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ShareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OwnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem InteractiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 按服务登陆类型ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localSvcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NTsvcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem localSysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sQLServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem KAVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iISToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UpdateToolStripMenuItem;
        private System.Windows.Forms.TextBox tbxSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem OutputBatchFile;
        private System.Windows.Forms.ToolStripMenuItem OutputConfig;
        private System.Windows.Forms.ToolStripMenuItem PluginToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbxDepandentSvc;
        private System.Windows.Forms.ListBox lbxSvcDependOn;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label lblService;
        private System.Windows.Forms.ListBox lbxRunning;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnDemand;
        private System.Windows.Forms.Button btnAuto;
        private System.Windows.Forms.Button btnDisable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label youhua;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel ServiceDetailTable;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label lblServiceName;
        private System.Windows.Forms.Label lblDisplayName;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblRunMode;
        private System.Windows.Forms.TextBox tbxSysDescription;
        private System.Windows.Forms.TextBox tbxComment;
        private System.Windows.Forms.Label lblSuggest;
        private System.Windows.Forms.Label lblSuggestAdv;


    }
}

