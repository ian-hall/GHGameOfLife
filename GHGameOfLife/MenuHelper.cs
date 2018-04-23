﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Core_Automata
{
    
    class MenuHelper
    {
        // TODO: Change this to be less ugly maybe?
        public enum FileError { None, Length, Width, Contents, Size, Not_Loaded };
        public const ConsoleColor Info_FG = ConsoleColor.Red;
        public const ConsoleColor Default_BG = ConsoleColor.Black;
        public const ConsoleColor Default_FG = ConsoleColor.White;
        public const ConsoleColor Board_FG = ConsoleColor.White;
        public const ConsoleColor Builder_FG = ConsoleColor.Cyan;

        public const string Msg_Welcome = "Welcome to Ian's Automata Simulator";
        public const string Msg_Choose = "Please choose an option!";
        public const string Msg_Change_Size = "[Ctrl + [+/-]] Change board size";
        public const string Prompt = "Your choice: ";
        public const string Msg_Entry_Error = "**Invalid entry**";
        public const string Msg_Press_Enter = "Press ENTER to confirm";
        public const string Msg_Loading_Rand = "Loading random pop.";

        public static string[] Run_Ctrls;
        public static string[] Create_Ctrls;

        public static int Window_Center; // Center Row
        public static int Left_Align;    // Align text with the Welcome message

        public static List<string> Large_Pops;
        public static List<string> Builder_Pops;

        private const int Info_Row = 3;
        private const int Welcome_Row = 6;
        private static int Menu_Start_Row;

        public static int Space = 5;
        public static int Choices_Per_Page = 7;
        
        static MenuHelper()
        {
            Window_Center = Console.WindowHeight / 2;
            Left_Align = (Console.WindowWidth / 2) - (Msg_Welcome.Length / 2);

            // Start the menus at 1/3 of the window
            Menu_Start_Row = Console.WindowHeight / 3 + 1;
            Large_Pops = new List<string>();
            Builder_Pops = new List<string>();

            ResourceManager rm = Core_Automata.LargePops.ResourceManager;
            rm.IgnoreCase = true;
            ResourceSet all = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true);


            foreach (DictionaryEntry res in all)
            {
                Large_Pops.Add(res.Key.ToString());
            }

            Large_Pops.Sort();

            rm = Core_Automata.BuilderPops.ResourceManager;
            rm.IgnoreCase = true;
            all = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            foreach (DictionaryEntry res in all)
            {
                Builder_Pops.Add(res.Key.ToString());
            }

            Run_Ctrls = new string[] {  "[SPACE] Step/Pause",
                                        "[R] Toggle running",
                                        "[ESC] Exit",
                                        "[+/-] Adjust speed" };

            Create_Ctrls = new string[] {   "[↑|↓|←|→] Move cursor",
                                            "[SPACE] Add/Remove cells",
                                            "[ENTER] Start Game",
                                            "[[#]] Load/Rotate pop",
                                            "[Ctrl + [#]] Mirror pop",
                                            "[C] Cancel pop mode"};

            ClearConsoleWindow();
        }
        
        public static void ReInitialize()
        {
            Window_Center = Console.WindowHeight / 2;
            Left_Align = (Console.WindowWidth / 2) - (Msg_Welcome.Length / 2);

            // Start the menus at 1/3 of the window
            Menu_Start_Row = Console.WindowHeight / 3 + 1;
        }
        
        public static void ClearLine(int row)
        {
            Console.SetCursorPosition(0, row);
            Console.Write("".PadRight(Console.WindowWidth - 1));
        }
        
        public static void ClearWithinBorder(int row)
        {
            Console.SetCursorPosition(5, row);
            Console.Write("".PadRight(Console.WindowWidth - 10));
        }
        
        public static void ClearConsoleWindow()
        {
            for( int i = 0; i < Console.WindowHeight; i++ )
            {
                ClearLine(i);
            }
        }
        
        public static void DrawBorder()
        {
            var rows = Console.WindowHeight;
            var cols = Console.WindowWidth;

            char vert = '║'; // '\u2551'
            char horiz = '═'; // '\u2550'
            char topLeft = '╔'; // '\u2554'
            char topRight = '╗'; // '\u2557'
            char botLeft = '╚'; // '\u255A'
            char botRight = '╝'; // '\u255D'

            int borderTop = 4;
            int borderBottom = rows - 5;
            int borderLeft = 4;
            int borderRight = cols - 5;


            // This draws the nice little border on the screen...
            Console.SetCursorPosition(borderLeft, borderTop);
            Console.Write(topLeft);
            for (int i = borderLeft; i < borderRight; i++)
                Console.Write(horiz);
            Console.SetCursorPosition(borderRight, borderTop);
            Console.Write(topRight);
            for (int i = borderTop + 1; i < borderBottom; i++)
            {
                Console.SetCursorPosition(borderLeft, i);
                Console.Write(vert);
                Console.SetCursorPosition(borderRight, i);
                Console.Write(vert);
            }
            Console.SetCursorPosition(borderLeft, borderBottom);
            Console.Write(botLeft);
            for (int i = 5; i < borderRight; i++)
                Console.Write(horiz);
            Console.Write(botRight);
        }
        
        /// <summary>
        /// Prints the controls for controlling the game while running
        /// </summary>
        public static void PrintRunControls()
        {
            Console.ForegroundColor = MenuHelper.Info_FG;
            int printRow = (Console.WindowHeight) - 4;

            Console.SetCursorPosition(5, printRow);
            Console.Write("{0,-25}{1,-25}", Run_Ctrls[0], Run_Ctrls[3]);
            Console.SetCursorPosition(5, ++printRow);
            Console.Write("{0,-25}", Run_Ctrls[1]);
            Console.SetCursorPosition(5, ++printRow);
            Console.Write("{0,-25}", Run_Ctrls[2]);
            Console.ForegroundColor = MenuHelper.Default_FG;
        }
        
        /// <summary>
        /// Prints the game status while running
        /// </summary>
        /// <param name="running">if the game is looping or stepping</param>
        /// <param name="paused">if the game is paused</param>
        /// <param name="wrapping">if the board has wrapping on</param>
        /// <param name="speed">the speed the board is running at</param>
        public static void PrintStatus(bool running, bool paused,
                                        bool wrapping, int speed)
        {
            Console.ForegroundColor = MenuHelper.Info_FG;
            StringBuilder sb = new StringBuilder();
            string runStr = (running) ? "LOOPING" : "STEPPING";
            string pauseStr = (running && paused) ? "PAUSED" : " ";
            string wrapStr = (wrapping) ? "WRAPPING" : " ";

            string speedStr = "SPEED - ";
            for (int i = 0; i < 5; i++)
            {
                if (i <= speed)
                    speedStr += "█ ";
                else
                    speedStr += "  ";
            }
            speedStr += "+";

            int colOne = 10;
            int colTwo = 10;
            int colThree = 10;
            int speedCol = Console.WindowWidth - colOne - colTwo - colThree - 10;
            //Hardcode 10 because of the border around the game board
            //when it is displayed, so its like space*2
            string formatStr = "{0,-" + colOne + "}{1,-" + colTwo +
                                                        "}{2,-" + colThree + "}{3," + speedCol + "}";
            sb.AppendFormat(formatStr, runStr, pauseStr, wrapStr, speedStr);
            ClearLine(Info_Row);
            Console.SetCursorPosition(5, Info_Row);
            Console.Write(sb);
            Console.ForegroundColor = MenuHelper.Default_FG;
        }
        
        /// <summary>
        /// Prints menu when user is building a population
        /// </summary>
        public static void PrintCreationControls()
        {
            Console.ForegroundColor = MenuHelper.Info_FG;
            int printStart = Console.WindowHeight - 4;
            int printRow = printStart;
            var textWidth = 25;

            Console.SetCursorPosition(5, printRow);
            Console.Write("{0,-25}{1,-25}{2,-25}", Create_Ctrls[0], Create_Ctrls[3], Create_Ctrls[5]);
            Console.SetCursorPosition(5, ++printRow);
            Console.Write("{0,-25}{1,-25}", Create_Ctrls[1], Create_Ctrls[4]);
            Console.SetCursorPosition(5, ++printRow);
            Console.Write("{0,-25}", Create_Ctrls[2]);

            /* Start count at 1 because of the above. Need to limit this to 3 entries per column because
             * of the scroll bar that shows up. printCol is calculated based on the above since we start
             * printing the pops directly beneath the last entry.
             */
            int count = 1;
            printRow = printStart;
            foreach (string popName in Builder_Pops)
            {
                printRow = printStart + (count % 3);
                var printCol = 5 + (textWidth * (2 + count / 3));
                Console.SetCursorPosition(printCol, printRow);
                Console.Write("[{0}]{1,-25}", Builder_Pops.IndexOf(popName) + 1, popName);
                ++count;
                ++printRow;
            }

            Console.ForegroundColor = MenuHelper.Default_FG;
        }
        
        /// <summary>
        /// Prints messages to prompt for another round
        /// </summary>
        public static void PromptForAnother()
        {
            ClearAllInBorder();
            ClearUnderBoard();
            ClearAboveBoard();
            ClearLine(Info_Row);

            int printRow = Menu_Start_Row + 1;
            Console.SetCursorPosition(Left_Align, printRow);
            Console.Write("Do you really want to exit?");
            Console.SetCursorPosition(Left_Align, ++printRow);
            Console.ForegroundColor = Info_FG;
            Console.Write("[ENTER] No, keep playing.");
            Console.SetCursorPosition(Left_Align, ++printRow);
            Console.Write("[ESC] Yes, let me out.");
            Console.ForegroundColor = Default_FG;
        }
        
        /// <summary>
        /// Clear all lines below the border
        /// </summary>
        public static void ClearUnderBoard()
        {
            for (int i = Console.WindowHeight - 4; i < Console.WindowHeight; i++)
            {
                ClearLine(i);
            }
        }
        
        /// <summary>
        /// Clear everything inside the board area
        /// </summary>
        public static void ClearAllInBorder()
        {
            for (int i = 5; i < Console.WindowHeight - 5; i++)
                ClearWithinBorder(i);

        }
        
        /// <summary>
        /// Clear above the border
        /// </summary>
        public static void ClearAboveBoard()
        {
            for (int i = 0; i < Space - 1; i++)
            {
                ClearLine(i);
            }
        }
        
        /// <summary>
        /// Prints a user friendly error message
        /// </summary>
        /// <param name="err">the error to get friendly with</param>
        public static void PrintFileError(MenuHelper.FileError err)
        {
            string errorStr;
            switch (err)
            {
                case FileError.Contents:
                    errorStr = "File not all .'s and O's";
                    break;
                case FileError.Length:
                    errorStr = "Too many lines for current window size";
                    break;
                case FileError.Not_Loaded:
                    errorStr = "No file loaded";
                    break;
                case FileError.Size:
                    errorStr = "File either empty or larger than 20KB";
                    break;
                case FileError.Width:
                    errorStr = "Too many characters per line";
                    break;
                default:
                    errorStr = "Generic file error...";
                    break;
            }

            int windowCenter = Console.WindowHeight / 2; //Vert position
            int welcomeLeft = (Console.WindowWidth / 2) -
                (MenuHelper.Msg_Welcome.Length / 2);
            int distToBorder = (Console.WindowWidth - 5) - welcomeLeft;

            MenuHelper.ClearWithinBorder(windowCenter);
            Console.SetCursorPosition(welcomeLeft, windowCenter - 1);
            Console.Write(errorStr);
            Console.SetCursorPosition(welcomeLeft, windowCenter);
            Console.Write(MenuHelper.Msg_Loading_Rand);
            Console.SetCursorPosition(welcomeLeft, windowCenter + 1);
            Console.Write(MenuHelper.Msg_Press_Enter);
        }
        
        /// <summary>
        /// Prints a menu from the given choices
        /// </summary>
        /// <param name="choices">IEnumerable<string> of choices to display</param>
        public static void PrintMenuFromList(IEnumerable<string> choices)
        {
            ClearAllInBorder();

            Console.ForegroundColor = MenuHelper.Info_FG;
            Console.SetCursorPosition(5, (Console.WindowHeight) - 4);
            Console.WriteLine(Msg_Change_Size);

            Console.ForegroundColor = MenuHelper.Default_FG;
            Console.SetCursorPosition(Left_Align, Welcome_Row);
            Console.Write(Msg_Welcome);

            int curRow = Menu_Start_Row;

            Console.SetCursorPosition(Left_Align, curRow);
            Console.Write(Msg_Choose);
            Console.SetCursorPosition(Left_Align, ++curRow);
            //Console.Write(Press_Enter);
            foreach (string choice in choices)
            {
                Console.SetCursorPosition(Left_Align + 4, ++curRow);
                Console.Write(choice);
            }
        }
        
        /// <summary>
        /// Changes enums to a list of strings prefixed by numbers 1-7
        /// </summary>
        /// <param name="enumVals"></param>
        /// <returns></returns>
        public static List<string> EnumToChoiceStrings(Array enumVals)
        {
            var choiceStrings = new List<string>();
            for (int i = 0; i < enumVals.Length; i++)
            {
                var enumStr = enumVals.GetValue(i).ToString();
                enumStr = enumStr.Replace('_', ' ');
                var choiceStr = String.Format("{0}) {1}", (i % 7) + 1, enumStr);
                choiceStrings.Add(choiceStr);
            }
            return choiceStrings;
        }
        
        /// <summary>
        /// Changes enums to a list of strings, and also adds a back option.
        /// NOTE: This only works on enums with 7 or less choices, otherwise the back
        /// option will not be in the correct position. 
        /// </summary>
        /// <param name="enumVals"></param>
        /// <returns></returns>
        public static List<string> EnumToChoiceStrings_WithBack(Array enumVals)
        {
            var choiceStrings = EnumToChoiceStrings(enumVals);
            var backString = String.Format("{0}) Back", enumVals.Length + 1);
            choiceStrings.Add(backString);
            return choiceStrings;
        }
        
        /// <summary>
        /// Displays options in a paged fashion.
        /// </summary>
        /// <returns>the index of the chosen choice</returns>
        ///       1-7 are used to select an option
        ///       8 goes back to prev page
        ///       9 goes to next page
        ///       0 cancels
        public static int PrintPagedMenu(List<string> choices, int pageNum, out bool onLastPage)
        {
            onLastPage = false;
            MenuHelper.ClearLine((Console.WindowHeight - 4));
            var totalNumChoices = choices.Count;
            var totalPages = totalNumChoices / MenuHelper.Choices_Per_Page;
            var lo = pageNum * MenuHelper.Choices_Per_Page;
            var hi = -1;
            if ((lo + MenuHelper.Choices_Per_Page) < totalNumChoices)
            {
                hi = (lo + MenuHelper.Choices_Per_Page);
            }
            else
            {
                hi = totalNumChoices;
                onLastPage = true;
            }

            string[] defaultPrompts = new string[] {    "8) Prev Page",
                                                        "9) Next Page",
                                                        "0) Back"};

            var currentPage = new List<string>();
            for (int i = lo; i < hi; i++)
            {
                //the strings in choices might or might not have keys (ex 1), 2), etc) associated with them
                //we need to add these prompts if they are not there. I think that only means when loading a resource
                var hasPrompt = Regex.IsMatch(choices[i], "^[0-9][)] ");
                if (hasPrompt)
                {
                    currentPage.Add(choices[i]);
                }
                else
                {
                    currentPage.Add(String.Format("{0}) {1}", (i - lo) + 1, choices[i]));
                }
            }
            if (pageNum != 0)
            {
                currentPage.Add(defaultPrompts[0]);
            }
            if (!onLastPage)
            {
                currentPage.Add(defaultPrompts[1]);
            }
            currentPage.Add(defaultPrompts[2]);

            MenuHelper.PrintMenuFromList(currentPage);
            return hi - 1;
        }
        
        /// <summary>
        /// Prints the given string centered on the given line.
        /// </summary>
        /// <param name="line">line to print on</param>
        /// <param name="toPrint">string to print</param>
        public static void PrintOnLine(int line, string toPrint)
        {
            MenuHelper.ClearWithinBorder(line);
            var left = (Console.WindowWidth/2) - (toPrint.Length/2);
            Console.SetCursorPosition(left, line);
            Console.WriteLine(toPrint);
        }
        
        /// <summary>
        /// Writes a prompt at the line and returns user input
        /// </summary>
        /// <param name="line"></param>
        /// <param name="toPrint"></param>
        /// <returns></returns>
        public static string PromptOnLine(int line, string prompt = MenuHelper.Prompt)
        {
            MenuHelper.ClearWithinBorder(line);
            var left = (Console.WindowWidth / 2) - (prompt.Length / 2);
            Console.SetCursorPosition(left, line);
            Console.Write(prompt);
            Console.CursorVisible = true;
            var input = Console.ReadLine();
            Console.CursorVisible = false;
            return input;
        }
        
        /// <summary>
        /// Checks if the input string is a valid range for 1D automata.
        /// </summary>
        /// <param name="input">user's input string</param>
        /// <param name="range">int representing the range</param>
        /// <returns></returns>
        ///  Gonna limit range to some magic number like 4
        public static bool ValidRangeInput(string input, out int range)
        {
            range = 0;
            try
            {
                var temp = Int32.Parse(input);
                if(temp < 1 || temp > 4)
                {
                    return false;
                }
                range = temp;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public static bool ValidHexInput(string input, out string hex)
        {
            hex = String.Empty;
            var temp = input.ToUpper();
            var allValid = temp.All(c => "0123456789ABCDEF".Contains(c));
            if (!allValid)
            {
                return false;
            }
            hex = temp;
            return true;
        }
        
        
    } // end class
    
}