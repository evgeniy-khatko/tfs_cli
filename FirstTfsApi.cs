using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace tfs_cli
{
    class FirstTfsApi : ITfsApi
    {
        private TfsTeamProjectCollection _tfs;

        public FirstTfsApi(TfsTeamProjectCollection tfs)
        {            
            _tfs = tfs;
        }

        public ITestManagementTeamProject GetProject(string name)
        {
            // get tfs project
            try
            {
                ITestManagementService test_service = (ITestManagementService)_tfs.GetService(typeof(ITestManagementService));
                return test_service.GetTeamProject(name);
            }
            catch (Exception) { TfsCliHelper.ExitWithError(string.Format("Could not get TFS project {0}", name)); }
            return null;
        }

        public ITestPlan GetTestPlan(ITestManagementTeamProject project, string testplan_name)
        {            
            string query = string.Format("SELECT * FROM TestPlan WHERE PlanName = \"{0}\"", testplan_name);
            ITestPlanCollection plans = project.TestPlans.Query(query);
            if (plans.Count == 0)
                TfsCliHelper.ExitWithError(string.Format("TestPlan {0} not found in project {1}", testplan_name, project.TeamProjectName));
            else if (plans.Count > 1)
                TfsCliHelper.ExitWithError(string.Format("More then one TestPlan found. This situation is currently not supported. Please fill a bug."));
            return plans.First();
        }

        public ITestConfiguration GetTestConfiguration(ITestManagementTeamProject project, string config_name)
        {            
            foreach (ITestConfiguration config in project.TestConfigurations.Query(
                    "Select * from TestConfiguration"))
            {
                if (config.Name == config_name)
                    return config;
            }
            // otherwise create new
            ITestConfiguration configuration = project.TestConfigurations.Create();
            configuration.Name = config_name;
            configuration.Description = "tfs_cli configuration";
            configuration.Values.Add(new KeyValuePair<string, string>("Browser", "IE"));
            configuration.Save();
            return configuration;
        }

        public ITestRun CreateRun(ITestPlan plan, ITestSuiteBase suite, string title, string comment, string buildNum, string attach)
        {
            ITestRun tfsRun = SaveRun(plan, suite, title, buildNum);
            tfsRun.Comment = comment;
            if (attach != null)
            {
                tfsRun.Attachments.Add(tfsRun.CreateAttachment(attach));
            }            
            tfsRun.Save();
            return tfsRun;
        }

        public String UpdateResult(
            ITestCaseResult result, 
            string outcome, 
            string duration,
            string comment, 
            string failure_type, 
            string error_message, 
            string attach
            )
        {
            result.Outcome = (TestOutcome)Enum.Parse(typeof(TestOutcome), outcome);
            result.Comment = comment;
            result.Owner = _tfs.AuthorizedIdentity;
            result.RunBy = _tfs.AuthorizedIdentity;
            result.DateStarted = DateTime.Now;
            result.Duration = new TimeSpan(10000000 * (int.Parse(duration)));
            result.DateCompleted = DateTime.Now.AddTicks(result.Duration.Ticks);
            result.FailureType = (FailureType)Enum.Parse(typeof(FailureType), failure_type);            
            result.ErrorMessage = error_message;            
            var iteration = CreateIteration(result, outcome, comment, duration, attach);
            result.Iterations.Add(iteration);
            result.State = TestResultState.Completed;        
            result.Save(false);
            return string.Format("Test {0} was updated with {1} through test run {2} (id)", result.TestCaseTitle, outcome, result.TestRunId);
        }

        public List<ITestSuiteBase> GetSuites(ITestPlan plan) 
        {
            List<ITestSuiteBase> res = new List<ITestSuiteBase>();
            if (plan.RootSuite.TestCases.Count > 0)
            {
                res.Add(plan.RootSuite);
            }            
            GetSuitesRecursive(plan.RootSuite, res);
            return res;
        }

        public ITestSuiteBase GetSuite(string name, ITestPlan plan)
        {
            foreach (ITestSuiteBase suite in GetSuites(plan)) {
                if (suite.Title == name)
                    return suite;
            }
            return null;
        }

        private void GetSuitesRecursive(IStaticTestSuite suite, List<ITestSuiteBase> res){
            foreach(ITestSuiteEntry entry in suite.Entries)
            {
                if(entry.EntryType == TestSuiteEntryType.StaticTestSuite)
                {
                    if (entry.TestSuite.AllTestCases.Count > 0)
                    {
                        res.Add(entry.TestSuite);
                        GetSuitesRecursive((IStaticTestSuite)entry.TestSuite, res);
                    }                    
                }
                else if(entry.EntryType == TestSuiteEntryType.DynamicTestSuite)
                {
                    TfsCliHelper.ExitWithError(string.Format("Testplan {0} has dynamic test suite {1}. Dynamic suites are not supported.", suite.Plan.Name, suite.Title));
                }
            }
            return;
        }

        private ITestRun SaveRun(ITestPlan plan, ITestSuiteBase suite, string title, string buildNum)
        {
            ITestRun tfsRun = plan.CreateTestRun(false);
            tfsRun.DateStarted = DateTime.Now;
            tfsRun.DateCompleted = DateTime.Now;
            tfsRun.Title = title;
            tfsRun.BuildNumber = buildNum;
            tfsRun.Owner = _tfs.AuthorizedIdentity;
            // Add testpoints to testrun
            tfsRun.AddTestPoints(plan.QueryTestPoints(string.Format("SELECT * FROM TestPoint WHERE SuiteId = {0}", suite.Id)), _tfs.AuthorizedIdentity);
            // save test run
            tfsRun.Save();
            return tfsRun;
        }

        private ITestIterationResult CreateIteration(ITestCaseResult result, string outcome, string comment, string duration, string attach)
        {
            // create and populate iteration
            try
            {
                var iteration = result.CreateIteration(1);
                iteration.Outcome = (TestOutcome)Enum.Parse(typeof(TestOutcome), outcome);
                iteration.DateStarted = DateTime.Now;
                iteration.DateCompleted = DateTime.Now;
                iteration.Duration = new TimeSpan(10000000*(int.Parse(duration)));
                iteration.Comment = comment;
                if(attach != null)
                    iteration.Attachments.Add(iteration.CreateAttachment(attach));
                return iteration;
            }
            catch (Exception)
            {
                TfsCliHelper.ExitWithError("Error occured while creating test iteration");
            }
            return null;
        }
    }
}
