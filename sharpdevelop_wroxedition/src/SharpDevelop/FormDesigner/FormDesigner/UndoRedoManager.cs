// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;


using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.SharpDevelop.FormDesigner.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;
using ICSharpCode.SharpDevelop.FormDesigner.Util;
using ICSharpCode.Core.AddIns.Codons;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	/*
	public class UndoableRemove : IUndoableOperation
	{
		IDesignerHost host;
		object        serializedComponent;
		ICollection   deserializedComponents;
		UndoRedoManager manager;
		
		public UndoableRemove(IDesignerHost host, UndoRedoManager manager, object serializedComponent)
		{
			this.host = host;
			this.serializedComponent = serializedComponent;
			this.manager = manager;
		}
		
		public void Undo()
		{
			manager.IgnoreEvents = true;
			IDesignerSerializationService designerSerializationService = (IDesignerSerializationService)host.GetService(typeof(IDesignerSerializationService));
			deserializedComponents = designerSerializationService.Deserialize(serializedComponent);
			manager.IgnoreEvents = false;
		}
		
		public void Redo()
		{
			manager.IgnoreEvents = true;
			foreach (IComponent component in deserializedComponents) {
				host.Container.Remove(component);
			}
			manager.IgnoreEvents = false;
		}
	}*/
	
	public class UndoableAdd : IUndoableOperation
	{
		IDesignerHost host;
		IComponent    component;
		object        serializedComponent;
		public UndoableAdd(IDesignerHost host, IComponent component)
		{
			this.host      = host;
			this.component = component;
		}
		
		public void Undo()
		{
			IDesignerSerializationService designerSerializationService = (IDesignerSerializationService)host.GetService(typeof(IDesignerSerializationService));
			serializedComponent = designerSerializationService.Serialize(new object[] { component });
			
			host.Container.Remove(component);
			host.DestroyComponent(component);
		}
		
		public void Redo()
		{
			IDesignerSerializationService designerSerializationService = (IDesignerSerializationService)host.GetService(typeof(IDesignerSerializationService));
			designerSerializationService.Deserialize(serializedComponent);
//			host.Container.Add(component);
		}
	}
	
	public class UndoRedoManager
	{
		IDesignerHost host;
		UndoStack     undoStack = new UndoStack();
		
		public bool IgnoreEvents = false;
		
		public UndoRedoManager(IDesignerHost host)
		{
			this.host = host;
			IComponentChangeService componentChangeService = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentAdding += new ComponentEventHandler(AddingComponent);
			componentChangeService.ComponentAdded  += new ComponentEventHandler(AddedComponent);
		}
		
		Stack addStack = new Stack();
		void AddingComponent(object sender, ComponentEventArgs e)
		{
			if (IgnoreEvents) {
				return;
			}
			if (e.Component != host.RootComponent) {
				addStack.Push(new UndoableAdd(host, e.Component));
			}
		}
		
		void AddedComponent(object sender, ComponentEventArgs e)
		{
			if (IgnoreEvents) {
				return;
			}
			
			if (e.Component != host.RootComponent) {
				undoStack.Push((IUndoableOperation)addStack.Pop());
			}
		}
		
		/*
		Stack removeStack = new Stack();
		void RemovingComponent(object sender, ComponentEventArgs e)
		{
			MessageBox.Show("Removing");
			if (IgnoreEvents) {
				return;
			}
			IDesignerSerializationService designerSerializationService = (IDesignerSerializationService)host.GetService(typeof(IDesignerSerializationService));
			removeStack.Push(new UndoableRemove(host, this, designerSerializationService.Serialize(new object[] { e.Component })));
		}
		
		void RemovedComponent(object sender, ComponentEventArgs e)
		{
			MessageBox.Show("Removed");
			if (IgnoreEvents) {
				return;
			}
			
			undoStack.Push((IUndoableOperation)removeStack.Pop());
		}
		*/
		
		public void Undo()
		{
			undoStack.Undo();
		}
		public void Redo()
		{
			undoStack.Redo();
		}
	}
}
