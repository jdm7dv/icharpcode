   M S B A M L   `   `   `  ���� R  NICSharpCode.TreeView, Version=4.1.0.7275, Culture=neutral, PublicKeyToken=nullY@clr-namespace:ICSharpCode.TreeView;assembly=ICSharpCode.TreeViewICSharpCode.TreeView  C ?ILSpy, Version=1.0.0.1000, Culture=neutral, PublicKeyToken=null5clr-namespace:ICSharpCode.ILSpyICSharpCode.ILSpy G(clr-namespace:ICSharpCode.ILSpy.TextViewICSharpCode.ILSpy.TextView G(clr-namespace:ICSharpCode.ILSpy.ControlsICSharpCode.ILSpy.Controls "   ICSharpCode.ILSpy.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Otv@clr-namespace:ICSharpCode.TreeView;assembly=ICSharpCode.TreeView     1localclr-namespace:ICSharpCode.ILSpy     =textView(clr-namespace:ICSharpCode.ILSpy.TextView     =controls(clr-namespace:ICSharpCode.ILSpy.Controls       � Title$  ILSpy��$	��250��$	��200�� � UseLayoutRounding . &   System.Windows.Media.TextOptions   TextFormattingMode$ Display=� +� FocusedElement ��  �� ElementName$ treeView�� � 	Resources     BooleanToVisibilityConverter��%   &	        ��  J� CommandBindings )  #System.Windows.Input.CommandBinding  -      Command$
 Open��  -   $ Refresh��  -   $
 Save��  -   $ 
BrowseBack��  -   $ BrowseForward��  -   $ Search��.��]� .+�o� -   $��mainMenu��$	��Top=�.@�m� $��_File��m� $��_View��.@�m� $&�� Show _internal types and members�� m� IsCheckable . 	 m� 	IsChecked	 �� * FilterSettings.ShowInternalApi+
 m� Icon
 �� $��16��$��16��$ ��Images/PrivateInternal.png��c� -   $��toolBar��$	��Top=�  �� 
TargetType �� Triggers �� Binding �� RelativeSource �� AncestorType �� AncestorLevel �� Path �� Value �� Property �� Value��%y   '��       �� " �� ��  ��  �� " ��$ 1��$ 	IsEnabled�	 False.a���  �@J�Opacity$
 0.30Q�.@��� �� ��  � ToolTip$% Show internal types and members�� d� 	IsChecked �� * FilterSettings.ShowInternalApi+.���� $��16��$��16��$ ��Images/PrivateInternal.png���� �� -	   $��languageComboBox�� �� DisplayMemberPath$
 Name��$	��100��!  ICSharpCode.ILSpy.Languages   AllLanguages#��Z  �� SelectedItem �� *FilterSettings.Language+�  � ColumnDefinitions �� -
    �Name$ 
leftColumn��$	��100��$
��0.4* ��� $��3 ��� -   $ rightColumn��$	��100��$
��0.6* � � RowDefinitions �� $��* �.+�(   "ICSharpCode.TreeView.SharpTreeView  -   $��treeView��   ShowRoot .     AllowDropOrder .  J� 	AllowDrop . ��   K� ZIndex$  1��$��1��$
��-5,0q�$	��5,0q�$��Center=�$��Stretch=�
������ � $��2�� �� $��* � �� $
��Auto ��� -   $ 
topPaneRow��$��0 ��� $��1 ��� -   $ textViewRow��$
��0.7* �$	��100���� $��1 ��� -   $ bottomPaneRow��$��0 �.+��� -   $��updateAvailablePanel��
���   �$��1q�$y�	Collapsed=�._�]� .+��� -   $��Right=�$��0��.��X�� ! �� Orientation$! 
Horizontal=�.+��� $	��4,0q�$��Center=�#!A new ILSpy version is available.�� -   .��
Download+ %ICSharpCode.ILSpy.Controls.DockedPane  -   "  Name$" topPane��+ %ICSharpCode.ILSpy.Controls.DockedPane#   CloseButtonClicked$ # TopPane_CloseButtonClicked��$��1��$   Title$	$ Top��$y�	Collapsed=��� $  1��$��2��$��	0,-2,0,-5q�$��0,2,0,5q�
������ $��Stretch=�$��Center=�y��� *
Visibility+$ topPane���� -   $��mainPane��$��3���� $  1��$��4��$��	0,-2,0,-5q�$��0,2,0,5q�
������ $��Stretch=�$��Center=�y��� *
Visibility+$ 
bottomPane��  -   $" 
bottomPane��$## BottomPane_CloseButtonClicked��$��5��$$ Bottom��$y�	Collapsed=�