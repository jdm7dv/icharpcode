// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.Windows.Forms;

namespace ResEdit
{
	/// <summary>
	/// Simple DictionaryEntry edit dialog
	/// </summary>
	public class EditEntry : Form 
	{
		private System.ComponentModel.Container components;
		private Button button2;
		private Button button1;
		private TextBox textBox2;
		private Label label2;
		private TextBox textBox1;
		private Label label1;
		private GroupBox groupBox1;
		DictionaryEntry entry = new DictionaryEntry("", "");
		
		public DictionaryEntry Entry {
			get {
				return entry;
			}
		}
		
		public EditEntry() 
		{
			this.Text = "Edit string entry";
			InitializeComponent();
		}
		
		public EditEntry(object key, object val)
		{
			this.Text   = "New string entry";
			entry.Key   = key;
			entry.Value = val;
			InitializeComponent();
		}
		public void Protect()
		{
			textBox1.ReadOnly = true;
			textBox2.ReadOnly = true;
		}
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.groupBox1  = new GroupBox();
			this.label2     = new Label();
			this.label1     = new Label();
			this.button1    = new Button();
			this.textBox2   = new TextBox();
			this.textBox1   = new TextBox();
			this.button2    = new Button();
			
			groupBox1.Location = new System.Drawing.Point(8, 8);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Entry";
			groupBox1.Size = new System.Drawing.Size(272, 104);
			
			label2.Location = new System.Drawing.Point(8, 56);
			label2.Text = "&Value";
			label2.Size = new System.Drawing.Size(40, 16);
			label2.TabIndex = 2;
			
			label1.Location = new System.Drawing.Point(8, 16);
			label1.Text = "&Name";
			label1.Size = new System.Drawing.Size(40, 16);
			label1.TabIndex = 0;
			
			button1.Location = new System.Drawing.Point(8, 120);
			button1.DialogResult = DialogResult.OK;
			button1.Size = new System.Drawing.Size(75, 23);
			button1.TabIndex = 1;
			button1.Text = "OK";
			button1.Click += new System.EventHandler(button1_Click);
			
			textBox2.Location = new System.Drawing.Point(8, 72);
			textBox2.TabIndex = 3;
			textBox2.Size = new System.Drawing.Size(256, 20);
			
			textBox1.Location = new System.Drawing.Point(8, 32);
			textBox1.TabIndex = 0;
			textBox1.Size = new System.Drawing.Size(256, 20);
			textBox1.Select();
			
			button2.Location = new System.Drawing.Point(96, 120);
			button2.DialogResult = DialogResult.Cancel;
			button2.Size = new System.Drawing.Size(75, 23);
			button2.TabIndex = 2;
			button2.Text = "Cancel";
			this.MaximizeBox = false;
			this.ControlBox = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.MinimizeBox = false;
			this.ClientSize = new System.Drawing.Size(290, 151);
			
			groupBox1.Controls.Add(textBox2);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(textBox1);
			groupBox1.Controls.Add(label1);
			this.Controls.Add(button2);
			this.Controls.Add(button1);
			this.Controls.Add(groupBox1);
			textBox1.Text = entry.Key.ToString();
			textBox2.Text = entry.Value.ToString();
			AcceptButton = button1;
			CancelButton = button2;
			StartPosition = FormStartPosition.CenterScreen;
			Icon = null;
		}
		protected void button1_Click(object sender, System.EventArgs e)
		{
			entry.Key   = textBox1.Text;
			entry.Value = textBox2.Text;
		}
	}
}
