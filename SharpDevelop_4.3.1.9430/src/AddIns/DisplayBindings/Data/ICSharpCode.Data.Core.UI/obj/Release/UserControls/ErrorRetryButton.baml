   M S B A M L   `   `   `  ����   ICSharpCode.Data.Core.UI<    6ICSharpCode.Data.Core.UI.UserControls.ErrorRetryButton   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     >�Name$
  This��$y�	Collapsed=� � 	Resources  �� MergedDictionaries �� Source��  �� $$ ../Images/ImageDictionary.xaml?�%    .��� -    �Name$	 grd��.+��� -    ��Name$ imgError��   
ErrorImage#��[  $��16��$��16��$y�	Collapsed=� � ToolTip �� *Exception.Message+ �� ElementName$
 This���� -   $ imgExclamation��  ExclamationImage#��[ $��16��$��16��$y�	Collapsed=� �� *Exception.Message+$
 This���� -   $ 
imgRefresh��  RefreshImage#��[ $��16��$��16�� J� Opacity$ 0Q�	 � Cursor$
	 Hand�� �� *Exception.Message+$
 This��
 � Triggers
 :�  :� 
SourceName$	 grd�� :� RoutedEvent$ Grid.MouseEnter��.[��� .���� .��W�  �� 
TargetName$ imgError�� �� TargetProperty$ Opacity�	 W� To$ 0X� g� Duration$ 0:0:0.5D�W� $ 
imgRefresh��$ Opacity�$ 1X�$ 0:0:0.5D�:� $	 grd��$ Grid.MouseEnter��.[��� .���� .��W� $ imgExclamation��$ Opacity�$ 0X�$ 0:0:0.5D�W� $ 
imgRefresh��$ Opacity�$ 1X�$ 0:0:0.5D�:� $	 grd��$ Grid.MouseLeave��.[��� .���� .��W� $ imgError��$ Opacity�$ 1X�$ 0:0:0.5D�W� $ 
imgRefresh��$ Opacity�$ 0X�$ 0:0:0.5D�:� $	 grd��$ Grid.MouseLeave��.[��� .���� .��W� $ imgExclamation��$ Opacity�$ 1X�$ 0:0:0.5D�W� $ 
imgRefresh��$ Opacity�$ 0X�$ 0:0:0.5D�