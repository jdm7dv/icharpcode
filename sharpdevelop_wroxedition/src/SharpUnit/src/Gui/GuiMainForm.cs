/*
 * MainForm.cs
 * Copyright (C) 2002 Mike Krueger, icsharpcode
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice, 
 *   this list of conditions and the following disclaimer. 
 * 
 * - Redistributions in binary form must reproduce the above copyright notice, 
 *   this list of conditions and the following disclaimer in the documentation 
 *   and/or other materials provided with the distribution. 
 * 
 * - Neither the name of icsharpcode nor the names of its contributors may 
 *   be used to endorse or promote products derived from this software without specific 
 *   prior written permission. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
 * SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
 * OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Threading;
using System.Resources;
using System.Windows.Forms;

using Crownwood.Magic.Controls;

using ICSharpCode.SharpUnit.Environment;

namespace ICSharpCode.SharpUnit.Gui {
	
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class GuiMainForm : Form
	{
		Panel panel1;
		Panel panel2;
		Button runTestsButton;
		TestEnvironmentProgressBar progressBar;
		Label testProgressLabel;
		Label testProcessLabel;
		Splitter splitter1;
		ExtendedRichTextBox reportRichTextBox;
		ExtendedRichTextBox overviewRichTextBox;
		
		TestView    testView;
		AssemblyView  assemblyView;
		
		Crownwood.Magic.Controls.TabControl viewTabControl   = new Crownwood.Magic.Controls.TabControl();
		Crownwood.Magic.Controls.TabControl reportTabControl = new Crownwood.Magic.Controls.TabControl();
		
		TestEventHandler       endEventHandler;
		TestEventHandler       succeedEventHandler;
		TestFailedEventHandler failedEventHandler;
		
		int testsExecuted  = 0;
		int testsFailed    = 0;
		int testsSucceeded = 0;
		
		long startTime;
		
		ITestRunner testRunner;
		
		public GuiMainForm(ITestRunner testRunner)
		{
			this.testRunner = testRunner;
			InitializeComponent();
			endEventHandler     = new TestEventHandler(TestEnded);
			succeedEventHandler = new TestEventHandler(TestSucceeded);
			failedEventHandler  = new TestFailedEventHandler(TestFailed);
			
			testRunner.TestEnded     += endEventHandler;
			testRunner.TestSucceeded += succeedEventHandler;
			testRunner.TestFailed    += failedEventHandler;
			testView.ShowLoadedTests(testRunner);
			assemblyView.ShowLoadedAssemblies(testRunner);
		}
		
		
		int CountTests(TreeNode node, int count)
		{
			if (node.Tag is ITest && node.Checked) {
				++count;
			}
			foreach (TreeNode subNode in node.Nodes) {
				count += CountTests(subNode, 0);
			}
			
			return count;
		}
		
		void RunTests(TreeNode node)
		{
			if (node.Tag is AssemblyLoader) {
				Console.WriteLine("reload assembly");
				((AssemblyLoader)node.Tag).ReloadAssembly();
			}
			if (node.Tag is ITestSuite) {
				ITestSuite suite = node.Tag as ITestSuite;
				try {
					suite.LoadTestClass();
					Console.WriteLine("suite loaded");
				} catch (Exception e) {
					reportRichTextBox.Append(new RichTextSegment("\tFailed to create test suite '" + suite.FullTypeName + "'" + e.InnerException.GetType().Name + (e.InnerException.Message != null ? " : " + e.InnerException.Message : "") + "\n", Color.Red));					
					node.ForeColor = Color.Red;
					foreach (TreeNode subNode in node.Nodes) {
						subNode.ForeColor = Color.Red;
					}
					return;
				}
				reportRichTextBox.Append(new RichTextSegment("Running tests in suite " + suite.FullTypeName + (suite.Description != null && suite.Description.Length > 0 ? " : " + suite.Description  : "") + "\n", new Font(reportRichTextBox.Font, FontStyle.Underline)));
			}
			if (node.Tag is ITest) {
				if (node.Checked) {
					int oldFailed = testsFailed;
					testRunner.RunTest((ITest)node.Tag);
					bool testFailed = oldFailed != testsFailed;
					node.ForeColor = testFailed ? Color.Red : Color.Green;
					if (testFailed) {
						node.EnsureVisible();
					}
				} else {
					node.ForeColor = Color.Black;
				}
			} else {
				node.ForeColor = node.Checked ? Color.Blue : Color.Black;
			}
			foreach (TreeNode subNode in node.Nodes) {
				RunTests(subNode);
			}
		}
		
		public void TestEnded(object sender, TestEventArgs e)
		{
			++testsExecuted;
			progressBar.Step();
			UpdateLabel();
		}
		
		public void TestSucceeded(object sender, TestEventArgs e)
		{
			++testsSucceeded;
			reportRichTextBox.Append(new RichTextSegment("\tPassed test '" + e.Test.Name + "'" + (e.Test.Description != null && e.Test.Description.Length > 0 ? " : " + e.Test.Description  : "") + "\n"));
			UpdateLabel();
		}
		
		public void TestFailed(object sender, TestFailedEventArgs e)
		{
			++testsFailed;
			if (!progressBar.Error) {
				progressBar.Error = true;
			} 
			reportRichTextBox.Append(new RichTextSegment("\tFailed test '" + e.Test.Name + "'" + (e.Test.Description != null && e.Test.Description.Length > 0 ? " : " + e.Test.Description  : "") + "\n", Color.Red));
			if (e.ExceptionThrown != null) {
				reportRichTextBox.Append(new RichTextSegment("\t\tGot exception " + e.ExceptionThrown.GetType().Name + (e.ExceptionThrown.Message != null ? " : " + e.ExceptionThrown.Message : "") + "\n", Color.Red));
				reportRichTextBox.Append(new RichTextSegment("\t\tStack Trace : \n", Color.Black));
				reportRichTextBox.Append(new RichTextSegment("\t\t" + e.ExceptionThrown.StackTrace.Replace("\n", "\n\t\t") + "\n", Color.Magenta));
			}
			UpdateLabel();
		}
		
		void StartTest()
		{
			try {
				runTestsButton.Enabled = testView.Enabled = false;
				RunTests(testView.RootNode);
				
				Invoke(new VoidDelegate(GenerateEndReport));
				Invoke(new VoidDelegate(GenerateOverview));
				try {
					testRunner.UnloadTestSuites();
				} catch (Exception e) {
					reportRichTextBox.Append(new RichTextSegment("\tGot exception while unloading " + e.GetType().Name + (e.Message != null ? " : " + e.Message : "") + "\n", Color.Red));
					reportRichTextBox.Append(new RichTextSegment("\tOriginal exception " + e.InnerException.GetType().Name + (e.InnerException.Message != null ? " : " + e.InnerException.Message : "") + "\n", Color.Red));
				}
			} finally {
				runTestsButton.Enabled = testView.Enabled = true;
			}
		}
		
		delegate void VoidDelegate();
		
		void RunTests(object sender, EventArgs e)
		{
			int testCount = CountTests(testView.RootNode, 0);
			progressBar.Value = 0;
			progressBar.Minimum = 0;
			progressBar.Maximum = testCount;
			progressBar.Error = false;
			
			testsExecuted = testsFailed = testsSucceeded = 0;
			UpdateLabel();
			
			overviewRichTextBox.Text = "";
			
			reportRichTextBox.Text = "";
			reportRichTextBox.Append(new RichTextSegment("Processing " + testCount + " tests\n", new Font(reportRichTextBox.Font, FontStyle.Bold)));
			
			Thread t = new Thread(new ThreadStart(new VoidDelegate(StartTest)));
			startTime = System.DateTime.Now.Ticks;
			t.Start();
		}
		
		void GenerateEndReport()
		{
			if (testsExecuted == 0) {
				reportRichTextBox.Append(new RichTextSegment("\nNo tests run\n", new Font(reportRichTextBox.Font, FontStyle.Bold), Color.Black));
			} else if (testsFailed == 0) {
				reportRichTextBox.Append(new RichTextSegment("\nAll tests passed\n", new Font(reportRichTextBox.Font, FontStyle.Bold), Color.Blue));
			} else if (testsSucceeded ==0) {
				reportRichTextBox.Append(new RichTextSegment("\nAll tests failed\n", new Font(reportRichTextBox.Font, FontStyle.Bold), Color.Red));
			} else {
				reportRichTextBox.Append(new RichTextSegment("\nSome tests failed\n", new Font(reportRichTextBox.Font, FontStyle.Bold), Color.Red));
			}
		}
		
		void GenerateOverview()
		{
			if (testsExecuted == 0) {
				return;
			}
			overviewRichTextBox.Append(new RichTextSegment("Project Overview\n\n", new Font(overviewRichTextBox.Font.FontFamily, 14, FontStyle.Underline), Color.Black));
			overviewRichTextBox.Append(new RichTextSegment("Tests\n", new Font(overviewRichTextBox.Font, FontStyle.Underline), Color.Black));
			overviewRichTextBox.Append(new RichTextSegment("Executed:\t" + testsExecuted + "\n"));
			overviewRichTextBox.Append(new RichTextSegment("Succeeded:\t" + testsSucceeded + " (" + (testsSucceeded * 100) / testsExecuted + "%)\n"));
			overviewRichTextBox.Append(new RichTextSegment("Failed:\t\t" + testsFailed + " (" + (testsFailed * 100) / testsExecuted + "%)\n"));
			long runTime = (System.DateTime.Now.Ticks - startTime) / 1000000;
			overviewRichTextBox.Append(new RichTextSegment("\nTests running time : " + (float)runTime / 10f + "sec\n"));
		}

		void UpdateLabel()
		{
			testProcessLabel.Text = "Executed : " + testsExecuted + " Passed : " + testsSucceeded + " Failed : " + testsFailed;
		}
		
		
		void AboutEvent(object sender, EventArgs e)
		{
			using (CommonAboutDialog cad = new CommonAboutDialog()) {
				cad.Owner = this;
				cad.ShowDialog();
			}
		}
		void ExitFormEvent(object sender, EventArgs e)
		{
			Close();
		}
		
		void AddAssemblyEvent(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Assemblies|*.dll;*.exe|All files (*.*)|*.*" ;
			openFileDialog.Multiselect     = true;
			openFileDialog.CheckFileExists = true;
			
			if (openFileDialog.ShowDialog() == DialogResult.OK) {
				foreach (string name in openFileDialog.FileNames) {
					testRunner.AddAssembly(name);
				}
//				testRunner.ReloadTestSuites();
				testView.ShowLoadedTests(testRunner);
				assemblyView.ShowLoadedAssemblies(testRunner);
				testRunner.UnloadTestSuites();
			}
		}
		
		void InitializeComponent()
		{
			this.panel1 = new Panel();
			this.panel2 = new Panel();
			this.runTestsButton = new Button();
			this.progressBar = new TestEnvironmentProgressBar();
			this.testProgressLabel = new Label();
			this.testProcessLabel = new Label();
			this.splitter1 = new Splitter();
			this.reportRichTextBox = new ExtendedRichTextBox();
			this.overviewRichTextBox = new ExtendedRichTextBox();
			this.SuspendLayout();
			
			Crownwood.Magic.Menus.MenuControl topMenu = new Crownwood.Magic.Menus.MenuControl();
			topMenu.Dock = DockStyle.Top;
			Crownwood.Magic.Menus.MenuCommand top1 = new Crownwood.Magic.Menus.MenuCommand("&File");
			
			top1.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[]{
				new Crownwood.Magic.Menus.MenuCommand("Add Assembly", new EventHandler(AddAssemblyEvent)),
				new Crownwood.Magic.Menus.MenuCommand("-"),
				new Crownwood.Magic.Menus.MenuCommand("E&xit", new EventHandler(ExitFormEvent)),
			});
			
			Crownwood.Magic.Menus.MenuCommand top2 = new Crownwood.Magic.Menus.MenuCommand("&Help");
			
			top2.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[]{
				new Crownwood.Magic.Menus.MenuCommand("&Help"),
				new Crownwood.Magic.Menus.MenuCommand("-"),
				new Crownwood.Magic.Menus.MenuCommand("&About", new EventHandler(AboutEvent))
			});
			
			topMenu.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { top1, top2 });
			
			//
			// panel1
			// 
			this.panel1.Controls.AddRange(new Control[] {this.reportTabControl, this.splitter1, this.viewTabControl, topMenu});
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(632, 336);
			this.panel1.TabIndex = 0;
			
			//
			// panel2
			// 
			this.panel2.Controls.AddRange(new Control[] {this.testProcessLabel, this.testProgressLabel, this.progressBar, this.runTestsButton});
			this.panel2.Dock = DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 336);
			this.panel2.Size = new System.Drawing.Size(632, 74);
			this.panel2.TabIndex = 1;
			
			//
			// runTestsButton
			// 
			this.runTestsButton.Location = new System.Drawing.Point(8, 8);
			this.runTestsButton.Name = "runTestsButton";
			this.runTestsButton.Size = new System.Drawing.Size(88, 24);
			this.runTestsButton.TabIndex = 0;
			this.runTestsButton.Text = "Run Tests";
			this.runTestsButton.FlatStyle = FlatStyle.Flat;
			runTestsButton.Click += new EventHandler(RunTests);
			
			// 
			// progressBar
			// 
			this.progressBar.ForeColor = Color.Red;
			this.progressBar.BackColor = Color.Blue;
			this.progressBar.Value = 0;
			this.progressBar.Location = new System.Drawing.Point(112, 24);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(512, 16);
			this.progressBar.Anchor = AnchorStyles.Right | AnchorStyles.Left;
			this.progressBar.TabIndex = 1;
			
			//
			// testProgressLabel
			// 
			this.testProgressLabel.Location = new System.Drawing.Point(112, 8);
			this.testProgressLabel.Name = "testProgressLabel";
			this.testProgressLabel.Size = new System.Drawing.Size(512, 16);
			this.testProgressLabel.TabIndex = 2;
			this.testProgressLabel.Text = "Test progress";
			
			//
			// testProcessLabel
			// 
			this.testProcessLabel.Location = new System.Drawing.Point(112, 48);
			this.testProcessLabel.Name = "testProcessLabel";
			this.testProcessLabel.Size = new System.Drawing.Size(512, 16);
			this.testProcessLabel.TabIndex = 3;
			UpdateLabel();
			
			//
			// testView
			//
			testView = new TestView();
			testView.Dock = DockStyle.Fill;
			
			//
			// assemblyView
			//
			assemblyView = new AssemblyView();
			assemblyView.Dock = DockStyle.Fill;
			
			//
			// viewTabControl
			// 
			ResourceManager iconResources = new ResourceManager("IconResources", Assembly.GetCallingAssembly());
			ImageList imgList = new ImageList();
			imgList.Images.Add((Bitmap)iconResources.GetObject("ClassViewIcon"));
			imgList.Images.Add((Bitmap)iconResources.GetObject("ProjectViewIcon"));
				
			Crownwood.Magic.Controls.TabPage tabPage1 = new Crownwood.Magic.Controls.TabPage("Class View");
			tabPage1.Controls.Add(testView);
			tabPage1.ImageIndex = 0;
			this.viewTabControl.TabPages.Add(tabPage1);
			
			Crownwood.Magic.Controls.TabPage tabPage2 = new Crownwood.Magic.Controls.TabPage("Project View");
			tabPage2.Controls.Add(assemblyView);
			tabPage2.ImageIndex = 1;
			
			this.viewTabControl.TabPages.Add(tabPage2);
			this.viewTabControl.Dock = DockStyle.Left;
			this.viewTabControl.SelectedIndex = 0;
			this.viewTabControl.Style = Crownwood.Magic.Common.VisualStyle.IDE;
			this.viewTabControl.Size  = new System.Drawing.Size(220, 336);
			this.viewTabControl.TabIndex = 0;
			this.viewTabControl.ImageList = imgList;
			this.viewTabControl.PositionTop = true;
			
			//
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(192, 0);
			this.splitter1.Size = new System.Drawing.Size(3, 336);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			
			
			//
			// reportTabControl
			// 
			Crownwood.Magic.Controls.TabPage reportTabPage1 = new Crownwood.Magic.Controls.TabPage("Report");
			reportTabPage1.Controls.Add(reportRichTextBox);
			reportTabPage1.ImageIndex = 0;
			reportTabControl.TabPages.Add(reportTabPage1);
			
			Crownwood.Magic.Controls.TabPage reportTabPage2 = new Crownwood.Magic.Controls.TabPage("Overview");
			reportTabPage2.Controls.Add(overviewRichTextBox);
			reportTabPage2.ImageIndex = 1;
			reportTabControl.TabPages.Add(reportTabPage2);
			
			reportTabControl.Dock = DockStyle.Fill;
			reportTabControl.SelectedIndex = 0;
			reportTabControl.Style = Crownwood.Magic.Common.VisualStyle.IDE;
			reportTabControl.Size  = new System.Drawing.Size(220, 336);
			reportTabControl.TabIndex = 0;
			reportTabControl.ImageList = imgList;
//			this.reportTabControl.PositionTop = true;
			
			//
			// reportRichTextBox
			// 
			this.reportRichTextBox.Dock = DockStyle.Fill;
			this.reportRichTextBox.Location = new System.Drawing.Point(195, 0);
			this.reportRichTextBox.Name = "reportRichTextBox";
			this.reportRichTextBox.ReadOnly = true;
			this.reportRichTextBox.Size = new System.Drawing.Size(437, 336);
			this.reportRichTextBox.TabIndex = 2;
			this.reportRichTextBox.Text = "";
			this.reportRichTextBox.BorderStyle = BorderStyle.FixedSingle;
			this.reportRichTextBox.WordWrap = false;
			
			//
			// overviewRichTextBox
			// 
			this.overviewRichTextBox.Dock = DockStyle.Fill;
			this.overviewRichTextBox.Location = new System.Drawing.Point(195, 0);
			this.overviewRichTextBox.Name = "reportRichTextBox";
			this.overviewRichTextBox.ReadOnly = true;
			this.overviewRichTextBox.Size = new System.Drawing.Size(437, 336);
			this.overviewRichTextBox.TabIndex = 2;
			this.overviewRichTextBox.Text = "";
			this.overviewRichTextBox.BorderStyle = BorderStyle.FixedSingle;
			this.overviewRichTextBox.Font = new Font("Arial", 10);
			//
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(632, 410);
			this.Controls.AddRange(new Control[] { panel1, panel2 });
			this.Text = "#Unit GUI";
			this.Icon = Icon.FromHandle(((Bitmap)iconResources.GetObject("ProgramIcon")).GetHicon());
//			Icon.Save(File.Create("C:\\SharpUnitIcon.ico"));
			this.ResumeLayout(false);
		}
	}
}
