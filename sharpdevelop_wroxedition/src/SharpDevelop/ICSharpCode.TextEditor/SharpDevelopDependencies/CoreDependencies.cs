// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.Drawing.Printing;

#if BuildAsStandalone
namespace ICSharpCode.Core.Services
{
	
}
namespace ICSharpCode.Core.Properties
{
	
	/// <summary>
	/// The <code>IProperties</code> interface defines a set of properties
	/// </summary>
	public interface IProperties : IXmlConvertable
	{
		/// <summary>
		/// Gets a property out of the collection. The defaultvalue must either have
		/// a cast to a string (and back) or implement the <code>IXmlConcertable</code>
		/// interface.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		object GetProperty(string key, object defaultvalue);
		
		/// <summary>
		/// Gets a property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>null</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		object GetProperty(string key);
		
		/// <summary>
		/// Gets a <code>int</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		int GetProperty(string key, int defaultvalue);
		
		/// <summary>
		/// Gets a <code>bool</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		bool GetProperty(string key, bool defaultvalue);

		/// <summary>
		/// Gets a <code>short</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		short GetProperty(string key, short defaultvalue);

		/// <summary>
		/// Gets a <code>byte</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		byte GetProperty(string key, byte defaultvalue);

		/// <summary>
		/// Gets a <code>string</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		string GetProperty(string key, string defaultvalue);
		
		/// <summary>
		/// Gets a <code>enum</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		System.Enum GetProperty(string key, System.Enum defaultvalue);
		
		/// <summary>
		/// Sets the property <code>key</code> to the value <code>val</code>.
		/// If <code>val</code> is null, the property will be taken out from the
		/// properties.
		/// </summary>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="val">
		/// The value of the property.
		/// </param>
		void SetProperty(string key, object val);
		
		/// <summary>
		/// Returns a new instance of <code>IProperties</code> which has equal properties.
		/// </summary>
		IProperties Clone();
		
  		/// <summary>
  		/// The property changed event handler, it is called
		/// when a property has changed.
		/// </summary>
		event PropertyEventHandler PropertyChanged;
	}
	
	
	/// <summary>
	/// Default <code>IProperties</code> implementation, should
	/// be enough for most cases :)
	/// </summary>
	public class DefaultProperties : IProperties
	{
		Hashtable properties = new Hashtable();
		
		/// <summary>
		/// Gets a property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public object GetProperty(string key, object defaultvalue)
		{
			if (!properties.ContainsKey(key)) {
				if (defaultvalue != null) {
					properties[key] = defaultvalue;
				}
				return defaultvalue;
			}
			
			object obj = properties[key];
			
			// stored an XmlElement in properties node ->
			// set a FromXmlElement of the defaultvalue type at this propertyposition.
			if (defaultvalue is IXmlConvertable && obj is XmlElement) {
				obj = properties[key] = ((IXmlConvertable)defaultvalue).FromXmlElement((XmlElement)((XmlElement)obj).FirstChild);
			}
			return obj;
		}
		
		/// <summary>
		/// Gets a property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>null</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		public object GetProperty(string key)
		{
			return GetProperty(key, (object)null);
		}
		
		/// <summary>
		/// Gets a <code>int</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public int GetProperty(string key, int defaultvalue)
		{
			return int.Parse(GetProperty(key, (object)defaultvalue).ToString());
		}
		
		/// <summary>
		/// Gets a <code>bool</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public bool GetProperty(string key, bool defaultvalue)
		{
			return bool.Parse(GetProperty(key, (object)defaultvalue).ToString());
		}

		/// <summary>
		/// Gets a <code>short</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public short GetProperty(string key, short defaultvalue)
		{
			return short.Parse(GetProperty(key, (object)defaultvalue).ToString());
		}

		/// <summary>
		/// Gets a <code>byte</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public byte GetProperty(string key, byte defaultvalue)
		{
			return byte.Parse(GetProperty(key, (object)defaultvalue).ToString());
		}
		
		/// <summary>
		/// Gets a <code>string</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public string GetProperty(string key, string defaultvalue)
		{
			return GetProperty(key, (object)defaultvalue).ToString();
		}
		
		/// <summary>
		/// Gets a <code>enum</code> property out of the collection.
		/// </summary>
		/// <returns>
		/// The property, or <code>defaultvalue</code>, if the property wasn't found.
		/// </returns>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="defaultvalue">
		/// The default value of the property.
		/// </param>
		public System.Enum GetProperty(string key, System.Enum defaultvalue)
		{
			return (System.Enum)Enum.Parse(defaultvalue.GetType(), GetProperty(key, (object)defaultvalue).ToString());
		}
		
		/// <summary>
		/// Sets the property <code>key</code> to the value <code>val</code>.
		/// If <code>val</code> is null, the property will be taken out from the
		/// properties.
		/// </summary>
		/// <param name="key">
		/// The name of the property.
		/// </param>
		/// <param name="val">
		/// The value of the property.
		/// </param>
		public void SetProperty(string key, object val)
		{
			object oldValue = properties[key];
			properties[key] = val;
			OnPropertyChanged(new PropertyEventArgs(this, key, oldValue, val));
		}
		
		public DefaultProperties()
		{
		}
		
		protected DefaultProperties(XmlElement element)
		{
			XmlNodeList nodes = element.ChildNodes;
			foreach (XmlElement el in nodes) {
				if (el.Name == "Property") {
					properties[el.Attributes["key"].InnerText] = el.Attributes["value"].InnerText;
				} else if (el.Name == "XmlConvertableProperty") {
					properties[el.Attributes["key"].InnerText] = el;
				} else {
					throw new Exception(el.Name);
				}
			}
		}
		
		/// <summary>
		/// Converts a <code>XmlElement</code> to an <code>DefaultProperties</code>
		/// </summary>
		/// <returns>
		/// A new <code>DefaultProperties</code> object 
		/// </returns>
		public virtual object FromXmlElement(XmlElement element)
		{
			return new DefaultProperties(element);
		}
		
		/// <summary>
		/// Converts the <code>DefaultProperties</code> object to a <code>XmlElement</code>
		/// </summary>
		/// <returns>
		/// A new <code>XmlElement</code> object which represents the state
		/// of the <code>DefaultProperties</code> object.
		/// </returns>
		public virtual XmlElement ToXmlElement(XmlDocument doc)
		{
			XmlElement propertiesnode  = doc.CreateElement("Properties");
			
			foreach (DictionaryEntry entry in properties) {
				if (entry.Value != null) {
					if (entry.Value is XmlElement) { // write unchanged XmlElement back
						propertiesnode.AppendChild(doc.ImportNode((XmlElement)entry.Value, true));
					} else if (entry.Value is IXmlConvertable) { // An Xml convertable object
						XmlElement convertableNode = doc.CreateElement("XmlConvertableProperty");
						
						XmlAttribute key = doc.CreateAttribute("key");
						key.InnerText = entry.Key.ToString();
						convertableNode.Attributes.Append(key);
						
						convertableNode.AppendChild(((IXmlConvertable)entry.Value).ToXmlElement(doc));
						
						propertiesnode.AppendChild(convertableNode);
					} else {
						XmlElement el = doc.CreateElement("Property");
						
						XmlAttribute key   = doc.CreateAttribute("key");
						key.InnerText      = entry.Key.ToString();
						el.Attributes.Append(key);
	
						XmlAttribute val   = doc.CreateAttribute("value");
						val.InnerText      = entry.Value.ToString();
						el.Attributes.Append(val);
						
						propertiesnode.AppendChild(el);
					}
				}
			}
			return propertiesnode;
		}
		
		/// <summary>
		/// Returns a new instance of <code>IProperties</code> which has equal properties.
		/// </summary>
		public IProperties Clone()
		{
			DefaultProperties df = new DefaultProperties();
			df.properties = (Hashtable)properties.Clone();
			return df;
		}
		
		protected virtual void OnPropertyChanged(PropertyEventArgs e)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, e);
			}
		}
		
		public event PropertyEventHandler PropertyChanged;
	}
	/// <summary>
	/// If you want define own, complex options you can implement this interface
	/// and save it in the main Option class, your class will be saved as xml in
	/// the global properties.
	/// Use your class like any other property. (the conversion will be transparent)
	/// </summary>
	public interface IXmlConvertable
	{
		/// <summary>
		/// Converts a <code>XmlElement</code> to an <code>IXmlConvertable</code>
		/// </summary>
		/// <returns>
		/// A new <code>IXmlConvertable</code> object 
		/// </returns>
		object FromXmlElement(XmlElement element);
		
		/// <summary>
		/// Converts the <code>IXmlConvertable</code> object to a <code>XmlElement</code>
		/// </summary>
		/// <returns>
		/// A new <code>XmlElement</code> object which represents the state
		/// of the <code>IXmlConvertable</code> object.
		/// </returns>
		XmlElement ToXmlElement(XmlDocument doc);
	}	
		
	public delegate void PropertyEventHandler(object sender, PropertyEventArgs e);
	
	public class PropertyEventArgs : EventArgs
	{
		IProperties properties;
		string      key;
		object      newValue;
		object      oldValue;
		
		/// <returns>
		/// returns the changed property object
		/// </returns>
		public IProperties Properties {
			get {
				return properties;
			}
		}
		
		/// <returns>
		/// The key of the changed property
		/// </returns>
		public string Key {
			get {
				return key;
			}
		}
		
		/// <returns>
		/// The new value of the property
		/// </returns>
		public object NewValue {
			get {
				return newValue;
			}
		}

		/// <returns>
		/// The new value of the property
		/// </returns>
		public object OldValue {
			get {
				return oldValue;
			}
		}
		
		public PropertyEventArgs(IProperties properties, string key, object oldValue, object newValue)
		{
			this.properties = properties;
			this.key        = key;
			this.oldValue   = oldValue;
			this.newValue   = newValue;
		}
	}
}

namespace ICSharpCode
{
	using System.Collections.Specialized;
	
	class propertyService
	{
		static public bool GetProperty(string property, bool aBool) 
		{
			return aBool;
		}

		static public object GetProperty(string property, object aObject) 
		{
			return aObject;
		}
	}	
}

namespace ICSharpCode
{
	using ICSharpCode.Core.Properties;
	
	/// <summary>
	/// This interface flags an object beeing "mementocapable". This means that the
	/// state of the object could be saved to an <see cref="IXmlConvertable"/> object
	/// and set from a object from the same class.
	/// This is used to save and restore the state of GUI objects.
	/// </summary>
	public interface IMementoCapable
	{
		/// <summary>
		/// Creates a new memento from the state.
		/// </summary>
		IXmlConvertable CreateMemento();
		
		/// <summary>
		/// Sets the state to the given memento.
		/// </summary>
		void SetMemento(IXmlConvertable memento);
	}
	
	/// <summary>
	/// If a IViewContent object is from the type IPrintable it signals
	/// that it's contents could be printed to a printer, fax etc.
	/// </summary>
	public interface IPrintable
	{
		/// <summary>
		/// Returns the PrintDocument for this object, see the .NET reference
		/// for more information about printing.
		/// </summary>
		PrintDocument PrintDocument {
			get;
		}
	}
	public interface IPositionable
	{
		/// <summary>
		/// Sets the 'caret' to the position pos, where pos.Y is the line (starting from 0).
		/// And pos.X is the column (starting from 0 too).
		/// </summary>
		void JumpTo(Point pos);
	}	
}
#endif
