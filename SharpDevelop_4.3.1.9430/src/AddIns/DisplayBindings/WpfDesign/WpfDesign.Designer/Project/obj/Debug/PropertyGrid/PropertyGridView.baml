   M S B A M L   `   `   `  ����h  dICSharpCode.WpfDesign.Designer, Version=4.3.1.9430, Culture=neutral, PublicKeyToken=f829da5c02be14eee7clr-namespace:ICSharpCode.WpfDesign.Designer.Converters)ICSharpCode.WpfDesign.Designer.Converters  5      i9clr-namespace:ICSharpCode.WpfDesign.Designer.PropertyGrid+ICSharpCode.WpfDesign.Designer.PropertyGrid  5      _ [ICSharpCode.WpfDesign, Version=4.3.1.9430, Culture=neutral, PublicKeyToken=f829da5c02be14eevOclr-namespace:ICSharpCode.WpfDesign.PropertyGrid;assembly=ICSharpCode.WpfDesign"ICSharpCode.WpfDesign.PropertyGrid 5      a5clr-namespace:ICSharpCode.WpfDesign.Designer.Controls'ICSharpCode.WpfDesign.Designer.Controls  5      B    <ICSharpCode.WpfDesign.Designer.PropertyGrid.PropertyGridView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/netfx/2007/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      N
Converters7clr-namespace:ICSharpCode.WpfDesign.Designer.Converters     5      RPropertyGrid9clr-namespace:ICSharpCode.WpfDesign.Designer.PropertyGrid     5      lPropertyGridBaseOclr-namespace:ICSharpCode.WpfDesign.PropertyGrid;assembly=ICSharpCode.WpfDesign     5      JControls5clr-namespace:ICSharpCode.WpfDesign.Designer.Controls     5      -   5          >�Name$
  root��5      #��Z�5	       J� SnapsToDevicePixels .  � 	Resources 5         ExpandButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� EndPoint	 �� 
StartPoint
 � Color � Offset F�Name �� Triggers P� Property P� Value �� 
TargetName  MoreButtonStyle  CategoryToggleStyle �� Orientation d� 	IsChecked  CategoryExpanderStyle ��Name  MoreExpanderStyle �� Mode �� RelativeSource �� DataType1  +ICSharpCode.WpfDesign.PropertyGrid.Category 9� 
IsExpandedB   <ICSharpCode.WpfDesign.Designer.Converters.CollapsedWhenFalse   Instance �� 	Converter5  /ICSharpCode.WpfDesign.PropertyGrid.PropertyNode ��Name �� AncestorTypeB   <ICSharpCode.WpfDesign.Designer.PropertyGrid.PropertyGridView d�Name>   8ICSharpCode.WpfDesign.Designer.Converters.LevelConverter   Instance?   9ICSharpCode.WpfDesign.Designer.Converters.HiddenWhenFalse   Instance � ToolTip<   6ICSharpCode.WpfDesign.Designer.Converters.BoldWhenTrue    InstanceA   ;ICSharpCode.WpfDesign.Designer.Converters.CollapsedWhenZero!   Instance" �� Triggers# �� Value$ �� Binding��%  &	        &	 \    &	 h    7   &	 X    7  &	 	    7  (��     5�      *, +)7  (�� �    5�      *, +)7   7  �� 5      " d�5   
   .�5      ��  � |� .  5      �� 5       � �� 5      �� 5      " d�6   .R�5      �� 
������ ._�5   	   �� $��9��6   $��9��5       . ���	#FF7898B5$��1q�5      $ 1��5      ��5   
   �� 5      $	 1,1&�6   $		 0,0&�5      .��5      � $
 White��6   $ .2Q�5       � 5!      $
 	#FFC0B7A6��6   $ 1Q�5"      5#      ._�5%   
   F� $ 
ExpandPath��5&      $��1,1,1,1q�5%      
���   �s����    �� �    ��- р� ��- р� @KL ���- @KL ���- ��- �@KL ��- �@KL �� ���- �� ���-     р�     р� �� 	5(      5)   
   5*   	    5+      P� 5,   	    �@d�	IsChecked$
 TrueY�5-      . �5.   
   �� $ 
ExpandPath��5/       � ��+ ��    �� �    ��- �@KL ��- �@KL �� 	50      51   
   53      55      56      �� 58      " d�59   
   .�5:      ��  � |� .  5;      �� 5<       � �� 5=      �� 5>      " d�6   .R�5?      �� ���#F9F9F4$��0 0 0 1q�5@      #��Z�5A      ._�5G   	   F� $ arrow��6   "����        �@KL @KL р��     	$��Center=�5I      $��Center=�5J      #��Z�5K      5L   	    5M      P� 5N   	    �@d�	IsChecked$
 TrueY�5O      . �5P   
   �� $ arrow��6    � ��" ��    @KL �@KL     р�� @KL 	5R      5S   
   5U      5W      5X      �� 5Z      " d�5[   
   .�5\      ��  � |� .  5]      �� 5^       � �� 5_      �� 5`      " d�6   .R�5a      �� $��0 1 0 0q�6   ���#ECE9D8._�5c   	   �� $ 
Horizontal=�6   #��Z�5d      .+�5e   
   d� 8��  $��Center=�5g      $��3 0 7 0q�5h       5f      �� *
IsExpanded+5h      �� 5i   
   $��Center=�5j      $
��Bold"�5k      #��Z�5l      ��5i      �� *Name+5l      5m   
   5n   	   5o      5q      5r      �� 5t      " 9�5u   
   .�5v      ��  � �� 5w      �� 5x      " 9�6   .R�5y      ]� .+�5z   	   d� $��17��6   $	��Top=�5|      8��   5{      �� *
IsExpanded+5}      �� 5~   	   $ 
ExpandSite��6   $y�	Collapsed=�5   	   5�   	    5�      P� 5�   	    �@9�
IsExpanded . . �5�   
   �� $ 
ExpandSite��5�       � y�$ Visible=�5�      5�      5�   
   5�      5�      5�      �� 5�      " 9�5�   
   .�5�      ��  � �� 5�      �� 5�      " 9�6   .R�5�      ]� .+�5�   	   d� 8��  $	��Top=�5�      $��12��5�       5�      �� *
IsExpanded+$ TwoWay=� �� *TemplatedParent+5�      �� 5�   	   $ 
ExpandSite��6   $y�	Collapsed=�5�   	   5�   	    5�      P� 5�   	    �@9�
IsExpanded . . �5�   
   �� $ 
ExpandSite��5�       � y�$ Visible=�5�      5�      5�   
   5�      5�      5�      �� 5�      "  6   .R�5�      9� 8��  ��6   �� *Name+ 5�      �� *
IsExpanded+y�5�      �� *	IsVisible+# Z .��5�      �� ��6   �� *
Properties+5�      5�      �� 5�      "  6   .R�5�      �� y�6   �� *	IsVisible+# Z .+�5�      �� $ uxPropertyNodeRow��6   $��20��5�      $��0 0 0 1q�5�      #��Z�5�      $	��Top=�5�      ._�5�      ]� .+�5�      ]� $
��Left=�5�      ��5�      �� *FirstColumnWidth+ �� *FindAncestor+"  .+�5�   	   d� $ expandButton��6   $
��Left=�5�      8��  ��5�      �� *Level+# Z  5�      �� *
IsExpanded+y�5�      �� *HasChildren+# Z �� 5�   	   $��CharacterEllipsis=�5�      $��Center=�5�      $��7 0 0 0q�5�      ��5�      �� *Name+ 5�      �� *Description+��5�      �� *IsSet+# Z  ��5�      �� *NameForeground+5�   	   �� 5�      $��1 0 0 0q�6   #��Z�5�      ._�5�   	   �� $ editorContainer��6   $��Center=�5�   	   $	��3 0q�5�   	   ��5�   	   �� *Editor+5�   	   5�   	   5�      5�      �� 5�      y�6   �� *
IsExpanded+# Z .+�5�      �� ��6   �� *
Children+y�5�      �� *Children.Count+# Z! 9� 5�      8�� y�6   �� *MoreChildren.Count+# Z! .��5�      �� ���#F9F9F4��6   �� *MoreChildren+5�      5�      5�      5�      " 5�      �� 5�      $# False��5�      $ 5�      �� *	IsEnabled+.a�5�      �� $ editorContainer��6    �@J�Opacity$	 0.5Q�5�      �� 5�      $ editorContainer��6    �@J�IsHitTestVisible .  5�      5�      5�      5�      .��5�      ]� .+�5�      � $	��Top=�6	   $��78��5�   	   .+�5�      �� $��Name:��6   $��6,30.275,0,33.948q�5�      $
��Left=�5�      $��32.033��5      �� 5     $��Type:��6   $	��Top=�5     $��6.424,7.998,0,0q�5     $��13.277��5     �� 5     $��Filter:��6   $
��Left=�5     $��6,53.553,0,0q�5     $	��Top=�5     :	   4ICSharpCode.WpfDesign.Designer.Controls.EnterTextBox	  5
     -   5        % 	 Name$% NameTextBox��5
     $��44.033,27.275,6,0q�5     $��19.277��5     $	��Top=�5     ��5     �� *Name+{�5     �� *IsNameEnabled+.��5     ��5     �� 5     " ��6   & �� Triggers& 5     �� 5  	   $ 6   �� *IsNameCorrect+	# False5     .a�5  
   ��  � ��
 �  ��5     5  
   ^� 5  	   ' ^� 
Conditions' 5  
   �� 5     ( �� Binding( 6   �� *IsNameEnabled+) �� Value) True5     �� 5     ( 6   �� *
CurrentTab+
) Events5     �� 5     ( 6   �� *Text+ �� *Self+)  5      .0�5"  
   ��  � ��
 �  ��5#     5$  
   5&     .��5'     5(     �� 5*     $��44.033,7.998,6,0q�5,     $��13.277��5-     $	��Top=�5.     ��5*     �� *SingleItem.ComponentType.Name+ 5+     �� *SingleItem.ComponentType+5.     	  5/     $��44.033,50.553,52,0q�50     $	��Top=�51     $��19.277��52     ��5/     �� *Filter+* �� UpdateSourceTrigger$* PropertyChanged=�52     �� 54     -   5        + ��Name$+ clearButton��54     $��Clear��55     $��Right=�57     $	��Top=�58     $��0,49.552,6,0q�59     $��21.277��5:     $��40��5;     5<     5
   /ICSharpCode.WpfDesign.Designer.Controls.EnumBar
  5>     $��5 0 0 5q�5@     $	��Top=�5A     , 
  Value, 5>     �� *
CurrentTab+- 
  	Container- 5?     �� . �� ElementName$. c1��/ 
  ButtonStyle/ 5B     �� 5C     " d�6   .�5D     ��  � ��$ 70��5E     5F     5H     � 5J     -   5        0 �Name$0 c1��5J  	   
�������.+�5L     �� $��Disabled=�6   .��5M     �� ��6   �� *
Categories+5N     �� 5O     $��Disabled=�6   .��5P     �� ��6   �� *Events+5Q     n� 5R     -   5        1 n�Name$1 thumb��5R     $
��Left=�5S     $��4��5T     $��-2 0 0 0q�5U     2 � Cursor$2 SizeWE��5V     z�5W     T� 5X     3 T� X3 6   �� *FirstColumnWidth+$
. root����5Z     �� 5[     .R�5\     �� 
������ 6   5]     5_     5`     5b     5d     