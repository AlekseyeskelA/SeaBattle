using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    internal class ComputerLogicLevel_1/* : IComputerLogic*/
    {
        //// =========== Члены класса ===========
        private string name = "ComputerLogicLevel_1";
        private int fieldSize;
        private static Random rnd = new Random();                               // Генератор случайных чисел для определения следующего хода (клетки на поле).
        private Fleet.SCell[,] arrFieldCompVision;
        private Dictionary<int, int> undetectedShip;
        private List<Fleet.SCell> potentialCellsForHit = new List<Fleet.SCell>();
        private List<List<Fleet.SCell>> hittedShips = new List<List<Fleet.SCell>>();  
        private int errorProbability = 10;



        //// =========== Конструктор ===========
        public ComputerLogicLevel_1(int fieldSize, Dictionary<int, int> shipInfo)
        {
            this.fieldSize = fieldSize;                                         // Размер игрового поля.
            this.undetectedShip = shipInfo;
            arrFieldCompVision = new Fleet.SCell[fieldSize, fieldSize];         // Создается чистое поле. По умолчанию при создании все значения 0, соответствующие значению Clear.

            for (int Y = 0; Y < fieldSize; Y++)
                for (int X = 0; X < fieldSize; X++)
                    potentialCellsForHit.Add(new Fleet.SCell { X = X, Y = Y, status = Fleet.CellStatus.Clear });
        }




        //// =========== Свойства ===========
        internal Fleet.SCell[,] ArrFieldCompVision // Для отладки
        { get { return arrFieldCompVision; } }

        internal string Name
        { get { return name; } }




        //// =========== Методы ===========
        // Метод 1. Корректировка в представлении компьютера об измененииях на поле иргы:
        internal void /*IComputerLogic.*/ChangeComputerVision(Fleet.SCell[] cells)
        {
            if (cells[0].status == Fleet.CellStatus.Hit || cells[0].status == Fleet.CellStatus.Destroyed)
                ChangeHitShipsList(cells);

            ChangeFieldCompVision(cells);
        }


        // Метод 2. Формирование представления об уничтоженных раненых кораблях:
        private void ChangeHitShipsList(Fleet.SCell[] cells)
        {
            bool bNewShip = true;
            int countDS = hittedShips.Count;
            for (int i = 0; i < countDS; i++)
            {
                int lengthDS = hittedShips[i].Count;
                for (int j = 0; j < lengthDS; j++)
                {
                    int lengthC = cells.Length;
                    for (int k = 0; k < lengthC; k++)
                    {
                        bool bMustInsetBefore = false;
                        if ((cells[k].X == hittedShips[i][j].X && cells[k].Y == hittedShips[i][j].Y - 1) || // Если клетка расположена рядом с уже обнаруженным кораблём, то ...
                            (cells[k].Y == hittedShips[i][j].Y && cells[k].X == hittedShips[i][j].X - 1))
                        {
                            bNewShip = false;
                            bMustInsetBefore = true;
                        }
                        else if ((cells[k].X == hittedShips[i][j].X && cells[k].Y == hittedShips[i][j].Y + 1) ||
                                (cells[k].Y == hittedShips[i][j].Y && cells[k].X == hittedShips[i][j].X + 1))
                        {
                            bNewShip = false;
                            bMustInsetBefore = false;
                        }

                        if (bNewShip == false)
                            if (cells[k].status == Fleet.CellStatus.Destroyed)              // ... а если клетка принадлежит подбитому кораблю ...
                            {
                                if (undetectedShip[lengthC] > 1)                            // ... и корабли с данной длинной ещё есть, то ...
                                {
                                    undetectedShip[lengthC]--;                              // ... уменьшаем количество кораблей в словаре.
                                }
                                else undetectedShip.Remove(lengthC);                        // Если корабль данной длины последний, то удаляем данный корабль из словаря.

                                hittedShips.RemoveAt(i);                                    // Во всех случаях, если корабль подбит, то удаляем его клеточки Hit из списка обнаруженных кораблей.
                                return;
                            }
                            else if (cells[k].status == Fleet.CellStatus.Hit)
                            {
                                if (bMustInsetBefore)
                                {
                                    hittedShips[i].Insert(0, cells[k]);                     // ... прибавляем клетку к подбитому кораблю вначале, ...
                                }
                                else if (bMustInsetBefore == false)
                                {
                                    hittedShips[i].Add(cells[k]);                           // ... прибавляем клетку к подбитому кораблю вконце, ...
                                }
                                return;
                            }
                    }
                }
            }
            if (bNewShip && cells[0].status != Fleet.CellStatus.Destroyed) hittedShips.Add(new List<Fleet.SCell>() { cells[0] });  // Если попадание не было рядом с другими кораблями, то добавляем новый корабль в список кораблей.
        }


        // Метод 3. Коректировка представления об игровом поле противника:
        private void ChangeFieldCompVision(Fleet.SCell[] cells)
        {
            // Меняем состояние активированных клаток поля:
            int length = cells.Length;
            for (int i = 0; i < length; i++)
                arrFieldCompVision[cells[i].Y, cells[i].X].status = cells[i].status;

            // Меняем состояние соседних угловых клеток в случае попадания и в случае уничтожения корабля: 
            if (cells[0].status == Fleet.CellStatus.Hit || cells[0].status == Fleet.CellStatus.Destroyed)
            {
                if (cells[0].Y > 0 && cells[cells.Length - 1].X < fieldSize - 1)                                                            // Если сегмент корабля не находится на верхней и не на правой границе игрового поля, ...
                    arrFieldCompVision[cells[0].Y - 1, cells[cells.Length - 1].X + 1].status = Fleet.CellStatus.RestAria;                   // ... то соседней диагональной верхней правой клетке поля присваивается статус запретной зоны.
                if (cells[cells.Length - 1].Y < fieldSize - 1 && cells[cells.Length - 1].X < fieldSize - 1)                                 // Если сегмент корабля не находится на нижней и не на правой границе игрового поля, ...
                    arrFieldCompVision[cells[cells.Length - 1].Y + 1, cells[cells.Length - 1].X + 1].status = Fleet.CellStatus.RestAria;    // ... то соседней диагональной нижней правой клетке поля присваивается статус запретной зоны.
                if (cells[cells.Length - 1].Y < fieldSize - 1 && cells[0].X > 0)                                                            // Если сегмент корабля не находится на нижней и не на левой границе игрового поля, ...
                    arrFieldCompVision[cells[cells.Length - 1].Y + 1, cells[0].X - 1].status = Fleet.CellStatus.RestAria;                   // ... то соседней диагональной нижней левой клетке поля присваивается статус запретной зоны.
                if (cells[0].Y > 0 && cells[0].X > 0)                                                                                       // Если сегмент корабля не находится на верхней и не на левой границе игрового поля, ...
                    arrFieldCompVision[cells[0].Y - 1, cells[0].X - 1].status = Fleet.CellStatus.RestAria;                                  // ... то соседней диагональной верхней левой клетке поля присваивается статус запретной зоны.


                // Меняем состояние примыкающих клеток в случае, если корабль многоклеточный: 
                if (cells[0].status == Fleet.CellStatus.Destroyed)
                {
                    int lastIndex;
                    int compIndex = 0;
                    lastIndex = length - 1;
                    if (cells.Length > 1) compIndex = 1;

                    // Для вертикального расположения корабля:
                    if (cells[0].X == cells[compIndex].X)
                    {
                        if (cells[0].Y > 0)
                            arrFieldCompVision[cells[0].Y - 1, cells[0].X].status = Fleet.CellStatus.RestAria;
                        if (cells[lastIndex].Y < fieldSize - 1)
                            arrFieldCompVision[cells[lastIndex].Y + 1, cells[0].X].status = Fleet.CellStatus.RestAria;

                        for (int i = 0; i <= lastIndex; i++)
                        {
                            if (cells[0].X > 0)
                                arrFieldCompVision[cells[i].Y, cells[i].X - 1].status = Fleet.CellStatus.RestAria;
                            if (cells[0].X < fieldSize - 1)
                                arrFieldCompVision[cells[i].Y, cells[i].X + 1].status = Fleet.CellStatus.RestAria;
                        }
                        return;
                    }

                    // Для горизонтального расположения корабля:
                    else if (cells[0].Y == cells[compIndex].Y)
                    {
                        if (cells[0].X > 0)
                            arrFieldCompVision[cells[0].Y, cells[0].X - 1].status = Fleet.CellStatus.RestAria;
                        if (cells[lastIndex].X < fieldSize - 1)
                            arrFieldCompVision[cells[0].Y, cells[lastIndex].X + 1].status = Fleet.CellStatus.RestAria;

                        for (int i = 0; i <= lastIndex; i++)
                        {
                            if (cells[0].Y > 0)
                                arrFieldCompVision[cells[i].Y - 1, cells[i].X].status = Fleet.CellStatus.RestAria;
                            if (cells[0].Y < fieldSize - 1)
                                arrFieldCompVision[cells[i].Y + 1, cells[i].X].status = Fleet.CellStatus.RestAria;
                        }
                    }
                }
            }
        }



        // Метод 4. Определение клетки для следующего хода (Вызывается из GameWindowViewModel):
        internal Fleet.SCell /*IComputerLogic.*/GetNextCellForTurn()
        {
            // Если раненные корабли есть, то продолжаем бить по ним:
            if (hittedShips.Count != 0)
            {
                int maxLength = 0;
                int indexMaxLen = 0;
                for (int j = 0; j < hittedShips.Count; j++)                //определяем корабль с максимальными повреждениями.
                    if (maxLength < hittedShips[j].Count)
                    {
                        maxLength = hittedShips[j].Count;
                        indexMaxLen = j;
                    }

                int i = 0;
                int rndCell;               
                List<int> neignboringCellcToHit = new List<int>();
                int shipLength = hittedShips[indexMaxLen].Count;

                // Если подбита только одна башня:
                if (shipLength == 1)
                {
                    neignboringCellcToHit = new List<int>() { 0, 1, 2, 3 };
                    i = 3;
                }

                // Если подбито больше одной башни и корабль расположен горизонтально:
                else if (hittedShips[indexMaxLen][0].Y == hittedShips[indexMaxLen][1].Y)
                {
                    neignboringCellcToHit = new List<int>() { 1, 3 };
                    i = 1;
                }

                // Если подбито больше одной башни и корабль расположен вертикально:
                else if (hittedShips[indexMaxLen][0].X == hittedShips[indexMaxLen][1].X)
                {
                    neignboringCellcToHit = new List<int>() { 0, 2 };
                    i = 1;
                }

                for (; i >= 0; i--)
                {
                    rndCell = rnd.Next(0, i + 1);
                    switch (neignboringCellcToHit[rndCell])
                    {
                        case 0:
                            {
                                if (hittedShips[indexMaxLen][0].Y > 0 &&
                                    arrFieldCompVision[hittedShips[indexMaxLen][0].Y - 1, hittedShips[indexMaxLen][0].X].status != Fleet.CellStatus.RestAria &&
                                    arrFieldCompVision[hittedShips[indexMaxLen][0].Y - 1, hittedShips[indexMaxLen][0].X].status != Fleet.CellStatus.Miss)         // !!! ВОЗМОЖНО, ЭТО НЕ ОБЯЗАТЕЛЬНОЕ УСЛОВИЕ, ТАК КАК ЕСТЬ ПЕРЕНАЗНАЧЕНИЕ ЯЧЕЕЕК В ChangeFieldCompVision() !!!
                                    return new Fleet.SCell { X = hittedShips[indexMaxLen][0].X, Y = hittedShips[indexMaxLen][0].Y - 1};
                                else
                                {
                                    neignboringCellcToHit.RemoveAt(rndCell);
                                    continue;
                                }
                            }
                        case 1:
                            {
                                if (hittedShips[indexMaxLen][shipLength - 1].X < fieldSize - 1 &&
                                    arrFieldCompVision[hittedShips[indexMaxLen][0].Y, hittedShips[indexMaxLen][shipLength - 1].X + 1].status != Fleet.CellStatus.RestAria &&
                                    arrFieldCompVision[hittedShips[indexMaxLen][0].Y, hittedShips[indexMaxLen][shipLength - 1].X + 1].status != Fleet.CellStatus.Miss)
                                    return new Fleet.SCell { X = hittedShips[indexMaxLen][shipLength - 1].X + 1, Y = hittedShips[indexMaxLen][0].Y };
                                else
                                {
                                    neignboringCellcToHit.RemoveAt(rndCell);
                                    continue;
                                }
                            }
                        case 2:
                            {
                                if (hittedShips[indexMaxLen][shipLength - 1].Y < fieldSize - 1 &&
                                    arrFieldCompVision[hittedShips[indexMaxLen][shipLength - 1].Y + 1, hittedShips[indexMaxLen][0].X].status != Fleet.CellStatus.RestAria &&
                                    arrFieldCompVision[hittedShips[indexMaxLen][shipLength - 1].Y + 1, hittedShips[indexMaxLen][0].X].status != Fleet.CellStatus.Miss)
                                    return new Fleet.SCell { X = hittedShips[indexMaxLen][0].X, Y = hittedShips[indexMaxLen][shipLength - 1].Y + 1 };
                                else
                                {
                                    neignboringCellcToHit.RemoveAt(rndCell);
                                    continue;
                                }
                            }
                        case 3:
                            {
                                if (hittedShips[indexMaxLen][0].X > 0 &&
                                    arrFieldCompVision[hittedShips[indexMaxLen][0].Y, hittedShips[indexMaxLen][0].X - 1].status != Fleet.CellStatus.RestAria &&
                                    arrFieldCompVision[hittedShips[indexMaxLen][0].Y, hittedShips[indexMaxLen][0].X - 1].status != Fleet.CellStatus.Miss)
                                    return new Fleet.SCell { X = hittedShips[indexMaxLen][0].X - 1, Y = hittedShips[indexMaxLen][0].Y };
                                else
                                {
                                    neignboringCellcToHit.RemoveAt(rndCell);
                                    continue;
                                }
                            }
                    }                    
                }
            }

            // Если раненных кораблей нет, то бьём в случайную свободную клетку:
            else
            {
                GetListOfPossibleTurn();
                int rndCell = rnd.Next(0, potentialCellsForHit.Count);
                return potentialCellsForHit[rndCell];
            }
            return new Fleet.SCell { X = -1, Y = -1 };            // Чтобы функция не ругалась.
        }

        // Метод 5. Формирование списка возможных клеток для дальнейшего хода.
        private void GetListOfPossibleTurn()
        {
            potentialCellsForHit.Clear();
            for (int Y = 0; Y < fieldSize; Y++)
                for (int X = 0; X < fieldSize; X++)
                    if (arrFieldCompVision[Y, X].status != Fleet.CellStatus.Destroyed &&
                        arrFieldCompVision[Y, X].status != Fleet.CellStatus.Hit &&
                        (arrFieldCompVision[Y, X].status != Fleet.CellStatus.RestAria || rnd.Next(0, errorProbability) == 0) &&
                        arrFieldCompVision[Y, X].status != Fleet.CellStatus.Miss)
                        potentialCellsForHit.Add(new Fleet.SCell { X = X, Y = Y });
        }
    }
}
