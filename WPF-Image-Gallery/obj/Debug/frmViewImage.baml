   M S B A M L   `   `   `  ����L  HWPF-Image-Gallery, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:WPF_Image_GalleryWPF_Image_Gallery  5   	   $    WPF_Image_Gallery.frmViewImage   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:WPF_Image_Gallery     5   	     � WindowState$  	Maximized=�5   	    � Title$ Gallery��5	   	   $	��450��6   $	��800��6&    � 	Resources 5       �� 
TargetType �� Property �� Value �� 
TargetType��%�   '��       �� 5   
   " ��6   .�5      ��  �@�OverridesDefaultStyle . 66   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� $��Center=�6+   $��Center=�5   '   5      5      5      5      .��5      �  � RowDefinitions 5   
   �� 5      �� 5      $��50 �6   .+�5"   
   �� -   5        $	��img��5"      $��Uniform=�6   $��Center=�6-   $��Center=�6J   $��10q�6e    J� RenderTransformOrigin$ 0.5,0.5&�6�   z�5#      �� 5$      -   5        
ImageScale��	 �� ScaleX$	 1Q�5$   5   
 �� ScaleY$
 1Q�6@   5&      �� 5)   
   $��1��6   ._�5+      �  � ColumnDefinitions 5,      �� 5-      �� 5.      .+�52      �� $��0��6    �� Orientation$ 
Horizontal=�6-   $��Right=�6F   $��0,0,20,0q�6b   .+�53      �� -   5        $��	btnZoomIn��53       � Cursor$
 Hand��6F   $��0,0,15,0q�6T   .��54      �� $��/Images/zoom in.png��6    $��25��6=   55      �� 56      -   5        $��
btnZoomOut��56      $
 Hand��6H   .��57      �� $��/Images/zoom out.png��6    $��25��6>   58      59      �� 5<      $��1��6   $
 Hand��6-   $ 
Horizontal=�6;   $
��Left=�6T   .+�5=      �� -   5        $��btnPrevious��5=      .��5>      �� $��0,0,10,0q�6    $��/Images/previous.png��62   $��35��6P   5?      �� 5@      -   5        $��btnNext��5@      $
 Hand��6B   .��5A      �� $��10,0,0,0q�6    $��/Images/next.png��62   $��35��6L   5B      5C      5D      5F      5G      5H      