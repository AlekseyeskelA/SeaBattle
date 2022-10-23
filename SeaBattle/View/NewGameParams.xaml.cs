using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SeaBattle.ViewModel;

namespace SeaBattle.View
{
    /// <summary>
    /// Логика взаимодействия для NewGameParams.xaml
    /// </summary>
    public partial class NewGameParams : Window
    {
        // ========== Члены класса ==========
        private bool bBeginNewGame = false;                                 // Переменная, регулирующия начало игры в зависимости от нажатия кнопки "Начать игру" в данном окне.



        // ========== Конструктор ==========
        public NewGameParams()
        {
            InitializeComponent();
        }


        // ========== Свойства ==========
        // Свойство 1. Клик на копку "Начать игру":
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bBeginNewGame = true;
            this.Close();                                                   // !!! Запретить закрытие и акцепт при одинаковых именах и больших длинах имём !!!
        }



        // Свойство 3. Значение перемменной bBeginNewGame:
        internal bool BBeginNewGame
        { get { return bBeginNewGame; } }

        private void rbHum_comp_Checked(object sender, RoutedEventArgs e)
        {
            tbName_1.IsEnabled = true;
            tbName_2.IsEnabled = false;
            NGPWindow.Width = 300;
            cbDiffLevel.IsEnabled = true;
        }


        private void rbHum_hum_Checked(object sender, RoutedEventArgs e)
        {
            tbName_1.IsEnabled = true;
            tbName_2.IsEnabled = true;
            cbDiffLevel.IsEnabled = false;
            NGPWindow.Width = 300;
        }


        private void rbComp_comp_Checked(object sender, RoutedEventArgs e)
        {
            tbName_1.IsEnabled = false;
            tbName_2.IsEnabled = false;
            cbDiffLevel.IsEnabled = false;
            NGPWindow.Width = 500;
        }
    }
}