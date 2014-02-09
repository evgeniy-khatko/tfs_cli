using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;

namespace tfs_cli
{
    interface ITfsCliUpdater
    {
        string UpdateTest(TfsTeamProjectCollection tfs, ITfsCliOptions opts);
        string UpdateFromJunit(TfsTeamProjectCollection tfs, ITfsCliOptions opts);
    }
}
