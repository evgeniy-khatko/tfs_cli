using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tfs_cli
{
    class JunitReportParser
    {
        private XDocument _doc;
        private Dictionary<IRunResultProvider, List<ITestResultProvider>> _runs = new Dictionary<IRunResultProvider, List<ITestResultProvider>>();
        public JunitReportParser(string report, string attach)
        {
            TfsCliHelper.Debug(string.Format("JunitParseFile: \"{0}\"", report));
            _doc = XDocument.Load(report);
            XElement ts = _doc.Element("testsuite");
            
            if (ts == null)
                throw new Exception("Could not parse " + report);
            string run_comment = String.Join(", ", ts.Attributes());
            var testcases = ts.Elements("testcase");
            foreach (var testcase in testcases)
            {                
                // parsing
                string suite = testcase.Attribute("classname").Value;
                string test = testcase.Attribute("name").Value;
                string duration = testcase.Attribute("time").Value.Split('.').First();
                string outcome = "Passed";
                if (testcase.Descendants("failure").Count() > 0)
                    outcome = "Failed";
                if (testcase.Descendants("skipped").Count() > 0)
                    outcome = "Blocked";
                string failure_message = "";                                
                if (outcome == "Failed")
                    failure_message = testcase.Descendants("failure").First().Attribute("message").Value;
                string comment = testcase.Descendants().First().Value;                
                // put to dict
                IRunResultProvider run = findRun(suite);
                if (run == null)
                {
                    run = new RunResultProvider(suite, "Autotest", "0", run_comment, attach, suite);
                    _runs[run] = new List<ITestResultProvider>();                   
                }
                _runs[run].Add(new TestResultProvider(test, outcome, suite, comment, null, "Unknown", failure_message, duration));
                TfsCliHelper.Debug(string.Format("JunitParseTest: {0} -> [suite={1}] [outcome={2}] [duration={3}] [failure={4}] ", 
                    test, suite, outcome, duration, failure_message
                    ));
            }
        }

        public IList<IRunResultProvider> GetRunResults()
        {
            return _runs.Keys.ToList<IRunResultProvider>();
        }

        public IList<ITestResultProvider> GetTestResults(IRunResultProvider run)
        {         
            return _runs[run];            
        }

        private IRunResultProvider findRun(string name){
            foreach (IRunResultProvider run in _runs.Keys)
                if (run.Title() == name)
                    return run;
            return null;
        }
    }
}
