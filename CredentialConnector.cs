using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;

namespace tfs_cli
{
    class CredentialConnector : ITfsCliConnector
    {
        private TfsTeamProjectCollection _tfs;
        public CredentialConnector(Uri uri, ICredentials credentials)
        {
            _tfs = new TfsTeamProjectCollection(uri, credentials);
        }

        public bool Connect()
        {
            try
            {
                _tfs.Authenticate();
                return _tfs.HasAuthenticated;
            }
            catch (Exception) { return false; }
        }

        public TfsTeamProjectCollection GetTfs()
        {
            return _tfs;
        }
    }
}
