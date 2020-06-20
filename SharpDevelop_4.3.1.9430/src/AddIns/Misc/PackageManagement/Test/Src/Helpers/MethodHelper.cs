﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class MethodHelper : MethodOrPropertyHelper
	{
		public IMethod Method;
		public ProjectContentHelper ProjectContentHelper = new ProjectContentHelper();
		
		/// <summary>
		/// Method name should include class prefix (e.g. "Class1.MyMethod")
		/// </summary>
		public void CreateMethod(string fullyQualifiedName)
		{
			Method = MockRepository.GenerateMock<IMethod, IEntity>();
			Method.Stub(m => m.ProjectContent).Return(ProjectContentHelper.ProjectContent);
			Method.Stub(m => m.FullyQualifiedName).Return(fullyQualifiedName);
			Method.Stub(m => m.Parameters).Return(parameters);
		}
		
		public void CreatePublicMethod(string name)
		{
			CreateMethod(name);
			Method.Stub(m => m.IsPublic).Return(true);
		}
		
		public void CreatePublicConstructor(string name)
		{
			CreatePublicMethod(name);
			Method.Stub(m => m.IsConstructor).Return(true);
		}
		
		public void CreatePrivateMethod(string name)
		{
			CreateMethod(name);
			Method.Stub(m => m.IsPublic).Return(false);
			Method.Stub(m => m.IsPrivate).Return(true);
		}
		
		public void FunctionStartsAtColumn(int column)
		{
			var region = new DomRegion(1, column);
			FunctionStartsAt(region);
		}
		
		public void FunctionStartsAt(DomRegion region)
		{
			Method.Stub(m => m.Region).Return(region);
		}
		
		public void FunctionStartsAtLine(int line)
		{
			var region = new DomRegion(line, 1);
			FunctionStartsAt(region);
		}
		
		public void FunctionBodyEndsAtColumn(int column)
		{
			var region = new DomRegion(1, 1, 1, column);
			FunctionBodyEndsAt(region);
		}
		
		void FunctionBodyEndsAt(DomRegion region)
		{
			Method.Stub(m => m.BodyRegion).Return(region);
		}
		
		public void FunctionBodyEndsAtLine(int line)
		{
			var region = new DomRegion(1, 1, line, 1);
			FunctionBodyEndsAt(region);
		}
		
		public void SetRegion(DomRegion region)
		{
			Method.Stub(m => m.Region).Return(region);
		}
		
		public void SetBodyRegion(DomRegion region)
		{
			Method.Stub(m => m.BodyRegion).Return(region);
		}
		
		public void SetCompilationUnitFileName(string fileName)
		{
			ICompilationUnit unit = MockRepository.GenerateStub<ICompilationUnit>();
			unit.FileName = fileName;
			Method.Stub(m => m.CompilationUnit).Return(unit);
		}
		
		public void AddDeclaringTypeAsInterface(string name)
		{
			IClass declaringType = ProjectContentHelper.AddInterfaceToProjectContent(name);
			SetDeclaringType(declaringType);
		}
		
		public void SetDeclaringType(IClass declaringType)
		{
			Method.Stub(m => m.DeclaringType).Return(declaringType);
		}
		
		public void AddDeclaringType(string name)
		{
			IClass declaringType = ProjectContentHelper.AddClassToProjectContent(name);
			SetDeclaringType(declaringType);
		}
		
		public void AddReturnTypeToMethod(string type)
		{
			var returnTypeHelper = new ReturnTypeHelper();
			returnTypeHelper.CreateReturnType(type);
			returnTypeHelper.AddDotNetName(type);
			
			Method.Stub(m => m.ReturnType).Return(returnTypeHelper.ReturnType);
		}
		
		public void MakeMethodStatic()
		{
			Method.Stub(m => m.IsStatic).Return(true);
		}
		
		public void MakeMethodAbstract()
		{
			Method.Stub(m => m.IsAbstract).Return(true);
		}
		
		public void MakeMethodVirtual()
		{
			Method.Stub(m => m.IsVirtual).Return(true);
		}
		
		public void AddAttributeToMethod(string attributeTypeName)
		{
			var attributeHelper = new AttributeHelper();
			attributeHelper.CreateAttribute(attributeTypeName);
			attributeHelper.AddAttributeToMethod(Method);
		}
		
		public void MakeMethodOverride()
		{
			Method.Stub(m => m.IsOverride).Return(true);
		}
		
		public void MakeMethodSealed()
		{
			Method.Stub(m => m.IsSealed).Return(true);
		}
		
		public void MakeMethodNewOverride()
		{
			Method.Stub(m => m.IsNew).Return(true);
		}
		
		public void MakeMethodOverridable()
		{
			Method.Stub(m => m.IsOverridable).Return(true);
		}
		
		public void AddTypeParameter(string name)
		{
			var typeParameterHelper = new TypeParameterHelper();
			typeParameterHelper.SetName(name);
			AddTypeParameters(typeParameterHelper.TypeParameterToList());
		}
		
		public void AddTypeParameters(List<ITypeParameter> typeParameters)
		{
			Method.Stub(m => m.TypeParameters).Return(typeParameters);
		}
		
		public void NoTypeParameters()
		{
			AddTypeParameters(new List<ITypeParameter>());
		}
	}
}
