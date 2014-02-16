using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tfs_cli
{
    public interface IRunResultProvider
    {
        string Title();
        string Configuration();
        string BuildNumber();
        string Comment();
        string Attachment();
        string Suite();
    }
}
