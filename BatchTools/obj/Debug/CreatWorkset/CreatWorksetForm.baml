   M S B A M L   `   `   `  ����E  ABatchTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FFETOOLSFFETOOLS  5   	       FFETOOLS.CreatWorksetForm   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:FFETOOLS     5   	   -   5        $
��this��5        � Title$  创建工作集��6	   $	��180��6#   $	��200��60    � 
ResizeMode$ NoResize=�6<    � WindowStartupLocation$ CenterScreen=�6S   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $	��70* �6   �� 5      $	��40* �6    � RowDefinitions 5   
   �� 5      $
��100* �6   �� 5      $	��45* �6   .+�5   
   �� $��工作集类型��6   $��0��6"   $��0��62   $��2��6?   $��6,5,6,5q�6S   
�������.��5      �  5      �� 5      $��* �6'   �� 5      $��* �6'   .+�5      	� -   5         	�Name$ SubMain��5      $��子项车间��6/   $��5,5,5,5q�6>   $��0��6O   $��0��6\   $��Center=�6l   	� 5      -   5        $
 Main��5      $��主项车间��6,   $��5,5,5,5q�6;   $��0��6L   $��1��6Y   $��Center=�6i   5      5      �� 5   
   -   5         ��Name$ OkButton��5      $��确定��6#   $��10,10,10,10q�60   $��1��6E   $��1��6R   ��5      �� 5        �� 
TargetType" ��6"   .R�5!      �� #��z��6   $��1q�6Q    �� CornerRadius$ 3��6e   ���	#FFDDDDDD._�5#      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5$      5%      5'      5)      5*      