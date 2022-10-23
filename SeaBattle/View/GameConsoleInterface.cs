using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using SeaBattle.Model;

namespace SeaBattle.View
{
    // Консольный интерфейс игры:
    static class GameConsoleInterface
    {
        // ========== Методы ==========
        // Метод 1. Отображение игровых полей:
        public static void GameInterfaceDraw(Fleet Player, Fleet Computer, int koefFieldSize)
        {
            int fieldLength = Player.ArrField.GetLength(0);
            string space = "                                                        ";
            string alf = "А Б В Г Д Е Ж З И К Л М Н О П Р С Т У Ф Х Ц Ч Ш Щ Э Ю Я ";
            ConsoleManager.Show();
            Console.Clear();
            Console.WriteLine("Мой флот:" + space.Substring(0, fieldLength * 2 - 1) + "Вражеский флот:");
            Console.WriteLine();
            Console.WriteLine("   " + alf.Substring(0, fieldLength * 2) + "        " + alf.Substring(0, fieldLength * 2));

            for (int Y = 0; Y < fieldLength; Y++)
            {
                string strField;
                if (Y >= 9)
                    strField = Y + 1 + " ";
                else
                    strField = Y + 1 + "  ";

                for (int X = 0; X < fieldLength; X++)
                {
                    if (Player.ArrField[Y, X].status == (int)Fleet.CellStatus.Clear) strField += "·" + " ";
                    else if (Player.ArrField[Y, X].status == Fleet.CellStatus.RestAria) strField += "·" + " ";
                    else if (Player.ArrField[Y, X].status == Fleet.CellStatus.Ship) strField += Player.ArrField[Y, X].shipIndex % 10 + " ";
                    else if (Player.ArrField[Y, X].status == Fleet.CellStatus.Miss) strField += "-" + " ";
                    else if (Player.ArrField[Y, X].status == Fleet.CellStatus.Hit) strField += "Ж" + " ";
                    else if (Player.ArrField[Y, X].status == Fleet.CellStatus.Destroyed) strField += "X" + " ";
                }

                if (Y >= 9)
                    strField += "     " + (Y + 1) + " ";
                else
                    strField += "     " + (Y + 1) + "  ";

                for (int X = 0; X < fieldLength; X++)
                {
                    if (Computer.ArrField[Y, X].status == Fleet.CellStatus.Clear) strField += "·" + " ";
                    else if (Computer.ArrField[Y, X].status == Fleet.CellStatus.RestAria) strField += "·" + " ";
                    else if (Computer.ArrField[Y, X].status == Fleet.CellStatus.Ship) strField += Computer.ArrField[Y, X].shipIndex % 10 + " ";
                    else if (Computer.ArrField[Y, X].status == Fleet.CellStatus.Miss) strField += "-" + " ";
                    else if (Computer.ArrField[Y, X].status == Fleet.CellStatus.Hit) strField += "Ж" + " ";
                    else if (Computer.ArrField[Y, X].status == Fleet.CellStatus.Destroyed) strField += "X" + " ";
                }
                Console.WriteLine(strField);
            }
            Console.WriteLine();
            Console.WriteLine("Корабли:" + space.Substring(0, fieldLength * 2) + "Корабли:");
            Console.WriteLine();

            for (int i = 0; i < Player.Ships.Count; i++)
            {
                ShipsDraw(Player.Ships[i], Computer.Ships[i], koefFieldSize, fieldLength);
            }

            return;
        }



        // Метод 2. Отображение состава кораблей под игровыми полями:
        internal static void ShipsDraw(Fleet.SCell[] playerShips, Fleet.SCell[] computerShips, int koefFieldSize, int fieldLength)
        {
            string space = "                                                                ";
            string strPlShip = "";
            string strCmpShip = "";
            int shipLength = playerShips.Length;
            for (int i = 0; i < shipLength; i++)
            {

                if (playerShips[i].status == Fleet.CellStatus.Ship) strPlShip += "П";
                else if (playerShips[i].status == Fleet.CellStatus.Hit) strPlShip += "Ж";
                else if (playerShips[i].status == Fleet.CellStatus.Destroyed) strPlShip += "X";

                if (computerShips[i].status == Fleet.CellStatus.Ship) strCmpShip += "П";
                else if (computerShips[i].status == Fleet.CellStatus.Hit) strCmpShip += "Ж";
                else if (computerShips[i].status == Fleet.CellStatus.Destroyed) strCmpShip += "X";
            }
            Console.WriteLine(strPlShip + space.Substring(0, fieldLength * 2 + 8 + koefFieldSize - shipLength) + strCmpShip);
        }
    }



    // Класс вывода консоли:
    [SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        private const string Kernel32_DllName = "kernel32.dll";

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32_DllName)]
        private static extern int GetConsoleOutputCP();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            //#if DEBUG
            if (!HasConsole)
            {
                AllocConsole();
                //InvalidateOutAndError();          // Я закомментировал.
            }
            //#endif
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            //#if DEBUG
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
            //#endif
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        static void InvalidateOutAndError()
        {
            Type type = typeof(System.Console);

            System.Reflection.FieldInfo _out = type.GetField("_out",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.FieldInfo _error = type.GetField("_error",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(_out != null);
            Debug.Assert(_error != null);

            Debug.Assert(_InitializeStdOutError != null);

            _out.SetValue(null, null);
            _error.SetValue(null, null);

            _InitializeStdOutError.Invoke(null, new object[] { true });
        }

        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
}
