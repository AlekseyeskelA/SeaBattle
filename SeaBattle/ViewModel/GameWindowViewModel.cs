using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaBattle.Model;
using SeaBattle.View;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using static SeaBattle.Model.GameOrder;
using static SeaBattle.Model.GameParams;
using System.ComponentModel;

namespace SeaBattle.ViewModel
{
    internal class GameWindowViewModel : INotifyPropertyChanged
    {
        //// ========== Члены класса ==========
        private int fieldSize;                                                          // Размер игрового поля.
        private int timeDelay;                                                          // Задержка времени при ходе компьютера.
        private Fleet player_1;                                                         // Игрок 1.
        private Fleet player_2;                                                         // Игрок 2.
        private ComputerLogicLevel_1 ComputerLogicLevel_1_Player_1;                     // Логика компьютера. Уровень 1.
        private ComputerLogicLevel_2 ComputerLogicLevel_2_Player_1;                     // Логика компьютера. Уровень 2.
        private ComputerLogicLevel_3 ComputerLogicLevel_3_Player_1;                     // Логика компьютера. Уровень 3.
        private ComputerLogicLevel_1 ComputerLogicLevel_1_Player_2;                     // Логика компьютера. Уровень 1.
        private ComputerLogicLevel_2 ComputerLogicLevel_2_Player_2;                     // Логика компьютера. Уровень 2.
        private ComputerLogicLevel_3 ComputerLogicLevel_3_Player_2;                     // Логика компьютера. Уровень 3.
        private GameOrder gameOrder = new GameOrder();                                  // Порядок иргы.
        private GameParams gameParams = new GameParams();
        private BaseCommand newGameCommand;                                             // Команда на открытие окна настроек новой игры.
        private BaseCommand clickOnFieldCellCommand;                                    // Команда - выбор клетки на поле.
        private ObservableCollection<Button> player_1FieldCellsButtons = new ObservableCollection<Button>();  // Коллекция для отображения флота игрока 1 в оконном интерфейсе.
        private ObservableCollection<Button> player_2FieldCellsButtons = new ObservableCollection<Button>();  // Коллекция для отображения флота игрока 2 в оконном интерфейсе.
        //private ViewCompLogic vCL_1;                                                    // Для отладки. Отображение представления компьютера 1.
        //private ViewCompLogic vCL_2;                                                    // Для отладки. Отображение представления компьютера 2.        
        private GameStatisticsVewModel vmGS = new GameStatisticsVewModel();
        private NewGameParamsViewModel vmNGP;
        int shipsCellsCount;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //// ========== Конструктор ==========
        internal GameWindowViewModel()
        {
            fieldSize = 10;                                                     // Стандартное поле при запуске ургы.
            GetNewDefaltGame(fieldSize, GameParams.ShipsDensity.Normal);        // Старт новой игры по умолчанию при запуске.
            PrepareGameInterface();                                             // Подготовка оконного интерфейса.
        }




        //// ========== Свойства ==========
        // Свойство 1. Команда Новая игра:
        public BaseCommand NewGameCommand
        {
            get
            {
                if (newGameCommand != null)                                     // Проверяем, существует ли данная команда.
                    return newGameCommand;                                      // Если не существует, то создаём её.
                else
                {
                                                                                // Action<object> Executive - обязательный аргумент в конструкторе. Это обработчик, который говорит, что делать, когда команда будет вызвана.
                    Action<object> Executive = o => GetNewGame();               // Первый обработчик. По правилам класса BaseCommand обработчик должен принимать только один аргумент (в данном случеа ничего передавать не надо).
                                                                                // o - параметр команды. Его можно использовать, чтобы что-нибудь передавать.
                                                                                // Обработчик Func здесь не нужен, так как новую игру можно начать всегда.
                    newGameCommand = new BaseCommand(Executive);                // Если поле saveCommand ещё не заполнено, тогда нужно создать эту команду.                                                                            
                    return newGameCommand;                                      // Возвращаем созданную команду.
                }
            }
        }

        // Свойство 2. Команда Клик по ячейке игрового поля:
        public BaseCommand ClickOnFieldCellCommand
        {
            get
            {
                if (clickOnFieldCellCommand != null)
                    return clickOnFieldCellCommand;
                else
                {
                    Action<object> Executive = o =>
                    {
                        if (((gameOrder.MoveIs == Move.Player_1 && (o as Fleet) == player_2) ||      // Противодействие тому, чтобы игрок ударил по своему полю.
                            gameOrder.MoveIs == Move.Player_2 && (o as Fleet) == player_1)
                            && vmGS.BGameStoped == false)                                           // чтобы после остановки игры двух компьютеров нельзя было вручную кликнуть по полю и вызвать ход
                            ChangeViewAndModel(o as Fleet);
                    };
                    clickOnFieldCellCommand = new BaseCommand(Executive);
                    return clickOnFieldCellCommand;
                }
            }
        }


        //// Свойство 3. Изменения в ObservableCollection игрока 1 для оконного интерфейса:
        public ObservableCollection<Button> Player_1FieldCellsButtons
        { get { return player_1FieldCellsButtons; } }


        //// Свойство 4. Изменения в ObservableCollection игрока 2 для оконного интерфейса:
        public ObservableCollection<Button> Player_2FieldCellsButtons
        { get { return player_2FieldCellsButtons; } }


        //// Свойство 5. Класс Игрок 1
        public Fleet Player_1
        {
            get { return player_1; }
            //set { player_1 = value; }
        }

        //// Свойство 6. Класс Игрок 2
        public Fleet Player_2
        {
            get { return player_2; }
            set { player_2 = value; }
        }

        //// Свойство 7. Количество клеток кораблей (для отладки):
        //public int ShipsCellCount
        //{
        //    set
        //    { 
        //        shipsCellsCount = value;
        //        OnPropertyChanged();
        //    }
        //    get
        //    {
        //        return shipsCellsCount;

        //    }
        //}



        //// ========== Методы ==========
        //// Метод 1. Запуск новой игра с заданными параметрами:
        private void GetNewDefaltGame(int fieldSize, GameParams.ShipsDensity shipsDensity)
        {
            if (player_1 == null)
            {
                player_1 = new Fleet(fieldSize, shipsDensity);                              // Создаём экземпляр класса Player_1.
                player_1.BIsHuman = true;                                                   // По умолчанию - человек.
                player_1.Name = "Unknown player";

                // По умолчанию - компьютер.
                Dictionary<int, int> shipsInfo_1 = GetShipListInfoForComputerLogic(player_1.Ships);     // Информация о количестве и длине кораблей (для случая игры с компьютером).
                ComputerLogicLevel_2_Player_2 = new ComputerLogicLevel_2(fieldSize, shipsInfo_1);       // Инициализация логики компьютера.
                                                                                                        //computerLogic.ChangeComputerVision(null);
                player_2 = new Fleet(fieldSize, shipsDensity);                                          // Создаём экземпляр класса Player_2.
                player_2.BIsHuman = false;
                player_2.Name = ComputerLogicLevel_2_Player_2.Name;

                gameOrder.MoveIs = Move.Player_1;
                gameParams.DiffLevelPlayer_2 = DifficultyLevel.Normal;
                gameParams.ComputerLogicsNamePlayer_1 = new ComputerLogicLevel_1(0, null).Name;
                gameParams.ComputerLogicsNamePlayer_2 = ComputerLogicLevel_2_Player_2.Name;

                GetTimeDelay();

                //vCL_2 = new ViewCompLogic(ComputerLogicLevel_2_Player_2.ArrFieldCompVision);          // Для отладки.
                //vCL_2.Show();                                                                         // Для отладки.
            }

            else
            {
                string tampName;
                bool bTempIsHuman;
                bool bIsWinner;

                tampName = player_1.Name;
                bTempIsHuman = player_1.BIsHuman;
                bIsWinner = player_1.BIsWinner;
                player_1 = new Fleet(fieldSize, shipsDensity);
                player_1.Name = tampName;
                player_1.BIsHuman = bTempIsHuman;
                player_1.BIsWinner = bIsWinner;

                tampName = player_2.Name;
                bTempIsHuman = player_2.BIsHuman;
                bIsWinner = player_2.BIsWinner;
                player_2 = new Fleet(fieldSize, shipsDensity);
                player_2.Name = tampName;
                player_2.BIsHuman = bTempIsHuman;
                player_2.BIsWinner = bIsWinner;

                if (player_1.BIsHuman == false)
                {
                    Dictionary<int, int> shipsInfo_2 = GetShipListInfoForComputerLogic(player_2.Ships);
                    if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_1(0, null).Name)
                        ComputerLogicLevel_1_Player_1 = new ComputerLogicLevel_1(fieldSize, shipsInfo_2);

                    else if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_2(0, null).Name)
                        ComputerLogicLevel_2_Player_1 = new ComputerLogicLevel_2(fieldSize, shipsInfo_2);

                    else if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_3(0, null).Name)
                        ComputerLogicLevel_3_Player_1 = new ComputerLogicLevel_3(fieldSize, shipsInfo_2);
                }

                if (player_2.BIsHuman == false)
                {
                    Dictionary<int, int> shipsInfo_1 = GetShipListInfoForComputerLogic(player_1.Ships);

                    if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Easy && player_1.BIsHuman) ||
                         (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_1(0, null).Name) && player_1.BIsHuman == false)
                        ComputerLogicLevel_1_Player_2 = new ComputerLogicLevel_1(fieldSize, shipsInfo_1);

                    else if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Normal && player_1.BIsHuman) ||
                        (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_2(0, null).Name) && player_1.BIsHuman == false)
                        ComputerLogicLevel_2_Player_2 = new ComputerLogicLevel_2(fieldSize, shipsInfo_1);

                    else if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Advanced && player_1.BIsHuman) ||
                        (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_3(0, null).Name) && player_1.BIsHuman == false)
                        ComputerLogicLevel_3_Player_2 = new ComputerLogicLevel_3(fieldSize, shipsInfo_1);
                }

                if (player_1.BIsWinner)
                    gameOrder.MoveIs = Move.Player_1;
                else if (player_2.BIsWinner)
                    gameOrder.MoveIs = Move.Player_2;

                GetTimeDelay();
            }
            if (vmGS.CbViewOff == false) // Если при игре компьютера с компьютером нажата галочка "Отключить отображение", то подготовка интерфайса перед началом каждой игры не производится.
                PrepareGameInterface();
        }




        //// Метод 2. Взаимодействие с меню новой игры:
        async private void GetNewGame()
        {
            vmNGP = new NewGameParamsViewModel(fieldSize, gameParams.ShipDensity, player_1, player_2, gameParams.DiffLevelPlayer_2, gameParams.ComputerLogicsNamePlayer_1, gameParams.ComputerLogicsNamePlayer_2);
            
            if (vmNGP.BBeginNewGame)
            {
                fieldSize = vmNGP.FieldSize;
                gameParams.ShipDensity = vmNGP.ShipsDensity;
                player_1 = vmNGP.Player_1;
                player_2 = vmNGP.Player_2;

                // Получение количества клеток кораблей (для отладки)
                //int shipsCellsCount = 0;
                //foreach (var ship in player_1.Ships)
                //    foreach (var cell in ship)
                //        shipsCellsCount++;
                //ShipsCellCount = shipsCellsCount;

                if (player_2.BIsHuman == false)
                {
                    player_2.Name = vmNGP.ConputerLogicNamesPlayer_2;
                    Dictionary<int, int> shipsInfo_1 = GetShipListInfoForComputerLogic(player_1.Ships); // Информация о количестве и длине кораблей игрока 1.                    
                    gameParams.DiffLevelPlayer_2 = vmNGP.DiffLevelPlayer_2;
                    gameParams.ComputerLogicsNamePlayer_2 = vmNGP.ConputerLogicNamesPlayer_2;

                    if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Easy && player_1.BIsHuman) ||
                        (vmNGP.ConputerLogicNamesPlayer_2 == new ComputerLogicLevel_1(0, null).Name && player_1.BIsHuman == false))
                    {
                        ComputerLogicLevel_1_Player_2 = new ComputerLogicLevel_1(fieldSize, shipsInfo_1);
                        //vCL_2 = new ViewCompLogic(ComputerLogicLevel_1_Player_2.ArrFieldCompVision);          // Для отладки.
                        //vCL_2.Show();                                                                         // Для отладки.
                    }
                    else if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Normal && player_1.BIsHuman) ||
                        (vmNGP.ConputerLogicNamesPlayer_2 == new ComputerLogicLevel_2(0, null).Name && player_1.BIsHuman == false))
                    {
                        ComputerLogicLevel_2_Player_2 = new ComputerLogicLevel_2(fieldSize, shipsInfo_1);
                        //vCL_2 = new ViewCompLogic(ComputerLogicLevel_2_Player_2.ArrFieldCompVision);          // Для отладки.
                        //vCL_2.Show();                                                                         // Для отладки.
                    }
                    else if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Advanced && player_1.BIsHuman) ||
                        (vmNGP.ConputerLogicNamesPlayer_2 == new ComputerLogicLevel_3(0, null).Name && player_1.BIsHuman == false))
                    {
                        ComputerLogicLevel_3_Player_2 = new ComputerLogicLevel_3(fieldSize, shipsInfo_1);
                        //vCL_2 = new ViewCompLogic(ComputerLogicLevel_3_Player_2.ArrFieldCompVision);          // Для отладки.
                        //vCL_2.Show();                                                                         // Для отладки.
                    }
                }
                if (player_1.BIsHuman == false)
                {
                    player_1.Name = vmNGP.ConputerLogicNamesPlayer_1;
                    Dictionary<int, int> shipsInfo_2 = GetShipListInfoForComputerLogic(player_2.Ships); // Информация о количестве и длине кораблей игрока 2.
                    gameParams.ComputerLogicsNamePlayer_1 = vmNGP.ConputerLogicNamesPlayer_1;

                    if (vmNGP.ConputerLogicNamesPlayer_1 == new ComputerLogicLevel_1(0, null).Name)
                    {
                        ComputerLogicLevel_1_Player_1 = new ComputerLogicLevel_1(fieldSize, shipsInfo_2);
                        //vCL_1 = new ViewCompLogic(ComputerLogicLevel_1_Player_1.ArrFieldCompVision);          // Для отладки.
                        //vCL_1.Show();                                                                         // Для отладки.
                    }
                    if (vmNGP.ConputerLogicNamesPlayer_1 == new ComputerLogicLevel_2(0, null).Name)
                    {
                        ComputerLogicLevel_2_Player_1 = new ComputerLogicLevel_2(fieldSize, shipsInfo_2);
                        //vCL_1 = new ViewCompLogic(ComputerLogicLevel_2_Player_1.ArrFieldCompVision);          // Для отладки.
                        //vCL_1.Show();                                                                         // Для отладки.
                    }
                    if (vmNGP.ConputerLogicNamesPlayer_1 == new ComputerLogicLevel_3(0, null).Name)
                    {
                        ComputerLogicLevel_3_Player_1 = new ComputerLogicLevel_3(fieldSize, shipsInfo_2);
                        //vCL_1 = new ViewCompLogic(ComputerLogicLevel_3_Player_1.ArrFieldCompVision);          // Для отладки.
                        //vCL_1.Show();                                                                         // Для отладки.
                    }
                }

                await Task.Delay(500);                                                  // Задержка для выхода из ChangeFleet(), если начинает играть компьютер.
                   
                if (vmGS != null)                                                       // Чтобы при создании новой игры GSvm.BGameStoped можно было присвоить false;
                {
                    if (vmGS.GS != null) vmGS.GS.Close();                               // Закрываем окно статистики.
                    vmGS = null;                                                        // Обнулыем ViewModel
                    vmGS = new GameStatisticsVewModel();                                // Создаем новый.
                }                                                                       // В GetNewDefaltGame этого делать не надо, так как после каждой игры компа с компом статистика будет обнуляться.

                if (player_1.BIsWinner == false && player_2.BIsWinner == false)
                    gameOrder.GetRandomTurn();

                PrepareGameInterface();

                GetTimeDelay();

                vmGS.Player_1Statistics = null;
                vmGS.Player_2Statistics = null;
                vmGS.BGameStoped = false;               // Возвращение параметра в false позволит сделать ход человеку из команды ClickOnFieldCellCommand после игры двух компьютеров 

                vmNGP = null;                           // Обнуление для правильного выхода из метода ChangeViewAndModel при игре двух компьютеров.

                if (gameOrder.MoveIs == Move.Player_1 && player_1.BIsHuman == false)
                    ChangeViewAndModel(player_2);
                else if (gameOrder.MoveIs == Move.Player_2 && player_2.BIsHuman == false)
                    ChangeViewAndModel(player_1);
            }
        }


        //// Метод 3. Подготовка элементов оконного интерфейса перед началом игры:
        private void PrepareGameInterface()
        {
            player_1FieldCellsButtons.Clear();
            player_2FieldCellsButtons.Clear();

            for (int Y = 0; Y < fieldSize; Y++)
                for (int X = 0; X < fieldSize; X++)
                {
                    player_1FieldCellsButtons.Add(new Button() { Background = GetButtonContent(player_1.ArrField[Y, X].status, player_1) });
                    player_2FieldCellsButtons.Add(new Button() { Background = GetButtonContent(player_2.ArrField[Y, X].status, player_2) });
                }
            //GameConsoleInterface.GameInterfaceDraw(player_1, player_2, player_1.KoefFieldSize);     // Запуск консольного интерфейса (для контроля).
        }


        //// Метод 4. Заполнение ячеек игрового поле необходимым контентом (цвета):
        private Brush GetButtonContent(Fleet.CellStatus cellStatus, Fleet fl)
        {
            Brush content = null;
            switch (cellStatus)
            {
                case Fleet.CellStatus.Clear: content = Brushes.LightBlue; break;
                case Fleet.CellStatus.RestAria:
                    {
                        if (fl.BIsHuman) content = Brushes.LightBlue;
                        else content = Brushes.LightBlue;
                    }
                    break;
                case Fleet.CellStatus.Ship:
                    {
                        if ((fl.BIsHuman && fl == player_1 && gameOrder.MoveIs == Move.Player_1) ||
                        (fl.BIsHuman && fl == player_2 && gameOrder.MoveIs == Move.Player_2) ||
                        (fl.BIsHuman && player_2.BIsHuman == false) ||
                        (player_1.BIsHuman == false && player_2.BIsHuman == false))
                            content = Brushes.DarkSlateGray;
                        else content = Brushes.LightBlue;
                    }
                    break;
                case Fleet.CellStatus.Miss: content = Brushes.LightSlateGray; break;
                case Fleet.CellStatus.Hit: content = Brushes.Red; break;
                case Fleet.CellStatus.Destroyed: content = Brushes.Black; break;
                case Fleet.CellStatus.Cover: content = Brushes.AliceBlue; break;
            }
            return content;
        }


        //// Метод 5. Статистика по игре компьютера с компьютером
        private void MaintainingGameStatistic(Fleet.CellStatus status, Fleet fl)
        {
            if (gameOrder.MoveIs == Move.Player_1)
                vmGS.MaintainingGameStatistic(status, player_1, fl);
            else if (gameOrder.MoveIs == Move.Player_2)
                vmGS.MaintainingGameStatistic(status, player_2, fl);            
        }

        //// Метод 6. Изменение состояния ячеек игровых полей и кораблей:
        async private void ChangeViewAndModel(Fleet fl)                                     // Когда ходит человек, то информация об изменяемом экземпляре класса Fleet передается с командой из  View.
        {                                                                                   // Создаётеся ObservableCollection, которая в зависимости от того, кто ходит, будет ссылаться на соответствующий...
            ObservableCollection<Button> FieldCellsButtons = null;                          // ... ObservableCollection для Player_1, Player_2 или Computer.
            Fleet.SCell[] changeCellsInfo = null;
            do
            {
                if (player_2.BIsHuman == false && gameOrder.MoveIs == Move.Player_2)        
                {
                    fl = player_1;                                                          // При переходе хода к компьютеру, нужно принудительно сменить ссылку на класс Fleet.
                    FieldCellsButtons = player_1FieldCellsButtons;                          // ... работа будет производиться с Player_1FieldCellsButtons.
                    Fleet.SCell cell = new();
                    if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Easy && player_1.BIsHuman) ||
                        (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_1(0, null).Name && player_1.BIsHuman == false))
                        cell = ComputerLogicLevel_1_Player_2.GetNextCellForTurn();
                    else if((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Normal && player_1.BIsHuman) ||
                        (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_2(0, null).Name && player_1.BIsHuman == false))
                        cell = ComputerLogicLevel_2_Player_2.GetNextCellForTurn();
                    else if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Advanced && player_1.BIsHuman) ||
                        (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_3(0, null).Name && player_1.BIsHuman == false))
                        cell = ComputerLogicLevel_3_Player_2.GetNextCellForTurn();

                    changeCellsInfo = fl.ChangeIndividualCells(cell.X, cell.Y);             // Изменение соответствующих отдельных клеток в экземпляре класса Fleet в Model.
                    PrepareActivatedCellInfoForComputerLogic(changeCellsInfo);              // Очистка информации от индексов кораблей для передачи в логику компьютера.

                    if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Easy && player_1.BIsHuman) ||
                        (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_1(0, null).Name && player_1.BIsHuman == false))                // Передача информации о активированных клетках в логику компьютера.
                        ComputerLogicLevel_1_Player_2.ChangeComputerVision(changeCellsInfo);
                    else if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Normal && player_1.BIsHuman) ||
                        (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_2(0, null).Name && player_1.BIsHuman == false))
                        ComputerLogicLevel_2_Player_2.ChangeComputerVision(changeCellsInfo);
                    else if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Advanced && player_1.BIsHuman) ||
                        (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_3(0, null).Name && player_1.BIsHuman == false))
                        ComputerLogicLevel_3_Player_2.ChangeComputerVision(changeCellsInfo);
                                
                    await Task.Delay(timeDelay);

                    // Для отладки.
                    //if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Easy && player_1.BIsHuman) ||
                    //    (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_1(0, null).Name && player_1.BIsHuman == false))
                    //    vCL_2.ArrField = ComputerLogicLevel_1_Player_2.ArrFieldCompVision;                             
                    //else if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Normal && player_1.BIsHuman) ||
                    //    (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_2(0, null).Name && player_1.BIsHuman == false))
                    //    vCL_2.ArrField = ComputerLogicLevel_2_Player_2.ArrFieldCompVision;
                    //else if ((gameParams.DiffLevelPlayer_2 == DifficultyLevel.Advanced && player_1.BIsHuman) ||
                    //    (gameParams.ComputerLogicsNamePlayer_2 == new ComputerLogicLevel_3(0, null).Name && player_1.BIsHuman == false))
                    //    vCL_2.ArrField = ComputerLogicLevel_3_Player_2.ArrFieldCompVision;
                }

                else if (player_1.BIsHuman == false && gameOrder.MoveIs == Move.Player_1)
                {
                    fl = player_2;                                                          // При переходе хода к компьютеру, нужно принудительно сменить ссылку на класс Fleet.
                    FieldCellsButtons = player_2FieldCellsButtons;                          // ... работа будет производиться с Player_2FieldCellsButtons.
                    Fleet.SCell cell = new();
                    if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_1(0, null).Name)
                        cell = ComputerLogicLevel_1_Player_1.GetNextCellForTurn();
                    else if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_2(0, null).Name)
                        cell = ComputerLogicLevel_2_Player_1.GetNextCellForTurn();
                    else if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_3(0, null).Name)
                        cell = ComputerLogicLevel_3_Player_1.GetNextCellForTurn();

                    changeCellsInfo = fl.ChangeIndividualCells(cell.X, cell.Y);             // Изменение соответствующих отдельных клеток в экземпляре класса Fleet в Model.
                    PrepareActivatedCellInfoForComputerLogic(changeCellsInfo);              // Очистка информации от индексов кораблей для передачи в логику компьютера.

                    if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_1(0, null).Name)                // Передача информации о активированных клетках в логику компьютера.
                        ComputerLogicLevel_1_Player_1.ChangeComputerVision(changeCellsInfo);
                    else if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_2(0, null).Name)
                        ComputerLogicLevel_2_Player_1.ChangeComputerVision(changeCellsInfo);
                    else if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_3(0, null).Name)
                        ComputerLogicLevel_3_Player_1.ChangeComputerVision(changeCellsInfo);
                                        
                    await Task.Delay(timeDelay);

                    // Для отладки. 
                    //if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_1(0, null).Name)
                    //    vCL_1.ArrField = ComputerLogicLevel_1_Player_1.ArrFieldCompVision;                            
                    //else if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_2(0, null).Name)
                    //    vCL_1.ArrField = ComputerLogicLevel_2_Player_1.ArrFieldCompVision;
                    //else if (gameParams.ComputerLogicsNamePlayer_1 == new ComputerLogicLevel_3(0, null).Name)
                    //    vCL_1.ArrField = ComputerLogicLevel_3_Player_1.ArrFieldCompVision;
                }

                else if (fl == player_2 && gameOrder.MoveIs == Move.Player_1)
                {
                    FieldCellsButtons = player_2FieldCellsButtons;                          // Если ход игрока 1, то работа будет производиться с Player_2FieldCellsButtons.
                    int X = -1, Y = -1;                                                     // Начальные значения -1 для ослещивания ошибки
                    int iOC = 0;                                                            // Индекс ObservableCollection<Button>.
                    foreach (Button btnCell in FieldCellsButtons)
                    {
                        if (btnCell.IsFocused)                                              // Поиск индекса нажатой клетки и преобразование в координаты двухмернного массива игрового поля.
                        {
                            X = iOC % fieldSize;
                            Y = iOC / fieldSize;
                            break;
                        }
                        iOC++;
                    }
                    changeCellsInfo = fl.ChangeIndividualCells(X, Y);                       // Изменение соответствующих отдельных клеток в экземпляре класса Fleet в Model.
                }

                else if (fl == player_1 && gameOrder.MoveIs == Move.Player_2)
                {                    
                    FieldCellsButtons = player_1FieldCellsButtons;                          // Если ход игрока 2, то работа будет производиться с Player_1FieldCellsButtons.
                    int X = -1, Y = -1;                                                     // Начальные значения -1 для ослещивания ошибки
                    int iOC = 0;                                                            // Индекс ObservableCollection<Button>.
                    foreach (Button btnCell in FieldCellsButtons)
                    {
                        if (btnCell.IsFocused)                                              // Поиск индекса нашатой клетки и преобразование в координаты двухмернного массива игрового поля.
                        {
                            X = iOC % fieldSize;
                            Y = iOC / fieldSize;
                            break;
                        }
                        iOC++;
                    }
                    changeCellsInfo = fl.ChangeIndividualCells(X, Y);                       // Изменение соответствующих отдельных клеток в экземпляре класса Fleet в Model.
                }

                    MaintainingGameStatistic(changeCellsInfo[0].status, fl);

                if (vmNGP != null && vmNGP.BBeginNewGame)                                   // Так как данный метод асинхронный, то при вызове меню новой игры (если кнопка "Остановить игру" не была нажата) компьютер продолжит ходить.                                            
                    { vmNGP = null; return; }                                               // Поэтому после создания новой игры данный процесс нужно принудительно остановить, а новая игра начнётся автоматически.

                if (vmGS.CbViewOff == false)             
                    ChangeGameInterface(changeCellsInfo, FieldCellsButtons, fl);

                if (player_1.СountOfRemainingShips == 0 || player_2.СountOfRemainingShips == 0) // Для быстрого тестирования.
                    await Task.Delay(1);

                if (ChangeMove(changeCellsInfo, fl) && player_1.BIsHuman && player_2.BIsHuman)// Определение порядка игры (определение, кто будет делать следующий ход и т.д.)
                    TurnGameInterface(); 
                
            } while (((player_2.BIsHuman == false && gameOrder.MoveIs == Move.Player_2) ||
                    (player_1.BIsHuman == false && gameOrder.MoveIs == Move.Player_1)) &&
                    vmGS.BGameStoped == false);                                             // Остановка игры из окна статистики при игре компьютера с компьютером.                                     
        }


        //// Метод 7. Определение порядка игры:
        private bool ChangeMove(Fleet.SCell[] changeCellsInfo, Fleet fl)
        {
            if (changeCellsInfo[0].status == Fleet.CellStatus.Destroyed)                    // Если очередной корабль подбит, то ...
            {
                if (GameOrder.CheckGameOver(fl, player_1, player_2))
                {
                    if (player_1.BIsHuman || player_2.BIsHuman)
                    {
                        GameOver go = new GameOver();

                        if (player_1.BIsWinner)
                            go.WinnerName.Text = "Победил " + player_1.Name + "!";

                        else if (fl.Name == player_1.Name)
                            go.WinnerName.Text = "Победил " + player_2.Name + "!";

                        go.ShowDialog();                        
                    }
                    GetNewDefaltGame(fieldSize, gameParams.ShipDensity);
                }                    
                return false;
            }
            // ... проверяем окончена ли игра. Если окончена, то происходит запуск игры с последними заданными настройками.

            else if (changeCellsInfo[0].status == Fleet.CellStatus.Miss ||
                changeCellsInfo[0].status == Fleet.CellStatus.RestAria)                     // Если был промах, то ...
            {
                gameOrder.ChangeMove();
                return true;
            }
            else return false;
        }


        //// Метод 8. Изменение интерфейса при смене хода при игре человека с человеком:
        async private void TurnGameInterface()
        {            
            MoveTransitionViewModel TMvm = new MoveTransitionViewModel();
            TMvm.IntarfaceTurnOff = true;
                for (int Y = 0; Y < fieldSize; Y++)
                {
                    for (int X = 0; X < fieldSize; X++)
                    {
                        player_1FieldCellsButtons[Y * fieldSize + X].Background = GetButtonContent(Fleet.CellStatus.Cover, player_1);
                        player_2FieldCellsButtons[Y * fieldSize + X].Background = GetButtonContent(Fleet.CellStatus.Cover, player_2);
                    }
                    await Task.Delay(10);
                }


            TMvm.IntarfaceTurnOff = false;
                for (int Y = 0; Y < fieldSize; Y++)
                {
                    for (int X = 0; X < fieldSize; X++)
                    {
                        player_1FieldCellsButtons[Y * fieldSize + X].Background = GetButtonContent(player_1.ArrField[Y, X].status, player_1);
                        player_2FieldCellsButtons[Y * fieldSize + X].Background = GetButtonContent(player_2.ArrField[Y, X].status, player_2);
                    }
                    await Task.Delay(10);
                }

        }

        //// Метод 9. Изменение в оконном интерфейсе:
        private void ChangeGameInterface(Fleet.SCell[] infoForChangeView, ObservableCollection<Button> FieldCellsButtons, Fleet fl)
        {
            int legth = infoForChangeView.Length;
            switch (infoForChangeView[0].status)
            {
                // Нажата ранее активированная клетка:
                case Fleet.CellStatus.AlreadyActivated:
                    return;

                // Изменение в клетках подбитого корабля:
                case Fleet.CellStatus.Hit:
                    {
                        // Изменение состояния одной клетки корабля и поля на Hit ДОРАБОТАТЬ!!!
                        goto default;
                    }

                // Изменение в клетках уничтоженного корабля:
                case Fleet.CellStatus.Destroyed:
                    {
                        // Изменение состояния всего корабля на Destroyed ДОРАБОТАТЬ!!!
                        goto default;
                    }

                // Изменение в клетках игрового поля:
                default:
                    {
                        for (int i = 0; i < legth; i++)
                            FieldCellsButtons[infoForChangeView[i].Y * fieldSize + infoForChangeView[i].X].Background =
                                GetButtonContent(infoForChangeView[i].status, fl);
                        break;
                    }
            }
            //GameConsoleInterface.GameInterfaceDraw(player_1, player_2, player_1.KoefFieldSize); // Запуск консольного интерфейса (для отладки).
        }

          
        //// Метод 10. Определение длины и количества кораблей для computerLogic:
        private Dictionary<int, int> GetShipListInfoForComputerLogic(List<Fleet.SCell[]> ships)
        {
            Dictionary<int, int> shipsInfo = ships
                .GroupBy(ship => ship.Length)
                .ToDictionary(ship => ship.Key, ship => ship.Count());
            return shipsInfo;
        }


        /// Метод 11. Очистка информации об активированных клетках от лишних данных перед передачей их в computerLogic:
        private void PrepareActivatedCellInfoForComputerLogic(Fleet.SCell[] cells)
        {
            int length = cells.Length;
            for (int i = 0; i < length; i++)
            {
                cells[i].shipIndex = 0;
                cells[i].shipCellIndex = 0;                
            }                
        }


        //// Метод 12. Корректировка задержки времени:
        private void GetTimeDelay()
        {
            if ((player_1.BIsHuman && player_2.BIsHuman) ||
                (!player_1.BIsHuman && !player_2.BIsHuman))
                timeDelay = 1;
            else timeDelay = 300;

            if (vmGS.CbViewOff)
                timeDelay = 0;
        }
    }
}
