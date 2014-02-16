﻿using System;
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
                new NetworkCredential(_connData.User(), _connData.Password(), _connData.Domen()) :
                null;
             
                _tfs = new TfsTeamProjectCollection(new Uri(_connData.Url()), cred);
                _tfs.Authenticate();
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
