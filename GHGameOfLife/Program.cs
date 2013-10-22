﻿using System;
using System.IO;
using System.Text;
using System.Reflection;


namespace GHGameOfLife
{
    enum LoadedPops { goose, grow, ship, spark, bees };
    class Program
    {
        enum PopType { RANDOM, FILE, PREMADE };

        // Don't go below these values or the text will be screwy
        const int MIN_WIDTH = 50;
        const int MIN_HEIGHT = 30;
        // Don't go below these values or the text will be screwy

        static int CONSOLE_WIDTH = 80; // Console width
        static int CONSOLE_HEIGHT = 50; // Console height
//------------------------------------------------------------------------------
        [STAThread]
        static void Main(string[] args)
        {
            int initBuffWidth = Console.BufferWidth;
            int initBuffHeight = Console.BufferHeight;
            int initConsWidth = Console.WindowWidth;
            int initConsHeight = Console.WindowHeight;            
            int initConsPosLeft = Console.WindowLeft;
            int initConsPosTop = Console.WindowTop;

            int[] initialValues = new int[] { initBuffWidth, initBuffHeight, 
                                              initConsWidth, initConsHeight, 
                                              initConsPosLeft, initConsPosTop };

            bool validWindowSize = InitializeConsole();

            if (!validWindowSize)
            {
                Console.WriteLine("Problem with console size");
            }
            else
            {
                MenuText.Initialize();
                MainMenu();
                //TODO Prompt to go again before resetting the console and closing
                ResetConsole(initialValues);
            }       
        }
//------------------------------------------------------------------------------
        /// <summary>
        /// Initializes the console for display. 
        /// </summary>
        private static bool InitializeConsole()
        {
            Console.BackgroundColor = MenuText.DefaultBG;
            Console.ForegroundColor = MenuText.DefaultFG;
            Console.Title = "Ian's Game of Life";
            /* Need to check the current window/buffer size before applying the
             * new size. Exits if the sizes are off.
             */
            if (CONSOLE_WIDTH < MIN_WIDTH || CONSOLE_HEIGHT < MIN_HEIGHT)
                return false;
            if (CONSOLE_WIDTH > Console.LargestWindowWidth ||
                                CONSOLE_HEIGHT > Console.LargestWindowHeight)
                return false;


            Console.SetWindowSize(CONSOLE_WIDTH, CONSOLE_HEIGHT);
            Console.SetWindowPosition(0, 0);
            Console.SetBufferSize(CONSOLE_WIDTH, CONSOLE_HEIGHT);                                 
            Console.CursorVisible = false;
            Console.Clear();

            //hurr unicode
            char vert =     '║'; // '\u2551'
            char horiz =    '═'; // '\u2550'
            char topLeft =  '╔'; // '\u2554'
            char topRight = '╗'; // '\u2557'
            char botLeft =  '╚'; // '\u255A'
            char botRight = '╝'; // '\u255D'

            int borderTop = 4;
            int borderBottom = CONSOLE_HEIGHT - 5;
            int borderLeft = 4;
            int borderRight = CONSOLE_WIDTH - 5;

            /* TODO: Add some pretty color to this
             */ 

            // This draws the nice little border on the screen...
            Console.SetCursorPosition(borderLeft, borderTop);
            Console.Write(topLeft);
            for (int i = borderLeft; i < borderRight; i++)
                Console.Write(horiz);
            Console.SetCursorPosition(borderRight,borderTop);
            Console.Write(topRight);
            for (int i = borderTop+1; i < borderBottom; i++)
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

            return true;
        }
//------------------------------------------------------------------------------
        /// <summary>
        /// Displays the main menu. Pick how to load the population and whether
        /// you want to individually go through generations or just let it go
        /// for a certain number of generations.
        /// </summary>
        /// <param name="validWindowSize">Makes sure the console window
        ///                                          is of adaquate size</param>
        ///                                          
        ///  TODO: Add menu to select preloaded populations
        private static void MainMenu()
        {
            PopType pop = PopType.RANDOM;
            LoadedPops? res = null;
            //int windowCenter = Console.WindowHeight / 2; //Vertical Position
            //int welcomeLeft = (Console.WindowWidth / 2) - 
            //                                (MenuText.Welcome.Length / 2);

            /*
            Console.SetCursorPosition(MenuText.LeftAlign, 8);
            Console.Write(MenuText.Welcome);

            Console.SetCursorPosition(MenuText.LeftAlign, MenuText.WindowCenter - 4);
            Console.Write(MenuText.PlsChoose);

            Console.SetCursorPosition(MenuText.LeftAlign + 4, MenuText.WindowCenter - 3);
            Console.Write(MenuText.PopChoice1);
            Console.SetCursorPosition(MenuText.LeftAlign + 4, MenuText.WindowCenter - 2);
            Console.Write(MenuText.PopChoice2);
            Console.SetCursorPosition(MenuText.LeftAlign + 4, MenuText.WindowCenter - 1);
            Console.Write(MenuText.PopChoice3);*/
            MenuText.PrintMainMenu();

            bool validEntry = false;
            while (!validEntry)
            {
                Console.SetCursorPosition(MenuText.LeftAlign, MenuText.WindowCenter + 2);
                Console.Write(MenuText.Choice);
                Console.CursorVisible = true;
                int input = 
                        (int)Char.GetNumericValue(Console.ReadKey().KeyChar);
                Console.CursorVisible = false;
                switch (input)
                {
                    case 1:
                        pop = PopType.RANDOM;
                        validEntry = true;
                        break;
                    case 2:
                        pop = PopType.FILE;
                        validEntry = true;
                        break;
                    case 3:
                        pop = PopType.PREMADE;
                        res = PromptForRes();
                        if (res != null)
                            validEntry = true;
                        else
                            MenuText.PrintMainMenu();
                        break;
                    default:
                        Console.SetCursorPosition(MenuText.LeftAlign, MenuText.WindowCenter + 3);
                        Console.Write(MenuText.Err);
                        break;
                }
            }
           
            //Clear the current options
            for (int i = -4; i <= 3; i++)
                MenuText.ClearWithinBorder(MenuText.WindowCenter + i);
            
            RunGame(pop,res);
        }
//------------------------------------------------------------------------------
        /// <summary>
        /// This starts the game going by getting the starting population loaded
        /// </summary>
        /// <param name="pop">The type of population to build</param>
        /// <param name="res">Resource to load, if needed</param>
        /// 
        private static void RunGame(PopType pop, LoadedPops? res = null)
        {
            GoLBoard initial = new GoLBoard(CONSOLE_HEIGHT - 10, 
                                                            CONSOLE_WIDTH - 10);
            switch (pop)
            {
                case PopType.RANDOM:
                    initial.BuildDefaultPop();
                    break;
                case PopType.FILE:
                    initial.BuildFromFile();
                    break;
                case PopType.PREMADE:
                    initial.BuildFromResource((LoadedPops)res);
                    break;
            }

            initial.Print();

            GoLRunner.NewRunStyle(initial);

        }
//------------------------------------------------------------------------------
        /// <summary>
        /// TODO: Finish this
        /// </summary>
        /// <returns></returns>
        private static LoadedPops? PromptForRes()
        {
            LoadedPops? retVal = null;
            int windowCenter = Console.WindowHeight / 2;
            for (int i = -4; i <= 3; i++)
                MenuText.ClearWithinBorder(windowCenter + i);

            return retVal;
        }
//------------------------------------------------------------------------------
        /// <summary>
        /// Makes sure the string can be converted to a valid int.
        /// This is used to get the generation to loop to.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /*private static Boolean IsValidNumber(String s)
        {
            try
            {
                int val = Int32.Parse(s);
                if (val >= 0)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }*/
//------------------------------------------------------------------------------
        private static void ResetConsole( int[] initValues)
        {
            int initBuffWidth = initValues[0];
            int initBuffHeight = initValues[1];
            int initConsoleWidth = initValues[2];
            int initConsHeight = initValues[3];
            int initConsolePosLeft = initValues[4];
            int initConsolePosTop = initValues[5];

            MenuText.ClearLine(CONSOLE_HEIGHT - 2);
            Console.SetCursorPosition(0, CONSOLE_HEIGHT - 2);
            Console.Write("Press any key to exit...");
            while (!Console.KeyAvailable)
                System.Threading.Thread.Sleep(50);
            
            Console.SetWindowSize(1, 1);
            Console.SetWindowPosition(initConsolePosLeft, initConsolePosTop);
            Console.SetWindowSize(initConsoleWidth, initConsHeight);
            Console.SetBufferSize(initBuffWidth, initBuffHeight);      
            Console.ResetColor();
            Console.CursorVisible = true;
        }
//------------------------------------------------------------------------------
    } // end class
}