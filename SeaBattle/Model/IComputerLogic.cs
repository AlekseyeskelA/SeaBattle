using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    interface IComputerLogic
    {
        void ChangeComputerVision(Fleet.SCell[] cells);
        public Fleet.SCell GetNextCellForTurn();   
    }
}
