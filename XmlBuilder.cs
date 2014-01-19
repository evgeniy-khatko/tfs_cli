using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void Header(ITfsCliOptions opts) {
            _root.Add(
                new XAttribute("url", opts.Get("url")), 
                new XAttribute("project", opts.Get("project")), 
                new XAttribute("testplan", opts.Get("testplan"))
                );
        }

        private void Append(ITestCase test, XElement root)
        {
            XElement xmltest = new XElement("test");
            xmltest.Add(
                new XAttribute("id", test.Id),
                new XAttribute("title", test.Title),
                new XAttribute("description", test.Description ?? ""),
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
                a.Add(
                    new XAttribute("id", action.Id),
                    new XAttribute("string", action.ToString())
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
