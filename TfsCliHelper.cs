using System;
using System.Collections.Generic;
using System.Linq;
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
