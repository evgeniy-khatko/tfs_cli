using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using CLAP;
using CLAP.Validation;

namespace tfs_cli
{
    class CLAPOptions : ITfsCliOptions
    {
        private static CLAPOptions _instance;
        private static IDictionary<string, string> _opts = new Dictionary<string, string>();
        private static string _verb;
        private static Configuration configManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private static KeyValueConfigurationCollection _appconf = configManager.AppSettings.Settings;
       
        public static CLAPOptions getOptions()
        {
            try
            {
                _opts["url"] = _appconf["url"].Value;
                _opts["project"] = _appconf["project"].Value;
                _opts["testplan"] = _appconf["testplan"].Value;
                _opts["login"] = _appconf["login"].Value;
                _opts["password"] = StringEncriptor.Decrypt(_appconf["password"].Value);
                _opts["domen"] = (_appconf.AllKeys.Contains("domen")) ? _appconf["domen"].Value : null;
            }
            catch (Exception e)
            {
                TfsCliHelper.ExitWithError(string.Format(
                    "Set correct parameters in the config file. Make sure you encrypt your password with tfs_cli enc <pwd>\n{0}\n{1}",
                    e.Message,
                    e.StackTrace
            ));
            }
            if (_instance == null)
            {
                _instance = new CLAPOptions();
                return _instance;
            }
            else
                return _instance;
        }
        public IDictionary<string, string> GetAll() 
        {
            return _opts;
        }
        public string Get(string key) 
        {
            string res = "";
            _opts.TryGetValue(key, out res);
            return res;
        }
        public string ProvidedVerb()
        {
            return _verb;
        }

        [Empty]
        static void NoInput()
        {
            Console.WriteLine(
@"This is command line utility to get tests from TFS or to update results of tests inside TFS.
Try -h option for more."
            );            
        }

        [Help]
        static void help()
        {
            Console.WriteLine("Help:");
        }

        [Verb(Aliases = "get", 
            Description = "Exports TFS tests from provided project and testplan. Please fill config file beforehand\nExample usage: tfs_cli get"
            )]
        static void get_tests(
            [DefaultValue("tests.xml"), DescriptionAttribute("Filename for tests export")]
            string output
            )
        {
            _verb = "get_tests";
            _opts["output"] = output;
        }

        [Verb(Aliases = "upd", Description = "Updates TFS test with provided attributes. Please fill config file beforehand\nExample usage: tfs_cli.exe u /rt=\"my test run\" /ts=\"testsuite\" /tn=\"test name\" /to=Passed /duration=1")]
        static void update_test(
            [AliasesAttribute("rconf"), DescriptionAttribute("Run configuration"), DefaultValue("tfs_cli")]
            string run_config,
            [AliasesAttribute("rd"), RequiredAttribute, DescriptionAttribute("Run duration")]
            string duration,
            [AliasesAttribute("rt"), RequiredAttribute, DescriptionAttribute("Run title")]
            string run_title,
            [AliasesAttribute("rb"), DescriptionAttribute("Run build number"), DefaultValue("0")]
            string run_build_number,
            [AliasesAttribute("rc"), DescriptionAttribute("Run comment"), DefaultValue("tfs_cli run")]
            string run_comment,
            [AliasesAttribute("ra"), DescriptionAttribute("Run attachment. E.g. overall run report")]
            [FileExists]
            string run_attachment,
            [RequiredAttribute, AliasesAttribute("ts"), DescriptionAttribute("Test suite name")]
            string test_suite_name,
            [RequiredAttribute, AliasesAttribute("tn"), DescriptionAttribute("Test name")]
            string test_name,
            [RequiredAttribute, AliasesAttribute("to"), DescriptionAttribute("Test outcome (result). Available: Aborted, Blocked, Error, Failed, Inconclusive, None, NotApplicable, NotExecuted, Passed, Paused, Timeout, Unspecified, Warning")]
            string test_outcome,            
            [AliasesAttribute("tc"), DescriptionAttribute("Test comment"), DefaultValue("tfs_cli test run")]
            string test_comment,
            [AliasesAttribute("tft"), DescriptionAttribute("Test failure type. Available: KnowIssue, NewIssue, None, Regression, Unknown"), DefaultValue("None")]
            string test_failure_type,
            [AliasesAttribute("tem"), DescriptionAttribute("Test error message"), DefaultValue("")]
            string test_error_message,
            [AliasesAttribute("ta"), DescriptionAttribute("Test attachment. E.g. test run report")]
            [FileExists]
            string test_attachment   
            )
        {
            _verb = "update_test";
            _opts["run_config"] = run_config;
            _opts["duration"] = duration;
            _opts["run_title"] = run_title;
            _opts["run_build_number"] = run_build_number;
            _opts["run_comment"] = run_comment;
            _opts["run_attachment"] = run_attachment;
            _opts["test_name"] = test_name;
            _opts["test_suite_name"] = test_suite_name;
            _opts["test_outcome"] = test_outcome;
            _opts["test_failure_type"] = test_failure_type;
            _opts["test_comment"] = test_comment;
            _opts["test_error_message"] = test_error_message;
            _opts["test_attachment"] = test_attachment;
        }

        [Verb(Aliases = "enc", Description = "Encrypts provided string to be used in App.config\nUsage: tfs_cli enc <password>")]
        static void encrypt(
            [RequiredAttribute, DescriptionAttribute("Password to be encrypted")]
            string password
            )
        {
            _verb = "encrypt";
            System.Console.WriteLine(StringEncriptor.Encrypt(password));
            System.Environment.Exit(0);
        }
    }
}
