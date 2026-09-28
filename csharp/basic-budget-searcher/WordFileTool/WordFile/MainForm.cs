
// using CCWin;
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using NPOI.XWPF.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
// using Word = Microsoft.Office.Interop.Word;

namespace WordFile
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

 //       private Word.Application G_WordApplication; //定义Word应用程序字段
   //     private object G_Missing = Type.Missing; //定义G_Missing字段并添加引用

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdia = new FolderBrowserDialog();

            if (fbdia.ShowDialog() == DialogResult.OK)
            {
                txtDocDirectory.Text = fbdia.SelectedPath;
            }
        }


        /// <summary>
        /// NPOI 组件调用的方法，只支持docx后缀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReplaceText_Click(object sender, EventArgs e)
        {
            string docDirectory = txtDocDirectory.Text;

            ThreadPool.QueueUserWorkItem( //开始线程池
                (o) => //使用Lambda表达式
                {
                    DirectoryInfo dires = new DirectoryInfo(docDirectory);
                    FileInfo[] files = dires.GetFiles("*.docx");

                    foreach (FileInfo f in files)
                    {
                        //文件路径
                        string filePath = f.ToString();
                        //替换key,value字典合集
                        var dic = new Dictionary<string, string> { };

                        using (var stream = File.OpenRead(filePath))
                        {
                            var doc = new XWPFDocument(stream);

                            try
                            {
                                if (txtSearchKey1.Text != "")
                                {
                                    dic.Add(txtSearchKey1.Text, txtReplace1.Text);
                                }

                                if (txtSearchKey2.Text != "")
                                {
                                    dic.Add(txtSearchKey2.Text, txtReplace2.Text);
                                }

                                if (txtSearchKey3.Text != "")
                                {
                                    dic.Add(txtSearchKey3.Text, txtReplace3.Text);
                                }

                                if (txtSearchKey4.Text != "")
                                {
                                    dic.Add(txtSearchKey4.Text, txtReplace4.Text);
                                }

                                if (txtSearchKey5.Text != "")
                                {
                                    dic.Add(txtSearchKey5.Text, txtReplace5.Text);
                                }

                                foreach (var para in doc.Paragraphs)
                                {
                                    //方法一
                                    ReplaceKey(para, dic);
                                    //方法二
                                    //ReplaceKeyword(para, dic);
                                }

                                using (var newstream = File.Create(f.Directory + "/" + f.Name)) // "/poutput.docx"
                                {
                                    doc.Write(newstream);
                                    newstream.Flush();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("工具异常:" + ex.Message);
                            }
                        }
                    }

                    MessageBox.Show("替换完成");
                });
        }

/*
            string dir1 = txtDocDirectory.Text;

            ThreadPool.QueueUserWorkItem( //开始线程池
                (o) => //使用Lambda表达式
                {
                    DirectoryInfo dires = new DirectoryInfo(dir1);
                    FileInfo[] files = dires.GetFiles("*.doc");
                    G_WordApplication = //创建Word应用程序对象
                        new Word.Application();
                    foreach (FileInfo f in files)
                    {
                        object P_FilePath = dir1 + "\\\\" + f; //创建Object对象

                        try
                        {
                            Word.Document P_Document = G_WordApplication.Documents.Open( //打开Word文档
                                ref P_FilePath, ref G_Missing, ref G_Missing,
                                ref G_Missing, ref G_Missing, ref G_Missing,
                                ref G_Missing, ref G_Missing, ref G_Missing,
                                ref G_Missing, ref G_Missing, ref G_Missing,
                                ref G_Missing, ref G_Missing, ref G_Missing,
                                ref G_Missing);

                            Word.Range P_Range = //得到文档范围
                                P_Document.Range(ref G_Missing, ref G_Missing);
                            Word.Find P_Find = //得到Find对象
                                P_Range.Find;

                            if (txtSearchKey1.Text != "")
                            {
                                this.Invoke( //在窗体线程中执行
                                    (MethodInvoker)(() => //使用Lambda表达式
                                    {
                                        P_Find.Text = txtSearchKey1.Text; //设置查找的文本
                                        P_Find.Replacement.Text = txtReplace1.Text; //设置替换的文本
                                    }));
                                object P_Replace = Word.WdReplace.wdReplaceAll; //定义替换方式对象
                                bool P_bl = P_Find.Execute( //开始替换
                                    ref G_Missing, ref G_Missing, ref G_Missing,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref P_Replace,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing);
                            }

                            if (txtSearchKey2.Text != "")
                            {
                                this.Invoke( //在窗体线程中执行
                                    (MethodInvoker)(() => //使用Lambda表达式
                                    {
                                        P_Find.Text = txtSearchKey2.Text; //设置查找的文本
                                        P_Find.Replacement.Text = txtReplace2.Text; //设置替换的文本
                                    }));
                                object P_Replace = Word.WdReplace.wdReplaceAll; //定义替换方式对象
                                bool P_bl = P_Find.Execute( //开始替换
                                    ref G_Missing, ref G_Missing, ref G_Missing,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref P_Replace,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing);
                            }

                            if (txtSearchKey3.Text != "")
                            {
                                this.Invoke( //在窗体线程中执行
                                    (MethodInvoker)(() => //使用Lambda表达式
                                    {
                                        P_Find.Text = txtSearchKey3.Text; //设置查找的文本
                                        P_Find.Replacement.Text = txtReplace3.Text; //设置替换的文本
                                    }));
                                object P_Replace = Word.WdReplace.wdReplaceAll; //定义替换方式对象
                                bool P_bl = P_Find.Execute( //开始替换
                                    ref G_Missing, ref G_Missing, ref G_Missing,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref P_Replace,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing);
                            }

                            if (txtSearchKey4.Text != "")
                            {
                                this.Invoke( //在窗体线程中执行
                                    (MethodInvoker)(() => //使用Lambda表达式
                                    {
                                        P_Find.Text = txtSearchKey4.Text; //设置查找的文本
                                        P_Find.Replacement.Text = txtReplace4.Text; //设置替换的文本
                                    }));
                                object P_Replace = Word.WdReplace.wdReplaceAll; //定义替换方式对象
                                bool P_bl = P_Find.Execute( //开始替换
                                    ref G_Missing, ref G_Missing, ref G_Missing,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref P_Replace,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing);
                            }

                            if (txtSearchKey5.Text != "")
                            {
                                this.Invoke( //在窗体线程中执行
                                    (MethodInvoker)(() => //使用Lambda表达式
                                    {
                                        P_Find.Text = txtSearchKey5.Text; //设置查找的文本
                                        P_Find.Replacement.Text = txtReplace5.Text; //设置替换的文本
                                    }));
                                object P_Replace = Word.WdReplace.wdReplaceAll; //定义替换方式对象
                                bool P_bl = P_Find.Execute( //开始替换
                                    ref G_Missing, ref G_Missing, ref G_Missing,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref P_Replace,
                                    ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing);
                            }

                            G_WordApplication.Documents.Save( //保存文档
                                ref G_Missing, ref G_Missing);
                            ((Word._Document)P_Document).Close( //关闭文档
                                ref G_Missing, ref G_Missing, ref G_Missing);
                        }
                        catch (Exception g)
                        {
                        }
                    }

                    ((Word._Application)G_WordApplication).Quit( //退出Word应用程序
                        ref G_Missing, ref G_Missing, ref G_Missing);
                    MessageBox.Show("替换完成");
                });
        }
*/
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReplaceKey(XWPFParagraph para, IDictionary<string, string> redic)
        {
            string text = para.ParagraphText; //段落文本
            foreach (var kv in redic)
            {
                if (text.Contains(kv.Key))
                {
                    para.ReplaceText(kv.Key, kv.Value);
                }
            }
        }

        private void ReplaceKeyword(XWPFParagraph para, IDictionary<string, string> redic)
        {
            string text = string.Empty; //段落文本
            string styleid = para.Style;
            var runs = para.Runs;
            for (int i = 0; i < runs.Count; i++)
            {
                var run = runs[i];
                text = run.ToString();
                foreach (var kv in redic)
                {
                    if (text.Contains(kv.Key))
                    {
                        text = text.Replace(kv.Key, kv.Value);
                    }
                }

                runs[i].SetText(text, 0);
            }
        }
    }
}