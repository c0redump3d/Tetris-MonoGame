using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Xna.Framework;
using Tetris.Multiplayer.Network;
using Tetris.Other;

namespace Tetris.GameDebug
{
    public class Debug
    {
        /*
         * 
         * There is nothing particularly important in this class.
         * This is mainly to help with development of this game when
         * adding new features, etc. as it outputs helpful debug messages
         * as well as providing other important things like player position,
         * rotation check, etc.
         * 
         */
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
        
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();
        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        
        private const int SC_CLOSE = 0xF060;
        private const int MF_BYCOMMAND = 0x00000000;
        
        private static int _selection = 0;
        private static bool _enabled = false;
        private static Thread _consoleThread;

        public static void SetUp()
        {
            if (Globals.CurrentOS.Equals("win"))
            {
                AllocConsole(); // open console window
                DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE,
                    MF_BYCOMMAND); // disable ability to close the cmd(We want the user to close the form, not the cmd)
                Console.Title = @"Tetris Debug Console";
                _consoleThread =
                    new Thread(RunConsole); // run our console code in separate thread to prevent gui thread from freezing
                _consoleThread.Start();
            }
            else
            {
                _selection = 1;
            }

            _enabled = true;
        }

        private static void RunConsole()
        {
            Console.WriteLine(@"Tetris Debug Console");
            HelpMessage();
            try
            {
                while (_enabled)
                {
                    string command = Console.ReadLine(); // wait for user input
                    RegisterCommand(command); // attempt to run the sent input
                }
            }
            catch (Exception ex)
            {
                if(ex is ThreadInterruptedException)
                    Console.WriteLine(@"Shutting down...");
            }
        }

        /// <summary>
        /// Prints all commands to the window.
        /// </summary>
        private static void HelpMessage()
        {
            Console.WriteLine(@"~=~=~=~=~=~=~=~=~=~=~=~=~=~=COMMANDS~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~");
            Console.WriteLine(@"| debug - Will output debug messages.                                   |");
            Console.WriteLine(@"| level (int) - Will set the level in game.                             |");
            Console.WriteLine(@"| end - Will end the current game.                                      |");
            Console.WriteLine(@"| clear *(game) - Will clear either the command prompt or game.         |");
            Console.WriteLine(@"| help - Will show this message again.                                  |");
            Console.WriteLine(@"| startserver - Will start running a server on port 25565.              |");
            Console.WriteLine(@"| connect (ip) - Will attempt to connect to a server with the given ip. |");
            Console.WriteLine(@"| disconnect - Disconnects from the connected client/server.            |");
            Console.WriteLine(@"| exit - Will exit the game.                                            |");
            Console.WriteLine(@"~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~=~");
        }

        private static void RegisterCommand(string cmd)
        {
            bool containsExtra = false; // if the commands contains extra information (ex: level 1)
            string cmdResponse = "[" + DateTime.Now.ToShortTimeString() + "] TETRIS_CMD: ";
            string actualCmd = ""; // will be the first message sent(before space)
            string inputCmd = ""; // will be the information provided with the command if there

            if (cmd.Contains(" ")) // if there is a space, there is extra data
                containsExtra = true;

            if (containsExtra)
            {
                actualCmd = cmd.Split(' ')[0];
                inputCmd = cmd.Split(' ')[1];
            }
            else
            {
                actualCmd = cmd;
            }

            try
            {
                switch (actualCmd) // This could be cleaned up by doing something similar to how packets work, but seems unnecessary as this is for debugging.
                {
                    case "debug":
                        _selection = 1;
                        Console.WriteLine($@"{cmdResponse} Console will now display debug messages.");
                        break;
                    case "help":
                        HelpMessage();
                        break;
                    case "level":
                        Instance.GetScoreHandler().Level = int.Parse(inputCmd);
                        Instance.GetSound().StopMusic();
                        Instance.GetSound().PlayMusic(Instance.GetScoreHandler().Level);
                        Console.WriteLine($@"{cmdResponse} Set level to: {inputCmd}");
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    case "end":
                        Instance.GetGame().EndGame();
                        Console.WriteLine($@"{cmdResponse} Ended Game.");
                        break;
                    case "clear":
                        if (inputCmd == "game")
                        {
                            Instance.GetPlayer().PlacedRect = new Rectangle[2];
                            Console.WriteLine($@"{cmdResponse} Cleared board.");
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine($@"{cmdResponse} Cleared screen.");
                        }
                        break;
                    case "connect":
                        if (inputCmd.Equals(""))
                        {
                            Console.WriteLine(@"Please type an IP address you would like to connect to. (ex: connect 127.0.0.1)");
                            break;
                        }
                    
                        Console.WriteLine($@"{cmdResponse} Connecting to host: {inputCmd}");
                        new Client(inputCmd);
                        break;
                    case "startserver":
                        Console.WriteLine($@"{cmdResponse} Starting server...");
                        new Server();
                        break;
                    case "disconnect":
                        if (Server.ConnectionEstablished() || Client.IsConnected())
                        {
                            Instance.GetPacket().SendPacketFromName("dis"); // let the server/client know we are disconnecting
                            Instance.GetMultiplayerHandler().HideMultiplayer(); // hide the multiplayer screen(needs to be invoked from gui thread)
                            Console.WriteLine($@"{cmdResponse} Disconnecting...");
                            Instance.GetPacket().RunPacketFromName("dis"); // run the disconnect packet on our side.
                        }
                        else
                        {
                            Console.WriteLine(@"You are not currently connected to any player.");
                        }
                        break;
                    default:
                        Console.WriteLine(cmdResponse + $@"Unknown command: '{actualCmd}', type help for a list of commands.");
                        break;
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Stops the console thread and closes console.
        /// </summary>
        public static void CloseConsole()
        {
            _enabled = false;
            _consoleThread = null;
            FreeConsole();
        }
        
        /// <summary>
        /// Returns the current output selection.
        /// </summary>
        /// <returns>Current selection(1 = debug, 2 = board)</returns>
        public static int GetSelection()
        {
            return _selection;
        }

        /// <summary>
        /// Returns true if console window has been opened.
        /// </summary>
        public static bool IsEnabled()
        {
            return _enabled;
        }

        /// <summary>
        /// Print the given message to the debug console window if open.
        /// </summary>
        /// <param name="message">Message to print to console</param>
        /// <param name="mode">Modes: 1(debug), 2(board)(removed), 3(Server), 4(Client)</param>
        /// <param name="critical">If true, will print message in red.</param>
        /// <param name="caller"></param>
        public static void DebugMessage(string message, int mode, bool critical = false, [CallerMemberName] string caller = "")
        {
            if (!_enabled || !Globals.CurrentOS.Equals("win"))
                return;

            if (critical)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                message.Insert(0, "CRITICAL: ");
            }
            else
                Console.ForegroundColor = ConsoleColor.Gray;

            if (_selection == 1 && mode == 1)
                Console.WriteLine($@"[{DateTime.Now.ToShortTimeString()}] TETRIS_DEBUG({caller}): {message}");
            else if (_selection == 2 && mode == 2)
            {
                Console.Clear();
                Console.WriteLine(message);
            }else if(mode == 3)
            {
                Console.WriteLine($@"[{DateTime.Now.ToShortTimeString()}] TETRIS_SERVER: {message}");
            }
            else if (mode == 4)
            {
                Console.WriteLine($@"[{DateTime.Now.ToShortTimeString()}] TETRIS_CLIENT: {message}");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

    }
}