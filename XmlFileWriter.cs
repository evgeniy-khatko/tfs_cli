using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.TeamFoundation.TestManagement.Client;

namespace tfs_cli
{
    class XmlFileWriter
    {
        private string _output;
        private ITfsCliBuilder _builder;
        private ConnectionData _conData;

        public XmlFileWriter(string output, ConnectionData con, ITfsCliBuilder builder) 
        {
            _conData = con;
            _output = output;
            _builder = builder;
        }

        public void CreateOutput() 
        {
            ITfsCliConnector connector = new CredentialConnector(_conData);
            connector.Connect();
            // generate header
            _builder.Header(_conData.Url(), _conData.Project(), _conData.Testplan());

            TfsApi tfsapi = new TfsApi(connector.Collection(), _conData);
            ITestPlan testplan = tfsapi.GetTestPlan();
            
            List<ITestSuiteBase> suites = tfsapi.GetSuites();
            foreach(ITestSuiteBase suite in suites){
                _builder.Append(suite);
            }
            // create file for output
            StreamWriter output;
            try
            {
                output = new StreamWriter(_output);
                output.Write(_builder.Finalize());
                output.Close();
            }
            catch (Exception)
            {
                TfsCliHelper.ExitWithError(string.Format("Unable to write results to file: {0}", _output));
            }
        }        
    }
}
