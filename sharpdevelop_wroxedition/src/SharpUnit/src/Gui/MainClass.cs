/*
 * Main.cs
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
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.SharpUnit.Environment;

namespace ICSharpCode.SharpUnit.Gui {
	
	public class MainClass
	{
		static void PrintUsage()
		{
			Console.WriteLine("usage :");
			Console.WriteLine("        sharptest [ASSEMBLYNAME] [/D:DEPENDENDASSEMBLY]");
		}
		
		public static void Main(string[] args)
		{		
			ITestRunner runner = new DefaultTestRunner();
			foreach (string arg in args) {
				string arg2 = arg.ToUpper();
				if (arg2.IndexOf('?') > 0 || arg2 == "/H" || arg2 == "HELP") {
					PrintUsage();
					return;
				}
				if (File.Exists(arg)) {
					runner.AddAssembly(arg);
				}
			}
			
//			if (args != null) {
//				ProjectManager.Project.ParseCommandline(args);
//			}
			Application.Run(new GuiMainForm(runner));
		}
	}
}
