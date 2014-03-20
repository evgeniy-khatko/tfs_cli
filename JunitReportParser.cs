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
        public JunitReportParser(string report)
        {
            TfsCliHelper.Debug(string.Format("JunitParseFile: \"{0}\"", report));
            _doc = XDocument.Load(report);
            XElement ts = _doc.Root.Element("testsuite");
            string run_comment = String.Concat(ts.Attributes());
            var testcases = _doc.Root.Element("testsuite").Elements("testcase");
            foreach (var testcase in testcases)
            {
                TfsCliHelper.Debug(string.Format("JunitParseTest: \"{0}\"", testcase.Name));
                // parsing
                string suite = testcase.Attribute("classname").Value;
                string test = testcase.Attribute("name").Value;
                string duration =(1000*(Math.Floor(double.Parse(testcase.Attribute("time").Value)))).ToString();
                bool failed = (testcase.Descendants("failure").Count() > 0);
                string failure_message = "";
                string outcome = (failed) ? "Failed" : "Passed";
                if (failed)
                    failure_message = testcase.Descendants("failure").First().Attribute("message").Value;
                string comment = testcase.Descendants().First().Value;

                // put to dict
                IRunResultProvider run = findRun(suite);
                if (run == null)
                {
                    run = new RunResultProvider(suite, "Autotest", "0", run_comment, report, suite);
                    _runs[run] = new List<ITestResultProvider>();
                    _runs[run].Add(new TestResultProvider(test, outcome, suite, comment, null, "Unknown", failure_message, duration));
                }
                else
                {
                    _runs[run].Add(new TestResultProvider(test, outcome, suite, comment, null, "Unknown", failure_message, duration));
                }
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
