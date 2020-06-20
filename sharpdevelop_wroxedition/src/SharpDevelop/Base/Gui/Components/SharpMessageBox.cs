// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class SharpMessageBox : Form
	{
		Button[] buttons;
		int      retvalue = -1;
		
		public SharpMessageBox(string header, string text, params string[] buttontexts)
		{
			Text = header;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MinimizeBox = HelpButton = MaximizeBox = false;
			StartPosition = FormStartPosition.CenterScreen;
			
			Icon = null;
			ShowInTaskbar = false;
			
			Label label1  = new Label();
			
			label1.Location = new Point(0, 0);
			label1.Text     = text;
			
			Width  = 320;
			Height = 108;
			
			label1.Width = Width;
			label1.Height = 50;
			
			buttons = new Button[buttontexts.Length];
			Bitmap b = new Bitmap(400, 60);
			Graphics g = Graphics.FromImage(b);
			
			for (int i = 0; i < buttontexts.Length; ++i) {
				buttons[i] = new Button();
				buttons[i].Text = buttontexts[i];
				buttons[i].Size = new Size(96, 23);
				
				int v = (Width - buttontexts.Length * (buttons[i].Width + 4)) / 2;
				buttons[i].Location = new Point(v + i * (buttons[i].Width + 4) + Width / buttons[i].Width, 50);
				buttons[i].Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
				buttons[i].Click += new EventHandler(ButtonClick);
				Controls.Add(buttons[i]);
			}
			
			Controls.Add(label1);
			
			AcceptButton = buttons[buttons.Length - 1];
			CancelButton = buttons[buttons.Length - 1];
			buttons[buttons.Length - 1].Select();
		}
		
		void ButtonClick(object sender, EventArgs e)
		{
			for (int i = 0; i < buttons.Length; ++i)
				if (sender == buttons[i]) {
					retvalue = i;
					break;
				}
			DialogResult = DialogResult.OK;
		}
		
		public int ShowMessageBox()
		{
			ShowDialog();
			return retvalue;
		}
	}
}
