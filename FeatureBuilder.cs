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
        private static string sep = Path.DirectorySeparatorChar.ToString();
        private static string featuresDir = "features";
        private static string featureHeader = "Feature: ";
        private static string scenarioHeader = "Scenario: ";
        private static string fileExtension = ".feature";
        private static string prefix = " ";
        private static string doublePrefix = "  ";
        private static string lb = Environment.NewLine;

        private string tmp = Path.GetTempPath()+sep+featuresDir;

        public string Finalize()
        {
            return "";
        }
        public void Append(ITestSuiteBase suite)
        {
            try
            {
                if (!Directory.Exists(tmp))
                    Directory.CreateDirectory(tmp);
                TfsCliHelper.Debug(string.Format("TmpDir: \"{0}\"", tmp));
                string featureName = tmp + sep + suite.Title + fileExtension;
                TfsCliHelper.Debug(string.Format("NewFeature: \"{0}\"", featureName));
                StreamWriter file = new StreamWriter(featureName);
                file.WriteLine(featureHeader + suite.Title);
                foreach (ITestSuiteEntry entry in suite.TestCases)
                {
                    TfsCliHelper.Debug(string.Format("AddSuite: \"{0}\"", suite.Title));
                    Append(entry.TestCase, file);
                }
                file.Close();
            }
            catch (IOException e)
            {
                TfsCliHelper.ExitWithError(string.Format("Could not create feature files in TMP dir: {0}\nError: {1}", tmp, e.Message));
            }            
        }
        public void Header(string url, string project, string testplan){}        

        public string GetFeatureFilesPath(){
            return tmp;
        }

        private void Append(ITestCase test, StreamWriter file)
        {
            TfsCliHelper.Debug(string.Format("AddTest: \"{0}\"", test.Title));
            file.WriteLine(prefix + scenarioHeader + test.Title);
            foreach (ITestAction action in test.Actions)
                Append((ITestStep)action, file);
        }

        private void Append(ITestStep step, StreamWriter file)
        {
            string act = TfsCliHelper.toFeatureStep(TfsCliHelper.fromHtml(step.Title), doublePrefix, lb);
            string er = TfsCliHelper.toFeatureStep(TfsCliHelper.fromHtml(step.ExpectedResult), doublePrefix, lb);
            
            if (act != "")
            {
                TfsCliHelper.Debug(string.Format("AddAction: \"{0}\"", act));
                file.WriteLine(doublePrefix + act);
            }
            if (er != "")
            {
                TfsCliHelper.Debug(string.Format("AddExpectedResult: \"{0}\"", er));
                file.WriteLine(doublePrefix + er);
            }
        }

    }
}
