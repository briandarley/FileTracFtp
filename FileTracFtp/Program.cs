using FileTracFtp.FTP;

namespace FileTracFtp
{
    class Program
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            LoggingExtensions.Logging.Log.InitializeWith<LoggingExtensions.log4net.Log4NetLog>();
            log4net.Config.XmlConfigurator.Configure();
            
            var ftpSsl = new FtpSsl
            {
                Host = "FTP:Host".AppSetting(),
                LicenseKey = "FTP:License-SecureFTP".AppSetting(),
                Password = "FTP:Password".AppSetting(),
                RemotePath = "FTP:RemoteNetworkPath".AppSetting(),
                Port = "FTP:Port".AppSetting().ToInt(),
                UserId = "FTP:UserId".AppSetting(),
                LocalPath = "FTP:LocalNetworkPath".AppSetting()
            };
            ftpSsl.DownloadFiles();

            
        }
    }
}



 
