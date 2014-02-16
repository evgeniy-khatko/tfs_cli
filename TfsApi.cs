using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace tfs_cli
{
    class TfsApi
    {
        private TfsTeamProjectCollection _tfs;
        private ConnectionData _conData;
        private ITestManagementTeamProject _project;
        private ITestPlan _plan;

        public TfsApi(TfsTeamProjectCollection tfs, ConnectionData con)
        {            
            _tfs = tfs;
            _conData = con;
        }

        public ITestManagementTeamProject GetProject()
        {
            // get tfs project
            try
            {
                ITestManagementService test_service = (ITestManagementService)_tfs.GetService(typeof(ITestManagementService));
                _project = test_service.GetTeamProject(_conData.Project());
                return _project;
            }
            catch (Exception) { TfsCliHelper.ExitWithError(string.Format("Could not get TFS project {0}", _conData.Project())); }
            return null;
        }

        public ITestPlan GetTestPlan()
        {
            if (_project == null)
                GetProject();
            string query = string.Format("SELECT * FROM TestPlan WHERE PlanName = \"{0}\"", _conData.Testplan());
            ITestPlanCollection plans = _project.TestPlans.Query(query);
            if (plans.Count == 0)
                TfsCliHelper.ExitWithError(string.Format("TestPlan {0} not found in project {1}", _conData.Testplan(), _conData.Project()));
            else if (plans.Count > 1)
                TfsCliHelper.ExitWithError("More then one TestPlan found. This situation is currently not supported. Please fill a feature request.");
            _plan = plans.First();
            return _plan;
        }

        public ITestConfiguration GetTestConfiguration(string name)
        {
            if(_project == null)
                GetProject();
            foreach (ITestConfiguration config in _project.TestConfigurations.Query(
                    "Select * from TestConfiguration"))
            {
                if (config.Name == name)
                    return config;
            }
            // otherwise create new
            ITestConfiguration configuration = _project.TestConfigurations.Create();
            configuration.Name = name;
            configuration.Description = "tfs_cli configuration";
            configuration.Values.Add(new KeyValuePair<string, string>("Browser", "IE"));
            configuration.Save();
            return configuration;
        }

        public ITestRun CreateRun(ITestSuiteBase suite, string title, string comment, string buildNum, string attach)
        {            
            if(_plan == null)
                GetTestPlan();
            ITestRun tfsRun = SaveRun(suite, title, buildNum);
            tfsRun.Comment = comment;
            if (attach != null)
            {
                tfsRun.Attachments.Add(tfsRun.CreateAttachment(attach));
            }            
            tfsRun.Save();
            return tfsRun;
        }

        public ITestCaseResult GetTestResult(ITestRun run, string test_name)
        {
            for (int i = 0; i < run.QueryResults().Count; i++)
            {
                ITestCaseResult res = run.QueryResults()[i];
                if(res.GetTestCase().Title == test_name)
                    return res;
            }
            return null;
        }

        public void UpdateTestResult(
            ITestCaseResult result, 
            string outcome, 
            string duration,
            string comment, 
            string failure_type, 
            string error_message, 
            string attach
            )
        {
            try
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
            }
            catch (System.FormatException)
            {
                TfsCliHelper.ExitWithError("Provided parameters are incorrect (e.g. duration is in wrong format)");
            }
            catch (Exception)
            {
                TfsCliHelper.ExitWithError("Error occured while creating test result");
            }
        }

        public List<ITestSuiteBase> GetSuites() 
        {
            if(_plan == null)
                GetTestPlan();
            List<ITestSuiteBase> res = new List<ITestSuiteBase>();
            if (_plan.RootSuite.TestCases.Count > 0)
            {
                res.Add(_plan.RootSuite);
            }            
            GetSuitesRecursive(_plan.RootSuite, res);
            return res;
        }

        public ITestSuiteBase GetSuite(string name)
        {
            if(_plan == null)
                GetTestPlan();
            if (name == null)
                return _plan.RootSuite;
            foreach (ITestSuiteBase suite in GetSuites())
            {
                if (suite.Title == name)
                    return suite;
            }            
            TfsCliHelper.ExitWithError(string.Format("Testsuite \"{0}\" not found inside testplan \"{1}\"", name, _plan.Name));
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

        private ITestRun SaveRun(ITestSuiteBase suite, string title, string buildNum)
        {
            ITestRun tfsRun = _plan.CreateTestRun(false);
            tfsRun.DateStarted = DateTime.Now;
            tfsRun.DateCompleted = DateTime.Now;
            tfsRun.Title = title;
            tfsRun.BuildNumber = buildNum;
            tfsRun.Owner = _tfs.AuthorizedIdentity;
            // Add testpoints to testrun
            tfsRun.AddTestPoints(_plan.QueryTestPoints(string.Format("SELECT * FROM TestPoint WHERE SuiteId = {0}", suite.Id)), _tfs.AuthorizedIdentity);
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
                iteration.Duration = new TimeSpan(10000000 * (int.Parse(duration)));
                iteration.Comment = comment;
                if (attach != null)
                    iteration.Attachments.Add(iteration.CreateAttachment(attach));
                return iteration;
            }
            catch (System.FormatException)
            {
                TfsCliHelper.ExitWithError("Provided parameters are incorrect (e.g. duration is in wrong format)");
            }
            catch (Exception)
            {
                TfsCliHelper.ExitWithError("Error occured while creating test iteration");
            }
            return null;
        }
    }
}
