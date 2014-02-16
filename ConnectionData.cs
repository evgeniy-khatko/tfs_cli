using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tfs_cli
{
    class ConnectionData
    {
        private string _url;
        private string _project;
        private string _testplan;
        private string _login;
        private string _pwd;
        private string _domen;

        public ConnectionData
            (
            string url,
            string project,
            string testplan,
            string login,
            string pwd,
            string domen
            )
        {
            _url = url;
            _project = project;
            _testplan = testplan;
            _login = login;
            _pwd = pwd;
            _domen = domen;
        }

        public void setTestPlan(string newtp) {
            _testplan = newtp;
        }

        public string Url() { return _url; }
        public string Project() { return _project; }
        public string Testplan() { return _testplan; }
        public string User() { return _login; }
        public string Password() { return _pwd; }
        public string Domen() { return _domen; }
    }
}
