using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Runtime.InteropServices;


public class SimpleBrowser : Form
{
    private System.Windows.Forms.WebBrowser webBrowser1;

    public SimpleBrowser()
    {
        InitializeComponent();
        webBrowser1.Navigate("http://localhost:8088/app#/users/sign-in");

    }
    private void InitializeComponent()
    {
        this.webBrowser1 = new System.Windows.Forms.WebBrowser();
        this.webBrowser1.Navigated += webBrowser1_Navigated;
        this.SuspendLayout();
        // 
        // webBrowser1
        // 
        this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.webBrowser1.Location = new System.Drawing.Point(0, 0);
        this.webBrowser1.Name = "webBrowser1";
        this.webBrowser1.Size = new System.Drawing.Size(600, 600);
        this.webBrowser1.TabIndex = 0;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(600, 600);
        this.Controls.Add(this.webBrowser1);
        this.Text = "Split Window";
        this.ResumeLayout(false);
    }
    // Updates the URL in TextBoxAddress upon navigation.
    private void webBrowser1_Navigated(object sender,
        WebBrowserNavigatedEventArgs e)
    {
        // wait for the user to successfully log in 
        // e.g. arrive at Environment page which route is
        // '/app#/environments'
        // then capture the global cookies
        string globalcookies = Form1Helper.GetGlobalCookies(webBrowser1.Url.ToString());
        Console.WriteLine(String.Format("{0}->{1}", webBrowser1.Url.ToString(), globalcookies.ToString()));
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new Form1());
    }
}