   M S B A M L   `   `   `  ���� C  ?ILSpy, Version=1.0.0.1000, Culture=neutral, PublicKeyToken=nullG(clr-namespace:ICSharpCode.ILSpy.ControlsICSharpCode.ILSpy.Controls  )    #ICSharpCode.ILSpy.OpenFromGacDialog   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     =controls(clr-namespace:ICSharpCode.ILSpy.Controls       � Title$  Open From GAC��   DialogWindow#���    � WindowStartupLocation$ CenterOwner=� � 
ResizeMode$ CanResizeWithGrip=�$	��200��$	��150��$	��350��$	��750�� +� FocusedElement ��  �� ElementName$ filterTextBox��.��� $
��12,8q� � RowDefinitions �� $
��Auto ��� $��1* ��� $
��Auto �.+�]� .+��� $
��Left=� �� Target �� $ filterTextBox��.��
_Search:�� -   $��filterTextBox���� -   $��listView��$��1��$
��0, 8q�7   1ICSharpCode.ILSpy.Controls.SortableGridViewColumn   SortMode$ 	Automatic=� �� View �� .P�  -   
nameColumn��	 �� Width$		 300��$��Reference Name��
 �� DisplayMemberBinding
 �� *	ShortName+  $	 75��$��Version��
 �� *	Version+  $	 65��$��Culture��
 �� *	Culture+  $		 115��$��Public Key Token��
 �� *PublicKeyToken+  $
	 1000��$��Location��
 �� *
FileName+�� $��2�� �� Orientation$ 
Horizontal=�$��Right=�.+��� -   $��okButton�� �� 	IsDefault . $	��2,0q�{�.  .��Open��  �� IsCancel . $	��2,0q�.��Cancel� -   $��gacReadingProgressBar��$��1��$��10��$��Stretch=�$��Bottom=�$y�Hidden=�