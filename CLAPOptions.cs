using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

       
        public static CLAPOptions getOptions()
        {
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
        /*
        [Global(Description = "TFS collection url, e.g. 'https://my.visualstudio.com/DefaultCollection'")]
        static void url(string value){
            _opts["url"] = value;
        }

        [Global(Description = "TFS project name to get tests from")]
        static void project(string value){
            _opts["project"] = value;
        }
        [Global(Description = "TFS testplan name to get tests from")]
        static void testplan(string value)
        {
            _opts["testplan"] = value;
        }
        */
        [Global(Aliases = "lgn", Description = "TFS user login")]
        static void login(string value)
        {
            _opts["login"] = value;
        }
        [Global(Aliases = "pwd", Description = "TFS user password")]
        static void password(string value)
        {
            _opts["password"] = value;
        }
        [Global(Aliases = "dmn", Description = "TFS user domen")]
        static void domen(string value)
        {
            _opts["domen"] = value;
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
            Description = "Exports TFS tests from provided project and testplan.\nExample: 'g -url=\"https://ekhatko.visualstudio.com/DefaultCollection\" -p=\"cmd\" -t=\"\"bestplan\"\" -lgn=\"evgeniy.khatko@gmail.com\" -pwd=\"Qwerty14\"'"
            )]
        static void get_tests(
            [RequiredAttribute, DescriptionAttribute("TFS collection url, e.g. 'https://my.visualstudio.com/DefaultCollection")]
            string url,
            [RequiredAttribute, DescriptionAttribute("TFS project name to get tests from")]
            string project,
            [RequiredAttribute, DescriptionAttribute("TFS testplan name to get tests from")]
            string testplan,
            [DefaultValue("tests.xml"), DescriptionAttribute("Filename for tests export")]
            string output
            )
        {
            _verb = "get_tests";
            _opts["url"] = url;
            _opts["project"] = project;
            _opts["testplan"] = testplan;
            _opts["output"] = output;
        }

        [Verb(Aliases = "upd", Description = "Updates TFS test with provided attributes.\nExample: ''")]
        static void update_test(
            [RequiredAttribute, DescriptionAttribute("TFS collection url, e.g. 'https://my.visualstudio.com/DefaultCollection")]
            string url,
            [RequiredAttribute, DescriptionAttribute("TFS project name to get tests from")]
            string project,
            [RequiredAttribute, DescriptionAttribute("TFS testplan name to get tests from")]
            string testplan,
            [AliasesAttribute("rc"), DescriptionAttribute("Run configuration"), DefaultValue("tfs_cli")]
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
            [RequiredAttribute, AliasesAttribute("tn"), DescriptionAttribute("Test name")]
            string test_name,
            [RequiredAttribute, AliasesAttribute("to"), DescriptionAttribute("Test outcome (result). Available: Aborted, Blocked, Error, Failed, Inconclusive, None, NotApplicable, NotExecuted, Passed, Paused, Timeout, Unspecified, Warning")]
            string test_outcome,            
            [AliasesAttribute("tc"), DescriptionAttribute("Test comment"), DefaultValue("tfs_cli test run")]
            string test_comment,
            [AliasesAttribute("tft"), DescriptionAttribute("Test failure type. Available: KnowIssue, NewIssue, None, Regression, Unknown")]
            string test_failure_type,
            [AliasesAttribute("tem"), DescriptionAttribute("Test error message"), DefaultValue("")]
            string test_error_message,
            [AliasesAttribute("ta"), DescriptionAttribute("Test attachment. E.g. test run report")]
            [FileExists]
            string test_attachment   
            )
        {
            _verb = "update_test";
            _opts["url"] = url;
            _opts["project"] = project;
            _opts["testplan"] = testplan;
            _opts["run_config"] = run_config;
            _opts["duration"] = duration;
            _opts["run_title"] = run_title;
            _opts["run_build_number"] = run_build_number;
            _opts["run_comment"] = run_comment;
            _opts["run_attachment"] = run_attachment;
            _opts["test_name"] = test_name;
            _opts["test_outcome"] = test_outcome;
            _opts["test_failure_type"] = test_failure_type;
            _opts["test_comment"] = test_comment;
            _opts["test_error_message"] = test_error_message;
            _opts["test_attachment"] = test_attachment;
        }
    }
}
