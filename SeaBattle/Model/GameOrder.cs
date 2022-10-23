using SeaBattle.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    internal class GameOrder
    {
        //// ========== Члены класса ==========
        private Move moveIs;

        internal enum Move : int                                            // Возможные состояния ячеек игровых полей и кораблей:
        {
            Player_1 = -1,                                                  // -1 - ход первого игрока.
            Nobodys,                                                        // 0 - ничей ход;
            Player_2                                                        // 1 - ход второго игрока.
        };


        //// ========== Свойства ==========
        internal Move MoveIs
        {
            get { return moveIs; }
            set { moveIs = value; }
        }


        //// ========== Методы ==========
        internal void GetRandomTurn()
        {
            Random rnd = new Random();
            int whoTurn = rnd.Next(0, 2);
            if (whoTurn == 0) MoveIs = Move.Player_1;
            else  MoveIs = Move.Player_2;
        }

        internal void ChangeMove()
        {
            MoveIs = (Move)((-1) * (int)moveIs);
        }

        internal static bool CheckGameOver(Fleet fl, Fleet player_1, Fleet player_2)
        {
            if (fl.СountOfRemainingShips == 0)
            {
                if (fl.Name == player_2.Name)
                {
                    player_1.BIsWinner = true;
                    player_2.BIsWinner = false;
                }
                else if (fl.Name == player_1.Name)
                {
                    player_2.BIsWinner = true;
                    player_1.BIsWinner = false;
                }
                return true;
            }
            else return false;
        }
    }
}
