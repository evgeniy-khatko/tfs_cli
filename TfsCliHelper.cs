using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace tfs_cli
{
    class TfsCliHelper
    {
        private static TraceSwitch _logger = new TraceSwitch("tfs_cli", "tfs_cli log", TraceLevel.Error.ToString());
        public static void ExitWithError(string message)
        {
            Trace.WriteLine(message);
            Console.WriteLine(message);
            Environment.Exit(1);
        }

        public static string fromHtml(string htmlText)
        {
            htmlText = htmlText.Replace("</P><P>", Environment.NewLine);
            htmlText = System.Text.RegularExpressions.Regex.Replace(htmlText, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", String.Empty);
            return htmlText;
        }

        public static string toFeatureStep(string inp, string prefix, string lb)
        {
            string[] inpAry = Regex.Split(inp, lb);
            if (inpAry.Length == 1)
                return inp;
            StringBuilder sb = new StringBuilder(inpAry[0] + lb);
            for (int i = 1; i < inpAry.Length - 1; i++)
                sb.Append(prefix + inpAry[i] + lb);
            sb.Append(prefix + inpAry[inpAry.Length - 1]);
            return sb.ToString();
        }

        public static void Info(string message)
        {
            Trace.WriteLineIf(_logger.Level == TraceLevel.Info, message);
            Console.WriteLine(message);
        }

        public static void Debug(string message)
        {
            Trace.WriteLineIf(_logger.Level == TraceLevel.Verbose, message);
            Console.WriteLine(message);
        }
    }
}
