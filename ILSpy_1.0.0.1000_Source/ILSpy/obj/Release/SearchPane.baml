   M S B A M L   `   `   `  ���� C  ?ILSpy, Version=1.0.0.1000, Culture=neutral, PublicKeyToken=nullG(clr-namespace:ICSharpCode.ILSpy.ControlsICSharpCode.ILSpy.Controls  "    ICSharpCode.ILSpy.SearchPane   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     =controls(clr-namespace:ICSharpCode.ILSpy.Controls     -     >�Name$
  self��.���  � RowDefinitions �� $��21 ��� $��* � � ColumnDefinitions �� $��* ��� $
��Auto �.+�*   $ICSharpCode.ILSpy.Controls.SearchBox  -     Name$ 	searchBox��*   $ICSharpCode.ILSpy.Controls.SearchBox   PreviewKeyDown$ SearchBox_PreviewKeyDown��$	��Top=�   WatermarkText$ Search��   WatermarkColor
 ����� � ToolTip$ Search��$��0��$��0��   UpdateDelay$ 0:0:0.1h����� *
SearchTerm+	 �� ElementName$
	 self���� $��1��$��0��
 �� Orientation$
 
Horizontal=�.+��� $
��0,-1q�.��Search _for:�� -   $��searchModeComboBox��$	��100������ .R��� $
 
Horizontal=�.+��� $��16��$��16��$��0,0,4,0q����� *Image+�� ���� *Name+�� -   $��listBox��$��Stretch=� �� SelectionMode$ Single=�$��1��$��2������ .R��  �� $��20 ��� $
��Auto ��� $��* �.+��� $��16��$��16��$
��Left=����� *Image+�� $��1������ *Name+�� $
 
Horizontal=�$��2��$��Right=�.+��� $��16��$��16��$��4,0,4,0q����� *LocationImage+�� $��CharacterEllipsis=����� *
Location+