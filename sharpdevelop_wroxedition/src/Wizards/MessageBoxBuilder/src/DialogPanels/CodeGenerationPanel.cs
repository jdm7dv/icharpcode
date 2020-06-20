// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using Plugins.Wizards.MessageBoxBuilder.Generator;

namespace Plugins.Wizards.MessageBoxBuilder.DialogPanels {
	/// <summary>
	/// Summary description for Form3.
	/// </summary>
	public class CodeGenerationPanel : AbstractWizardPanel
	{
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.RichTextBox richTextBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		MessageBoxGenerator generator = null;
		IProperties customizer = null;
		
		public override object CustomizationObject {
			get {
				return customizer;
			}
			
			set {
				this.customizer = (IProperties)value;
				generator = (MessageBoxGenerator)customizer.GetProperty("Generator");
				generator.Changed += new EventHandler(OnGeneratorChanged);
			}
		}
		
		void OnGeneratorChanged(object sender, EventArgs e)
		{
			richTextBox1.Text = generator.GenerateCode();
		}
		
		void ChangedEvent(object sender, EventArgs e)
		{
			generator.GenerateReturnValue = checkBox1.Checked;
			generator.VariableName        = textBox1.Text;
			generator.GenerateSwitchCase  = checkBox2.Checked;
			
			textBox1.Text    = "result";
			textBox1.Enabled = checkBox2.Enabled = checkBox1.Checked;
		}

		public CodeGenerationPanel()
		{
			InitializeComponent();
			
			checkBox1.CheckedChanged += new EventHandler(ChangedEvent);
			textBox1.TextChanged     += new EventHandler(ChangedEvent);
			checkBox2.CheckedChanged += new EventHandler(ChangedEvent);
			
			textBox1.Enabled = checkBox2.Enabled = checkBox1.Checked = false;
			
			richTextBox1.WordWrap = false;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
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
			
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label2.Location = new System.Drawing.Point(8, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(384, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Code generation";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Location = new System.Drawing.Point(8, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(384, 2);
			this.label1.TabIndex = 1;
			this.label1.Text = "label1";
			// 
			// checkBox1
			// 
			this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.checkBox1.Location = new System.Drawing.Point(16, 40);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(376, 24);
			this.checkBox1.TabIndex = 3;
			this.checkBox1.Text = "Generate &return value";
			checkBox1.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(32, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 24);
			this.label3.TabIndex = 4;
			this.label3.Text = "Variable &name:";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(136, 72);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(168, 20);
			this.textBox1.TabIndex = 5;
			this.textBox1.Text = "";
			textBox1.BorderStyle = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			// 
			// checkBox2
			// 
			this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.checkBox2.Location = new System.Drawing.Point(16, 104);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(376, 24);
			this.checkBox2.TabIndex = 6;
			this.checkBox2.Text = "Include Switch Case";
			checkBox2.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			//
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label4.Location = new System.Drawing.Point(8, 144);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(384, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "Code Preview";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label5.Location = new System.Drawing.Point(8, 160);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(384, 2);
			this.label5.TabIndex = 7;
			this.label5.Text = "label5";
			// 
			// richTextBox1
			// 
			this.richTextBox1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.richTextBox1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.richTextBox1.Location = new System.Drawing.Point(8, 168);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(384, 144);
			this.richTextBox1.TabIndex = 9;
			this.richTextBox1.Text = "";
			// 
			// Form3
			// 
			this.ClientSize = new System.Drawing.Size(400, 317);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.richTextBox1,
																		  this.label4,
																		  this.label5,
																		  this.checkBox2,
																		  this.textBox1,
																		  this.label3,
																		  this.checkBox1,
																		  this.label2,
																		  this.label1});
			this.Name = "Form3";
			this.Text = "Form3";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
