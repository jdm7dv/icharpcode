   M S B A M L   `   `   `  ����  ICSharpCode.XamlBindingL    FICSharpCode.XamlBinding.PowerToys.Dialogs.EditGridColumnsAndRowsDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      g cICSharpCode.Core.Presentation, Version=4.3.1.9430, Culture=neutral, PublicKeyToken=f829da5c02be14ee1sd(http://icsharpcode.net/sharpdevelop/core  5        � Title$   Edit grid columns and rows��5      0  *ICSharpCode.Core.Presentation.GlobalStyles   DialogWindowStyle#��Z 5       � WindowStartupLocation$ CenterScreen=�5       � WindowStyle$ 
ToolWindow=�6'    � 	Resources 5       
  normal �� 
TargetType �� Property �� Value  itemTemplate �� 
TargetType	 �� Triggers
 :� RoutedEvent �� 
TargetName �� TargetProperty �� From	 �� To g� Duration ��Name �� CornerRadius �� Padding ��Name
 � Tag��%�  &	        &	 j     �� 5	      " ��6   .�5
      ��  � ��$ 3q�6   �� 5       � ��$ 3q�6   5      �� 5      " ��6)   	 5      :� 5      $
 
MouseEnter��6   .[�5      �� .��5      �� .��5   	   �� $ BorderControl��5   	   $ Background.Color�6/   $
 GrayX�5   	   $ DarkGrayX�6   $ 0:0:0.1D�6#   5   	   5      5      :� 5      $
 
MouseLeave��6   .[�5      �� .��5      �� .��5   	   �� $ BorderControl��5   	   $ Background.Color�6/   $ DarkGrayX�5   	   $
 GrayX�6   $ 0:0:0.1D�6#   5   	   5      5       .R�5"      �� $ BorderControl��6   
�������$ 5��65   $��3q�6F   $ 5q�6Q   ._�5#      ]� .+�5$      �� $ btnDeleteItem��6   -   5        $��r��5$   %   # z 61   $��Webdings)�6M   $��10(�5%      $
��Bold"�6   $��Right=�6'   6>   �� 5&      # z 6   #��z��6-   5'      5(      5)      5*      .��5+      ]� .+�5,      �� -   5         ��Name$ buttonPanel��5,   8   
������� J� 	AllowDrop . $��Bottom=�6�   $��Stretch=�6�   .+�5-      ��  �� Orientation$ 
Horizontal=�6   $��Center=�6)   .+�5.      �� -   5        $ btnOK��5.      #��[  6[   ��6   5  /ICSharpCode.Core.Presentation.LocalizeExtension  *Global.OKButtonText+6[   �� 5/      -   5        $ 	btnCancel��5/      #��[  6g   ��6      *Global.CancelButtonText+6g   50      51      �� 52      -   5        $ 	dropPanel��52   8   
������� . $��Right=�6}   $��25��6�   � 53       � ColumnDefinitions 54      �� 55      $
��Auto �6   �� 56      $��* �6   �� 57      $
��Auto �6    � RowDefinitions 59      �� 5:      $
��Auto �6   �� 5;      $��* �6   �� 5<      $
��Auto �6   .+�5>      � -   5         �Name$ columnWidthGrid��5>      $��1��6
   6   � 5?      -   5        $ rowHeightGrid��5?      $��1��6
   6   � 5@      -   5        $ gridDisplay��5@   '   $��1��6
   $��1��6    � ShowGridLines . 6<   5A      5B      5C      