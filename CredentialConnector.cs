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
        private ConnectionData _connData;
        public CredentialConnector(ConnectionData data)
        {
            _connData = data;
        }

        public void Connect()
        {
            try
            {
                // Try to connect first
                var cred = (_connData.User() != null && _connData.Password() != null) ?
                new NetworkCredential(_connData.User(), _connData.Password(), _connData.Domen()) : null;
                if (_connData.ProxyLogin() != "" && _connData.ProxyPassword() != "" && _connData.ProxyDomain() != "")
                {
                    NetworkCredential credentials = new System.Net.NetworkCredential(_connData.ProxyLogin(), _connData.ProxyPassword(), _connData.ProxyDomain());
                    WebRequest.DefaultWebProxy.Credentials = credentials;
                }
                _tfs = new TfsTeamProjectCollection(new Uri(_connData.Url()), cred);
                _tfs.Authenticate();
                TfsCliHelper.Debug(string.Format("Connection: \"{0}\" \"{1}\"", _tfs.Name, _tfs.Uri));
                if (!_tfs.HasAuthenticated)
                {
                    TfsCliHelper.ExitWithError("Could not authenticate to TFS");
                }
            }
            catch (UriFormatException) { TfsCliHelper.ExitWithError("Invalid Url provided"); }
        }

        public TfsTeamProjectCollection Collection()
        {
            return _tfs;
        }
    }
}
