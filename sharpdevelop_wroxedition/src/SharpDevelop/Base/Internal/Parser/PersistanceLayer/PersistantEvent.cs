// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.IO;
using System.Reflection;
using ICSharpCode.SharpDevelop.Services;

namespace SharpDevelop.Internal.Parser
{
	public sealed class PersistantEvent : AbstractEvent
	{
		public PersistantEvent(BinaryReader reader, ClassProxyCollection classProxyCollection)
		{
			fullyQualifiedName = reader.ReadString();
			documentation      = reader.ReadString();
			modifiers          = (ModifierEnum)reader.ReadUInt32();
			
			returnType         = new PersistantReturnType(reader, classProxyCollection);
			if (returnType.Name == null) {
				returnType = null;
			}
		}
		
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(fullyQualifiedName);
			writer.Write(documentation);
			writer.Write((uint)modifiers);
			((PersistantReturnType)returnType).WriteTo(writer);
		}
		
		public PersistantEvent(ClassProxyCollection classProxyCollection, IEvent e)
		{
			modifiers          = e.Modifiers;
			fullyQualifiedName = e.Name;
			documentation      = e.Documentation;
			if (documentation == null) {
				documentation = String.Empty;
			}
			
			returnType         = new PersistantReturnType(classProxyCollection, e.ReturnType);
		}
	}
}
