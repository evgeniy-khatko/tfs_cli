using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace tfs_cli
{
    class FeatureBuilder : ITfsCliBuilder
    {
        public string Finalize()
        {
            return "";
        }
        public void Append(ITestSuiteBase suite)
        {

        }
        public void Header(string url, string project, string testplan){}        
    }
}
