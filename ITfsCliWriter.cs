using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tfs_cli
{
    interface ITfsCliWriter
    {
        void CreateOutput(TfsTeamProjectCollection tfs, ITfsCliOptions opts, ITfsCliBuilder builder);
    }
}
