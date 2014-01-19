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
            // Init provided options
            Parser.RunConsole<CLAPOptions>(args);
            var options = CLAPOptions.getOptions();
            if (options.GetAll().Count == 0) { Environment.Exit(1); }
                      
            // Try to connect first
            var cred = (options.Get("login") != null && options.Get("password") != null) ?
            new NetworkCredential(options.Get("login"), options.Get("password"), options.Get("domen")) :
            null;
            var connector = new CredentialConnector(new Uri(options.Get("url")), cred);
            var updater = new SimpleTfsUpdater();
            var writer = new XmlFileWriter();
            if (connector.Connect())
            {
                // Following depends on CommandLine verb    
                try
                {
                    if (options.ProvidedVerb() == "get_tests")
                    {
                        writer.CreateOutput(connector.GetTfs(), options, new XmlBuilder());
                    }
                    else if (options.ProvidedVerb() == "update_test")
                    {
                        updater.UpdateTest(connector.GetTfs(), options);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Runtime error occured: {0}\n{1}", e.Message, e.StackTrace);
                }
            }
            else { Console.WriteLine("Could not connect to TFS. Check provided data and Internet connection."); }
         }
    }    
}
