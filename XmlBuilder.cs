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
        public void Append(ITestCase test) {
            XElement xmltest = new XElement(
                "test",
                new XElement("id", test.Id),
                new XElement("title", test.Title),
                new XElement("description", test.Description),
                new XElement("state", test.State),
                new XElement("priority", test.Priority),
                new XElement("owner", test.OwnerName),
                new XElement("reason", test.Reason),                
                new XElement("is_automated", test.IsAutomated),
                new XElement("area", test.Area)
                );
            IEnumerator iterator = test.Actions.GetEnumerator();
            foreach(ITestAction action in test.Actions){
                iterator.MoveNext();
                XElement a = new XElement("action");
                a.Add(
                    new XAttribute("id", action.Id),
                    new XAttribute("string", action.ToString())
                    );
                xmltest.Add(a);
            }
            IEnumerator custom = test.CustomFields.GetEnumerator();
            /*
            while (custom.MoveNext())
            {
                xmltest.Add("custom", custom.Current.ToString());
            }
            */
            // add this staff to root
            _root.Add(xmltest);
        }
        public void Header(ITfsCliOptions opts) {
            _root.Add(
                new XAttribute("url", opts.Get("url")), 
                new XAttribute("project", opts.Get("project")), 
                new XAttribute("testplan", opts.Get("testplan"))
                );
        }
    }
}
