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
	public sealed class PersistantMethod : AbstractMethod
	{
		public PersistantMethod(BinaryReader reader, ClassProxyCollection classProxyCollection)
		{
			fullyQualifiedName = reader.ReadString();
			documentation      = reader.ReadString();
			
			modifiers          = (ModifierEnum)reader.ReadUInt32();
			returnType         = new PersistantReturnType(reader, classProxyCollection);
			if (returnType.Name == null) {
				returnType = null;
			}
			
			uint count = reader.ReadUInt32();
			for (uint i = 0; i < count; ++i) {
				parameters.Add(new PersistantParameter(reader, classProxyCollection));
			}
		}
		
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(fullyQualifiedName);
			writer.Write(documentation);
			writer.Write((uint)modifiers);
			((PersistantReturnType)returnType).WriteTo(writer);
			
			writer.Write((uint)parameters.Count);
			foreach (PersistantParameter p in parameters) {
				p.WriteTo(writer);
			}
		}
		
		public PersistantMethod(ClassProxyCollection classProxyCollection, IMethod method)
		{
			fullyQualifiedName = method.Name;
			documentation      = method.Documentation;
			if (documentation == null) {
				documentation = String.Empty;
			}
			
			modifiers  = method.Modifiers;
			returnType = new PersistantReturnType(classProxyCollection, method.ReturnType);
			
			foreach (IParameter param in method.Parameters) {
				parameters.Add(new PersistantParameter(classProxyCollection, param));
			}
			
			region = null;
		}
		
	}
}
