   M S B A M L   `   `   `  ���� X  TXceed.Wpf.Toolkit, Version=3.3.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit   TestPostgreInWpf!   TestPostgreInWpf.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Q
extToolkit:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit       � Title$  
MainWindow��$	��600��$	��900��.���  J� RenderTransformOrigin$ 0.5,0.5&�$��0,-1,0,0q� � RowDefinitions �� $	��81* ��� $
��202* �z�V� .~��� �� ��  �� Angle$ -0.178Q�T� .+��� -    ��Name$ Connect��$,��&Встановити з'єднання��$
��Left=�$��	10,31,0,0q�$	��Top=�$	��144��$��55���� -    ��Name$ Departments��$��553,63,200,77q��� -   $ ChoiseQuery��$
��Left=�$��159,63,0,77q�$	��120��o� $��
0,0,10,137q�.@�m� $��_File��.@�m� $��_Exit��m� -    m�Name$ Set_connection��$��_Setings��.@�m� -   $ Set_Connection_form��$��_Set connection��&   System.Windows.Controls.Calendar  $��697,0,0,403q�$��2��$	��168��$��Bottom=�$
��Left=�$	��195��(  "System.Windows.Controls.DatePicker  -     Name$ 
Start_date��$
��Left=�$��284,63,0,75q�$	��102��  -   $ End_date��$
��Left=�$��391,63,0,75q�$	��102���� -   $ Query1��$��Виконати��$
��Left=�$��572,110,0,10q�$	��120��"   Xceed.Wpf.Toolkit.TimePicker  -	     Name$ 
Time_start��$��284,93,0,45q�$
��Left=�$	��102���   �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]	   Value	 �� 
 �� Path$
 Value� �� Mode$ TwoWay=�  -
   $ Time_end��$��391,93,0,45q�$
��Left=�$	��102��	 �� $
 Value�$ TwoWay=�	� -    	�Name$ Enter��$��Вхід��$
��Left=�$��284,123,0,0q�$	��Top=�$��16��$��38�� d� 	IsChecked$
 TrueY�	� -   $
 Exit��$��
Вихід��$
��Left=�$��284,139,0,0q�$	��Top=�$��16��$��45��$
 TrueY��� -   $ button_test��$��Тест��$
��Left=�$��
10,131,0,0q�$	��Top=�$	��103���� -    ��Name$ textbox��$
��Left=�$��23��$��
498,65,0,0q� �� TextWrapping$
 Wrap=�$��18762��$	��Top=�$��50��&   System.Windows.Controls.DataGrid  -     Name$ 	DataGrid1��$��
10,10,10,0q�$��1���� $��Виберіть дію��$
��Left=�$��
159,32,0,0q�$	��Top=�$	��120���� $!��Початкова дата��$
��Left=�$��
284,32,0,0q�$	��Top=�$	��102���� $��Кінцева дата��$
��Left=�$��
391,32,0,0q�$	��Top=�$	��102���� $��
Номер��$
��Left=�$��
498,31,0,0q�$	��Top=�$��50��