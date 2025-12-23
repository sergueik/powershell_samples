
namespace Program {
    partial class FormAI {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.aiLabel1 = new System.Windows.Forms.Label();
            this.aiSelectAI = new System.Windows.Forms.ComboBox();
            this.aiButtonSend = new System.Windows.Forms.Button();
            this.aiButtonApply = new System.Windows.Forms.Button();
            this.aiTextField = new System.Windows.Forms.RichTextBox();
            this.aiSelectAction = new System.Windows.Forms.GroupBox();
            this.aiSelectAction_Expand = new System.Windows.Forms.RadioButton();
            this.aiSelectAction_Squeeze = new System.Windows.Forms.RadioButton();
            this.aiSelectAction_Retelling = new System.Windows.Forms.RadioButton();
            this.aiLabelError = new System.Windows.Forms.Label();
            this.aiButtonCancel = new System.Windows.Forms.Button();
            this.aiSelectAction.SuspendLayout();
            this.SuspendLayout();
            // 
            // aiLabel1
            // 
            this.aiLabel1.AutoSize = true;
            this.aiLabel1.Location = new System.Drawing.Point(15, 15);
            this.aiLabel1.Name = "aiLabel1";
            this.aiLabel1.Size = new System.Drawing.Size(94, 17);
            this.aiLabel1.TabIndex = 0;
            this.aiLabel1.Text = "Выберите AI:";
            // 
            // aiSelectAI
            // 
            this.aiSelectAI.FormattingEnabled = true;
            this.aiSelectAI.Items.AddRange(new object[] {
            "YaGPT Lite",
            "MonkeyLearn"});
            this.aiSelectAI.Location = new System.Drawing.Point(12, 45);
            this.aiSelectAI.Name = "aiSelectAI";
            this.aiSelectAI.Size = new System.Drawing.Size(144, 24);
            this.aiSelectAI.TabIndex = 1;
            // 
            // aiButtonSend
            // 
            this.aiButtonSend.Location = new System.Drawing.Point(12, 280);
            this.aiButtonSend.Name = "aiButtonSend";
            this.aiButtonSend.Size = new System.Drawing.Size(200, 40);
            this.aiButtonSend.TabIndex = 2;
            this.aiButtonSend.Text = "Отправить";
            this.aiButtonSend.UseVisualStyleBackColor = true;
            this.aiButtonSend.Click += new System.EventHandler(this.aiButtonSend_Click);
            // 
            // aiButtonApply
            // 
            this.aiButtonApply.Location = new System.Drawing.Point(12, 340);
            this.aiButtonApply.Name = "aiButtonApply";
            this.aiButtonApply.Size = new System.Drawing.Size(200, 40);
            this.aiButtonApply.TabIndex = 3;
            this.aiButtonApply.Text = "Применить к тексту";
            this.aiButtonApply.UseVisualStyleBackColor = true;
            this.aiButtonApply.Click += new System.EventHandler(this.aiButtonApply_Click);
            // 
            // aiTextField
            // 
            this.aiTextField.Location = new System.Drawing.Point(228, 10);
            this.aiTextField.Name = "aiTextField";
            this.aiTextField.Size = new System.Drawing.Size(542, 410);
            this.aiTextField.TabIndex = 4;
            this.aiTextField.Text = "";
            // 
            // aiSelectAction
            // 
            this.aiSelectAction.Controls.Add(this.aiSelectAction_Expand);
            this.aiSelectAction.Controls.Add(this.aiSelectAction_Squeeze);
            this.aiSelectAction.Controls.Add(this.aiSelectAction_Retelling);
            this.aiSelectAction.Location = new System.Drawing.Point(12, 88);
            this.aiSelectAction.Name = "aiSelectAction";
            this.aiSelectAction.Size = new System.Drawing.Size(200, 100);
            this.aiSelectAction.TabIndex = 5;
            this.aiSelectAction.TabStop = false;
            this.aiSelectAction.Text = "Выберите действие:";
            // 
            // aiSelectAction_Expand
            // 
            this.aiSelectAction_Expand.AutoSize = true;
            this.aiSelectAction_Expand.Location = new System.Drawing.Point(6, 73);
            this.aiSelectAction_Expand.Name = "aiSelectAction_Expand";
            this.aiSelectAction_Expand.Size = new System.Drawing.Size(102, 21);
            this.aiSelectAction_Expand.TabIndex = 2;
            this.aiSelectAction_Expand.TabStop = true;
            this.aiSelectAction_Expand.Text = "Расширить";
            this.aiSelectAction_Expand.UseVisualStyleBackColor = true;
            // 
            // aiSelectAction_Squeeze
            // 
            this.aiSelectAction_Squeeze.AutoSize = true;
            this.aiSelectAction_Squeeze.Location = new System.Drawing.Point(6, 46);
            this.aiSelectAction_Squeeze.Name = "aiSelectAction_Squeeze";
            this.aiSelectAction_Squeeze.Size = new System.Drawing.Size(69, 21);
            this.aiSelectAction_Squeeze.TabIndex = 1;
            this.aiSelectAction_Squeeze.TabStop = true;
            this.aiSelectAction_Squeeze.Text = "Сжать";
            this.aiSelectAction_Squeeze.UseVisualStyleBackColor = true;
            // 
            // aiSelectAction_Retelling
            // 
            this.aiSelectAction_Retelling.AutoSize = true;
            this.aiSelectAction_Retelling.Location = new System.Drawing.Point(6, 21);
            this.aiSelectAction_Retelling.Name = "aiSelectAction_Retelling";
            this.aiSelectAction_Retelling.Size = new System.Drawing.Size(114, 21);
            this.aiSelectAction_Retelling.TabIndex = 0;
            this.aiSelectAction_Retelling.TabStop = true;
            this.aiSelectAction_Retelling.Text = "Пересказать";
            this.aiSelectAction_Retelling.UseVisualStyleBackColor = true;
            // 
            // aiLabelError
            // 
            this.aiLabelError.AutoSize = true;
            this.aiLabelError.Location = new System.Drawing.Point(225, 424);
            this.aiLabelError.Name = "aiLabelError";
            this.aiLabelError.Size = new System.Drawing.Size(0, 17);
            this.aiLabelError.TabIndex = 6;
            // 
            // aiButtonCancel
            // 
            this.aiButtonCancel.Location = new System.Drawing.Point(12, 400);
            this.aiButtonCancel.Name = "aiButtonCancel";
            this.aiButtonCancel.Size = new System.Drawing.Size(200, 40);
            this.aiButtonCancel.TabIndex = 7;
            this.aiButtonCancel.Text = "Отмена";
            this.aiButtonCancel.UseVisualStyleBackColor = true;
            this.aiButtonCancel.Click += new System.EventHandler(this.aiButtonCancel_Click);
            // 
            // FormAI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 453);
            this.Controls.Add(this.aiButtonCancel);
            this.Controls.Add(this.aiLabelError);
            this.Controls.Add(this.aiSelectAction);
            this.Controls.Add(this.aiTextField);
            this.Controls.Add(this.aiButtonApply);
            this.Controls.Add(this.aiButtonSend);
            this.Controls.Add(this.aiSelectAI);
            this.Controls.Add(this.aiLabel1);
            this.Location = new System.Drawing.Point(850, 500);
            this.Name = "FormAI";
            this.Text = "AI";
            this.aiSelectAction.ResumeLayout(false);
            this.aiSelectAction.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label aiLabel1;
        private System.Windows.Forms.ComboBox aiSelectAI;
        private System.Windows.Forms.Button aiButtonSend;
        private System.Windows.Forms.Button aiButtonApply;
        private System.Windows.Forms.GroupBox aiSelectAction;
        private System.Windows.Forms.RadioButton aiSelectAction_Expand;
        private System.Windows.Forms.RadioButton aiSelectAction_Squeeze;
        private System.Windows.Forms.RadioButton aiSelectAction_Retelling;
        private System.Windows.Forms.Label aiLabelError;
        internal System.Windows.Forms.RichTextBox aiTextField;
        private System.Windows.Forms.Button aiButtonCancel;
    }
}