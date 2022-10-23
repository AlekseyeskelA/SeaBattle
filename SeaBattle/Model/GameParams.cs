using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    internal class GameParams
    {
        private List<int> fieldSizesList = new List<int>();
        private List<DifficultyLevel> diffLevelsList = new List<DifficultyLevel>();
        private List<ShipsDensity> shipsDensityList = new List<ShipsDensity>();
        private List<string> computerLogicsNames = new List<string>();
        private DifficultyLevel diffLevelPlayer_2;
        private ShipsDensity shipsDensity;
        private string computerLogicsNamePlayer_1;
        private string computerLogicsNamePlayer_2;
        
        internal enum DifficultyLevel : int
        {
            Easy = 0,
            Normal = 1,
            Advanced = 2
        }
        internal enum ShipsDensity : int
        {
            Low = -1,
            Normal = 0,
            High = 1
        }

        internal GameParams()
        {
            for (int i = 2; i <= 28; i++)
                fieldSizesList.Add(i);

            for (DifficultyLevel i = DifficultyLevel.Easy; i <= DifficultyLevel.Advanced; i++)
                diffLevelsList.Add(i);

            for (ShipsDensity i = ShipsDensity.Low; i <= ShipsDensity.High; i++)
                shipsDensityList.Add(i);
        }

        internal DifficultyLevel DiffLevelPlayer_2
        {
            get { return diffLevelPlayer_2; }
            set { diffLevelPlayer_2 = value; }
        }

        internal ShipsDensity ShipDensity
        {
            get { return shipsDensity; }
            set { shipsDensity = value; }
        }

        internal string ComputerLogicsNamePlayer_1
        {
            get { return computerLogicsNamePlayer_1; }
            set { computerLogicsNamePlayer_1 = value; }
        }

        internal string ComputerLogicsNamePlayer_2
        {
            get { return computerLogicsNamePlayer_2; }
            set { computerLogicsNamePlayer_2 = value; }
        }

        internal List<int> FieldSizesList
        {
            get { return fieldSizesList; } 
        }

        internal List<DifficultyLevel> DiffLevelsList
        {
            get { return diffLevelsList; }
        }

        internal List<string> ComputerLogicsNames
        {
            get { return computerLogicsNames; }
            set { computerLogicsNames = value; }
        }

        internal List<ShipsDensity> ShipsDensityList
        {
            get { return shipsDensityList; }
            set { shipsDensityList = value; }
        }
    }
}