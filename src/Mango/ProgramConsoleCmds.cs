using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Mango
{
    static class ProgramConsoleCmds
    {
        /// <summary>
        /// Parses any console command input.
        /// </summary>
        /// <param name="i">Input command.</param>
        public static void Parse(string i)
        {
            i = i.ToLower();

            switch (i)
            {
                default:
                    Console.WriteLine("You typed an invalid command.");
                    break;

                case "shutdown":
                case "exit":
                    Mango.GetServer().Shutdown();
                    break;

                case "cls":
                case "clear":
                    Console.Clear();
                    break;

                case "online":
                    int Online = Mango.GetServer().GetPlayerManager().Count;
                    Console.WriteLine("Players Online: " + Online);
                    break;
            }
        }
    }
}
