using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    class GameStatistics
    {
        private string name;
        private bool bIsHuman;
        private uint countHits = 0;
        private uint countMisses = 0;
        private uint countWins = 0;
        private uint countDefeats = 0;        
        private double hits_Misses = 0;
        private double wins_Defeats = 0;
        private uint countGames = 0;

        internal string Name
        {
            get { return name; }
            set { name = value; }
        }

        internal bool BIsHuman
        {
            get { return bIsHuman; }
            set { bIsHuman = value; }
        }        
        internal uint CountHits
        {
            get { return countHits; }
            set { countHits = value; }
        }
        internal uint CountMisses
        {
            get { return countMisses; }
            set { countMisses = value; }
        }
        internal uint CountWins
        {
            get { return countWins; }
            set { countWins = value; }
        }
        internal uint CountDefeats
        {
            get { return countDefeats; }
            set { countDefeats= value; }
        }
        internal double Hits_Misses
        { get { return GetHits_Misses(); } }

        internal double Wins_Defeats
        { get { return GetWins_Defeats(); } }

        internal uint CountGames
        { get { return GetCountGames(); } }

        private uint GetCountGames()
        {
            countGames = countWins + countDefeats;
            return countGames;
        }

        private double GetHits_Misses()
        {
            if (countMisses == 0) hits_Misses = countHits;
            hits_Misses = Math.Round((double)countHits / countMisses, 5);
            return hits_Misses;
        }

        private double GetWins_Defeats()
        {
            if (countDefeats == 0) wins_Defeats = countWins;
            else wins_Defeats = Math.Round((double)countWins / countDefeats, 5);

            return wins_Defeats;
        }        
    }
}
