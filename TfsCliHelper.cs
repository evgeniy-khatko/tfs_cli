using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace tfs_cli
{
    class TfsCliHelper
    {
        public static void ExitWithError(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(1);
        }

        public static string fromHtml(string htmlText)
        {
            htmlText = htmlText.Replace("</P><P>", Environment.NewLine);
            htmlText = System.Text.RegularExpressions.Regex.Replace(htmlText, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", String.Empty);
            return htmlText;
        }
    }
}
