using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;

namespace ICSharpCode.StartPage 
{
	/// <summary>
	/// This is the ViewContent implementation for the Start Page.
	/// </summary>
	public class StartPageView : AbstractViewContent
	{
		// defining the control variables used
		Panel panel    		= new Panel();
		Label capLabel 		= new Label();
		Label titleLabel	= new Label();
		Label modLabel		= new Label();
		Label versionLabel	= new Label();
		Button openBtn		= new Button();
		Button newBtn		= new Button();
		PictureBox startPic = new PictureBox();
		PictureBox topPic	= new PictureBox();
		PictureBox bderPic1	= new PictureBox();
		PictureBox bderPic2	= new PictureBox();
		PictureBox gradPic1 = new PictureBox();
		PictureBox gradPic2 = new PictureBox();
		LinkLabel[] prjLink = new LinkLabel[10];
		Label[] modifiedLabel = new Label[10];
		
		ToolTip toolTip;		
		
		// return the panel that contains all of our controls
		public override Control Control {
			get {
				return panel;
			}
		}
		
		// the content cannot be modified
		public override bool IsViewOnly {
			get {
				return true;
			}
		}
		
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		// these methods are unused in this view
		public override void SaveFile(string fileName) 
		{}
		public override void LoadFile(string fileName) 
		{}
		
		// the redraw should get new add-in tree information
		// and update the view, the language or layout manager
		// may have changed.
		public override void RedrawContent()
		{
			
		}
		
		// Dispose all controls contained in this panel
		public override void Dispose()
		{
			try {
				foreach(Control ctl in panel.Controls) {
					ctl.Dispose();
				}
				panel.Dispose();
			} catch {}
		}
		
		
		// Default constructor: Initialize controls and display recent projects.
		public StartPageView()
		{
			Color linkColor, lightColor;
			
			// using very primitive exception handling here...
			// since we don't want to bug the user with ugly error messages
			// from a noncritical addin, we just do nothing on an exception
			try {
				toolTip				= new ToolTip();
				
				// colors used:
				// linkColor  = color for links and button captions
				// lightColor = color for border
				linkColor 	= Color.FromArgb(0, 51, 153);
				lightColor	= Color.FromArgb(204, 137, 32);
				
				panel.SuspendLayout();
				
				startPic.Location 	= new Point(0, 0);
				startPic.SizeMode	= PictureBoxSizeMode.AutoSize;
				startPic.Image 		= Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("start.png"));
				
				topPic.Location		= new Point(startPic.Width, 0);
				topPic.Size			= new Size(panel.Width - startPic.Width, startPic.Height);
				topPic.Anchor		= AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
				topPic.BackgroundImage = Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("top.png"));
				
				capLabel.Text     	= " Recent Projects";
				capLabel.Location 	= new Point(25, 115);
				capLabel.Font 		= new Font(panel.Font, FontStyle.Bold);
				capLabel.BackColor 	= lightColor;
				capLabel.Size		= new Size(250, 22);
				capLabel.TextAlign	= ContentAlignment.MiddleLeft;
				capLabel.ForeColor	= Color.White;
				
				bderPic1.Location	= new Point(capLabel.Left, capLabel.Top + 20);
				bderPic1.Size		= new Size(3, 230);
				bderPic1.BackColor	= lightColor;
				
				bderPic2.Location	= new Point(capLabel.Left, capLabel.Top + 250);
				bderPic2.Size		= new Size(capLabel.Width, 3);
				bderPic2.BackColor	= bderPic1.BackColor;
				
				Image gradImg		= Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("gradient.png"));
				
				gradPic1.Image		= gradImg;
				gradPic1.Location	= new Point(capLabel.Left + capLabel.Width, capLabel.Top);
				gradPic1.Size		= new Size(100, capLabel.Height);
				
				gradPic2.Image		= gradImg;
				gradPic2.Location	= new Point(bderPic2.Left + bderPic2.Width, bderPic2.Top);
				gradPic2.Size		= new Size(100, bderPic2.Height);
				
				titleLabel.Text		= "Name";
				titleLabel.ForeColor = Color.Gray;
				titleLabel.Location	= new Point(capLabel.Left + 10, capLabel.Top + 28);
				
				modLabel.Text		= "Modified";
				modLabel.ForeColor	= titleLabel.ForeColor;
				modLabel.Location	= new Point(capLabel.Left + 225, titleLabel.Top);
				
				openBtn.Text 		= "Open Project ";
				openBtn.Location	= new Point(capLabel.Left, capLabel.Top + 270);
				openBtn.Size		= new Size(140, 24);
				openBtn.Font		= capLabel.Font;
				openBtn.Click		+= new EventHandler(OpenBtnClicked);
				openBtn.FlatStyle	= FlatStyle.Popup;
				openBtn.BackColor	= Color.FromArgb(240, 240, 224);
				openBtn.ForeColor	= linkColor;
				openBtn.Cursor		= Cursors.Hand;
				
				newBtn.Text 		= "New Combine ";
				newBtn.Location		= new Point(openBtn.Right + 20, openBtn.Top);
				newBtn.Size			= openBtn.Size;
				newBtn.Font			= openBtn.Font;
				newBtn.Click		+= new EventHandler(NewBtnClicked);
				newBtn.FlatStyle	= openBtn.FlatStyle;
				newBtn.BackColor	= openBtn.BackColor;
				newBtn.ForeColor	= openBtn.ForeColor;
				newBtn.Cursor		= openBtn.Cursor;
				
				Version v = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
				versionLabel.Text	= "SharpDevelop version " + v.Major + "." + v.Minor + " build " + v.Revision + "." + v.Build;
				versionLabel.Location = new Point(0, panel.Height - 25);
				versionLabel.Size	= new Size(panel.Width, 25);
				versionLabel.Anchor	= AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				versionLabel.BackColor = Color.FromArgb(133, 170, 204);
				versionLabel.ForeColor = Color.White;
				versionLabel.TextAlign = ContentAlignment.MiddleRight;
				versionLabel.Font	= capLabel.Font;
				versionLabel.BackgroundImage = topPic.BackgroundImage;
				
				toolTip.SetToolTip(openBtn, "Open a combine or project");
				toolTip.SetToolTip(newBtn, "Create a new combine");
				
			} catch { 
				return; 
			}
			
			try {
				// Get the recent projects
				Core.Properties.DefaultProperties svc = (Core.Properties.DefaultProperties)Core.Services.ServiceManager.Services.GetService(typeof(Core.Services.PropertyService));
				object recentOpenObj = svc.GetProperty("ICSharpCode.SharpDevelop.Gui.MainWindow.RecentOpen");
				if (recentOpenObj is ICSharpCode.SharpDevelop.Services.RecentOpen) {
					ICSharpCode.SharpDevelop.Services.RecentOpen recOpen = (ICSharpCode.SharpDevelop.Services.RecentOpen)recentOpenObj;
					
					int i = 0;  // is used to count the number of overall items; 10 is maximum
					foreach(string fileName in recOpen.RecentProject) {
						// if the file does not exist, goto next one
						if (!System.IO.File.Exists(fileName)) {
							continue;
						}
						
						// set lk and mod to the controls at i.
						LinkLabel lk  = (prjLink[i] = new LinkLabel());
						Label mod = (modifiedLabel[i] = new Label());
						
						lk.Location 	= new Point(titleLabel.Left, titleLabel.Top + 22 + i * 20);
						lk.AutoSize 	= true;
						lk.LinkBehavior = LinkBehavior.HoverUnderline;
						lk.LinkArea 	= new LinkArea(0, fileName.Length);
						lk.LinkColor 	= linkColor;
						
						lk.Text = ParseCombineFile(fileName);
						lk.Tag  = fileName;
						
						// used to retrieve last-write information
						System.IO.FileInfo fInfo = new System.IO.FileInfo(fileName);
						
						mod.Location	= new Point(modLabel.Left, lk.Top);
						mod.AutoSize	= true;
						mod.Text		= fInfo.LastWriteTime.ToShortDateString();
						mod.ForeColor	= titleLabel.ForeColor;
						
						// TODO: somehow the tooltips for the linklabels don't work...
						toolTip.SetToolTip(lk, fileName);
						
						// Add handler to recieve Click events
						lk.LinkClicked += new LinkLabelLinkClickedEventHandler(ProjectLinkLabelClicked);
						
						panel.Controls.Add(lk);
						panel.Controls.Add(mod);
						
						++i;
						if(i > 9) break;
					}
				}
			} catch {}
			
			try {
				// Add the controls to the panel
				panel.Controls.AddRange(new Control[] {versionLabel, topPic, gradPic1, gradPic2, startPic, bderPic1, bderPic2, capLabel, openBtn, newBtn, titleLabel, modLabel});
				panel.Controls.AddRange(prjLink);
				panel.Controls.AddRange(modifiedLabel);
				
				panel.BackColor		= Color.White;
				panel.ResumeLayout();
				
				// Description of the tab shown in #develop
				ContentName = "Start Page";
			} catch {
				return;
			}
		}
		
		/// <summary>
		/// Extracts a combine name from the specified file; return fileName on error
		/// </summary>
		private string ParseCombineFile(string fileName) 
		{
			ICSharpCode.SharpDevelop.Internal.Project.Combine cmb;
			
			try {
				cmb = new ICSharpCode.SharpDevelop.Internal.Project.Combine(fileName);
				return cmb.Name;
			} catch {
				return fileName;
			}
		}
		
		/// <summary>
		/// Handles Click events on Links
		/// </summary>
		public void ProjectLinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e) 
		{
			try {
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				projectService.OpenCombine((string)((LinkLabel)sender).Tag);
			} catch (Exception ex) {
				MessageBox.Show("Could not access project service or load project:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void OpenBtnClicked(object sender, EventArgs e) 
		{
			try {
				ICSharpCode.SharpDevelop.Commands.OpenCombine cmd = new ICSharpCode.SharpDevelop.Commands.OpenCombine();
				cmd.Run();
			} catch (Exception ex) {
				MessageBox.Show("Could not access command:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void NewBtnClicked(object sender, EventArgs e) 
		{
			try {
				ICSharpCode.SharpDevelop.Commands.CreateNewProject cmd = new ICSharpCode.SharpDevelop.Commands.CreateNewProject();
				cmd.Run();
			} catch (Exception ex) {
				MessageBox.Show("Could not access command:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
