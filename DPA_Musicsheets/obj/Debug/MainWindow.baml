   M S B A M L   `   `   `  ����P  LPSAMWPFControlLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\Bclr-namespace:PSAMWPFControlLibrary;assembly=PSAMWPFControlLibraryPSAMWPFControlLibrary  5   	   J FDPA_Musicsheets, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DPA_MusicsheetsDPA_Musicsheets 5   	       DPA_Musicsheets.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ]PSAMWPFControlBclr-namespace:PSAMWPFControlLibrary;assembly=PSAMWPFControlLibrary     5   	   /localclr-namespace:DPA_Musicsheets     5   	   -   5          � Title$  
MainWindow��5	   	   $	��500��6   $
��1050��6)   .��5
      � -   5         �Name$ MainGrid��5
      .+�5   
   �� -   5         ��Name$ txt_MidiFilePath��5      $
��Left=�6,   $��23��6G   $��	90,10,0,0q�6S    �� TextWrapping$
 Wrap=�6f   $	��Top=�6z   $	��233��6�   �� 5   
   -   5         ��Name$ btnPlay��5      $
��Play��6"   $
��Left=�61   $��
353,11,0,0q�6L   $	��Top=�6`   $��75��6x   6�   �� 5   
   -   5        $ btnOpen��5      $
��Open��6"   $
��Left=�61   $��	10,11,0,0q�6L   $	��Top=�6_   $��75��6w   6�   �� 5   
   -   5        $ btn_Stop��5      $
��Stop��6#   $
��Left=�62   $��
433,11,0,0q�6M   $	��Top=�6a   $��75��6y   6�   �� 5   
   -   5         ��Name$ tabCtrl_MidiContent��5      $
��Left=�62   $	��248��6M   $��	10,38,0,0q�6Z   $	��Top=�6m   $	��498��6�   ��6�   �� ��5      �� 5       �� 
TargetType" ��6   .�5      ��  �� Property � �� �� Value 5      �� 5      	 �� Path$	 	TrackName�6&   5      5      
 �� ContentTemplate
 5      �� 5      .R�5      �� ��6   �� *
Messages+��5      �� 5      .R�5   "   �� ��6(   �� *.+5       5"      5#      5%      �� 5'   
   -   5        $ editor��5'      $��Right=�6"   $	��275��6>   $��0,11,10.2,0q�6K   $
 Wrap=�6`   $	��Top=�6t   $	��510��6�   �� 5+   
   -	   5        $ btn_Save��5+      $
��Save��6#   $��Right=�62   $��0,11,10.2,0q�6N   $	��Top=�6c   $��75��6{   6�   �� 5,   
   -
   5        $ btn_Undo��5,      $
��Undo��6#   $��Right=�62   $��0,35,10.2,0q�6N   $	��Top=�6c   $��75��6{   6�   �� 5-   
   -   5        $ btn_Redo��5-      $
��Redo��6#   $��Right=�62   $��0,59,10.2,0q�6N   $	��Top=�6c   $��75��6{   6�   �� 5/   
   $��Hidden=�6   $
��Auto=�6<   $��0,291,0.4,0.4q�6a   .��50      ,   &PSAMWPFControlLibrary.IncipitViewerWPF  -   5          Name$ staff��50   I   $
��Left=�6.   $
��9999��6X   $	��128��6e   51      52      53      54      