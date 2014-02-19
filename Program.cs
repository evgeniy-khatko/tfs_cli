using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using CLAP;

namespace tfs_cli
{
    class Program
    {
        static void Main(string[] args)
        {
            TfsCliHelper.Debug(string.Format("Start: \"{0}\"", DateTime.Now.ToLongDateString()));
            // Init provided config
            CLAPOptions.readConfig();
            // Run CLAP app
            Parser.RunConsole<CLAPOptions>(args);         
        }
    }    
}
