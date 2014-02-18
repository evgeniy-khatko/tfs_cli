using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace tfs_cli
{
    class XmlBuilder : ITfsCliBuilder
    {
        private XElement _root = new XElement("tests");
        public string Finalize() 
        {
            return _root.ToString();
        }

        public XElement DocRoot()
        {
            return _root;
        }

        public void Append(ITestSuiteBase suite)
        {
            XElement xmlsuite = new XElement("testsuite");
            xmlsuite.Add(
                new XAttribute("id", suite.Id),
                new XAttribute("title", suite.Title),
                new XAttribute("description", suite.Description ?? ""),
                new XAttribute("state", suite.State),
                new XAttribute("type", suite.TestSuiteType),
                new XAttribute("user_data", suite.UserData ?? "")
                );
            foreach (ITestSuiteEntry entry in suite.TestCases)
            {
                Append(entry.TestCase, xmlsuite);
            }
            _root.Add(xmlsuite);
        }

        public void Header(string url, string project, string testplan) {
            _root.Add(
                new XAttribute("url", url), 
                new XAttribute("project", project), 
                new XAttribute("testplan", testplan)
                );
        }

        private void Append(ITestCase test, XElement root)
        {
            XElement xmltest = new XElement("test");
            xmltest.Add(
                new XAttribute("id", test.Id),
                new XAttribute("title", test.Title),
                new XAttribute("description", TfsCliHelper.fromHtml(test.Description) ?? ""),
                new XAttribute("state", test.State),
                new XAttribute("priority", test.Priority),
                new XAttribute("owner", test.OwnerName),
                new XAttribute("reason", test.Reason),
                new XAttribute("is_automated", test.IsAutomated),
                new XAttribute("area", test.Area ?? "")
                );
            foreach (ITestAction action in test.Actions)
            {
                XElement a = new XElement("action");
                ITestStep step = (ITestStep)action;
                
                a.Add(
                    new XAttribute("id", action.Id),
                    new XAttribute("action", TfsCliHelper.fromHtml(step.Title)),
                    new XAttribute("expected_result", TfsCliHelper.fromHtml(step.ExpectedResult))
                    );
                xmltest.Add(a);
            }
            /*
            IEnumerator custom = test.CustomFields.GetEnumerator();            
            while (custom.MoveNext())
            {
                xmltest.Add("custom", custom.Current.ToString());
            }
            */
            // add this staff to root
            root.Add(xmltest);
        }        
    }
}
