using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    internal class Fleet
    {
        //// =========== Члены класса ===========
        private string name;                                                    // Имя игрока или компьютера.
        private bool bIsHuman;                                                  // Определяет, человек это или компьютер.
        private bool bIsWinner;                                                 // Статус победителя.
        private int fieldSize;                                                  // Размер игрового поля.
        private int koefFieldSize;                                              // Коэффициент, меняющий количество и длину кораблей пропорционально размеру игрового поля.
        private int countOfRemainingShips;                                      // Счётчик оставшихся (не уничноженных) кораблей. Используется для определения конца игры и победителя. 
        GameParams.ShipsDensity shipsDensity;
        private SCell[,] arrField;                                              // Игровое поле (размерность 0 - координата X; размерность 1 -  координата Y).
        private List<SCell[]> ships = new List<SCell[]>();                      // Список кораблей игрока или компьютера.
        private static Random rnd = new Random();                               // Генератор случайных чисел для выбора расположения кораблей.
        private int shipBuildMaxNumAttempts;                                    // Предел по количеству попыток установить корабль на поле

        internal enum CellStatus : int                                          // Возможные состояния ячеек игровых полей и кораблей:
        {
            AlreadyActivated = -1,                                              // -1 - уже активирована.
            Clear,                                                              // 0 - чистая клетка.
            Ship,                                                               // 1 - корабль (не подбит).
            Hit,                                                                // 2 - подбит.
            Destroyed,                                                          // 3 - уничтожен.
            RestAria,                                                           // 4 - запретная зона вокруг корабля.
            Miss,                                                               // 5 - промах.
            Cover                                                               // 6 - визуальное отображение скрыто на время перехода хода.
        };

        internal struct SCell                                                   // Структура клетки игрового поля.
        {
            internal int X;                                                     // Координата X на игровом поле.
            internal int Y;                                                     // Координата Y на игровом поле.
            internal int shipIndex;                                             // Индекс корабля в списке кораблей.
            internal int shipCellIndex;                                         // Индекс башни корабля.
            internal CellStatus status;                                         // Состояние клетки игрового поля.
        }




        //// =========== Конструктор ===========
        internal Fleet(int fieldSize, GameParams.ShipsDensity shipsDensity)
        {
            this.fieldSize = fieldSize;
            this.shipsDensity = shipsDensity;
            this.shipBuildMaxNumAttempts = fieldSize * fieldSize;
            arrField = new SCell[fieldSize, fieldSize];
            ShipsListBuilding();                                                // Вызов метода-конструктора кораблей.
            this.countOfRemainingShips = ships.Count;
        }




        //// =========== Свойства ===========
        // Свойство 1. Получение массива игрового поля:
        internal SCell[,] ArrField
        { get { return arrField; } }


        // Свойство 2. Получение списка кораблей:
        internal List<SCell[]> Ships
        { get { return ships; } }


        // Свойство 3. Получение коэффициента количества и длины кораблей (!!! используется только для консольного интерфейса во время отладки !!!):
        internal int KoefFieldSize
        { get { return koefFieldSize; } }


        // Свойство 4. Получение имени (имя игрока или компьютер):
        internal string Name
        {
            get { return name; }
            set { name = value; }
        }


        // Свойство 5. Получение количества оставшихся (не уничноженных) кораблей.
        internal int СountOfRemainingShips
        { get { return countOfRemainingShips; } }


        // Свойство 6. Статус игрока (человек или компьютер).
        internal bool BIsHuman
        {
            get { return bIsHuman; }
            set { bIsHuman = value; }
        }


        // Свойство 7. Статус победителя.
        internal bool BIsWinner
        {
            get { return bIsWinner; }
            set { bIsWinner = value; }
        }




        //// =========== Методы ===========
        // Метод 1. Создание списка кораблей:
        private void ShipsListBuilding()
        {
            bool bTryBuildAgain  = false;
            do
            {
                koefFieldSize = fieldSize * 4 / 10;                                 // Коэффициент, меняющий количество и длину кораблей пропорционально размеру игрового поля.

                // Корректировка длины и количества кораблей в зависимости от размеров поля:
                koefFieldSize += (int)shipsDensity;
                int quantuty = 1;                                                   // Задаёт лимит по количеству кораблей определённой длины.
                switch (fieldSize)
                {
                    case 2:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { koefFieldSize +=2; } break;
                                case GameParams.ShipsDensity.Normal: { koefFieldSize++; } break;
                                default: break;
                            }
                            break;
                        }
                    case 3:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { koefFieldSize++; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 2; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 3; koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 4:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 2; koefFieldSize++; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 3; } break;
                                default: break;
                            }
                            break;
                        }
                    case 5:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 2; } break; 
                                case GameParams.ShipsDensity.High: { quantuty = 2; koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 6:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { koefFieldSize++; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 2; } break;
                                default: break;
                            }
                            break;
                        }
                    case 7:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { koefFieldSize++; } break;
                                case GameParams.ShipsDensity.Normal: { koefFieldSize++; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 2; } break;
                                default: break;
                            }
                            break;
                        }
                    case 8:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 2; } break;
                                default: break;
                            }
                            break;
                        }                        
                    case 9:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { koefFieldSize++; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 2; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 3; koefFieldSize--; } break;
                                default:break;
                            }
                            break;
                        }
                    case 10:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.High: { quantuty = 2; koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 11:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Normal: { quantuty = 3; koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 12:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 2; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 2; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 3; koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 13:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 2; koefFieldSize--; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 2; koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 14:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Normal: { quantuty = 3; koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 15:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { koefFieldSize--; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 3; koefFieldSize -=2; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 3; koefFieldSize -=2; } break;
                                default: break;
                            }
                            break;
                        }
                    case 16:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 3; koefFieldSize -=2; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 2; koefFieldSize--; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 2; koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 17:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 2; koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 18:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 2; koefFieldSize -=2; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 3; koefFieldSize -= 2; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 3; koefFieldSize -= 2; } break;
                                default: break;
                            }
                            break;
                        }
                    case 19:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { koefFieldSize--; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 2; koefFieldSize--; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 2; koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 20:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 3; koefFieldSize -=3; } break;
                                case GameParams.ShipsDensity.Normal: { koefFieldSize--; } break;
                                case GameParams.ShipsDensity.High: { koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 21:                        
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 3; koefFieldSize -=3; } break;
                                case GameParams.ShipsDensity.Normal: { koefFieldSize--; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 3; koefFieldSize -=2; } break;
                                default: break;
                            }
                            break;
                        }
                    case 22:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 2; koefFieldSize -= 2; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 3; koefFieldSize -= 2; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 3; koefFieldSize -= 2; } break;
                                default: break;
                            }
                            break;
                        }
                    case 23:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { koefFieldSize -=2; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 2; koefFieldSize -= 2; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 2; koefFieldSize -=2; } break;
                                default: break;
                            }
                            break;
                        }
                    case 24:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { koefFieldSize -=2; } break;
                                case GameParams.ShipsDensity.Normal: { koefFieldSize--; } break;
                                case GameParams.ShipsDensity.High: { koefFieldSize--; } break;
                                default: break;
                            }
                            break;
                        }
                    case 25:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 3; koefFieldSize -= 4; } break;
                                case GameParams.ShipsDensity.Normal: { koefFieldSize -=2; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 3; koefFieldSize -= 3; } break;
                                default: break;
                            }
                            break;
                        }
                    case 26:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 3; koefFieldSize -= 4; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 3;  koefFieldSize -= 3; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 2; koefFieldSize -= 2; } break;
                                default: break;
                            }
                            break;
                        }
                    case 27:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { quantuty = 2; koefFieldSize -= 3; } break;
                                case GameParams.ShipsDensity.Normal: { quantuty = 3; koefFieldSize -= 3; } break;
                                case GameParams.ShipsDensity.High: { quantuty = 2; koefFieldSize -= 2; } break;
                                default: break;
                            }
                            break;
                        }
                    case 28:
                        {
                            switch (shipsDensity)
                            {
                                case GameParams.ShipsDensity.Low: { koefFieldSize -= 3; } break;
                                case GameParams.ShipsDensity.Normal: { koefFieldSize -= 2; } break;
                                case GameParams.ShipsDensity.High: { koefFieldSize -= 2; } break;
                                default: break;
                            }
                            break;
                        }
                }
                                                                                   
                int shipIndex = 0;                                              // Индекс корабля в списке кораблей.
                for (; koefFieldSize >= 1; koefFieldSize--, quantuty++)
                {
                    for (int c = 0; c < quantuty; c++, shipIndex++)             // c - cчётчик кораблей определённой длины.
                    {
                        SCell[] arrShip = new SCell[koefFieldSize];             // Создание нового корабля определённой длины.
                        if (ShipBuilder(arrShip, shipIndex))                    // Инициализация координат корабля случайным образом с помощью функции ShipBuilder.
                        {
                            ships.Add(arrShip);
                            bTryBuildAgain = false;
                        }                                                       // Запись корабля в новую ячейку массива кораблей.
                        else
                        {
                            arrField = new SCell[fieldSize, fieldSize];
                            ships.Clear();
                            bTryBuildAgain = true;
                            break;
                        }
                    }
                    if (bTryBuildAgain) break;
                }
            } while (bTryBuildAgain);
        }


        // Метод 2: Инициализация отдельных кораблей:
        private bool ShipBuilder(SCell[] arrShip, int shipIndex)
        {
            int shipLength = arrShip.Length;                                    // Длина корабля.
            bool NotOverlay;                                                    // Используется для проверки наложения нового корабля на предыдущий.
            int countAttempt = 0;
            do                                                                  // Для второго и более кораблей попытка нарисовать повторится, если новый корабль попадёт на уже нарисованный или на запретную зону вокруг корабля.
            {
                NotOverlay = true;
                int X = rnd.Next(0, fieldSize);                                 // Координата X, с которой начинает рисоваться корабль.
                int Y = rnd.Next(0, fieldSize);                                 // Координата Y, с которой начинает рисоваться корабль.

                // Выбор направления для рисования корабля:
                int DrawDirectRnd;
                do { DrawDirectRnd = rnd.Next(-2, 3); } while (DrawDirectRnd == 0); // знак "-" - рисуем влево или вверх; знак "+" - рисуем вправо или вниз; 1 - рисуем вертикально; 2 - рисуем горизонтально; 0 исключается.
                int v_h = Math.Abs(DrawDirectRnd);                              // Расположение: вертикально/горизонтиально.
                int ul_dr = Math.Sign(DrawDirectRnd);                           // Рисуем вверх/влево или вниз/вправо.


                // Инициализация многоклеточных кораблей:  
                if (shipLength > 1)
                {
                    // Вертикальное расположение корабля:
                    if (v_h == 1)
                    {
                        if (ul_dr < 0)
                        {
                            if (X - shipLength + 1 < 0) X = 0;
                            else X = X - shipLength + 1;
                        }
                        else if (X - 1 + shipLength > fieldSize - 1) X = fieldSize - shipLength;

                        for (int i = 0; i < shipLength; i++)
                        {
                            arrShip[i].X = X;
                            arrShip[i].Y = Y;                                   // Координаты Y всех сегментов корабля равны при горизонтальном расположении корабля.
                            X += 1;
                        }
                    }
                    // Горизонтальное расположение корабля:
                    else if (v_h == 2)
                    {
                        if (ul_dr < 0)
                        {
                            if (Y - shipLength + 1 < 0) Y = 0;
                            else Y = Y - shipLength + 1;
                        }
                        else if (Y - 1 + shipLength > fieldSize - 1) Y = fieldSize - shipLength;

                        for (int i = 0; i < shipLength; i++)
                        {
                            arrShip[i].X = X;                                   // Координаты X всех сегментов корабля равны при горизонтальном расположении корабля.
                            arrShip[i].Y = Y;
                            Y += 1;
                        }
                    }
                }

                // Инициализация одноклеточных кораблей:
                else
                {
                    arrShip[0].X = X;
                    arrShip[0].Y = Y;
                }

                // Проверка попадания корабля на ранее нарисованный корабль, или на запретную зону вокруг уже нарисованных кораблей:
                for (int i = 0; i < shipLength; i++)
                {
                    if (arrField[arrShip[i].Y, arrShip[i].X].status == CellStatus.Ship ||
                        arrField[arrShip[i].Y, arrShip[i].X].status == CellStatus.RestAria)
                    { NotOverlay = false; countAttempt++; break; }
                }
                if (NotOverlay == false) continue;                                              // Если корабль попал на уже занятые клетки или на запретную зону, то делается попытка нарисовать его заново.

                // Инициализация корабля, как нового (не подорванного) и запись корабля на игровое поле:
                for (int i = 0; i < shipLength; i++)
                {
                    arrShip[i].status = CellStatus.Ship;
                    arrField[arrShip[i].Y, arrShip[i].X].status = CellStatus.Ship;              // Запись в клетки игрового поля информации о том, что здесь расположен корабль.
                    arrField[arrShip[i].Y, arrShip[i].X].shipIndex = shipIndex;                 // Запись в клетки игрового поля информации о индексе корабля в списке кораблей.
                    arrField[arrShip[i].Y, arrShip[i].X].shipCellIndex = i;                     // Запись в клетки игрового поля информации о индексе клетки корабля в массиве-корабле.
                }                
            } while (NotOverlay == false && countAttempt < shipBuildMaxNumAttempts);

            
            if (countAttempt == shipBuildMaxNumAttempts)
                return false;

            // Формирования вокруг корабля запретной зоны, в которой не должно быть других кораблей:
            // Проврека того, чтобы угловые примыкающие клетки, не заходили за границы игрового поля:
            if (arrShip[0].Y > 0 && arrShip[shipLength - 1].X < fieldSize - 1)                                                  // Если сегмент корабля не находится на верхней и не на правой границе игрового поля, ...
                arrField[arrShip[0].Y - 1, arrShip[shipLength - 1].X + 1].status = CellStatus.RestAria;                         // ... то соседней диагональной верхней правой клетке поля присваивается статус запретной зоны.
            
            if (arrShip[shipLength - 1].Y < fieldSize - 1 && arrShip[shipLength - 1].X < fieldSize - 1)                         // Если сегмент корабля не находится на нижней и не на правой границе игрового поля, ...
                arrField[arrShip[shipLength - 1].Y + 1, arrShip[shipLength - 1].X + 1].status = CellStatus.RestAria;            // ... то соседней диагональной нижней правой клетке поля присваивается статус запретной зоны.
            
            if (arrShip[shipLength - 1].Y < fieldSize - 1 && arrShip[0].X > 0)                                                  // Если сегмент корабля не находится на нижней и не на левой границе игрового поля, ...
                arrField[arrShip[shipLength - 1].Y + 1, arrShip[0].X - 1].status = CellStatus.RestAria;                         // ... то соседней диагональной нижней левой клетке поля присваивается статус запретной зоны.
            
            if (arrShip[0].Y > 0 && arrShip[0].X > 0)                                                                           // Если сегмент корабля не находится на верхней и не на левой границе игрового поля, ...
                arrField[arrShip[0].Y - 1, arrShip[0].X - 1].status = CellStatus.RestAria;                                      // ... то соседней диагональной верхней левой клетке поля присваивается статус запретной зоны.


            // Проврека того, чтобы клетки, примыкающие к бортам, носу и концу корабля, не заходили за границы поля и не попадали на соседние сегменты данного корабля: 
            for (int i = 0; i < shipLength; i++)
            {
                if ((arrShip[i].Y > 0) && (arrField[arrShip[i].Y - 1, arrShip[i].X].status != CellStatus.Ship))                // Если сегмент корабля не находится на верхней границе игрового поля и соседняя верхняя клетка поля не является корпусом этого корабля, ...
                    arrField[arrShip[i].Y - 1, arrShip[i].X].status = CellStatus.RestAria;                                     // то данной клетке присваивается статус запретной зоны.
                
                if ((arrShip[i].X < fieldSize - 1) && (arrField[arrShip[i].Y, arrShip[i].X + 1].status != CellStatus.Ship))    // Если сегмент корабля не находится на правой границе игрового поля и соседняя правая клетка поля не является корпусом этого корабля, ...
                    arrField[arrShip[i].Y, arrShip[i].X + 1].status = CellStatus.RestAria;                                     // то данной клетке присваивается статус запретной зоны.
                
                if ((arrShip[i].Y < fieldSize - 1) && (arrField[arrShip[i].Y + 1, arrShip[i].X].status != CellStatus.Ship))    // Если сегмент корабля не находится на нижней границе игрового поля и соседняя нижняя клетка поля не является корпусом этого корабля, ...
                    arrField[arrShip[i].Y + 1, arrShip[i].X].status = CellStatus.RestAria;                                     // то данной клетке присваивается статус запретной зоны.
                
                if ((arrShip[i].X > 0) && (arrField[arrShip[i].Y, arrShip[i].X - 1].status != CellStatus.Ship))                // Если сегмент корабля не находится на левой границе игрового поля и соседняя левая клетка поля не является корпусом этого корабля, ...
                    arrField[arrShip[i].Y, arrShip[i].X - 1].status = CellStatus.RestAria;                                     // то данной клетке присваивается статус запретной зоны.
            }
            return true;
        }


        // Метод 3. Изменение отдельных ячеек игровых полей и кораблей в процессе игры:
        // (производятся изменения в Model и передаются данные для изменений в View)
        internal SCell[] ChangeIndividualCells(int X, int Y)
        {
            // Случай с промахом:
            if (arrField[Y, X].status == CellStatus.Clear || arrField[Y, X].status == CellStatus.RestAria)
            {
                arrField[Y, X].status = CellStatus.Miss;
                return new SCell[]
                {
                    new SCell
                    {
                        X = X,
                        Y = Y,
                        status = CellStatus.Miss
                    }
                };
            }

            // Случай с попаданием в корабль:
            else if (arrField[Y, X].status == CellStatus.Ship)
            {
                int shipIndex = arrField[Y, X].shipIndex;
                int shipCellIndex = arrField[Y, X].shipCellIndex;
                int shipLength = ships[shipIndex].Length;

                ships[shipIndex][shipCellIndex].status = CellStatus.Hit;
                arrField[Y, X].status = CellStatus.Hit;                

                // Если корабль не уничножен, то возвращяем данные о повреждённом сегменте корабля:                
                for (int i = 0; i < shipLength; i++)
                    if (ships[shipIndex][i].status != CellStatus.Hit)
                        return new SCell[]
                        {
                            new SCell
                            {
                                X = X,
                                Y = Y,
                                shipIndex = shipIndex,
                                shipCellIndex = shipCellIndex,
                                status = CellStatus.Hit
                            }
                        };

                // Если корабль уничножен, то возвращяем данные об уничтожении корабля:
                SCell[] arrChangedCells = new SCell[shipLength];
                for (int i = 0; i < shipLength; i++)
                {
                    ships[shipIndex][i].status = CellStatus.Destroyed;
                    arrField[ships[shipIndex][i].Y, ships[shipIndex][i].X].status = CellStatus.Destroyed;
                                        
                    arrChangedCells[i].X = ships[shipIndex][i].X;
                    arrChangedCells[i].Y = ships[shipIndex][i].Y;
                    arrChangedCells[i].shipIndex = ships[shipIndex][i].shipIndex;
                    arrChangedCells[i].shipCellIndex = ships[shipIndex][i].shipCellIndex;
                    arrChangedCells[i].status = CellStatus.Destroyed;
                }
                countOfRemainingShips--;
                return arrChangedCells;
            }

            // Случай с кликом на уже открытую клетку:
            else return new SCell[] { new SCell {status = CellStatus.AlreadyActivated } };
        }
    }
}
