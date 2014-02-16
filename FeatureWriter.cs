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
        private ITfsCliBuilder _builder;
        private ConnectionData _conData;

        public FeatureWriter(string output, ConnectionData con, ITfsCliBuilder builder) : base(output, con, builder){
            Directory.CreateDirectory(output);
        }

        private void WriteToOutput(){}    
    }
}
