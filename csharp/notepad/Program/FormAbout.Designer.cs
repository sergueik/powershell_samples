
namespace Program {
    partial class FormAbout
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.Label_Main = new System.Windows.Forms.Label();
            this.SuspendLayout();
 
            this.Label_Main.AutoSize = true;
            this.Label_Main.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_Main.Location = new System.Drawing.Point(12, 53);
            this.Label_Main.Name = "Label_Main";
            this.Label_Main.Size = new System.Drawing.Size(478, 25);
            this.Label_Main.TabIndex = 0;
            this.Label_Main.Text = "Программу создал Башмаков Владислав 2231122";
            this.Label_Main.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(582, 153);
            this.Controls.Add(this.Label_Main);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormAbout";
            this.Text = "О программе";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_Main;
    }
}