using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tfs_cli
{
    class FeatureWriter : TestsWriter
    {       
        public FeatureWriter(string output, ConnectionData con, ITfsCliBuilder builder) : base(output, con, builder){}

        protected override void WriteToOutput()
        {
            string from = ((FeatureBuilder)_builder).GetFeatureFilesPath();
            try
            {
                TfsCliHelper.Debug(string.Format("CopyFeaturesTo: \"{0}\"", _output));
                foreach (var file in Directory.GetFiles(from))
                    File.Copy(file, Path.Combine(_output, Path.GetFileName(file)), true);
                Directory.Delete(from, true);
            }
            catch (IOException e)
            {
                TfsCliHelper.ExitWithError(string.Format("Could not copy feature files from {0} to {1}.\nError: {2}", from, _output, e.Message.ToString()));
            }
        }    
    }
}
