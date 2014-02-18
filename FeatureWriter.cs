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
            try
            {
                foreach (var file in Directory.GetFiles(((FeatureBuilder)_builder).GetFeatureFilesPath()))
                    File.Copy(file, Path.Combine(_output, Path.GetFileName(file)));
            }
            catch (IOException e)
            {
                TfsCliHelper.ExitWithError(string.Format("Could not create feature files: {0}", e.Message));
            }
        }    
    }
}
