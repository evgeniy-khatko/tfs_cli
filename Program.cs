#define TRACE
using System;
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
