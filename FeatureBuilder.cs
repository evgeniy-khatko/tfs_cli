using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.TestManagement.Client;
using System.IO;

namespace tfs_cli
{
    class FeatureBuilder : ITfsCliBuilder
    {
        private static string sep = Path.PathSeparator.ToString();
        private static string featuresDir = "features";
        private static string featureHeader = "Feature: ";
        private static string scenarioHeader = "Scenario: ";
        private static string prefix = " ";
        private static string doublePrefix = "  ";

        private string tmp = Path.GetTempPath()+sep+featuresDir;

        public string Finalize()
        {
            return "";
        }
        public void Append(ITestSuiteBase suite)
        {
            try
            {
                if (Directory.Exists(tmp))
                    Directory.Delete(tmp);
                Directory.CreateDirectory(tmp);
                StreamWriter file = new StreamWriter(tmp + sep + suite.Title);
                file.WriteLine(featureHeader + suite.Title);
                file.WriteLine(suite.Description ?? "");
                foreach (ITestSuiteEntry entry in suite.TestCases)
                    Append(entry.TestCase, file);
            }
            catch (IOException e)
            {
                TfsCliHelper.ExitWithError(string.Format("Could not create feature files: {0}", e.Message));
            }
        }
        public void Header(string url, string project, string testplan){}        

        public string GetFeatureFilesPath(){
            return tmp;
        }

        private void Append(ITestCase test, StreamWriter file)
        {
            file.WriteLine(prefix + scenarioHeader + test.Title);
            foreach (ITestAction action in test.Actions)
                Append((ITestStep)action, file);
        }

        private void Append(ITestStep step, StreamWriter file)
        {
            string act = TfsCliHelper.fromHtml(step.Title);
            string er = TfsCliHelper.fromHtml(step.Title);
            if (act != "")
                file.WriteLine(doublePrefix + act);
            if(er != "")
                file.WriteLine(doublePrefix + er);
        }

    }
}
