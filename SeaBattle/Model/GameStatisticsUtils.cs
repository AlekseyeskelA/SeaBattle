using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SeaBattle.Model
{
    static internal class GameStatisticsUtils
    {
        // Метод 1. Получечние имени файла и папки:
        private static string GetFileLOcation()
        {
            string strFaleName = "gamestathum.xml";
            string strDirectoryAndFile = Directory.GetCurrentDirectory() + "\\" + strFaleName;
            return strDirectoryAndFile;
        }


        // Метод 2. Сохранение в формате XML:
        static internal void SaveStatisticsToXml(List<GameStatistics> playersGS)
        {
            List<GameStatistics> players = new List<GameStatistics>();            

            if (File.Exists(GetFileLOcation()))
            {
                players = OpenStatisticsFromXml();                
                int countP = players.Count;
                int countPGS = playersGS.Count;                
                for (int i = 0; i < countPGS; i++)
                {       
                    for (int j = 0; j < countP; j++)
                    {
                        if (players[j].Name == playersGS[i].Name &&
                            players[j].BIsHuman == playersGS[i].BIsHuman)           // !!! И расмер поля тоже !!!
                        {
                            players[j].CountWins += playersGS[i].CountWins;
                            players[j].CountDefeats += playersGS[i].CountDefeats;
                            players[j].CountHits += playersGS[i].CountHits;
                            players[j].CountMisses += playersGS[i].CountMisses;
                            playersGS.RemoveAt(i); countPGS--; i--;
                            break;
                        }
                    }
                }
            }

            foreach (GameStatistics playerGS in playersGS)
                players.Add(new GameStatistics
                    {
                        Name = playerGS.Name,
                        CountWins = playerGS.CountWins,
                        CountDefeats = playerGS.CountDefeats,
                        CountHits = playerGS.CountHits,
                        CountMisses = playerGS.CountMisses,
                        BIsHuman = playerGS.BIsHuman
                    });

            XElement x = new XElement("GameStatistics",
                from player in players
                select new XElement("Player",
                    new XElement("Имя", player.Name),
                    new XElement("Победы", player.CountWins),
                    new XElement("Поражения", player.CountDefeats),
                    new XElement("Попадания", player.CountHits),
                    new XElement("Промахи", player.CountMisses),
                    new XElement("Чел._комп.", player.BIsHuman)));

            File.WriteAllText(GetFileLOcation(), x.ToString()); //!!! сделать Catch, если файл занят!!! 
        }


        // Метод 3. Открытие в формате XML:
        static private List<GameStatistics> OpenStatisticsFromXml()
        {
            string strDirectoryAndFile = GetFileLOcation();
            XElement x = XElement.Parse(File.ReadAllText(strDirectoryAndFile));

            return (from e in x.Elements()
                    select new GameStatistics()
                    {
                        Name = e.Element("Имя").Value,
                        CountWins = uint.Parse(e.Element("Победы").Value),
                        CountDefeats = uint.Parse(e.Element("Поражения").Value),
                        CountHits = uint.Parse(e.Element("Попадания").Value),
                        CountMisses = uint.Parse(e.Element("Промахи").Value),
                        BIsHuman = bool.Parse(e.Element("Чел._комп.").Value)
                    }).ToList();
        }
    }
}
