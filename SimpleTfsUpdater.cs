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
            // get tfs helper
            TfsApi tfsapi = new FirstTfsApi(tfs);
            // get testplan
            ITestManagementTeamProject project = tfsapi.GetProject(opts.Get("project"));
            ITestPlan testplan = tfsapi.GetTestPlan(project, opts.Get("testplan"));
            // get configuration
            ITestConfiguration config = tfsapi.GetTestConfiguration(project, opts.Get("config"));
            // create test run
            // add test points to test run
            // save test run
            ITestRun run = tfsapi.CreateRun(
                testplan, 
                opts.Get("run_title"), 
                opts.Get("run_comment"), 
                opts.Get("run_build_number"), 
                opts.Get("run_attachment")
                );
            // find test to update
            ITestCaseResult result = null;
            ITestCaseCollection tests = testplan.RootSuite.AllTestCases;
            for (int i = 0; i < tests.Count; i++)
            {
                if (tests[i].Title == opts.Get("test_name"))
                    result = run.QueryResults()[i];
            }
            if (result == null)
                TfsCliHelper.ExitWithError(string.Format("Could not find testcase {0} inside testplan {1}", opts.Get("test_name"), testplan.Name));
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
    }
}
