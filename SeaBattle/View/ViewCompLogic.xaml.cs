using SeaBattle.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для TestCompLogic.xaml
    /// </summary>
    public partial class ViewCompLogic : Window
    {
        private Fleet.SCell[,] arrField;
        ObservableCollection<Button> playerFieldCellsButtons = new ObservableCollection<Button>();
        internal ViewCompLogic(Fleet.SCell[,] arrField)
        {
            InitializeComponent();            
            this.arrField = arrField;
            GetButtons(arrField);
            this.DataContext = this;
        }



        public ObservableCollection<Button> PlayerFieldCellsButtons
        {
            get { return playerFieldCellsButtons; }
            set { playerFieldCellsButtons = value; }
        }


        internal Fleet.SCell[,] ArrField
        { 
            set 
            { 
                arrField = value;
                GetButtons(arrField);
            } 
        }


        private void GetButtons(Fleet.SCell[,] arrField)
        {
            playerFieldCellsButtons.Clear();
            int fieldSize = arrField.GetLength(0);
            for (int Y = 0; Y < fieldSize; Y++)
                for (int X = 0; X < fieldSize; X++)
                {
                    playerFieldCellsButtons.Add(new Button() { Background = GetButtonContent(arrField[Y, X]) });
                }
        }

        private Brush GetButtonContent(Fleet.SCell cell)
        {
            Brush content = null;
            if (cell.status == Fleet.CellStatus.Clear) content = Brushes.LightBlue;
            else if (cell.status == Fleet.CellStatus.RestAria) content = Brushes.Wheat;
            else if (cell.status == Fleet.CellStatus.Ship) content = Brushes.DarkSlateGray;
            else if (cell.status == Fleet.CellStatus.Miss) content = Brushes.LightSlateGray;
            else if (cell.status == Fleet.CellStatus.Hit) content = Brushes.Red;
            else if (cell.status == Fleet.CellStatus.Destroyed) content = Brushes.Black;

            return content;
        }
    }
}
