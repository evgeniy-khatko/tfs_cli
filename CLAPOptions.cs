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
    class CLAPOptions
    {
        private static Configuration configManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private static KeyValueConfigurationCollection _appconf = configManager.AppSettings.Settings;
        private static ConnectionData _conData;
       
        public static void readConfig()
        {
            try
            {
                _conData = new ConnectionData(
                    _appconf["url"].Value,
                    _appconf["project"].Value,
                    _appconf["testplan"].Value,
                    _appconf["login"].Value,
                    StringEncriptor.Decrypt(_appconf["password"].Value),
                    _appconf.AllKeys.Contains("domen") ? _appconf["domen"].Value : null,
                    _appconf.AllKeys.Contains("proxy_login") ? _appconf["proxy_login"].Value : null,
                    _appconf.AllKeys.Contains("proxy_password") ? _appconf["proxy_password"].Value : null,
                    _appconf.AllKeys.Contains("proxy_domen") ? _appconf["proxy_domen"].Value : null
                    );
            }
            catch (Exception e)
            {
                TfsCliHelper.ExitWithError(string.Format(
                    "Set correct parameters in the config file. Make sure you encrypt your password with tfs_cli enc /p=<pwd>\n{0}\n{1}",
                    e.Message,
                    e.StackTrace
            ));
            }
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
            Description = "Exports TFS tests from provided project and testplan. Please fill config file beforehand\nExample simple default usage: tfs_cli get\nExample usage with parameters: tfs_cli get /o=tests.xml /t=testplanname"
            )]
        static void get_tests(            
            [DefaultValue("tests.xml"), DescriptionAttribute("Filename for tests export")]
            string output,
            [AliasesAttribute("tp"), DefaultValue(""), DescriptionAttribute("Testplan to get tests from (overrides .config option)")]
            string testplan
            )
        {              
            if (testplan != "")
                _conData.setTestPlan(testplan);            
            TestsWriter writer = new XmlFileWriter(output, _conData, new XmlBuilder());
            writer.CreateOutput();
        }

        [Verb(Aliases = "features",
            Description = "Exports TFS tests from provided project and testplan to Cucumber features. Please fill config file beforehand\nExample simple default usage: tfs_cli features =\nExample usage with parameters: tfs_cli features /o=/path/to/features /t=testplanname"
            )]
        static void get_features(
            [RequiredAttribute, DescriptionAttribute("Folder for features export")]
            [DirectoryExists]
            string output,
            [AliasesAttribute("tp"), DefaultValue(""), DescriptionAttribute("Testplan to get tests from (overrides .config option)")]
            string testplan
            )
        {
            if (testplan != "")
                _conData.setTestPlan(testplan);
            TestsWriter writer = new FeatureWriter(output, _conData, new FeatureBuilder());
            writer.CreateOutput();
        }

        [Verb(Aliases = "upd", Description = "Updates TFS test with provided values. Please fill config file beforehand\n \nExample simple default usage: tfs_cli.exe u /rt=\"my test run\" /ts=\"testsuite\" /tn=\"test name\" /to=Passed /duration=1\nExample usage with parameters: ?")]
        static void update_test(
            [AliasesAttribute("rconf"), DescriptionAttribute("Run configuration"), DefaultValue("tfs_cli")]
            string run_config,            
            [AliasesAttribute("rt"), RequiredAttribute, DescriptionAttribute("Run title")]
            string run_title,
            [AliasesAttribute("rb"), DescriptionAttribute("Run build number"), DefaultValue("0")]
            string run_build_number,
            [AliasesAttribute("rc"), DescriptionAttribute("Run comment"), DefaultValue("tfs_cli run")]
            string run_comment,
            [AliasesAttribute("ra"), DescriptionAttribute("Run attachment. E.g. overall run report")]
            [FileExists]
            string run_attachment,
            [AliasesAttribute("ts"), DescriptionAttribute("Test suite name")]
            string test_suite_name,
            [RequiredAttribute, AliasesAttribute("tn"), DescriptionAttribute("Test name")]
            string test_name,
            [RequiredAttribute, AliasesAttribute("to"), DescriptionAttribute("Test outcome (result). Available: Aborted, Blocked, Error, Failed, Inconclusive, None, NotApplicable, NotExecuted, Passed, Paused, Timeout, Unspecified, Warning")]
            string test_outcome,            
            [AliasesAttribute("tc"), DescriptionAttribute("Test comment"), DefaultValue("tfs_cli test run")]
            string test_comment,
            [AliasesAttribute("td"), RequiredAttribute, DescriptionAttribute("Test duration (ms)")]
            string duration,
            [AliasesAttribute("tft"), DescriptionAttribute("Test failure type. Available: KnowIssue, NewIssue, None, Regression, Unknown"), DefaultValue("None")]
            string test_failure_type,
            [AliasesAttribute("tem"), DescriptionAttribute("Test error message"), DefaultValue("")]
            string test_error_message,
            [AliasesAttribute("ta"), DescriptionAttribute("Test attachment. E.g. test run report")]
            [FileExists]
            string test_attachment,
            [AliasesAttribute("tp"), DefaultValue(""), DescriptionAttribute("Testplan to get tests from (overrides .config option)")]
            string testplan
            )
        {
            if (testplan != "")
                _conData.setTestPlan(testplan);
            IRunResultProvider RunRes = new RunResultProvider
                (
                run_title,
                run_config,
                run_build_number,
                run_comment,
                run_attachment,
                test_suite_name
                );
            ITestResultProvider TestRes = new TestResultProvider
                (
                test_name,
                test_outcome,
                test_suite_name,
                test_comment,
                test_attachment,
                test_failure_type,
                test_error_message,
                duration
                );
            IList<ITestResultProvider> results = new List<ITestResultProvider>();
            results.Add(TestRes);
            TfsCliUpdater updater = new TfsCliUpdater(RunRes, results, _conData);
            updater.UpdateResults();
        }

        [Verb(Aliases = "ju",
            Description = "Updates TFS test with provided junit report. Please fill config file beforehand\nExample simple default usage: tfs_cli.exe ju /r=/path/to/junit_report\nExample usage with parameters:tfs_cli.exe ju /r=/path/to/junit_report /t=testplanname"
            )]
        static void junit_update(
            [AliasesAttribute("r"), DescriptionAttribute("Junit report file"), RequiredAttribute]
            [FileExists]
            string junit_report,
            [AliasesAttribute("tp"), DefaultValue(""), DescriptionAttribute("Testplan to get tests from (overrides .config option)")]
            string testplan,
            [AliasesAttribute("ra"), DescriptionAttribute("Run attachment. E.g. overall run report")]
            [FileExists]
            string run_attachment
            )
        {
            if (testplan != "")
                _conData.setTestPlan(testplan);
            JunitReportParser jrp = new JunitReportParser(junit_report, run_attachment);
            IList<IRunResultProvider> RunResults = jrp.GetRunResults();
            foreach (IRunResultProvider run in RunResults)
            {
                IList<ITestResultProvider> results = jrp.GetTestResults(run);
                TfsCliUpdater updater = new TfsCliUpdater(run, results, _conData);
                updater.UpdateResults();
            }            
        }

        [Verb(Aliases = "enc", Description = "Encrypts provided string to be used in App.config\nUsage: tfs_cli enc /p=<password>")]
        static void encrypt(
            [RequiredAttribute, DescriptionAttribute("Password to be encrypted")]
            string password
            )
        {
            System.Console.WriteLine(StringEncriptor.Encrypt(password));
            System.Environment.Exit(0);
        }
    }
}
