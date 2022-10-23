using Microsoft.Win32;
using SeaBattle.Model;
using SeaBattle.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SeaBattle.ViewModel
{
    internal class GameStatisticsVewModel
    {
        private Model.GameStatistics player_1Statistics;
        private Model.GameStatistics player_2Statistics;
        private View.GameStatistics vGS;
        private bool bGameStoped = false;

        internal Model.GameStatistics Player_1Statistics
        {
            set
            {
                if (player_1Statistics != null)
                    player_1Statistics = value;
            }
        }

        internal Model.GameStatistics Player_2Statistics
        {
            set
            {
                if (player_2Statistics != null)
                    player_2Statistics = value;
            }
        }

        internal bool BGameStoped                               // Для запрета реагирования поля на нажания мышкой после остановки игры компа с компом.
        {
            get
            {
                if (vGS != null)
                {
                    bGameStoped = vGS.BGameStoped;
                    return bGameStoped;
                }
                else return false;
            }
            set { bGameStoped = value; }
        }

        internal bool CbViewOff
        {
            get
            {
                if (vGS != null && vGS.CbViewOff.IsChecked == true) return true;
                else return false;
            }
        }

        internal View.GameStatistics GS                     // Для сбрасывания при начале новой игры с отличными параметрами.
        {
            get { return vGS; }
            set { vGS = value; }
        }




        //// Методы:
        internal void MaintainingGameStatistic(Fleet.CellStatus status, Fleet player, Fleet fl)
        {
            if (player_1Statistics != null &&
                player_2Statistics != null &&
                player_1Statistics.BIsHuman == false &&
                player_2Statistics.BIsHuman == false)
            {
                if (vGS == null)
                {
                    vGS = new View.GameStatistics();
                    vGS.Show();

                    vGS.tbName1.Text = player_1Statistics.Name.ToString();
                    vGS.tbName2.Text = player_2Statistics.Name.ToString();
                }
                else
                {
                    vGS.tbCG.Text = "Количество сыгранных игр: " + (player_1Statistics.CountWins + player_1Statistics.CountDefeats).ToString();

                    vGS.tbWD1.Text = player_1Statistics.Wins_Defeats.ToString();
                    vGS.tbW1.Text = player_1Statistics.CountWins.ToString();
                    vGS.tbD1.Text = player_1Statistics.CountDefeats.ToString();
                    vGS.tbHM1.Text = player_1Statistics.Hits_Misses.ToString();
                    vGS.tbH1.Text = player_1Statistics.CountHits.ToString();
                    vGS.tbM1.Text = player_1Statistics.CountMisses.ToString();

                    vGS.tbWD2.Text = player_2Statistics.Wins_Defeats.ToString();
                    vGS.tbW2.Text = player_2Statistics.CountWins.ToString();
                    vGS.tbD2.Text = player_2Statistics.CountDefeats.ToString();
                    vGS.tbHM2.Text = player_2Statistics.Hits_Misses.ToString();
                    vGS.tbH2.Text = player_2Statistics.CountHits.ToString();
                    vGS.tbM2.Text = player_2Statistics.CountMisses.ToString();
                }
            }

            if (player_1Statistics == null)
            {
                player_1Statistics = new Model.GameStatistics();
                player_1Statistics.Name = player.Name;
                player_1Statistics.BIsHuman = player.BIsHuman;
            }

            if (player_2Statistics == null)
            {
                player_2Statistics = new Model.GameStatistics();
                player_2Statistics.Name = fl.Name;
                player_2Statistics.BIsHuman = fl.BIsHuman;
            }

            if (player.Name == player_1Statistics.Name)
                switch (status)
                {
                    case Fleet.CellStatus.Miss:
                        player_1Statistics.CountMisses++;
                        break;

                    case Fleet.CellStatus.Hit:
                        goto case Fleet.CellStatus.Destroyed;

                    case Fleet.CellStatus.Destroyed:
                        {
                            player_1Statistics.CountHits++;
                            if (fl.СountOfRemainingShips == 0)
                            {
                                player_1Statistics.CountWins++;
                                player_2Statistics.CountDefeats++;
                                if (player_1Statistics.BIsHuman || player_2Statistics.BIsHuman)
                                    GameStatisticsUtils.SaveStatisticsToXml(new List<Model.GameStatistics> { player_1Statistics, player_2Statistics });
                            }
                            break;
                        }
                }

            if (player.Name == player_2Statistics.Name)
                switch (status)
                {
                    case Fleet.CellStatus.Miss:
                        player_2Statistics.CountMisses++;
                        break;

                    case Fleet.CellStatus.Hit:
                        goto case Fleet.CellStatus.Destroyed;

                    case Fleet.CellStatus.Destroyed:
                        {
                            player_2Statistics.CountHits++;
                            if (fl.СountOfRemainingShips == 0)
                            {
                                player_2Statistics.CountWins++;
                                player_1Statistics.CountDefeats++;
                                if (player_1Statistics.BIsHuman || player_2Statistics.BIsHuman)
                                    GameStatisticsUtils.SaveStatisticsToXml(new List<Model.GameStatistics> { player_1Statistics, player_2Statistics });
                            }
                            break;
                        }
                }
        }
    }
}
