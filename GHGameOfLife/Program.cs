﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;


namespace GHGameOfLife
{
///////////////////////////////////////////////////////////////////////////////
    class Program
    {
        // Don't go below these values or the text will be screwy
        static int Min_Cols = 100;
        static int Min_Rows = 30;
        // Don't go below these values or the text will be screwy

        static int Current_Cols, Current_Rows;
        static int Max_Cols = 175;
        static int Max_Rows = 52;  
  
        static int Num_Sizes = 3;  // The amount of different sizes allowed
        static BoardSize[] Valid_Sizes = new BoardSize[Num_Sizes];
        static int Curr_Size_Index = 1; // Which size to default to, 1 is med
//------------------------------------------------------------------------------
        [STAThread]
        static void Main(string[] args)
        {
            
            int initBuffWidth = Console.BufferWidth;
            int initBuffHeight = Console.BufferHeight;
            int initConsWidth = Console.WindowWidth;
            int initConsHeight = Console.WindowHeight;
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            int[] initialValues = new int[] { initBuffWidth, initBuffHeight, 
                                              initConsWidth, initConsHeight };     

            InitializeConsole();
            bool exit = false;
            do
            {
                MainMenu();
                //NewMenu();

                MenuText.PromptForAnother();
                bool validKey = false;             
                while (!validKey)
                {
                    while (!Console.KeyAvailable)
                        System.Threading.Thread.Sleep(50);

                    ConsoleKey pressed = Console.ReadKey(true).Key;
                    if (pressed == ConsoleKey.Escape)
                    {
                        exit = true;
                        validKey = true;
                    }
                    if (pressed == ConsoleKey.Enter)
                    {
                        validKey = true;
                    }

                }
                

            } while (!exit);
            
            ResetConsole(initialValues);
        }
//------------------------------------------------------------------------------
        /// <summary>
        /// Initializes the console for display. 
        /// </summary>
        private static void InitializeConsole()
        {
            Console.OutputEncoding = Encoding.Unicode;
            MenuText mt = new MenuText();
            Console.BackgroundColor = MenuText.Default_BG;
            Console.ForegroundColor = MenuText.Default_FG;
            Console.Title = "Ian's Conway's Game of Life";        
            
            
            Max_Cols = (Console.LargestWindowWidth < Max_Cols)? Console.LargestWindowWidth : Max_Cols;
            Max_Rows = (Console.LargestWindowHeight < Max_Rows)? Console.LargestWindowHeight : Max_Rows;

            int difWid = (Max_Cols - Min_Cols) / (Num_Sizes - 1);
            int difHeight = Math.Max(1, (Max_Rows - Min_Rows) / (Num_Sizes - 1));

            // Initialize with the smallest window size and build from there
            // keeping the window ratio near that of the max window size ratio
            // I am just moving the width because we have more play there than height
            // unless you have some weird portrait set up I guess then enjoy your
            // small windows??

            BoardSize max = new BoardSize(Max_Cols, Max_Rows);
            double consRatio = max.Ratio;

            // Don't actually allow use of the full area to account for something probably
            Valid_Sizes[Num_Sizes - 1] = new BoardSize(Max_Cols, Max_Rows - 4);
            for (int i = Num_Sizes - 2; i >= 0; i--)
            {
                int tempCols = Math.Max(Min_Cols, Valid_Sizes[i + 1].Cols - difWid);
                int tempRows = Math.Max(Min_Rows, Valid_Sizes[i + 1].Rows - difHeight);
                Valid_Sizes[i] = new BoardSize(tempCols, tempRows);
            }


            // Check the ratios and adjust as needed
            foreach (BoardSize cs in Valid_Sizes)
            {
                while (cs.Ratio > consRatio)
                {
                    if ((cs.Cols - 1) <= Min_Cols)
                    {
                        cs.Rows = Math.Min(Max_Rows, cs.Rows + 1);
                    }
                    else
                    {
                        cs.Cols = Math.Max(Min_Cols, cs.Cols - 1);
                    }
                }

                while (cs.Ratio < consRatio)
                {
                    if ((cs.Cols + 1) >= Max_Cols)
                    {
                        cs.Rows = Math.Max(Min_Rows, cs.Rows = cs.Rows - 1);
                    }
                    else
                    {
                        cs.Cols = Math.Min(Max_Cols, (cs.Cols + 1));
                    }

                }
            }


            AdjustWindowSize(Valid_Sizes[Curr_Size_Index]);
            Current_Rows = Console.WindowHeight;
            Current_Cols = Console.WindowWidth;
            MenuText.Initialize();
            MenuText.DrawBorder();
        }
//------------------------------------------------------------------------------
        /// <summary>
        /// Displays the main menu for starting the game running.
        /// TODO: Change the look of this whole thing:
        ///         Prompt for 1d or 2d
        ///             if 1d   -> prompt for which rule
        ///                     -> prompt for type of starting population
        ///             if 2d   -> prompt for which rule (once implemented)
        ///                     -> prompt for type of starting population
        ///                     -> prompt for resource if needed
        /// </summary>
        private static void NewMenu()
        {
            var typePrompts = new string[] {"1) 1D Automata",
                                            "2) 2D Automata",
                                            "3) Exit"};

            var ruleTypes1D = Enum.GetValues(typeof(Automata1D.RuleTypes));
            var ruleTypes1DStrings = MenuText.EnumToChoiceStrings(ruleTypes1D);

            var initTypes1D = Enum.GetValues(typeof(Automata1D.BuildTypes));
            var initTypes1DStrings = MenuText.EnumToChoiceStrings(initTypes1D);

            var ruleTypes2D = Enum.GetValues(typeof(Automata2D.RuleTypes));
            var ruleTypes2DStrings = MenuText.EnumToChoiceStrings(ruleTypes2D);

            var initTypes2D = Enum.GetValues(typeof(Automata2D.BuildTypes));
            var initTypes2DStrings = MenuText.EnumToChoiceStrings(initTypes2D);

            var promptRow = MenuText.PrintMenuFromList(typePrompts);
            var numChoices = typePrompts.Length;
            var choice = -1;

            var inputFinished = false;
            var validEntry = false;
            var consoleResized = false;
            //promptRow changes when we resize the console window
            var newPromptRow = promptRow;

            while (!inputFinished)
            {
                if(consoleResized)
                {
                    promptRow = newPromptRow;
                }
                consoleResized = false;
                Console.CursorVisible = true;
                MenuText.ClearWithinBorder(promptRow);
                Console.SetCursorPosition(MenuText.Left_Align, promptRow);
                Console.Write(MenuText.Prompt);
                var userInput = "";
                var maxInputLen = 1;
                var readingInput = true;
                while(readingInput)
                {
                    var keyInfo = Console.ReadKey(true);
                    var charIn = keyInfo.KeyChar;
                    if (charIn == '\r')
                    {
                        readingInput = false;
                        break;
                    }
                    if (charIn == '\b')
                    {
                        if (userInput != "")
                        {
                            userInput = userInput.Substring(0, userInput.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                    if(keyInfo.Modifiers == ConsoleModifiers.Control)
                    {
                        switch(keyInfo.Key)
                        {
                            case ConsoleKey.OemPlus:
                            case ConsoleKey.Add:
                                if (Curr_Size_Index < Valid_Sizes.Count() - 1)
                                {
                                    Curr_Size_Index++;
                                }
                                newPromptRow = ReInitializeConsoleWithPrompts(typePrompts);
                                consoleResized = true;
                                break;
                            case ConsoleKey.OemMinus:
                            case ConsoleKey.Subtract:
                                if (Curr_Size_Index > 0)
                                {
                                    Curr_Size_Index--;
                                }
                                newPromptRow = ReInitializeConsoleWithPrompts(typePrompts);
                                consoleResized = true;
                                break;
                        }
                        if(consoleResized)
                        {
                            break;
                        }
                    }
                    if (!(""+charIn).All(c => char.IsLetterOrDigit(c)))
                    {
                        //Ignore input that is not a letter or digit
                    }
                    else if (userInput.Length < maxInputLen)
                    {
                        Console.Write(charIn);
                        userInput += charIn;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(50);
                    }
                }
                inputFinished = true;
            }
        }
//------------------------------------------------------------------------------
        /// <summary>
        /// Displays the main menu. Pick how to load the population.
        /// Display the choice and ask for confirmation instead of just
        /// jumping to the next screen incase someone hits a wrong button
        /// </summary>
        ///                                          
        private static void MainMenu()
        {
            var buildType = Automata2D.BuildTypes.Random;
            string res = null;

            int numChoices = MenuText.Menu_Choices.Count();
            int currPromptRow = MenuText.PrintMainMenu();
            int choice = -1;

            //Only allow letters and numbers to be written as a choice
            //TODO: Add this to the resource selection menu
            string allCharDec =  "abcdefghijklmnopqrstuvwxyz1234567890";

            bool validEntry = false;
            int newPromptRow = currPromptRow;
            bool resized = false;
            var tryAuto = false;
            while (!validEntry)
            {
                if (resized)
                    currPromptRow = newPromptRow;

                resized = false;
                Console.CursorVisible = true;
                MenuText.ClearWithinBorder(currPromptRow);
                Console.SetCursorPosition(MenuText.Left_Align, currPromptRow);
                Console.Write(MenuText.Prompt);                   

                string input = "";
                int maxLen = 1;

                while (true)
                {
                    ConsoleKeyInfo cki = Console.ReadKey(true);
                    char c = cki.KeyChar;
                    if (c == '\r')
                        break;
                    if (c == '\b')
                    {
                        if (input != "")
                        {
                            input = input.Substring(0, input.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                    //Need to reinitialize the console/menu positioning after changing window size
                    else if ((cki.Key == ConsoleKey.OemPlus || cki.Key == ConsoleKey.Add) && cki.Modifiers == ConsoleModifiers.Control)
                    {
                        if (Curr_Size_Index < Valid_Sizes.Count() - 1)
                        {
                            Curr_Size_Index++;
                        }
                        newPromptRow = ReInitializeConsole();
                        resized = true;
                        break;
                    }
                    else if ((cki.Key == ConsoleKey.OemMinus || cki.Key == ConsoleKey.Subtract) && cki.Modifiers == ConsoleModifiers.Control)
                    {
                        if (Curr_Size_Index > 0)
                        {
                            Curr_Size_Index--;
                        }
                        newPromptRow = ReInitializeConsole();
                        resized = true;
                        break;
                    }
                    else if (!allCharDec.Contains(c))
                    {
                        //This is here so these characters are not written
                    }
                    else if (input.Length < maxLen)
                    {
                        Console.Write(c);
                        input += c;
                    }
                    else
                        System.Threading.Thread.Sleep(50);
                }

                if (resized)
                    continue;
               
                if (IsValidNumber(input, numChoices))
                {
                    choice = Int32.Parse(input);
                    validEntry = true;
                }
                else
                {
                    Console.SetCursorPosition(MenuText.Left_Align, currPromptRow + 1);
                    Console.Write(MenuText.Entry_Error);
                    continue;
                }

                Console.CursorVisible = false;
                switch (choice)
                {
                    case 1:
                        buildType = Automata2D.BuildTypes.Random;
                        validEntry = true;
                        break;
                    case 2:
                        buildType = Automata2D.BuildTypes.File;
                        validEntry = true;
                        break;
                    case 3:
                        //Clear the line telling you how to change window size
                        MenuText.ClearLine((Console.WindowHeight) - 4);
                        buildType = Automata2D.BuildTypes.Resource;
                        res = PromptForRes();
                        if (res != null)
                            validEntry = true;
                        else
                        {
                            MenuText.PrintMainMenu();
                            validEntry = false;
                        }
                        break;
                    case 4:
                        buildType = Automata2D.BuildTypes.User;
                        validEntry = true;
                        break;
                    case 5:
                        validEntry = true;
                        tryAuto = true;
                        break;
                    case 6:
                        validEntry = true;
                        return;
                    default:
                        Console.SetCursorPosition(MenuText.Left_Align, currPromptRow + 2);
                        Console.Write(MenuText.Entry_Error);
                        validEntry = false;
                        break;
                }
            }

            if(tryAuto)
            {
                MenuText.ClearAllInBorder();
                var autoBoard = Automata1D.InitializeAutomata(Current_Rows - 10,Current_Cols - 10,Automata1D.BuildTypes.Single,Automata1D.RuleTypes.Rule90);
                ConsoleRunHelper.ConsoleAutomataRunner(autoBoard);
            }
            else
            {
                //Clear the current options
                MenuText.ClearAllInBorder();

                //Move out into the main loop maybe
                Automata2D game = Automata2D.InitializeAutomata(Current_Rows-10, Current_Cols-10, buildType, Automata2D.RuleTypes.Life, res);
                ConsoleRunHelper.ConsoleAutomataRunner(game);
            }
           
        }
//------------------------------------------------------------------------------
        /// <summary>
        /// Display a list of all resources built in to the program
        /// TODO: Probably make this better support more resources or something
        /// </summary>
        /// <returns>The string key value of the resource to load</returns>
        private static string PromptForRes()
        {
            string retVal = null;
            int numRes = MenuText.Large_Pops.Count;

            int promptRow = 0;
            int pageIndex = 0;
            bool reprintPage = true;
            bool onLastPage = (MenuText.Large_Pops_Pages.Count == 1);
            bool onFirstPage = true;
            List<string> currPage = null;
            
            
            bool go = true;
            while (go)
            {
                if( reprintPage )
                {
                    MenuText.ClearAllInBorder();
                    currPage = (List<string>)MenuText.Large_Pops_Pages[pageIndex];
                    promptRow = MenuText.PrintResourceMenu(currPage,onLastPage,onFirstPage);                   
                }
                reprintPage = false;

                Console.SetCursorPosition(MenuText.Left_Align, promptRow);
                Console.Write(MenuText.Prompt);              
                Console.CursorVisible = true;

                string input = "";
                int maxLen = 1;
                while (true)
                {
                    char c = Console.ReadKey(true).KeyChar;
                    if (c == '\r')
                        break;
                    if (c == '\b')
                    {
                        if (input != "")
                        {
                            input = input.Substring(0, input.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                    else if (input.Length < maxLen)
                    {
                        Console.Write(c);
                        input += c;
                    }
                }

                switch (input)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                        MenuText.ClearWithinBorder(promptRow + 1);
                        int keyVal = Int32.Parse(input);
                        if (keyVal <= currPage.Count)
                            retVal = MenuText.Large_Pops[(pageIndex * 7) + keyVal - 1];
                        go = false;
                        break;
                    case "8":
                        MenuText.ClearWithinBorder(promptRow + 1);
                        if (!onFirstPage)
                        {
                            --pageIndex;
                            reprintPage = true;
                            onLastPage = false;
                            if (pageIndex == 0)
                                onFirstPage = true;
                        }
                        break;
                    case "9":
                        MenuText.ClearWithinBorder(promptRow + 1);
                        if (!onLastPage)
                        {
                            ++pageIndex;
                            reprintPage = true;
                            onFirstPage = false;
                            if (pageIndex == MenuText.Large_Pops_Pages.Count - 1)
                                onLastPage = true;
                        }
                        break;
                    case "0":
                        MenuText.ClearWithinBorder(promptRow + 1);
                        go = false;
                        break;
                    default:
                        Console.SetCursorPosition(MenuText.Left_Align, promptRow+1);
                        Console.Write(MenuText.Entry_Error);
                        break;
                }
            }

            Console.CursorVisible = false;
            return retVal;
        }
//------------------------------------------------------------------------------
        /// <summary>
        /// Reinitialize the console after resizing
        /// </summary>
        /// <returns>Returns the new row to print the response text on</returns>
        /// TODO: This does not seem to get run after having the user create their own population....?
        private static int ReInitializeConsole()
        {
            Console.Clear();
            AdjustWindowSize(Valid_Sizes[Curr_Size_Index]);

            Current_Rows = Console.WindowHeight;
            Current_Cols = Console.WindowWidth;
            MenuText.ReInitialize();
            MenuText.DrawBorder();
            return MenuText.PrintMainMenu();            
        }
//------------------------------------------------------------------------------
        /// <summary>
        /// Makes sure the string can be converted to a valid int.
        /// Also makes sure it is in range of the number of choices presented
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool IsValidNumber(string s, int numPrinted)
        {
            try
            {
                int val = Int32.Parse(s);
                if (val > 0 && val <= numPrinted)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
//------------------------------------------------------------------------------
        private static void ResetConsole(int[] initValues)
        {
            int initBuffWidth = initValues[0];
            int initBuffHeight = initValues[1];
            int initConsoleWidth = initValues[2];
            int initConsHeight = initValues[3];

            MenuText.ClearLine(Current_Rows - 2);
            Console.SetCursorPosition(0, Current_Rows - 2);
                        
            Console.SetWindowSize(1, 1);
            Console.SetWindowPosition(0, 0);
            Console.SetWindowSize(initConsoleWidth, initConsHeight);
            Console.SetBufferSize(initBuffWidth, initBuffHeight);      
            Console.ResetColor();
            Console.CursorVisible = true;
        }
//------------------------------------------------------------------------------
        private static void AdjustWindowSize(BoardSize size)
        {              
            //Resize the console window
            Console.SetWindowSize(1, 1);
            Console.SetWindowPosition(0, 0);
            Console.SetBufferSize(size.Cols, size.Rows);
            Console.SetWindowSize(size.Cols, size.Rows);
            Console.SetCursorPosition(0, 0);
        }
//------------------------------------------------------------------------------
        /// <summary>
        /// Reinitialize the console after resizing
        /// </summary>
        /// <returns>Returns the new row to print the response text on</returns>
        /// TODO: This does not seem to get run after having the user create their own population....?
        private static int ReInitializeConsoleWithPrompts(IEnumerable<string> prompts)
        {
            Console.Clear();
            AdjustWindowSize(Valid_Sizes[Curr_Size_Index]);

            Current_Rows = Console.WindowHeight;
            Current_Cols = Console.WindowWidth;
            MenuText.ReInitialize();
            MenuText.DrawBorder();
            return MenuText.PrintMenuFromList(prompts);
        }
//------------------------------------------------------------------------------
    } // end class
//------------------------------------------------------------------------------
////////////////////////////////////////////////////////////////////////////////
} 