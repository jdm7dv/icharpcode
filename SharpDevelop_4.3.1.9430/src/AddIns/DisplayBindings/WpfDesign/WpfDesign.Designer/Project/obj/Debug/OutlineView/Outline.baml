   M S B A M L   `   `   `  ����h  dICSharpCode.WpfDesign.Designer, Version=4.3.1.9430, Culture=neutral, PublicKeyToken=f829da5c02be14eeg8clr-namespace:ICSharpCode.WpfDesign.Designer.OutlineView*ICSharpCode.WpfDesign.Designer.OutlineView  5      8    2ICSharpCode.WpfDesign.Designer.OutlineView.Outline   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      LDefault8clr-namespace:ICSharpCode.WpfDesign.Designer.OutlineView     5      -   5          >�Name$
  root��5       � 	Resources 5       �� DataType<   6ICSharpCode.WpfDesign.Designer.OutlineView.OutlineNode �� ItemsSource9   3ICSharpCode.WpfDesign.Designer.OutlineView.IconItem   Icon   Text��%�   (��       5	      *, +)�� 6   "  6    5
      �� *
Children+.R�5        $ ../Images/Tag.png��6    5      �� *Name+5      5      .��5      @   :ICSharpCode.WpfDesign.Designer.OutlineView.OutlineTreeView  =   7ICSharpCode.WpfDesign.Designer.OutlineView.DragTreeView   Root 6   �� *Root+ �� ElementName$
 root����5      �� 5       �� 
TargetTypeA   ;ICSharpCode.WpfDesign.Designer.OutlineView.DragTreeViewItem"  6   .�5      �� 	 �� Property	 �@ 
IsSelected
 �� Value
 5      �� *
IsSelected+�� 5      	 �@Q�
IsExpanded
 5      �� *
IsExpanded+ �� Mode$ TwoWay=�5      5      5      