   M S B A M L   `   `   `  ����h  dICSharpCode.WpfDesign.Designer, Version=4.3.1.9430, Culture=neutral, PublicKeyToken=f829da5c02be14eeg8clr-namespace:ICSharpCode.WpfDesign.Designer.OutlineView*ICSharpCode.WpfDesign.Designer.OutlineView  5      e7clr-namespace:ICSharpCode.WpfDesign.Designer.Converters)ICSharpCode.WpfDesign.Designer.Converters  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089>    8ICSharpCode.WpfDesign.Designer.Converters.LevelConverter   LevelConverter9   3ICSharpCode.WpfDesign.Designer.OutlineView.IconItem  �� 
TargetType �� Property �� Value �� 
TargetType �� Orientation   Icon   Text  ExpandButtonStyle J� SnapsToDevicePixels �� CornerRadius	 �� EndPoint
 �� 
StartPoint � Color � Offset F�Name �� Triggers P� Property P� Value �� 
TargetName  InsertBrush=   7ICSharpCode.WpfDesign.Designer.OutlineView.DragTreeView ��Name J� IsHitTestVisibleA   ;ICSharpCode.WpfDesign.Designer.OutlineView.DragTreeViewItem ]�Name d�Name �� 	Converter d� 	IsChecked �� RelativeSource ��Name ��Name�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      DDefault8clr-namespace:ICSharpCode.WpfDesign.Designer.OutlineView  5      F
Converters7clr-namespace:ICSharpCode.WpfDesign.Designer.Converters  5      %�
  &	        '       &	 ,    &	 �    '  �    7  '  b    7  7   7     5      6   �� 5      "   6	   .�5	      ��  � �� 5
      �� 5      "  6   .R�5      �� $ 
Horizontal=�6   .+�5      �� #��z 6   $
��None=�5      �� 5      #��z 6   $��Center=�5      $��5 0 0 0q�5      5      5      5      5      �� 5      "  d�5   	   .�5      ��  � |� .  5      �� 5       �@��	ClickMode$ Press=�5      �� 5       � �� 5      �� 5       " d�6   .R�5!      �� 
������ ._�5"      �� $��9��6   $��9��5#       . ���	#FF7898B5$��1q�5&      $ 1��5'      ��5(   	   �� 5)   
   $		 1,1&�6   $	
 0,0&�5*   
   .��5+      � $ White��6   $ .2Q�5,      � 5-      $ 	#FFC0B7A6��6   $ 1Q�5.      5/      ._�51   	   F� $ 
ExpandPath��52      $��1,1,1,1q�51      
���   �s����    �� �    ��- р� ��- р� @KL ���- @KL ���- ��- �@KL ��- �@KL �� ���- �� ���-     р�     р� �� 	54      55   	   56       57      P� 58       �@d�	IsChecked$
 TrueY�59      . �5:   	   �� $ 
ExpandPath��5;       � ��+ ��    �� �    ��- �@KL ��- �@KL �� 	5<      5=   	   5?      5A      5B      ��5D      #FFC73C��6&   �� 5F      "   6	   .�5G      ��  � �� 5H      �� 5I      "  6   .R�5J      � 
�������.+�5K      �� $
��Auto=�6   $
��Auto=�5L      .��5M   	   �� 5N   	   �� 5O      $ PART_InsertLine��6   8��  $��2��5Q      $��50��5R      $
��Left=�5S      $	��Top=�5T      $y�	Collapsed=�5U       .  5V      5W      5X      5Z      5[      �� 5]      "   6	   .�5^      ��  � ��# Z�5_      �� 5`       � �� 5a      �� 5b      "  6   .R�5d      ]� 
�������.+�5e      ]� $ bg��6   $	��Top=�5f      #��z��5g      .+�5h   	   d� $ expandButton��6   8��  $
��Left=�5j      ��5k      �� *Level+8   5l      �� *
IsExpanded+ �� *TemplatedParent+�� 5m   	   $ contentBorder��6   $
��Left=�5n      ._�5o   
   �� $ PART_Header��6   $��Header��5p   
   5q   
   5r   	   �� 5s      $ 	itemsHost��6   5t       5v      P� 5w       �@Q�
IsExpanded .  . �5y   	   �� $ 	itemsHost��6    � y�$ 	Collapsed=�5{      5|   	   P� 5}       �@��HasItems .  . �5   	   �� $ expandButton��6    � y�$ Hidden=�5�      5�   	   P� 5�       �@ 
IsSelected . . �5�   	   �� $ bg��6   # Z	�5�       � ���� 5�   	   # Z�5�       � ��5�   	   P� 5�       �@ IsDragHover . . �5�   	   �� $ contentBorder��6   8   � ��5�      5�   	   5�      5�      5�      5�      