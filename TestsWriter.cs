using System;
using System.Collections.Generic;
using System.IO;

namespace tfs_cli
{
    abstract class TestsWriter
    {
        protected string _output;
        protected ITfsCliBuilder _builder;
        protected ConnectionData _conData;

        public TestsWriter(string output, ConnectionData con, ITfsCliBuilder builder) 
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
            foreach (var suite in tfsapi.GetSuites())
            {
                _builder.Append(suite);
            }
            WriteToOutput();            
        }
        
        abstract protected void WriteToOutput();
    }
}
