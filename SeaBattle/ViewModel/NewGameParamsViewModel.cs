using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SeaBattle.Model;
using SeaBattle.View;

namespace SeaBattle.ViewModel
{
    class NewGameParamsViewModel
    {
        //// ========== Члены класса ==========
        private Fleet player_1;
        private Fleet player_2;
        private bool bIsWinner_1;
        private bool bIsWinner_2;
        private View.NewGameParams vNGP = new View.NewGameParams();
        private Model.GameParams mNGP = new Model.GameParams();
        private bool bBeginNewGame = false;


        //// ========== Конструктор ==========
        internal NewGameParamsViewModel(int fieldSize, GameParams.ShipsDensity shipsDensity, Fleet player_1, Fleet player_2, GameParams.DifficultyLevel diffLevelPlayer_2, string computerLogicsNamePlayer_1, string computerLogicsNamePlayer_2)
        {            
            this.player_1 = player_1;
            this.player_2 = player_2;
            this.bIsWinner_1 = player_1.BIsWinner;
            this.bIsWinner_2 = player_2.BIsWinner;

            mNGP.ComputerLogicsNames = new List<string>
            {
                new ComputerLogicLevel_1(0, null).Name,
                new ComputerLogicLevel_2(0, null).Name,
                new ComputerLogicLevel_3(0, null).Name
            };

            mNGP.ShipDensity = shipsDensity;
            
            if (player_1.BIsHuman && player_2.BIsHuman == false)
                vNGP.rbHum_comp.IsChecked = true;
            else if (player_1.BIsHuman && player_2.BIsHuman)
                vNGP.rbHum_hum.IsChecked = true;
            else if (player_1.BIsHuman == false && player_2.BIsHuman == false)
                vNGP.rbComp_comp.IsChecked = true;
                            
            // !!! Сделать через DataBinding?? !!!
            vNGP.tbName_1.Text = player_1.Name;
            vNGP.tbName_2.Text = player_2.Name;

            vNGP.cbDiffLevel.ItemsSource = mNGP.DiffLevelsList;
            vNGP.cbDiffLevel.SelectedItem = diffLevelPlayer_2;

            vNGP.cbFieldSize.ItemsSource = mNGP.FieldSizesList;
            vNGP.cbFieldSize.SelectedItem = fieldSize;

            vNGP.cbShipsDensity.ItemsSource = mNGP.ShipsDensityList;
            vNGP.cbShipsDensity.SelectedItem = shipsDensity;

            vNGP.cbPlayer1Logic.ItemsSource = mNGP.ComputerLogicsNames;
            vNGP.cbPlayer1Logic.SelectedItem = computerLogicsNamePlayer_1;

            vNGP.cbPlayer2Logic.ItemsSource = mNGP.ComputerLogicsNames;
            vNGP.cbPlayer2Logic.SelectedItem = computerLogicsNamePlayer_2;

            vNGP.ShowDialog();
        }




        //// ========== Свойства ==========
        internal int FieldSize
        { get { return (int)vNGP.cbFieldSize.SelectedItem; } }
        internal GameParams.ShipsDensity ShipsDensity
        { get { return (GameParams.ShipsDensity)vNGP.cbShipsDensity.SelectedItem; } }

        internal string ConputerLogicNamesPlayer_1
        { get { return (string)vNGP.cbPlayer1Logic.SelectedItem; } }

        internal string ConputerLogicNamesPlayer_2
        { get { return (string)vNGP.cbPlayer2Logic.SelectedItem; } }

        internal GameParams.DifficultyLevel DiffLevelPlayer_2
        { get { return (GameParams.DifficultyLevel)vNGP.cbDiffLevel.SelectedItem; } }

        // Экземпляр Player класса Fleet:
        internal Fleet Player_1
        { get { return GetNewPlayer_1(); } }

        // Экземпляр Computer класса Fleet:
        internal Fleet Player_2
        { get { return GetNewPlayer_2(); } }

        // Значение перемменной bBeginNewGame:
        internal bool BBeginNewGame
        { get { return vNGP.BBeginNewGame; } }



        private Fleet GetNewPlayer_1()
        {
            player_1 = new Fleet(FieldSize, ShipsDensity);

            if (vNGP.rbHum_comp.IsChecked == true || vNGP.rbHum_hum.IsChecked == true)
                player_1.BIsHuman = true;
            else player_1.BIsHuman = false;

            if (vNGP.rbComp_comp.IsChecked == true)
                player_1.Name = vNGP.cbPlayer1Logic.Text;
            else player_1.Name = vNGP.tbName_1.Text;

            player_1.BIsWinner = bIsWinner_1;

            bBeginNewGame = vNGP.BBeginNewGame;

            return player_1;
        }


        private Fleet GetNewPlayer_2()
        {
            player_2 = new Fleet(FieldSize, ShipsDensity);

            if (vNGP.rbHum_comp.IsChecked == true || vNGP.rbComp_comp.IsChecked == true)
                player_2.BIsHuman = false;
            else player_2.BIsHuman = true;

            if (vNGP.rbComp_comp.IsChecked == true)
                player_2.Name = vNGP.cbPlayer2Logic.Text;
            else player_2.Name = vNGP.tbName_2.Text;
            player_2.BIsWinner = bIsWinner_2;

            bBeginNewGame = vNGP.BBeginNewGame;

            return player_2;
        }
    }
}
