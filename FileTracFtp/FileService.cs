using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTracFtp
{
    public class FileService
    {

        public string FtpAddress { get; set; }
        public string FtpUserId { get; set; }
        public string FtpPassword { get; set; }
        public string FtpPath { get; set; }


        public FileService()
        {
            
        }


        public IEnumerable<string> ListFiles()
        {
            return null;

        }

    }
}
