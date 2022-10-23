using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaBattle.View;

namespace SeaBattle.ViewModel
{
    class MoveTransitionViewModel
    {
        private MoveTransition vMT;
        internal bool IntarfaceTurnOff
        {
            set
            {  
                if (value)
                {
                    vMT = new MoveTransition();
                    vMT.tbMT.Text = "Промах!";
                    vMT.btnMT.Content = "Передать ход противнику";                    
                    vMT.ShowDialog();
                }
                else if (!value)
                {
                    vMT = new MoveTransition();
                    vMT.tbMT.Text = "Противник промахнулся";
                    vMT.btnMT.Content = "Перейти к ходу";
                    vMT.ShowDialog();
                }
            }
        }
    }
}
