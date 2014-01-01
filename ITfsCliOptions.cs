using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tfs_cli
{
    interface ITfsCliOptions
    {
        IDictionary<string, string> GetAll();
        string Get(string key);
        string ProvidedVerb();
    }
}
