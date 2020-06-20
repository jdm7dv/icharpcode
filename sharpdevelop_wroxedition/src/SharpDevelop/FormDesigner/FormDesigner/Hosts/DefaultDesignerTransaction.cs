// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel.Design;

namespace ICSharpCode.SharpDevelop.FormDesigner.Hosts
{
	public class DefaultDesignerTransaction : DesignerTransaction
	{
		DefaultDesignerHost host = null;
		
		public DefaultDesignerTransaction(DefaultDesignerHost host) : base()
		{
			this.host = host;
		}
		
		public DefaultDesignerTransaction(DefaultDesignerHost host, string desc) : base(desc)
		{
			this.host = host;
		}
		
		protected override void OnCancel()
		{
			host.FireTransactionClosing(false);
			host.FireTransactionClosed(false);
		}
	
		protected override void OnCommit()
		{
			host.FireTransactionClosing(true);
			host.FireTransactionClosed(true);
		}
	}
}
