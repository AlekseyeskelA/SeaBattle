using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    class ComputerLogicLevel_2
    {
        //// =========== Члены класса ===========
        private string name = "ComputerLogicLevel_2";
        int fieldSize;
        private static Random rnd = new Random();
        private Fleet.SCell[,] arrFieldCompVision;
        private Dictionary<int, int> undetectedShip;
        private List<Fleet.SCell> potentialCellsForHit = new List<Fleet.SCell>();
        private List<List<Fleet.SCell>> hittedShips = new List<List<Fleet.SCell>>();

        internal enum Direction : int
        {
            Up = 0,
            Down = 1,
            Right = 2,
            Left = 3
        }



        //// =========== Конструктор ===========
        public ComputerLogicLevel_2(int fieldSize, Dictionary<int, int> shipInfo)
        {
            this.fieldSize = fieldSize;                                         // Размер игрового поля.
            this.undetectedShip = shipInfo;
            arrFieldCompVision = new Fleet.SCell[fieldSize, fieldSize];         // Создается чистое поле. По умолчанию при создании все значения 0, соответствующие значению Clear.

            for (int Y = 0; Y < fieldSize; Y++)
                for (int X = 0; X < fieldSize; X++)
                    potentialCellsForHit.Add(new Fleet.SCell { X = X, Y = Y, status = Fleet.CellStatus.Clear });
        }




        //// =========== Свойства ===========
        internal Fleet.SCell[,] ArrFieldCompVision
        { get { return arrFieldCompVision; } }

        internal string Name
        { get { return name; } }




        //// =========== Методы ===========
        // Метод 1. Изменение в представлении компьютера измененииях на поле иргы:
        internal void ChangeComputerVision(Fleet.SCell[] cells)
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
        internal Fleet.SCell GetNextCellForTurn()
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

                int maxLimRnd = 0;
                List<Direction> neignboringCellcToHit = new List<Direction>();
                Dictionary<Direction, int> freeSpaces = new Dictionary<Direction, int>()
                {
                    { Direction.Up, 0 },
                    { Direction.Right, 0 },
                    { Direction.Down, 0 },
                    { Direction.Left, 0 },
                };
                int shipLength = hittedShips[indexMaxLen].Count;

                // Если подбита только одна башня:
                if (shipLength == 1)
                {
                    // Исследуем количество свободных клеток с разных сторон от подбитой башни:

                    bool bLookLeft = true;
                    bool bLookRight = true;
                    bool bLookUp = true;
                    bool bLookDown = true;
                    int c = 1;
                    do
                    {
                        if (bLookUp &&
                            hittedShips[indexMaxLen][0].Y - c >= 0 &&
                            arrFieldCompVision[hittedShips[indexMaxLen][0].Y - c, hittedShips[indexMaxLen][0].X].status == Fleet.CellStatus.Clear)
                            freeSpaces[Direction.Up]++;
                        else bLookUp = false;

                        if (bLookRight &&
                            hittedShips[indexMaxLen][shipLength - 1].X + c < fieldSize &&
                            arrFieldCompVision[hittedShips[indexMaxLen][0].Y, hittedShips[indexMaxLen][shipLength - 1].X + c].status == Fleet.CellStatus.Clear)
                            freeSpaces[Direction.Right]++;
                        else bLookRight = false;

                        if (bLookDown &&
                            hittedShips[indexMaxLen][shipLength - 1].Y + c < fieldSize &&
                            arrFieldCompVision[hittedShips[indexMaxLen][shipLength - 1].Y + c, hittedShips[indexMaxLen][0].X].status == Fleet.CellStatus.Clear)
                            freeSpaces[Direction.Down]++;
                        else bLookDown = false;

                        if (bLookLeft &&
                            hittedShips[indexMaxLen][0].X - c >= 0 &&
                            arrFieldCompVision[hittedShips[indexMaxLen][0].Y, hittedShips[indexMaxLen][0].X - c].status == Fleet.CellStatus.Clear)
                            freeSpaces[Direction.Left]++;
                        else bLookLeft = false;

                        c++;
                    } while (bLookLeft || bLookRight || bLookUp || bLookDown);

                    if (undetectedShip.Keys.Min() > 1)
                    {
                        int minAfterOneLength = undetectedShip.Keys.Max();
                        foreach (KeyValuePair<int, int> shipCount in undetectedShip)
                            if (minAfterOneLength > shipCount.Key)
                                minAfterOneLength = shipCount.Key;

                        if (minAfterOneLength > freeSpaces[Direction.Up] + freeSpaces[Direction.Down])
                        {
                            freeSpaces.Remove(Direction.Up);
                            freeSpaces.Remove(Direction.Down);
                        }
                        if (minAfterOneLength > freeSpaces[Direction.Left] + freeSpaces[Direction.Right])
                        {
                            freeSpaces.Remove(Direction.Left);
                            freeSpaces.Remove(Direction.Right);
                        }
                    }

                    int maxValue = freeSpaces.Values.Max();
                    int rndDirection = rnd.Next(1, maxValue + 1);

                    for (Direction dir = Direction.Up; dir <= Direction.Left; dir++)
                        if (freeSpaces.ContainsKey(dir))
                            if (freeSpaces[dir] < rndDirection) continue;
                            //if (freeSpaces[dir] < freeSpaces.Values.Max()) continue;
                            else neignboringCellcToHit.Add(dir);
                    maxLimRnd = neignboringCellcToHit.Count;
                }

                // Если подбито больше одной башни и корабль расположен горизонтально:
                else if (hittedShips[indexMaxLen][0].Y == hittedShips[indexMaxLen][1].Y)
                {
                    bool bLookLeft = true;
                    bool bLookRight = true;
                    int c = 1;
                    do
                    {
                        if (bLookRight &&
                            hittedShips[indexMaxLen][shipLength - 1].X + c < fieldSize &&
                            arrFieldCompVision[hittedShips[indexMaxLen][0].Y, hittedShips[indexMaxLen][shipLength - 1].X + c].status == Fleet.CellStatus.Clear)
                            freeSpaces[Direction.Right]++;
                        else bLookRight = false;

                        if (bLookLeft &&
                            hittedShips[indexMaxLen][0].X - c >= 0 &&
                            arrFieldCompVision[hittedShips[indexMaxLen][0].Y, hittedShips[indexMaxLen][0].X - c].status == Fleet.CellStatus.Clear)
                            freeSpaces[Direction.Left]++;
                        else bLookLeft = false;

                        c++;
                    } while (bLookLeft || bLookRight);

                    int maxValue = freeSpaces.Values.Max();
                    int rndDirection = rnd.Next(1, maxValue + 1);

                    for (Direction dir = Direction.Right; dir <= Direction.Left; dir++)
                        if (freeSpaces[dir] < rndDirection) continue;
                    //if (freeSpaces[dir] < freeSpaces.Values.Max()) continue;
                        else neignboringCellcToHit.Add(dir);
                    maxLimRnd = neignboringCellcToHit.Count;
                }

                // Если подбито больше одной башни и корабль расположен вертикально:
                else if (hittedShips[indexMaxLen][0].X == hittedShips[indexMaxLen][1].X)
                {
                    bool bLookUp = true;
                    bool bLookDown = true;
                    int c = 1;
                    do
                    {
                        if (bLookUp &&
                            hittedShips[indexMaxLen][0].Y - c >= 0 &&
                            arrFieldCompVision[hittedShips[indexMaxLen][0].Y - c, hittedShips[indexMaxLen][0].X].status == Fleet.CellStatus.Clear)
                            freeSpaces[Direction.Up]++;
                        else bLookUp = false;

                        if (bLookDown &&
                            hittedShips[indexMaxLen][shipLength - 1].Y + c < fieldSize &&
                            arrFieldCompVision[hittedShips[indexMaxLen][shipLength - 1].Y + c, hittedShips[indexMaxLen][0].X].status == Fleet.CellStatus.Clear)
                            freeSpaces[Direction.Down]++;
                        else bLookDown = false;

                        c++;
                    } while (bLookUp || bLookDown);

                    int maxValue = freeSpaces.Values.Max();
                    int rndDirection = rnd.Next(1, maxValue + 1);

                    for (Direction dir = Direction.Up; dir <= Direction.Down; dir++)
                        if (freeSpaces[dir] < rndDirection) continue;
                        //if (freeSpaces[dir] < freeSpaces.Values.Max()) continue;
                        else neignboringCellcToHit.Add(dir);
                    maxLimRnd = neignboringCellcToHit.Count;
                }

                int rndCell = rnd.Next(0, maxLimRnd);
                switch (neignboringCellcToHit[rndCell])
                {
                    case Direction.Up:
                        return new Fleet.SCell { X = hittedShips[indexMaxLen][0].X, Y = hittedShips[indexMaxLen][0].Y - 1 };

                    case Direction.Right:
                        return new Fleet.SCell { X = hittedShips[indexMaxLen][shipLength - 1].X + 1, Y = hittedShips[indexMaxLen][0].Y };

                    case Direction.Down:
                        return new Fleet.SCell { X = hittedShips[indexMaxLen][0].X, Y = hittedShips[indexMaxLen][shipLength - 1].Y + 1 };

                    case Direction.Left:
                        return new Fleet.SCell { X = hittedShips[indexMaxLen][0].X - 1, Y = hittedShips[indexMaxLen][0].Y };
                }
            }

            else
            {
                GetListOfPossibleTurn();
                int rndCell = rnd.Next(0, potentialCellsForHit.Count);
                return potentialCellsForHit[rndCell];
            }
            
            return new Fleet.SCell { X = -1, Y = -1 };            // Для отслеживания ошибки.
        }


        // Метод 5. Формирование списка возможных клеток для дальнейшего хода.
        private void GetListOfPossibleTurn()
        {
            potentialCellsForHit.Clear();
            for (int Y = 0; Y < fieldSize; Y++)
                for (int X = 0; X < fieldSize; X++)
                    if (arrFieldCompVision[Y, X].status != Fleet.CellStatus.Destroyed &&
                        arrFieldCompVision[Y, X].status != Fleet.CellStatus.Hit &&
                        arrFieldCompVision[Y, X].status != Fleet.CellStatus.RestAria &&
                        arrFieldCompVision[Y, X].status != Fleet.CellStatus.Miss)
                        potentialCellsForHit.Add(new Fleet.SCell { X = X, Y = Y });
        }
    }
}
