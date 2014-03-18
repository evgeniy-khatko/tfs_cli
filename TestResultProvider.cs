using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tfs_cli
{
    class TestResultProvider : ITestResultProvider
    {
        private string _name;
        private string _outcome;
        private string _comment;
        private string _attachment;
        private string _failure_type;
        private string _error_message;
        private string _duration;

        private IList<string> possibleOutcomes = new List<string>()
        {
            "Aborted", 
            "Blocked", 
            "Error", 
            "Failed", 
            "Inconclusive",
            "None", 
            "NotApplicable", 
            "NotExecuted", 
            "Passed", 
            "Paused", 
            "Timeout", 
            "Unspecified", 
            "Warning"
        };

        private IList<string> possibleFailureTypes = new List<string>()
        {
            "KnowIssue", 
            "NewIssue", 
            "None", 
            "Regression", 
            "Unknown"
        };

        public TestResultProvider
            (
            string name,
            string outcome,
            string suite,
            string comment,
            string attach,
            string failure_type,
            string error_message,
            string duration
            )
        {
            _name = name;
            _outcome = outcome;
            _comment = comment;
            _attachment = attach;
            _failure_type = failure_type;
            _error_message = error_message;
            _duration = duration;
            if (!possibleOutcomes.Contains(_outcome))
                TfsCliHelper.ExitWithError(string.Format("Outcome \"{0}\" not known.\nKnow are: {1}", _outcome, possibleOutcomes.Aggregate((a, b) => a + ", " + b)));
            if (!possibleFailureTypes.Contains(_failure_type))
                TfsCliHelper.ExitWithError(string.Format("Failure type {0} not know.\nKnow are: {1}", _failure_type, possibleFailureTypes.Aggregate((a, b) => a + ", " + b)));
        }
        public string Title() { return _name; }
        public string Outcome() { return _outcome; }
        public string Comment() { return _comment; }
        public string Attachment() { return _attachment; }
        public string FailureType() { return _failure_type; }
        public string ErrorMessage() { return _error_message; }
        public string Duration() { return _duration; }
    }
}
