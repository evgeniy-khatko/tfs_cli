using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tfs_cli
{
    class RunResultProvider : IRunResultProvider
    {
        private string _title;
        private string _configuration;
        private string _build;
        private string _comment;
        private string _attachment;
        private string _suite;
        public RunResultProvider
            (
            string title,
            string conf,
            string build_num,
            string comment,
            string attach,
            string suite
            )
        {
            _title = title;
            _configuration = conf;
            _comment = comment;
            _attachment = attach;
            _build = build_num;
            _suite = suite;
            if (_attachment != null && !File.Exists(_attachment)) { TfsCliHelper.ExitWithError(string.Format("File {0} not found", _attachment)); }
        }
        public string Title(){ return _title; }
        public string Configuration(){ return _configuration; }
        public string BuildNumber() { return _build; }
        public string Comment() { return _comment; }
        public string Attachment() { return _attachment; }
        public string Suite() { return _suite; }
    }
}
