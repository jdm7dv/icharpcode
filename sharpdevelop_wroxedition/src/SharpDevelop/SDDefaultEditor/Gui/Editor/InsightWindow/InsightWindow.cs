// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using SharpDevelop.Internal.Parser;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class InsightWindow : Form
	{
		TextAreaControl control;
		Stack           insightDataProviderStack = new Stack();
		
		CaretEventHandler    caretOffsetChangedEventHandler;
		EventHandler         focusEventHandler;
		KeyPressEventHandler keyPressEventHandler;
		TextAreaControl.DialogKeyProcessor dialogKeyProcessor;
		
		class InsightDataProviderStackElement 
		{
			public int                  currentData;
			public IInsightDataProvider dataProvider;
			
			public InsightDataProviderStackElement(IInsightDataProvider dataProvider)
			{
				this.currentData  = 0;
				this.dataProvider = dataProvider;
			}
		}
		
		public void AddInsightDataProvider(IInsightDataProvider provider)
		{
			provider.SetupDataProvider(fileName, control.Document);
			if (provider.InsightDataCount > 0) {
				insightDataProviderStack.Push(new InsightDataProviderStackElement(provider));
			}
		}
		
		int CurrentData {
			get {
				return ((InsightDataProviderStackElement)insightDataProviderStack.Peek()).currentData;
			}
			set {
				((InsightDataProviderStackElement)insightDataProviderStack.Peek()).currentData = value;
			}
		}
		
		IInsightDataProvider DataProvider {
			get {
				if (insightDataProviderStack.Count == 0) {
					return null;
				}
				return ((InsightDataProviderStackElement)insightDataProviderStack.Peek()).dataProvider;
			}
		}
		
		void CloseCurrentDataProvider()
		{
			insightDataProviderStack.Pop();
			if (insightDataProviderStack.Count == 0) {
				Close();
			} else {
				resized = false;
				Refresh();
			}
		}
		
		public void ShowInsightWindow()
		{
			if (!Visible) {
				if (insightDataProviderStack.Count > 0) {
					control.TextAreaPainter.IHaveTheFocusLock = true;
					Show();
					dialogKeyProcessor = new TextAreaControl.DialogKeyProcessor(ProcessTextAreaKey);
					control.ProcessDialogKeyProcessor += dialogKeyProcessor;
					control.Focus();
					control.TextAreaPainter.IHaveTheFocus     = true;
					control.TextAreaPainter.IHaveTheFocusLock = false;
				}
			} else {
				resized = false;
				Refresh();
			}
		}
		string fileName;
		
		public InsightWindow(TextAreaControl control, string fileName)
		{
			this.control             = control;
			this.fileName = fileName;
			Point caretPos  = control.Document.OffsetToView(control.Document.Caret.Offset);
			Point visualPos = new Point(control.TextAreaPainter.GetVirtualPos(control.Document.GetLineSegmentForOffset(control.Document.Caret.Offset), caretPos.X),
			                            (int)((1 + caretPos.Y) * control.TextAreaPainter.FontHeight) - control.TextAreaPainter.ScreenVirtualTop);
			
			focusEventHandler = new EventHandler(TextEditorLostFocus);
			caretOffsetChangedEventHandler = new CaretEventHandler(CaretOffsetChanged);
			
			control.TextAreaPainter.IHaveTheFocusChanged += focusEventHandler;
			control.Document.Caret.OffsetChanged += caretOffsetChangedEventHandler;
			keyPressEventHandler = new KeyPressEventHandler(KeyPressEvent);
			control.TextAreaPainter.KeyPress += keyPressEventHandler;
			
	 		Location = control.TextAreaPainter.PointToScreen(visualPos);
			
			StartPosition   = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			TopMost         = true;
			ShowInTaskbar   = false;
			Size            = new Size(0, 0);
			
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
		}
		
		// Methods that are inserted into the TextArea :
		bool ProcessTextAreaKey(Keys keyData)
		{
			switch (keyData) {
				case Keys.Escape:
					Close();
					return true;
				case Keys.Down:
					if (DataProvider != null && DataProvider.InsightDataCount > 0) {
						CurrentData = (CurrentData + 1) % DataProvider.InsightDataCount;
						resized = false;
						Refresh();
					}
					return true;
				case Keys.Up:
					if (DataProvider != null && DataProvider.InsightDataCount > 0) {
						CurrentData = (CurrentData + DataProvider.InsightDataCount - 1) % DataProvider.InsightDataCount;
						resized = false;
						Refresh();
					}
					return true;
			}
			return false;
		}
		
		void KeyPressEvent(object sender, KeyPressEventArgs  e)
		{
			if (DataProvider != null && DataProvider.CharTyped()) {
				CloseCurrentDataProvider();
			}
		}
		
		void CaretOffsetChanged(object sender, CaretEventArgs e)
		{
			// move the window under the caret (don't change the x position)
			Point caretPos  = control.Document.OffsetToView(control.Document.Caret.Offset);
			int y = (int)((1 + caretPos.Y) * control.TextAreaPainter.FontHeight) - control.TextAreaPainter.ScreenVirtualTop;
			Point p = control.TextAreaPainter.PointToScreen(new Point(0, y));
			p.X = Location.X;
			if (p.Y != Location.Y) {
				Location = p;
			}
			
			while (DataProvider != null && DataProvider.CaretOffsetChanged()) {
				 CloseCurrentDataProvider();
			}
		}
		///// END
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			
			// take out the inserted methods
			control.ProcessDialogKeyProcessor            -= dialogKeyProcessor;
			control.Document.Caret.OffsetChanged         -= caretOffsetChangedEventHandler;
			control.TextAreaPainter.IHaveTheFocusChanged -= focusEventHandler;
			control.TextAreaPainter.KeyPress             -= keyPressEventHandler;
		}
		
		protected void TextEditorLostFocus(object sender, EventArgs e)
		{
			if (!control.TextAreaPainter.IHaveTheFocus) {
				Close();
			}
		}
		
		public bool resized = false;
		protected override void OnPaint(PaintEventArgs pe)
		{
			pe.Graphics.DrawRectangle(new Pen(SystemColors.WindowFrame), new Rectangle( 0, 0, Width - 1, Height - 1));
			 
			Graphics g = pe.Graphics;
			
			string description;
			if (DataProvider == null || DataProvider.InsightDataCount < 1) {
				description = "Unknown Method";
			} else if (DataProvider.InsightDataCount > 1) {
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				stringParserService.Properties["CurrentMethodNumber"]  = (CurrentData + 1).ToString();
				stringParserService.Properties["NumberOfTotalMethods"] = DataProvider.InsightDataCount.ToString();
				description = String.Concat(stringParserService.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.InsightWindow.NumberOfText}"), " ", DataProvider.GetInsightData(CurrentData));
			} else {
				description = DataProvider.GetInsightData(CurrentData);
			}
			
			SizeF size = g.MeasureString(description, Font);
			int h = (int)size.Height + 4;
			int w = (int)size.Width  + 4;
			
			if (!resized && (h != Height || w != Width)) {
				resized = true;
				size = g.MeasureString(description, Font, Width);
				Height = h;
				Width  = w;
				SizeF size2 = g.MeasureString(description, Font, Width);
				Height = (int)size2.Height;
				Refresh();
				return;
			}
			RectangleF rect = new RectangleF(0, 0, Width, Height);
			g.DrawString(description, Font, new SolidBrush(SystemColors.InfoText), rect);
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			pe.Graphics.FillRectangle(new SolidBrush(SystemColors.Info), pe.ClipRectangle);
		}
	}
}
