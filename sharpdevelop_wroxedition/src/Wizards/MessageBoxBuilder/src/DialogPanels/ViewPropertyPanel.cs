// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using Plugins.Wizards.MessageBoxBuilder.Generator;

namespace Plugins.Wizards.MessageBoxBuilder.DialogPanels {
	/// <summary>
	/// Summary description for Form2.
	/// </summary>
	public class ViewPropertyPanel : AbstractWizardPanel
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.RadioButton radioButton4;
		private System.Windows.Forms.RadioButton radioButton5;
		private System.Windows.Forms.Button button1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		MessageBoxGenerator generator = null;
		IProperties customizer        = null;
		
		public override object CustomizationObject {
			get {
				return customizer;
			}
			set {
				this.customizer = (IProperties)value;
				generator = (MessageBoxGenerator)customizer.GetProperty("Generator");
			}
		}
		
		void ChangedEvent(object sender, EventArgs e)
		{
			generator.Text    = textBox2.Text;
			generator.Caption = textBox1.Text;
			generator.MessageBoxButtons       = (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), comboBox1.Text);
			generator.MessageBoxDefaultButton = (MessageBoxDefaultButton)Enum.Parse(typeof(MessageBoxDefaultButton), comboBox2.Text);
			
			if (radioButton1.Checked) {
				generator.MessageBoxIcon = MessageBoxIcon.Error;
			} else if (radioButton2.Checked) {
				generator.MessageBoxIcon = MessageBoxIcon.Question;
			} else if (radioButton3.Checked) {
				generator.MessageBoxIcon = MessageBoxIcon.Information;
			} else if (radioButton4.Checked) {
				generator.MessageBoxIcon = MessageBoxIcon.Exclamation;
			} else {
				generator.MessageBoxIcon = MessageBoxIcon.None;
			}
		}
		
		void PreviewButton(object sender, EventArgs e)
		{
			generator.PreviewMessageBox();
		}
		
		public ViewPropertyPanel()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			textBox1.TextChanged += new EventHandler(ChangedEvent);
			textBox2.TextChanged += new EventHandler(ChangedEvent);
			
			button1.Click += new EventHandler(PreviewButton);
			
			comboBox1.Items.AddRange(Enum.GetNames(typeof(MessageBoxButtons)));
			comboBox1.SelectedIndex = 0;
			
			comboBox2.Items.AddRange(Enum.GetNames(typeof(MessageBoxDefaultButton)));
			comboBox2.SelectedIndex = 0;
			
			radioButton5.Checked = true;
			
			comboBox1.DropDownStyle = comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
			
			comboBox1.SelectedIndexChanged += new EventHandler(ChangedEvent);
			comboBox2.SelectedIndexChanged += new EventHandler(ChangedEvent);
			
			ResourceManager icons = new ResourceManager("Icons", Assembly.GetAssembly(typeof(ViewPropertyPanel)));

			radioButton1.Image = (Image)icons.GetObject("error");
			radioButton1.CheckedChanged += new EventHandler(ChangedEvent);
			
			radioButton2.Image = (Image)icons.GetObject("ques");
			radioButton2.CheckedChanged += new EventHandler(ChangedEvent);
			
			radioButton3.Image = (Image)icons.GetObject("information");
			radioButton3.CheckedChanged += new EventHandler(ChangedEvent);
			
			radioButton4.Image = (Image)icons.GetObject("exclamation");
			radioButton4.CheckedChanged += new EventHandler(ChangedEvent);
			
			radioButton5.CheckedChanged += new EventHandler(ChangedEvent);
			
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing){
				if(components != null) {
					components.Dispose();
				}
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
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this.radioButton4 = new System.Windows.Forms.RadioButton();
			this.radioButton5 = new System.Windows.Forms.RadioButton();
			this.button1 = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Location = new System.Drawing.Point(8, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(432, 2);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label2.Location = new System.Drawing.Point(8, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(432, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Text";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 24);
			this.label3.TabIndex = 2;
			this.label3.Text = "&Caption";
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.textBox1.Location = new System.Drawing.Point(80, 32);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(352, 20);
			this.textBox1.TabIndex = 3;
			this.textBox1.Text = "";
			textBox1.BorderStyle = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 23);
			this.label4.TabIndex = 4;
			this.label4.Text = "&Message";
			// 
			// textBox2
			// 
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.textBox2.Location = new System.Drawing.Point(80, 56);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(352, 64);
			this.textBox2.TabIndex = 5;
			this.textBox2.Text = "";
			textBox2.BorderStyle = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label5.Location = new System.Drawing.Point(8, 152);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(432, 2);
			this.label5.TabIndex = 0;
			this.label5.Text = "label5";
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label6.Location = new System.Drawing.Point(8, 136);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(432, 16);
			this.label6.TabIndex = 0;
			this.label6.Text = "Buttons";
			// 
			// comboBox1
			// 
			this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.comboBox1.Location = new System.Drawing.Point(96, 168);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(152, 21);
			this.comboBox1.TabIndex = 7;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 168);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(88, 24);
			this.label7.TabIndex = 6;
			this.label7.Text = "Buttons &shown";
			// 
			// label8
			// 
			this.label8.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.label8.Location = new System.Drawing.Point(256, 168);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(80, 16);
			this.label8.TabIndex = 8;
			this.label8.Text = "&Default Button:";
			// 
			// comboBox2
			// 
			this.comboBox2.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.comboBox2.Location = new System.Drawing.Point(344, 168);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(96, 21);
			this.comboBox2.TabIndex = 9;
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label9.Location = new System.Drawing.Point(8, 208);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(432, 16);
			this.label9.TabIndex = 0;
			this.label9.Text = "Icons";
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label10.Location = new System.Drawing.Point(8, 224);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(432, 2);
			this.label10.TabIndex = 0;
			this.label10.Text = "label10";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.radioButton5,
																					this.radioButton4,
																					this.radioButton3,
																					this.radioButton2,
																					this.radioButton1});
			this.groupBox1.Location = new System.Drawing.Point(8, 240);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(256, 72);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Icon Shown";
			groupBox1.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			// 
			// radioButton1
			// 
			this.radioButton1.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.radioButton1.Location = new System.Drawing.Point(16, 24);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(40, 32);
			this.radioButton1.TabIndex = 0;
			radioButton1.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			// 
			// radioButton2
			// 
			this.radioButton2.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.radioButton2.Location = new System.Drawing.Point(64, 24);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(40, 32);
			this.radioButton2.TabIndex = 1;
			radioButton2.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;

			//
			// radioButton3
			// 
			this.radioButton3.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.radioButton3.Location = new System.Drawing.Point(112, 24);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.Size = new System.Drawing.Size(40, 32);
			this.radioButton3.TabIndex = 2;
			radioButton3.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			// 
			// radioButton4
			// 
			this.radioButton4.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.radioButton4.Location = new System.Drawing.Point(160, 24);
			this.radioButton4.Name = "radioButton4";
			this.radioButton4.Size = new System.Drawing.Size(40, 32);
			this.radioButton4.TabIndex = 3;
			radioButton4.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// radioButton5
			// 
			this.radioButton5.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.radioButton5.Location = new System.Drawing.Point(208, 24);
			this.radioButton5.Name = "radioButton5";
			this.radioButton5.Size = new System.Drawing.Size(40, 32);
			this.radioButton5.TabIndex = 4;
			this.radioButton5.Text = "None";
			radioButton5.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(272, 248);
			this.button1.Name = "button1";
			this.button1.TabIndex = 11;
			this.button1.Text = "Preview";
			button1.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			// 
			// Form2
			// 
			this.ClientSize = new System.Drawing.Size(448, 325);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.button1,
																		  this.groupBox1,
																		  this.label9,
																		  this.label10,
																		  this.comboBox2,
																		  this.label8,
																		  this.label7,
																		  this.comboBox1,
																		  this.label6,
																		  this.label5,
																		  this.textBox2,
																		  this.label4,
																		  this.textBox1,
																		  this.label3,
																		  this.label2,
																		  this.label1});
			this.Name = "Form2";
			this.Text = "Form2";
			this.Load += new System.EventHandler(this.Form2_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void Form2_Load(object sender, System.EventArgs e)
		{
		
		}
	}
}
