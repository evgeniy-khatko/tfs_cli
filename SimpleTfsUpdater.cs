using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tfs_cli
{
    class SimpleTfsUpdater : ITfsCliUpdater 
    {
        public string UpdateTest(TfsTeamProjectCollection tfs, ITfsCliOptions opts)
        {
            // get tfs api helper
            ITfsApi tfsapi = new FirstTfsApi(tfs);
            // get testplan
            ITestManagementTeamProject project = tfsapi.GetProject(opts.Get("project"));
            ITestPlan testplan = tfsapi.GetTestPlan(project, opts.Get("testplan"));
            // get testsuite
            string suitename = (opts.Get("test_suite_name") == null) ? opts.Get("testplan") : opts.Get("test_suite_name");
            ITestSuiteBase testsuite = tfsapi.GetSuite(suitename, testplan);
            if (testsuite == null)
                TfsCliHelper.ExitWithError(string.Format(
                    "Test suite {0} not found in testplan {1}",
                    suitename,
                    testplan.Name
                    ));
            // get configuration
            ITestConfiguration config = tfsapi.GetTestConfiguration(project, opts.Get("run_config"));
            // create test run
            // add test points to test run
            // save test run
            ITestRun run = tfsapi.CreateRun(
                testplan,
                testsuite,
                opts.Get("run_title"), 
                opts.Get("run_comment"), 
                opts.Get("run_build_number"), 
                opts.Get("run_attachment")
                );
            // find test to update
            ITestCaseResult result = null;            
            ITestCaseCollection tests = testsuite.AllTestCases;
            for (int i = 0; i < tests.Count; i++)
            {
                if (tests[i].Title == opts.Get("test_name"))
                    result = run.QueryResults()[i];
            }
            if (result == null)
                TfsCliHelper.ExitWithError(string.Format("Could not find testcase {0} inside testsuite {1}", opts.Get("test_name"), testsuite.Title));
            // update test reults
            // create and populate test iteration
            // save test results
            return tfsapi.UpdateResult(
                result,
                opts.Get("test_outcome"),
                opts.Get("duration"),
                opts.Get("test_comment"),
                opts.Get("test_failure_type"),
                opts.Get("test_error_message"),
                opts.Get("test_attachment")
                );
        }  
     
        public string UpdateFromJunit(TfsTeamProjectCollection tfs, ITfsCliOptions opts)
        {
            return "TODO";
        }
    }
}
