/*
 * TestView.cs
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
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.SharpUnit.Environment;

namespace ICSharpCode.SharpUnit.Gui {
	
	public class TestView : TreeView
	{
		TreeNode  rootNode = new TreeNode();
		
		public TreeNode RootNode {
			get {
				return rootNode;
			}
		}
		
		public TestView()
		{
			rootNode.Text    = "Test Suites";
			rootNode.Checked = true;
			
			Nodes.Add(rootNode);
			CheckBoxes = true;
		}
		
		public void ShowLoadedTests(ITestRunner testRunner)
		{
			rootNode.Nodes.Clear();
			
			foreach (AssemblyLoader loader in testRunner.AssemblyLoader) {
				TreeNode assemblyNode = new TreeNode();
				
				assemblyNode.Text    = Path.GetFileName(loader.AssemblyLocation);
				assemblyNode.Tag     = loader;
				assemblyNode.Checked = true;
				
				foreach (ITestSuite suite in loader.TestSuites) {
					
					TreeNode  testNode = new TreeNode();
					testNode.Text = suite.Type.Name;
					testNode.Tag  = suite;
					testNode.Checked = true;
					
					foreach (ITest test in suite.Tests) {
						TreeNode testMethodNode = new TreeNode();
						testMethodNode.Text = test.Name;
						testMethodNode.Tag  = test;
						testMethodNode.Checked = true;
						
						testNode.Nodes.Add(testMethodNode);
					}
					assemblyNode.Nodes.Add(testNode);
				}
				
				rootNode.Nodes.Add(assemblyNode);
			}
			rootNode.Expand();
		}
		
		bool checkOff = false;
		
		protected override void OnAfterCheck(TreeViewEventArgs e)
		{
			base.OnAfterCheck(e);
			if (!checkOff) {
				SetSubNodes(e.Node, e.Node.Checked);
				TreeNode parent = e.Node.Parent;
				checkOff = true;
				if (e.Node.Checked) {
					while (parent != null) {
						parent.Checked = true;
						parent = parent.Parent;
					}
				} else {
					while (parent != null) {
						CheckParentCondition(parent);
						parent = parent.Parent;
					}
				}
				checkOff = false;
			}
		}
		
		void CheckParentCondition(TreeNode parent)
		{
			bool atLeastOneNodeChecked = false;
			
			foreach (TreeNode node in parent.Nodes) {
				atLeastOneNodeChecked |= node.Checked;
			}
			
			if (!atLeastOneNodeChecked) {
				parent.Checked = false;
			}
		}
		
		void SetSubNodes(TreeNode node, bool isChecked)
		{
			foreach (TreeNode subNode in node.Nodes) {
				subNode.Checked = isChecked;
				SetSubNodes(subNode, isChecked);
			}
		}
	}
}
