using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mango
{
    static class MangoDebugger
    {
        public static void ConsoleOut(string Message)
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine(Message);
            }
        }
    }
}
