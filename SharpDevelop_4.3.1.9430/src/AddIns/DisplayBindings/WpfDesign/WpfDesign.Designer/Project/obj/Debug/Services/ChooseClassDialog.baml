   M S B A M L   `   `   `  ����h  dICSharpCode.WpfDesign.Designer, Version=4.3.1.9430, Culture=neutral, PublicKeyToken=f829da5c02be14eea5clr-namespace:ICSharpCode.WpfDesign.Designer.Services'ICSharpCode.WpfDesign.Designer.Services  5   	   ?    9ICSharpCode.WpfDesign.Designer.Services.ChooseClassDialog   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   JServices5clr-namespace:ICSharpCode.WpfDesign.Designer.Services     5   	   #��Z�5   	     J� SnapsToDevicePixels  .  � WindowStartupLocation$ CenterScreen=�5   	    � 
ResizeMode$ CanResizeWithGrip=�5   	    � Title$ Choose Class��5	   	   $	��438��5
   	   $	��488��5   	   .��5      � .+�5   
   �� $��Starts with:��6   $
��Left=�5      $��	12,12,0,0q�5      $	��Top=�5      $��13��5      $��55��5      �� 5   
   -   5         ��Name$ uxFilter��5      $��23��5      $��
12,31,12,0q�5      $	��Top=�5      ��5      �� *Filter+ �� UpdateSourceTrigger$ PropertyChanged=�5      :  4ICSharpCode.WpfDesign.Designer.Services.ClassListBox  5   
   -   5          Name$ uxList��5       $��12,60,12,78q�5       $ �� IsSynchronizedWithCurrentItem$
 TrueY�5       $��Hidden=�5       ��5       �� *	Classes+��5      �� 5       .R�5!      ��  �� Orientation$ 
Horizontal=�6!   .+�5"      �� $��../Images/Class.png��6    $��16��5#       $��16��5$       �� 5&      $��5 0 0 0q�6$   $��Center=�5(   $   ��5'   $   �� @   :ICSharpCode.WpfDesign.Designer.Services.ClassNameConverter	   Instance
 �� 	Converter#
 Z	 5(   $   5)      5*      5,      �� 5.   
   $��Show System Classes��6   $��16��50      $
��Left=�51      $��	12,0,0,56q�52      $��Bottom=�53      $	��120��54       d� 	IsChecked 5/      �� *ShowSystemClasses+54      �� 55   
   -   5         ��Name$
 uxOk��55      $��OK��56      $��23��57      $��	0,0,93,12q�58      $��Bottom=�59      $��Right=�5:      $��75��5;       �� 	IsDefault . {�5=      �� *SelectedItem+ �� ElementName$ uxList��A   ;ICSharpCode.WpfDesign.Designer.Services.NullToBoolConverter   Instance#
 Z �� 5>   
   $��Cancel��6   $��23��5?      $��Right=�5@      $��	0,0,12,12q�5A      $��Bottom=�5B      $��75��5C       �� IsCancel . 5D      5E      5F      