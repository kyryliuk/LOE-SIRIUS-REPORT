   M S B A M L   `   `   `  ����  SiriusReport&     TestPostgreInWpf.EmployersDT_Set   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   X TXceed.Wpf.Toolkit, Version=3.3.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	     � Title$  EmployersDT_Set��5   	   $	��400��6!   $	��600��6.      ActiveBrush2#���   6:   .��5      � .+�5   
   �� -   5         ��Name$
 Save��5        ButtonGreen#���  6   $��Зберегти��6F   $
��Left=�6Y   $��
445,51,0,0q�6t   $	��Top=�6�   $	��119��6�   $��30��6�   "  Xceed.Wpf.Toolkit.TimePicker  5	   
   -   5          Name$ Time_startE��5	      
��� ���$��2q�6E   �  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   DefaultValue$ 08:00X�6[     TimePickerStyle1#���  6p   $��
321,10,0,0q�6�   $
��Left=�6�   $	��119��6�   $��30��6�   $	��Top=�6     Value 6�   �� *Value+ �� Mode$ TwoWay=�6    5
   
   -   5        $ 	Time_EndE��5
      
��� ���$��2q�6B   $ 08:00X�6X   #���  6m   $��
445,10,0,0q�6�   $��31��6�   $	��Top=�6�   $
��Left=�6�   $	��119��6   6�   �� *Value+$ TwoWay=�6  �� 5   
   -   5         ��Name$ Find_tableN��5      $��	10,10,0,0q�6'    �� TextWrapping$
 Wrap=�6:   $�� ��6N   $��30��6|   $
��Left=�6�   $	��Top=�6�   $	��306��6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ View_Employers��5      $��10,133,22,10q�6+   �� 5   
   -   5        $ Add_Employer��5        
ButtonBlue#���  6'   $
��Left=�6M   $��Додати��6h   $��
321,51,0,0q�6y   $	��Top=�6�   $	��119��6�   $��30��6�   6�   �� 5   
   -   5        $ DeleteButton��5        ButtonOrange#���  6(   $
��Left=�6O   $��Видалити��6j   $��
321,98,0,0q�6}   $	��Top=�6�   $	��119��6�   $��30��6�   6�   �� 5   
   -   5        	 ��Name$	 listboxIntellishell��5      $
��Left=�6/   $��77��6J   $��	10,51,0,0q�6V   $	��Top=�6i   $	��306��6�   �� 5   
   -	   5        $ 
ExitButton��5      $��
Вихід��6%   #���  66   $
��Left=�6]   $��
445,98,0,0q�6x   $	��Top=�6�   $	��119��6�   $��30��6�   6�   5      5      