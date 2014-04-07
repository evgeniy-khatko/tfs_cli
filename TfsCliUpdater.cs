using System;
using System.Collections.Generic;

namespace tfs_cli
{
    class TfsCliUpdater
    {
        private IRunResultProvider _runRes;
        private IList<ITestResultProvider> _results;
        private ConnectionData _conData;
        public TfsCliUpdater(IRunResultProvider runres, IList<ITestResultProvider> results, ConnectionData con){
            _runRes = runres;
            _results = results;
            _conData = con;
        }

        public void UpdateResults() 
        {
            ITfsCliConnector connector = new CredentialConnector(_conData);
            connector.Connect();
           
            TfsApi tfsapi = new TfsApi(connector.Collection(), _conData);
            Microsoft.TeamFoundation.TestManagement.Client.ITestRun run = tfsapi.CreateRun(
                        tfsapi.GetSuite(_runRes.Suite()),
                        _runRes.Title(), _runRes.Comment(),
                        _runRes.BuildNumber(),
                        _runRes.Attachment()
                    );
            foreach (ITestResultProvider res in _results)
            {
                tfsapi.UpdateTestResult
                (
                    tfsapi.GetTestResult(run, res.Title()),
                    res.Outcome(),
                    res.Duration(),
                    res.Comment(),
                    res.FailureType(),
                    res.ErrorMessage(),
                    res.Attachment()
                );
            }
        }
    }
}
