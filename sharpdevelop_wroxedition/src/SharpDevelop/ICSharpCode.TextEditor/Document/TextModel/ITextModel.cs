// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Drawing;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace ICSharpCode.TextEditor.Document
{
	public interface ITextModel
	{
		int GetViewXPos(LineSegment line, int logicalXPos);
		int GetLogicalXPos(LineSegment line, int viewXPos);
		
		Point OffsetToView(int offset);
		int   ViewToOffset(Point p);
	}
}
