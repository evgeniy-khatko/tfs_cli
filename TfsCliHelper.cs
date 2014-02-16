using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace tfs_cli
{
    class TfsCliHelper
    {
        public static void ExitWithError(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(1);
        }        
    }
}
