using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Program {
    public partial class FormMain : Form {
        public const string title = "БлокнотПлюсПлюс";
        public const string unNamed = "Безымянный";

        public static string pathIconBlue = "csharp_blue.ico";
        public static string pathIconRed = "csharp_red.ico";

        public string fileName = unNamed;
        public bool fileIsNew = true;
        public bool fileIsSaved = true;

        string printString = "";

        OpenFileDialog ofd = new OpenFileDialog();
        SaveFileDialog sfd = new SaveFileDialog();

        public FormMain() {
            InitializeComponent();

            ofd.Filter = "Text file|*.txt|Rich Text Format file|*.rtf|Any format|*.*";
            sfd.Filter = "Text file|*.txt|Rich Text Format file|*.rtf|Any format|*.*";

            resizeForm();
            changeTitle();
            changeIcon();
            updateStatusBar();
        }

        private void FormMain_Resize(object sender, EventArgs e) {
            resizeForm();
        }

        private void menuItem_File_CreateFile_Click(object sender, EventArgs e) {
            if (fileIsSaved == false) {
                if (showFormSave() == false) {
                    return;
                }
            }

            newFile();
        }

        private void menuItem_File_OpenFile_Click(object sender, EventArgs e) {
            if (fileIsSaved == false) {
                if (showFormSave() == false) {
                    return;
                }
            }

            newFile();

            if (ofd.ShowDialog() == DialogResult.OK) {
                fileName = ofd.FileName;

                if (fileName.EndsWith(".txt")) {
                    mainTextField.LoadFile(fileName, RichTextBoxStreamType.PlainText);
                } else if (fileName.EndsWith(".rtf")) {
                    mainTextField.LoadFile(fileName, RichTextBoxStreamType.RichText);
                } else {
                    mainTextField.LoadFile(fileName, RichTextBoxStreamType.PlainText);
                }

                fileIsNew = false;
                fileIsSaved = true;
            } else {
                return;
            }

            changeTitle();
            changeIcon();
        }

        private void menuItem_File_SaveFile_Click(object sender, EventArgs e) {
            if (fileIsNew == true) {
                if (sfd.ShowDialog() == DialogResult.OK) {
                    saveFile();

                    fileIsNew = false;
                } else {
                    fileIsSaved = false;
                }
            } else {
                saveFile();
            }
        }

        private void menuItem_File_SaveFileAs_Click(object sender, EventArgs e) {
            if (sfd.ShowDialog() == DialogResult.OK) {
                saveFile();
            }
        }

        private void menuItem_File_Print_Click(object sender, EventArgs e) {
            printString = mainTextField.Text;
            if (printString == "") return;

            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintPageHandler;

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;

            if (printDialog.ShowDialog() == DialogResult.OK) {
                printDialog.Document.Print();
            }
        }

        private void PrintPageHandler(object sender, PrintPageEventArgs e) {
            e.Graphics.DrawString(printString, new Font("Arial", 14), Brushes.Black, 0, 0);
        }

        private void menuItem_File_Exit_Click(object sender, EventArgs e) {
            if (fileIsSaved == false) {
                if (showFormSave() == false) {
                    return;
                }
            }

            this.Close();
        }

        private void mainTextFieldZ(object sender, EventArgs e) {
            mainTextField.Undo();
        }

        private void mainTextFieldX(object sender, EventArgs e) {
            mainTextField.Cut();
        }

        private void mainTextFieldC(object sender, EventArgs e) {
            if (mainTextField.SelectedText.Length == 0) return;
            Clipboard.SetText(mainTextField.SelectedText);
        }

        private void mainTextFieldV(object sender, EventArgs e) {
            mainTextField.Paste();
        }

        private void mainTextFieldDel(object sender, EventArgs e) {
            mainTextField.Cut();
        }

        private void mainTextFieldA(object sender, EventArgs e) {
            mainTextField.SelectAll();
            mainTextField.Focus();
        }

        private void menuItem_Format_Font_Click(object sender, EventArgs e) {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == DialogResult.OK) {
                mainTextField.Font = fontDialog.Font;
            }
        }

        private void menuItem_View_Scale_Up_Click(object sender, EventArgs e) {
            changeZoom("up");
        }

        private void menuItem_View_Scale_Down_Click(object sender, EventArgs e) {
            changeZoom("down");
        }

        private void menuItem_View_Scale_Recover_Click(object sender, EventArgs e) {
            changeZoom("recover");
        }

        private void menuItem_View_StatusBar_Click(object sender, EventArgs e) {
            if (menuItem_View_StatusBar.CheckState == CheckState.Checked) {
                menuItem_View_StatusBar.CheckState = CheckState.Unchecked;
                mainStatusBar.Visible = false;
                mainTextField.Size = new Size(mainTextField.Size.Width, mainTextField.Size.Height + mainStatusBar.Size.Height);
            } else {
                menuItem_View_StatusBar.CheckState = CheckState.Checked;
                mainStatusBar.Visible = true;
                mainTextField.Size = new Size(mainTextField.Size.Width, mainTextField.Size.Height - mainStatusBar.Size.Height);
            }
        }

        private void menuItem_Reference_About_Click(object sender, EventArgs e) {
            new FormAbout().Show();
        }

        private void menuItem_AI_Click(object sender, EventArgs e) {
            FormAI formAI = new FormAI(mainTextField.Text);

            if (formAI.ShowDialog() == DialogResult.OK) {
                this.mainTextField.Text = formAI.aiTextField.Text;
            }
        }

        private void mainTextField_TextChanged(object sender, EventArgs e) {
            fileIsSaved = false;
            changeTitle();
            changeIcon();
            updateStatusBar();
        }

        private void mainTextField_KeyUp(object sender, KeyEventArgs e) {
            if (e.Control == true && e.KeyCode == Keys.F) {
                new FormFind(ref mainTextField).Show();
            } else if (e.Control == true && e.KeyCode == Keys.Oemplus) {
                changeZoom("up");
            } else if (e.Control == true && e.KeyCode == Keys.OemMinus) {
                changeZoom("down");
            }
        }

        private void changeTitle() {
            string newTitle = "";

            if (fileIsSaved == false) {
                newTitle += "* ";
            }

            newTitle += fileName;
            newTitle += " - ";
            newTitle += title;

            this.Text = newTitle;    
        }

        private void changeIcon() {
            if (fileIsSaved == true) {
                this.Icon = new Icon(pathIconBlue);
            } else {
                this.Icon = new Icon(pathIconRed);
            }
        }

        private void resizeForm() {
            mainTextField.Width = this.Width - 30;
            mainTextField.Height = this.Height - 120;
        }

        private void updateStatusBar() {
            mainStatusBar_Symbols.Text = String.Format("Символов: {0}", mainTextField.Text.Length);
            mainStatusBar_Strings.Text = String.Format("Строк: {0}", mainTextField.Text.Split('\n').Length);
            mainStatusBar_Scale.Text = String.Format("Масштаб: {0}%", mainTextField.ZoomFactor);
            mainStatusBar.Refresh();
            mainStatusBar.Update();
        }

        private bool showFormSave() {
            FormSave formSave = new FormSave(fileName);
            DialogResult dialogResult = formSave.ShowDialog();

            return handlerFormSave(dialogResult);
        }

        private bool handlerFormSave(DialogResult dialogResult) {
            if (dialogResult == DialogResult.Yes) {
                if (fileIsNew == true) {
                    if (sfd.ShowDialog () == DialogResult.OK) {

                        return saveFile();
                    } else {
                        return false;
                    }
                } else {
                    return saveFile();
                }
            } else if (dialogResult == DialogResult.No) {
                this.Close();
                return false;
            } else if (dialogResult == DialogResult.Cancel) {
                return false;
            } else {
                return false;
            }
        }

        private void newFile() {
            fileName = unNamed;
            fileIsNew = true;
            fileIsSaved = true;

            mainTextField.Text = "";

            changeTitle();
            changeIcon();
        }

        private bool saveFile() {
            if (sfd.FileName.EndsWith(".txt")) {
                mainTextField.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
            } else if (sfd.FileName.EndsWith(".rtf")) {
                mainTextField.SaveFile(sfd.FileName, RichTextBoxStreamType.RichText);
            } else {
                mainTextField.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
            }

            fileName = sfd.FileName;
            fileIsSaved = true;
            changeTitle();
            changeIcon();

            return true;
        }

        private void changeZoom(string value) {
            if (value == "up") {
                if (mainTextField.ZoomFactor >= 63) return;

                mainTextField.ZoomFactor += 1;
            } else if (value == "down") {
                if (mainTextField.ZoomFactor < 0.3) return;

                if (mainTextField.ZoomFactor > 1) {
                    mainTextField.ZoomFactor -= 1;
                } else {
                    mainTextField.ZoomFactor -= 0.1f;
                }
            } else if (value == "recover") {
                mainTextField.ZoomFactor = 1;
            }
            updateStatusBar();
        }
    }
}
