// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.Core.Services
{
	/// <summary>
	/// This Class contains two ResourceManagers, which handle string and image resources
	/// for the application. It do handle localization strings on this level.
	/// </summary>
	public class ResourceService : AbstractService
	{
		readonly static string uiLanguageProperty = "CoreProperties.UILanguage";
		
		readonly static string stringResources  = "StringResources";
		readonly static string imageResources   = "IconResources";
		
		readonly static string resourceDirctory = Application.StartupPath + 
		                                          Path.DirectorySeparatorChar + ".." +
		                                          Path.DirectorySeparatorChar + "data" +
		                                          Path.DirectorySeparatorChar + "resources";
		
		ResourceManager strings = null;
		ResourceManager icon    = null;
		
		Hashtable localStrings = null;
		Hashtable localIcons   = null;
		
		void ChangeProperty(object sender, PropertyEventArgs e)
		{
			if (e.Key == uiLanguageProperty && e.OldValue != e.NewValue) {
			    LoadLanguageResources();
			} 
		}
		void LoadLanguageResources()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string language = propertyService.GetProperty(uiLanguageProperty, Thread.CurrentThread.CurrentUICulture.Name);
			
			localStrings = Load(stringResources, language);
			if (localStrings == null && language.IndexOf('-') > 0) {
				localStrings = Load(stringResources, language.Split(new char[] {'-'})[0]);
			}
			
			localIcons = Load(imageResources, language);
			if (localIcons == null && language.IndexOf('-') > 0) {
				localIcons = Load(imageResources, language.Split(new char[] {'-'})[0]);
			}
		}
		
		// core service : Can't use Initialize, because all other stuff needs this service before initialize is called.
		public ResourceService()
		{
			strings = new ResourceManager(stringResources, Assembly.GetCallingAssembly());
			icon    = new ResourceManager(imageResources,  Assembly.GetCallingAssembly());
			
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			propertyService.PropertyChanged += new PropertyEventHandler(ChangeProperty);
			
			LoadLanguageResources();
		}
		
		public Font LoadFont(string fontName, int size)
		{
			return LoadFont(fontName, size, FontStyle.Regular);
		}
		
		public Font LoadFont(string fontName, int size, FontStyle style)
		{
			try {
				return new Font(fontName, size, style);
			} catch (Exception) {
				return SystemInformation.MenuFont;
			}
		}
		
		public Font LoadFont(string fontName, int size, GraphicsUnit unit)
		{
			return LoadFont(fontName, size, FontStyle.Regular, unit);
		}
		
		public Font LoadFont(string fontName, int size, FontStyle style, GraphicsUnit unit)
		{
			try {
				return new Font(fontName, size, style, unit);
			} catch (Exception) {
				return SystemInformation.MenuFont;
			}
		}
		
		Hashtable Load(string name, string language)
		{
			string fname = resourceDirctory + Path.DirectorySeparatorChar + name + "." + language + ".resources";
			
			if (File.Exists(fname)) {
					Hashtable resources = new Hashtable();
					ResourceReader rr = new ResourceReader(fname);
					foreach (DictionaryEntry entry in rr) {
						resources.Add(entry.Key, entry.Value);
					}
					rr.Close();
					return resources;
			}
			return null;
		}
		
		/// <summary>
		/// Returns a string from the resource database, it handles localization
		/// transparent for the user.
		/// </summary>
		/// <returns>
		/// The string in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested resource.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public string GetString(string name)
		{
			if (localStrings != null && localStrings[name] != null) {
				return localStrings[name].ToString();
			}
			
			string s = strings.GetString(name);
			
			if (s == null) {
				throw new ResourceNotFoundException("string >" + name + "<");
			}
			
			return s;
		}
		
		/// <summary>
		/// Returns a icon from the resource database, it handles localization
		/// transparent for the user. In the resource database can be a bitmap
		/// instead of an icon in the dabase. It is converted automatically.
		/// </summary>
		/// <returns>
		/// The icon in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested icon.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public Icon GetIcon(string name)
		{
			object iconobj = null;
			
			if (localIcons != null && localIcons[name] != null) {
				iconobj = localIcons[name];
			} else {
				iconobj = icon.GetObject(name);
			}
			
			if (iconobj == null) {
				return null;
			}
			
			if (iconobj is Icon) {
				return (Icon)iconobj;
			} else {
				return Icon.FromHandle(((Bitmap)iconobj).GetHicon());
			}
		}
		
		/// <summary>
		/// Returns a bitmap from the resource database, it handles localization
		/// transparent for the user. 
		/// </summary>
		/// <returns>
		/// The bitmap in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public Bitmap GetBitmap(string name)
		{
			if (localIcons != null && localIcons[name] != null) {
				return (Bitmap)localIcons[name];
			}
			return (Bitmap)icon.GetObject(name);
		}
	}
}
