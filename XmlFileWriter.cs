using System;
using System.Collections.Generic;
using System.IO;

namespace tfs_cli
{
    class XmlFileWriter : TestsWriter
    {        
        private string _output;
        private ITfsCliBuilder _builder;
        private ConnectionData _conData;
        
        public XmlFileWriter(string output, ConnectionData con, ITfsCliBuilder builder) : base(output, con, builder){}
               
        private void WriteToOutput(){
            StreamWriter output;
            try
            {
                output = new StreamWriter(_output);
                output.Write(_builder.Finalize());
                output.Close();
            }
            catch (Exception)
            {
                TfsCliHelper.ExitWithError(string.Format("Unable to write results to file: {0}", _output));
            }
        }        
    }
}
