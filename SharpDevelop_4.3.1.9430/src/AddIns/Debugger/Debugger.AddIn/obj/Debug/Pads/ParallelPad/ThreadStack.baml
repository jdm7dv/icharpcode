   M S B A M L   `   `   `  ����  Debugger.AddIn1    +Debugger.AddIn.Pads.ParallelPad.ThreadStack   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     68   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6z   $
��Auto��5      $
��Auto��5      
������   � 	Resources  5         RowBackground �� 
StartPoint �� EndPoint � Color � Offset��%�   &	        �� 5      $ 0.5,0&�6.   $ 0.5,1&�6A   .��5	      � $ #F7F7F7��6   $	 0.2Q�6"   � 5
      $ #EAEAEA��6   $ .5Q�6"   � 5      $ #E5E5E5��6   $ .8Q�6"   5      5      .��5      �� -   5         ��Name$ BorderParent��5      
���   �$��3q�5       �� CornerRadius$ 5��5      $
��Auto��5      ._�5      �� 
������ $��-5q�5      .+�5      �� $��23��5      $��5,5,5,0q�5      $ 5,5,0,0��5      
������ ��5      �� 5      $ 0,0.5&�5      $ 1,0.5&�5      .��5       � $ 	#FFE2F6FE��5!      $ 0Q�5"      � 5#      $ White��5$      $ 1Q�5%      5&      ._�5(      �� -   5         ��Name$ 
HeaderText��5*      $��Center=�5)      $��Khmer UI)�5+      $��12(�5,      $��2q�5-      $��Center=�5.      5/      &   System.Windows.Controls.DataGrid  50      -   5          Name$ datagrid��53      
������ $��5,0,5,5q�52      "	   VerticalScrollBarVisibility$	 Disabled=�54      $
   HorizontalScrollBarVisibility$
 Disabled=�55         GridLinesVisibility$
 None=�56         	RowHeight$ 18Q�57         SelectionMode$ Single=�58         SelectionUnit$ FullRow=�59         AutoGenerateColumns .     CanUserAddRows .     HeadersVisibility$
 None=�5=      $��0q�5>      ��5:      ��    	CellStyle 5A      �� 5B       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  5C      .�5D      ��  �� Property � �� �� Value
 �����6%   �� 5E       � |� .  5G      �� 5H       � ��$ 0q�5J       �� Triggers 5K      P� 5L   	    P� Property �@ 
IsSelected P� Value . . �5P   
   ��  � ��
 �   �5R   
   �� 5S   
    � �� 5U   
   Z� 5V   
   5X         RowStyle 5Z      �� 5[      )  #System.Windows.Controls.DataGridRow"  6   .�5\      ��  � ��
 �����65    5]      P� 5^   	    �@J�IsMouseOver . . �5_   
   ��  � ��# [  6'   5`   
   5b         Columns 5d      4  .System.Windows.Controls.DataGridTemplateColumn  5e         CellTemplate 5f      �� 5g   	   .R�5h   
   �� 
�������$��0,1,1,0q�6$   $��25��6>   $��22��6I   ._�5i      �� $��Center=�6   $��0,-5,0,0q�6,   $��14��6>   $��14��6I   $��Center=�6U   ��5j      �� *Image+5k      5l   
   5n        5o      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 5p       5q      �� 5r   	   .R�5s   
   �� 
�������$��0,1,0,0q�6$   ._�5t      �� $��5,0,10,0q�6   $��Center=�5u      $��Khmer UI)�5v      $��12(�5w      ��5x      ��  �� Path$ 
MethodName���5y      �� $ 
FontWeight���5z      �� $ 
Foreground�5|      5}   
   5      5�      5�      5�      5�      