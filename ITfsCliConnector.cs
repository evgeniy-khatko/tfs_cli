using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tfs_cli
{
    interface ITfsCliConnector
    {
        void Connect();
        TfsTeamProjectCollection Collection();
    }
}
