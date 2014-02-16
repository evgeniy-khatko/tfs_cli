using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tfs_cli
{
    interface ITestResultProvider
    {
        string Title();
        string Outcome();
        string Comment();
        string Attachment();
        string FailureType();
        string ErrorMessage();
        string Duration();
    }
}
