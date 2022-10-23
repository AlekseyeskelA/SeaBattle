using SeaBattle.ViewModel;
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

namespace SeaBattle.View
{
    /// <summary>
    /// Логика взаимодействия для GameStatistics.xaml
    /// </summary>
    public partial class GameStatistics : Window
    {
        bool bGameStoped = false;

        public GameStatistics()
        {
            InitializeComponent();
        }

        internal bool BGameStoped
        { get { return bGameStoped; } }

        internal CheckBox CbViewOff
        { get { return cbViewOff; } }


        private void Window_Closed(object sender, EventArgs e)
        {
            bGameStoped = true;
        }

        private void btnStopGame_Click(object sender, RoutedEventArgs e)
        {
            bGameStoped = true;
        }
    }
}
