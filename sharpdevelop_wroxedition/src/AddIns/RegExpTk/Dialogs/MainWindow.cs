// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Plugins.RegExpTk {

	public class RegExpTkDialog : Form
	{
		private ListView MatchListView = new ListView();
		private Label RegularExpressionLabel = new Label();
		private TextBox RegularExpressionTextBox= new TextBox();
		private Label InputLabel = new Label();
		private TextBox InputTextBox = new TextBox();
		private Label ReplaceLabel = new Label();
		private TextBox ReplaceTextBox= new TextBox();
		private CheckBox MuliLineCheckBox = new CheckBox();
		private CheckBox IgnoreCaseCheckBox = new CheckBox();
		private Button DoItButton = new Button();
		private ColumnHeader StringColumnHeader = new ColumnHeader();
		private ColumnHeader StartColumnHeader = new ColumnHeader();
		private ColumnHeader EndColumnHeader = new ColumnHeader();
		private ColumnHeader LengthColumnHeader = new ColumnHeader();
		private CheckBox ReplaceCheckBox = new CheckBox();
		private StatusBar Statusbar = new StatusBar();
		private StatusBarPanel statusPanel  = new StatusBarPanel();
		private Label ReplaceResultLabel = new Label();
		private TextBox ReplaceResultTextBox = new TextBox();
		
		public RegExpTkDialog()
		{
			InitializeComponent();
		}
		
		void InitializeComponent() {
		RegularExpressionLabel.Location = new System.Drawing.Point(8, 8);
			RegularExpressionLabel.Name = "RegularExpressionLabel";
			RegularExpressionLabel.Size = new System.Drawing.Size(112, 16);
			RegularExpressionLabel.TabIndex = 0;
			RegularExpressionLabel.Text = "Regular expression:";

			RegularExpressionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			RegularExpressionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			RegularExpressionTextBox.Location = new System.Drawing.Point(8, 24);
			RegularExpressionTextBox.Name = "RegularExpressionTextBox";
			RegularExpressionTextBox.Size = new System.Drawing.Size(472, 20);
			RegularExpressionTextBox.TabIndex = 1;
			RegularExpressionTextBox.Text = "";

			InputLabel.Location = new System.Drawing.Point(8, 56);
			InputLabel.Name = "InputLabel";
			InputLabel.Size = new System.Drawing.Size(56, 16);
			InputLabel.TabIndex = 2;
			InputLabel.Text = "Input:";

			InputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			InputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			InputTextBox.Location = new System.Drawing.Point(8, 72);
			InputTextBox.Multiline = true;
			InputTextBox.HideSelection = false;
			InputTextBox.Name = "InputTextBox";
			InputTextBox.Size = new System.Drawing.Size(472, 48);
			InputTextBox.TabIndex = 3;
			InputTextBox.Text = "";

			ReplaceLabel.Location = new System.Drawing.Point(8, 128);
			ReplaceLabel.Name = "ReplaceLabel";
			ReplaceLabel.Size = new System.Drawing.Size(96, 16);
			ReplaceLabel.TabIndex = 4;
			ReplaceLabel.Text = "Replace with:";

			ReplaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			ReplaceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			ReplaceTextBox.Location = new System.Drawing.Point(8, 144);
			ReplaceTextBox.Name = "ReplaceTextBox";
			ReplaceTextBox.Size = new System.Drawing.Size(472, 20);
			ReplaceTextBox.TabIndex = 5;
			ReplaceTextBox.Text = "";

			MatchListView.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			MatchListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			MatchListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							StringColumnHeader,
																							StartColumnHeader,
																							EndColumnHeader,
																							LengthColumnHeader});
			MatchListView.FullRowSelect = true;
			MatchListView.GridLines = true;
			MatchListView.Location = new System.Drawing.Point(8, 248);
			MatchListView.Name = "MatchListView";
			MatchListView.Size = new System.Drawing.Size(472, 112);
			MatchListView.TabIndex = 6;
			MatchListView.View = System.Windows.Forms.View.Details;
			MatchListView.SelectedIndexChanged += new EventHandler(this.MatchListView_SelectedIndexChanged);

			StringColumnHeader.Text = "String";
			StartColumnHeader.Text = "Start";
			EndColumnHeader.Text = "End";
			LengthColumnHeader.Text = "Length";

			MuliLineCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			MuliLineCheckBox.Location = new System.Drawing.Point(112, 176);
			MuliLineCheckBox.Name = "MuliLineCheckBox";
			MuliLineCheckBox.Size = new System.Drawing.Size(72, 16);
			MuliLineCheckBox.TabIndex = 7;
			MuliLineCheckBox.Text = "Multiline";

			IgnoreCaseCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			IgnoreCaseCheckBox.Location = new System.Drawing.Point(200, 176);
			IgnoreCaseCheckBox.Name = "IgnoreCaseCheckBox";
			IgnoreCaseCheckBox.Size = new System.Drawing.Size(88, 16);
			IgnoreCaseCheckBox.TabIndex = 8;
			IgnoreCaseCheckBox.Text = "Ignore case";

			DoItButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			DoItButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			DoItButton.Location = new System.Drawing.Point(376, 172);
			DoItButton.Name = "DoItButton";
			DoItButton.Size = new System.Drawing.Size(104, 24);
			DoItButton.TabIndex = 9;
			DoItButton.Text = "Ok";
			DoItButton.Click += new System.EventHandler(this.DoItButton_Click);
			
			ReplaceCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			ReplaceCheckBox.Location = new System.Drawing.Point(8, 176);
			ReplaceCheckBox.Name = "ReplaceCheckBox";
			ReplaceCheckBox.Size = new System.Drawing.Size(104, 16);
			ReplaceCheckBox.TabIndex = 11;
			ReplaceCheckBox.Text = "Replace";
			ReplaceCheckBox.Checked = false;
			ReplaceCheckBox.CheckedChanged += new System.EventHandler(this.ReplaceCheckBox_CheckedChanged);

			Statusbar.Location = new System.Drawing.Point(0, 373);
			Statusbar.Name = "Statusbar";
			Statusbar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						 statusPanel});
			Statusbar.ShowPanels = true;
			Statusbar.Size = new System.Drawing.Size(488, 24);
			Statusbar.TabIndex = 12;
			Statusbar.Text = "statusBar1";
			statusPanel.AutoSize = StatusBarPanelAutoSize.Spring;

			ReplaceResultLabel.Location = new System.Drawing.Point(8, 200);
			ReplaceResultLabel.Name = "ReplaceResultLabel";
			ReplaceResultLabel.Size = new System.Drawing.Size(168, 16);
			ReplaceResultLabel.TabIndex = 13;
			ReplaceResultLabel.Text = "Replace result:";

			ReplaceResultTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			ReplaceResultTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			ReplaceResultTextBox.Location = new System.Drawing.Point(8, 216);
			ReplaceResultTextBox.Name = "ReplaceResultTextBox";
			ReplaceResultTextBox.Size = new System.Drawing.Size(472, 20);
			ReplaceResultTextBox.TabIndex = 14;
			ReplaceResultTextBox.Text = "";

			AcceptButton = DoItButton;
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			ClientSize = new System.Drawing.Size(488, 397);
			Controls.AddRange(new System.Windows.Forms.Control[] {
																		  ReplaceResultTextBox,
																		  ReplaceResultLabel,
																		  Statusbar,
																		  ReplaceCheckBox,
																		  DoItButton,
																		  IgnoreCaseCheckBox,
																		  MuliLineCheckBox,
																		  MatchListView,
																		  ReplaceTextBox,
																		  ReplaceLabel,
																		  InputTextBox,
																		  InputLabel,
																		  RegularExpressionTextBox,
																		  RegularExpressionLabel});
			Text = "Regular expression toolkit";
			ResumeLayout(false);
			ReplaceCheckBox_CheckedChanged(this, null);
		}
		
		private void DoItButton_Click(object sender, System.EventArgs e)
		{
			MatchCollection matches = null;
			RegexOptions options = new RegexOptions();

			if(IgnoreCaseCheckBox.Checked) {
				options = options | RegexOptions.IgnoreCase;
			}
			if(MuliLineCheckBox.Checked) {
				options = options | RegexOptions.Multiline;
			}
		
			MatchListView.Items.Clear();
			try {
				matches = Regex.Matches(InputTextBox.Text, RegularExpressionTextBox.Text, options);
				if(ReplaceCheckBox.Checked) {
					ReplaceResultTextBox.Text = Regex.Replace(InputTextBox.Text, RegularExpressionTextBox.Text, ReplaceTextBox.Text, options);
				}
			}
			catch(Exception exception) {
				statusPanel.Text = exception.Message;
				return;
			}

			if(matches.Count != 1) {
				statusPanel.Text = matches.Count.ToString() + " matches";
			} else {
				statusPanel.Text = matches.Count.ToString() + " match";
			}

			foreach (Match match in matches) {
				ListViewItem lvwitem = MatchListView.Items.Add(match.ToString());
				lvwitem.SubItems.Add(match.Index.ToString());
				lvwitem.SubItems.Add((match.Index + match.Length).ToString());
				lvwitem.SubItems.Add(match.Length.ToString());
			}
		}
		
		private void MatchListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try {
				InputTextBox.Select(System.Convert.ToInt32(MatchListView.SelectedItems[0].SubItems[1].Text),
										System.Convert.ToInt32(MatchListView.SelectedItems[0].SubItems[3].Text));
			} catch {}
		}
		
		private void ReplaceCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			ReplaceResultTextBox.Enabled = ReplaceCheckBox.Checked;
			ReplaceTextBox.Enabled = ReplaceCheckBox.Checked;
		}
	}
}
