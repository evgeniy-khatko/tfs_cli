using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace tfs_cli
{
    class XmlFileWriter : ITfsCliWriter
    {
        public void CreateOutput(TfsTeamProjectCollection tfs, ITfsCliOptions opts, ITfsCliBuilder builder) 
        { 
            // generate xml
            // generate header
            builder.Header(opts);
            
            // get helper
            TfsApi tfsapi = new FirstTfsApi(tfs);

            // get testplan
            ITestManagementTeamProject project = tfsapi.GetProject(opts.Get("project"));
            ITestPlan testplan = tfsapi.GetTestPlan(project, opts.Get("testplan"));
            foreach(ITestCase test in testplan.RootSuite.AllTestCases){
                builder.Append(test);
            }
            
            // create file for output
            StreamWriter output;
            try
            {
                output = new StreamWriter(opts.Get("output"));
                output.Write(builder.Finalize());
                output.Close();
            }
            catch (Exception)
            {
                TfsCliHelper.ExitWithError(string.Format("Unable to write results to file: {0}", opts.Get("output")));
            }
        }        
    }
}
