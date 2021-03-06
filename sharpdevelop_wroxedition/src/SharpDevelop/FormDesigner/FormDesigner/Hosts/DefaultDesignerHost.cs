// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.SharpDevelop.FormDesigner.Services;

using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.FormDesigner.Hosts 
{
	public class DefaultDesignerHost : DefaultServiceContainer, IDesignerLoaderHost
	{
		DesignComponentContainer container    = null;
		Stack                    transactions = new Stack();
		DesignerLoader           myLoader     = null;
		
		public DefaultDesignerHost()
		{
			myLoader  = new DefaultDesignerLoader();
			container = new DesignComponentContainer(this);
			Reload();
		}
		
#region Designer loading 
		public bool Loading {
			get {
				return myLoader.Loading;
			}
		}
		
		public void Activate()
		{
			OnActivated(EventArgs.Empty);
		}
		
		public void Deactivate()
		{
			OnDeactivated(EventArgs.Empty);
		}
		
		public void EndLoad(string baseClassName, bool successful, ICollection errorCollection)
		{
			OnLoadComplete(EventArgs.Empty);
		}

		public void Reload()
		{
			myLoader.BeginLoad(this);
			OnLoadComplete(EventArgs.Empty);
		}
		
		protected virtual void OnActivated(EventArgs e)
		{
			if (Activated != null) {
				Activated(this, e);
			}
		}
		
		protected virtual void OnDeactivated(EventArgs e)
		{
			if (Deactivated != null) {
				Deactivated(this, e);
			}
		}
		
		protected virtual void OnLoadComplete(EventArgs e)
		{
			if (LoadComplete != null) {
				LoadComplete(this, e);
			}
		}
		
		public event EventHandler Activated;
		public event EventHandler Deactivated;
		public event EventHandler LoadComplete;
#endregion		

#region Designer component management
		public IContainer Container {
			get {
				return container;
			}
		}
		
		public IComponent RootComponent {
			get {
				return container.RootComponent;
			}
		}

		public string RootComponentClassName {
			get {
				return RootComponent.GetType().Name;
			}
		}
		
		public IComponent CreateComponent(Type componentClass)
		{
			INameCreationService nameCreationService = (INameCreationService)GetService(typeof(INameCreationService));
			return this.CreateComponent(componentClass, nameCreationService.CreateName(container, componentClass));
		}
		
		public IComponent CreateComponent(Type componentClass, string name)
		{
			IComponent component = Activator.CreateInstance(componentClass) as IComponent;
			if (component == null) {
				throw new ArgumentException("The specified Type id not an IComponent", "componentClass");
			}
			
			INameCreationService nameCreationService = (INameCreationService)GetService(typeof(INameCreationService));
			
			if (!nameCreationService.IsValidName(name)) {
				name = nameCreationService.CreateName(container, componentClass);
			}
			container.Add(component, name);
			
			return component;
		}
		
		public void DestroyComponent(IComponent component)
		{
			component.Dispose();
		}
		
		public IDesigner GetDesigner(IComponent component)
		{
			if (component == null) {
				return null;
			}
			return (IDesigner)container.Designers[component];
		}
		
		public Type GetType(string typeName)
		{
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				Type type = asm.GetType(typeName);
				if (type != null) {
					return type;
				}
			}
			return Type.GetType(typeName);
		}
#endregion		

		
#region Designer transaction management
		public bool InTransaction {
			get {
				return transactions.Count > 0;
			}
		}
		
		public string TransactionDescription {
			get {
				if (InTransaction) {
					DesignerTransaction trans = (DesignerTransaction)transactions.Peek();
					return trans.Description;
				}
				return null;
			}
		}
		
		public DesignerTransaction CreateTransaction()
		{
			return this.CreateTransaction(null);
		}
		
		public DesignerTransaction CreateTransaction(string description)
		{
			System.Console.WriteLine("DefaultDesignerHost:Transaction being created: " + description);
			
			OnTransactionOpening(EventArgs.Empty);
			
			DesignerTransaction transaction = null;
			
			if (description == null) {
				transaction = new DefaultDesignerTransaction(this);
			} else {
				transaction = new DefaultDesignerTransaction(this, description);
			}
			
			transactions.Push(transaction);
			
			OnTransactionOpened(EventArgs.Empty);
			
			return transaction;
		}
		
		internal void FireTransactionClosing(bool commit)
		{
			OnTransactionClosing(new DesignerTransactionCloseEventArgs(commit));
		}

		internal void FireTransactionClosed(bool commit)
		{
			OnTransactionClosed(new DesignerTransactionCloseEventArgs(commit));
			transactions.Pop();
		}
		
		protected virtual void OnTransactionOpened(EventArgs e)
		{
			if (TransactionOpened != null) {
				TransactionOpened(this, e);
			}
		}

		protected virtual void OnTransactionOpening(EventArgs e)
		{
			if (TransactionOpening != null) {
				TransactionOpening(this, e);
			}
		}
		
		protected virtual void OnTransactionClosing(DesignerTransactionCloseEventArgs e)
		{
			if (TransactionClosing != null) {
				TransactionClosing(this, e);
			}
		}
		
		protected virtual void OnTransactionClosed(DesignerTransactionCloseEventArgs e)
		{
			if (TransactionClosed != null) {
				TransactionClosed(this, e);
			}
		}
		
		public event EventHandler TransactionOpened;
		public event EventHandler TransactionOpening;
		
		public event DesignerTransactionCloseEventHandler TransactionClosed;
		public event DesignerTransactionCloseEventHandler TransactionClosing;
#endregion
	}
}
