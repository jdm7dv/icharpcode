// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class EventBindingServiceImpl : IEventBindingService
	{
		public string CreateUniqueMethodName(IComponent component, EventDescriptor e)
		{
			return "new_name";
		}
		
		public ICollection GetCompatibleMethods(EventDescriptor e)
		{
			return null;
		}
		
		public EventDescriptor GetEvent(PropertyDescriptor property)
		{
			if (property is EventPropertyDescriptor) {
				return ((EventPropertyDescriptor)property).WrappedEventDescriptor;
			}
			
			return null;
		}
		
		public PropertyDescriptorCollection GetEventProperties(EventDescriptorCollection events)
		{
			PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			foreach (EventDescriptor e in events) {
				pdc.Add(new EventPropertyDescriptor(e));
			}
			return pdc;
		}
		
		public PropertyDescriptor GetEventProperty(EventDescriptor e)
		{
			return new EventPropertyDescriptor(e);
		}
		
		public bool ShowCode()
		{
			return false;
		}
		
		public bool ShowCode(int lineNumber)
		{
			return false;
		}
		
		public bool ShowCode(IComponent component, EventDescriptor e)
		{
			return false;
		}
	}
	
	internal class EventPropertyDescriptor : PropertyDescriptor
	{
		EventDescriptor ed = null;
		
		public EventPropertyDescriptor(EventDescriptor ed) : base(ed)
		{
			this.ed = ed;
		}
		
		public EventDescriptor WrappedEventDescriptor {
			get {
				return ed;
			}
		}
		
		public override Type ComponentType {
			get {
				return ed.ComponentType;
			}
		}

		public override bool IsReadOnly {
			get {
				return true;
			}
		}
		
		public override Type PropertyType {
			get {
				return typeof(string);
			}
		}
		
		public override bool CanResetValue(object component)
		{
			return false;
		}
		
		public override object GetValue(object component)
		{
			return null;
		}
		
		public override void ResetValue(object component)
		{
		}
		
		public override void SetValue(object component, object value)
		{
		}
		
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
