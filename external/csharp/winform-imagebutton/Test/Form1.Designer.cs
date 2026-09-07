using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System;

namespace ImageButtonDemo {
	partial class Form1
	{
		private static float fontSize = 9.25F;
			
		private IContainer components = null;

		protected override void Dispose(bool disposing)
		{
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
		private void InitializeComponent()
		{
			imageButton1 = new ImageButton();
			imageButton13 = new ImageButton();
			imageButton2 = new ImageButton();
			label1 = new Label();
			imageButton4 = new ImageButton();
			imageButton5 = new ImageButton();
			imageButton6 = new ImageButton();
			imageButton7 = new ImageButton();
			imageButton8 = new ImageButton();
			label2 = new Label();
			((ISupportInitialize)(imageButton1)).BeginInit();
			((ISupportInitialize)(imageButton13)).BeginInit();
			((ISupportInitialize)(imageButton2)).BeginInit();
			((ISupportInitialize)(imageButton4)).BeginInit();
			((ISupportInitialize)(imageButton5)).BeginInit();
			((ISupportInitialize)(imageButton6)).BeginInit();
			((ISupportInitialize)(imageButton7)).BeginInit();
			((ISupportInitialize)(imageButton8)).BeginInit();
			SuspendLayout();
			// 
			// imageButton1
			// 
			imageButton1.DialogResult = DialogResult.None;
			imageButton1.DownImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonDownA;
			imageButton1.Font = new Font("Segoe UI", 28F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton1.HoverImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonHoverA;
			imageButton1.Location = new Point(22, 22);
			imageButton1.Margin = new Padding(6);
			imageButton1.Name = "imageButton1";
			imageButton1.NormalImage = null;
			imageButton1.Size = new Size(100, 50);
			imageButton1.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton1.TabIndex = 0;
			imageButton1.TabStop = false;
			imageButton1.Text = "+";
			imageButton1.Click += new EventHandler(imageButton1_Click);
			// 
			// imageButton2
			// 
			imageButton2.DialogResult = DialogResult.None;
			imageButton2.DownImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonDownA;
			imageButton2.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton2.HoverImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonHoverA;
			imageButton2.Location = new Point(216, 22);
			imageButton2.Margin = new Padding(6);
			imageButton2.Name = "imageButton2";
			imageButton2.NormalImage = null;
			imageButton2.Size = new Size(100, 50);
			imageButton2.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton2.TabIndex = 1;
			imageButton2.TabStop = true;
			imageButton2.Text = "\uE174";
			imageButton2.Click += new EventHandler(imageButton2_Click);
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.BackColor = Color.Transparent;
			label1.Location = new Point(87, 166);
			label1.Margin = new Padding(6, 0, 6, 0);
			label1.Name = "label1";
			label1.Size = new Size(172, 25);
			label1.TabIndex = 3;
			label1.Text = "Disable click alerts";
			// 
			// imageButton4
			// 
			imageButton4.DialogResult = DialogResult.None;
			imageButton4.DownImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonDownA;
			imageButton4.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton4.HoverImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonHoverA;
			imageButton4.Location = new Point(411, 22);
			imageButton4.Margin = new Padding(6);
			imageButton4.Name = "imageButton4";
			imageButton4.NormalImage = null;
			imageButton4.Size = new Size(100, 50);
			imageButton4.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton4.TabIndex = 4;
			imageButton4.TabStop = false;
			imageButton4.Text = "\uE102";
			imageButton4.Click += new EventHandler(imageButton4_Click);
			// 
			// imageButton5
			// 
			imageButton5.DialogResult = DialogResult.None;
			imageButton5.DownImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonDownA;
			imageButton5.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton5.HoverImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonHoverA;
			imageButton5.Location = new Point(605, 22);
			imageButton5.Margin = new Padding(6);
			imageButton5.Name = "imageButton5";
			imageButton5.NormalImage = null;
			imageButton5.Size = new Size(100, 50);
			imageButton5.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton5.TabIndex = 5;
			imageButton5.TabStop = true;
			imageButton5.Text = "\uE103";
			imageButton5.Click += new EventHandler(imageButton5_Click);
			// 
			// imageButton6
			// 
			imageButton6.DialogResult = DialogResult.None;
			imageButton6.DownImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonDownA;
			imageButton6.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton6.HoverImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonHoverA;
			imageButton6.Location = new Point(801, 22);
			imageButton6.Margin = new Padding(6);
			imageButton6.Name = "imageButton6";
			imageButton6.NormalImage = null;
			imageButton6.Size = new Size(100, 50);
			imageButton6.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton6.TabIndex = 6;
			imageButton6.TabStop = true;
			imageButton6.Text = "\uE184";
			imageButton6.Click += new EventHandler(imageButton6_Click);
			// 
			// imageButton7
			// 
			imageButton7.DialogResult = DialogResult.None;
			imageButton7.DownImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonDownA;
			imageButton7.Font = new Font("Segoe UI", 19F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton7.HoverImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonHoverA;
			imageButton7.Location = new Point(979, 22);
			imageButton7.Margin = new Padding(6);
			imageButton7.Name = "imageButton7";
			imageButton7.NormalImage = null;
			imageButton7.Size = new Size(100, 50);
			imageButton7.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton7.TabIndex = 7;
			imageButton7.TabStop = true;
			imageButton7.Text = "\uE179";
			imageButton7.Click += new EventHandler(imageButton7_Click);

			// 
			// imageButton7
			// 
			imageButton8.DialogResult = DialogResult.None;
			imageButton8.DownImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonDownA;
			imageButton8.Font = new Font("Segoe UI", 19F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton8.HoverImage = global::ImageButtonDemo.Properties.Resources.ExampleButtonHoverA;
			imageButton8.Location = new Point(979, 22);
			imageButton8.Margin = new Padding(6);
			imageButton8.Name = "imageButton8";
			imageButton8.NormalImage = null;
			imageButton8.Size = new Size(100, 50);
			imageButton8.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton8.TabIndex = 7;
			imageButton8.TabStop = true;
			imageButton8.Text = "\uE107";
			imageButton8.Click += new EventHandler(imageButton8_Click);

			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(31, 113);
			label2.Margin = new Padding(6, 0, 6, 0);
			label2.Name = "label2";
			label2.Size = new Size(144, 25);
			label2.TabIndex = 8;
			label2.Text = "Default button ^";

			// 
			// imageButton13
			// 
			imageButton13.DialogResult = DialogResult.None;
			imageButton13.DownImage = global::ImageButtonDemo.Properties.Resources.CUncheckedDown;
			imageButton13.HoverImage = global::ImageButtonDemo.Properties.Resources.CUncheckedHover;
			imageButton13.Location = new Point(31, 166);
			imageButton13.Name = "imageButton13";
			imageButton13.NormalImage = global::ImageButtonDemo.Properties.Resources.CUncheckedNormal;
			imageButton13.Size = new Size(20, 20);
			imageButton13.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton13.TabIndex = 2;
			imageButton13.TabStop = false;
			imageButton13.Click += new EventHandler(imageButton13_Click);

			// 
			// Form1
			// 
			AcceptButton = imageButton1;
			AutoScaleDimensions = new SizeF(11F, 24F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1280, 234);
			Controls.Add(imageButton8);
			Controls.Add(imageButton7);
			Controls.Add(imageButton6);
			Controls.Add(imageButton5);
			Controls.Add(imageButton4);
			Controls.Add(imageButton13);
			Controls.Add(label1);
			Controls.Add(imageButton2);
			Controls.Add(imageButton1);
			Controls.Add(label2);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Margin = new Padding(6);
			MaximizeBox = false;
			Name = "Form1";
			ShowIcon = false;
			Text = "ImageButton Demo";
			((ISupportInitialize)(imageButton1)).EndInit();
			((ISupportInitialize)(imageButton13)).EndInit();
			((ISupportInitialize)(imageButton2)).EndInit();
			((ISupportInitialize)(imageButton4)).EndInit();
			((ISupportInitialize)(imageButton5)).EndInit();
			((ISupportInitialize)(imageButton6)).EndInit();
			((ISupportInitialize)(imageButton7)).EndInit();
			((ISupportInitialize)(imageButton8)).EndInit();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private ImageButton imageButton1;
		private ImageButton imageButton2;
		private ImageButton imageButton13;
		private Label label1;
		private ImageButton imageButton4;
		private ImageButton imageButton5;
		private ImageButton imageButton6;
		private ImageButton imageButton7;
		private ImageButton imageButton8;
		private Label label2;
	}
}

