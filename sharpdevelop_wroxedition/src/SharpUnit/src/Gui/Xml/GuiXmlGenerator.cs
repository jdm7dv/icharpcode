/*
 * GuiXmlGenerator.cs
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
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;


namespace ICSharpCode.GUI.Xml {
	
	/// <summary>
	/// This class is able to generate a GUI definition out of a XML file.
	/// </summary>
	public class GuiXmlGenerator
	{
		ControlDictionary controlDictionary = new ControlDictionary();
		object            customizationObject;
		
		// required for the accept/cancel button workaround
		Form mainForm = null;
		string acceptButtonName = String.Empty;
		string cancelButtonName = String.Empty;
		
		/// <summary>
		/// Gets the ControlDictionary for this generator.
		/// </summary>
		public ControlDictionary ControlDictionary {
			get {
				return controlDictionary;
			}
		}
		
		/// <summary>
		/// Creates a new instance of GuiXmlGenerator.
		/// </summary>
		/// <param name="customizationObject">
		/// The object to customize. (should be a control or form)
		/// This is object, because this class may be extended later.
		/// </param>
		/// <param name="fileName">
		/// The filename of the XML definition file to load.
		/// </param>
		public GuiXmlGenerator(object customizationObject, string fileName)
		{
			this.customizationObject = customizationObject;
			LoadDefinition(fileName);
			
			// little hack to set the Accept & Cancel Button
			if (mainForm != null) {
				if (acceptButtonName != null && acceptButtonName.Length > 0) {
					mainForm.AcceptButton = (Button)controlDictionary[acceptButtonName];
				}
				if (cancelButtonName != null && cancelButtonName.Length > 0) {
					mainForm.CancelButton = (Button)controlDictionary[cancelButtonName];
				}
			}
		}
		
		/// <summary>
		/// Loads the XML definition from fileName and sets up the control.
		/// </summary>
		void LoadDefinition(string fileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			SetUpObject(customizationObject, doc.DocumentElement);
		}
		
		/// <summary>
		/// Sets the properties of an object <code>currentObject</code> to the
		/// contents of the xml element <code>element</code>
		/// </summary>
		void SetUpObject(object currentObject, XmlElement element)
		{
			foreach (XmlNode subNode in element.ChildNodes) {
				if (subNode is XmlElement){
					XmlElement subElement = (XmlElement)subNode;
					SetAttributes(currentObject, subElement);
				}
			}
		}
		
		/// <summary>
		/// Sets a property called propertyName in object <code>o</code> to <code>val</code>. This method performs
		/// all neccessary casts.
		/// </summary>
		void SetValue(object o, string propertyName, string val)
		{
			PropertyInfo propertyInfo = o.GetType().GetProperty(propertyName);
			if (propertyInfo.PropertyType.IsEnum) {
				propertyInfo.SetValue(o, Enum.Parse(propertyInfo.PropertyType, val), null);
			} else if (propertyInfo.PropertyType == typeof(Color)) {
				propertyInfo.SetValue(o, Color.FromName(val), null);
			} else if (propertyInfo.PropertyType == typeof(Image)) {
				ResourceManager iconResources = new ResourceManager("IconResources", Assembly.GetCallingAssembly());
				propertyInfo.SetValue(o, (Image)iconResources.GetObject(val), null);
			} else if (propertyInfo.PropertyType == typeof(Font)) {
				string[] font = val.Split(new char[]{','});
				propertyInfo.SetValue(o, new Font(font[0], Int32.Parse(font[1])), null);
			} else {
				propertyInfo.SetValue(o, Convert.ChangeType(val, propertyInfo.PropertyType), null);
			}
		}
		
		/// <summary>
		/// Sets all properties in the object <code>o</code> to the xml element <code>el</code> defined properties.
		/// </summary>
		void SetAttributes(object o, XmlElement el)
		{
			if (el.Name == "AcceptButton") {
				mainForm = (Form)o;
				acceptButtonName = el.Attributes["value"].InnerText;
				return;
			}
			
			if (el.Name == "CancelButton") {
				mainForm = (Form)o;
				cancelButtonName = el.Attributes["value"].InnerText;
				return;
			}

			if (el.Attributes["value"] != null) {
				SetValue(o, el.Name, el.Attributes["value"].InnerText);
			} else if (el.Attributes["event"] != null) {
				EventInfo eventInfo = o.GetType().GetEvent(el.Name);
				eventInfo.AddEventHandler(o, Delegate.CreateDelegate(eventInfo.EventHandlerType, customizationObject, el.Attributes["event"].InnerText));
			} else {
				PropertyInfo propertyInfo = o.GetType().GetProperty(el.Name);
				object pv = propertyInfo.GetValue(o, null);
				if (pv is IList) {
					foreach (XmlNode subNode in el.ChildNodes) {
						if (subNode is XmlElement){
							XmlElement subElement = (XmlElement)subNode;
							object collectionObject = CreateObject(subElement.Name);
							SetUpObject(collectionObject, subElement);
							
							if (collectionObject is Control) {
								string name = ((Control)collectionObject).Name;
								if (name != null && name.Length > 0) {
									ControlDictionary[name] = (Control)collectionObject;
								}
							}
							
							((IList)pv).Add(collectionObject);
						}
					}
				} else {
					object propertyObject = CreateObject(o.GetType().GetProperty(el.Name).PropertyType.Name);
					SetUpObject(propertyObject, el);
					propertyInfo.SetValue(o, propertyObject, null);
				}
			}
		}
		
		/// <summary>
		/// Creates a new instance of object name. It tries to resolve name in <code>System.Windows.Forms</code>,
		/// <code>System.Drawing</code> and <code>System namespace</code>.
		/// </summary>
		object CreateObject(string name)
		{
			object newObject = typeof(Control).Assembly.CreateInstance("System.Windows.Forms." + name);
			
			if (newObject == null) {
				newObject = Assembly.GetCallingAssembly().CreateInstance(name);
			}
			
			if (newObject == null) {
				newObject = typeof(Size).Assembly.CreateInstance("System.Drawing." + name);
			}
			
			if (newObject == null) {
				newObject = typeof(String).Assembly.CreateInstance("System." + name);
			}
			
			return newObject;
		}
	}
}
