using System;
using System.Diagnostics;
using System.IO;
using Jscape.Ftp;

namespace FileTracFtp.FTP
{
    public class FtpSsl
    {

        public string Host { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string RemotePath { get; set; }
        public string LicenseKey { get; set; }
        public int Port { get; set; }
        public string LocalPath { get; set; }
        public FtpSsl()
        {
            Port = 21;

        }

        public void DownloadFiles()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            this.Log().Info("Begin GetFileNames");

            var totalFiles = 0;
            var totalDownloadedFiles = 0;
            var totalSkippedFiles = 0;

            try
            {
                if (!Directory.Exists(LocalPath))
                {
                    this.Log().Info("Path {0} does not exist, creating direcory.", LocalPath);
                    Directory.CreateDirectory(LocalPath);
                }


                using (var ftpService = new Ftp(Host, UserId, Password, Port))
                {
                    ftpService.LicenseKey = LicenseKey;
                    ftpService.Connect();
                    ftpService.LocalDir = LocalPath;


                    if (!ftpService.Connected)
                    {
                        this.Log().Fatal("Unable to connect to remote host");
                        return;
                    }



                    ftpService.RemoteDir = RemotePath;
                    var dirListing = ftpService.GetDirListing();

                    while (dirListing.MoveNext())
                    {
                        totalFiles++;

                        var file = dirListing.Current as Jscape.Ftp.FtpFile;
                        if (file == null) continue;

                       

                        var localFileName = Path.Combine(LocalPath, file.Filename);
                        if (File.Exists(localFileName))
                        {
                            totalSkippedFiles++;
                            this.Log().Info("Skipping file {0} it already exists in destination path.", file.Filename);
                            continue;

                        }
                        this.Log().Info("Downloading file {0}.", file.Filename);
                        ftpService.Download(file.Filename, file.Filename);

                        var fileTime = string.Format("{0} {1}", file.Date, file.Time);

                        this.Log().Info("Altering file metrics {0}.", fileTime);

                        File.SetCreationTime(localFileName, DateTime.Parse(fileTime));
                        File.SetLastWriteTime(localFileName, DateTime.Parse(fileTime));

                        totalDownloadedFiles++;
                        
                    }



                }
            }
            catch (Exception ex)
            {
                ex.Log().Error("Error dowloading files Details : {0}", ex.Message);
            }
            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            this.Log().Info("Completed downloading files. Elapsed time {0}", elapsedTime);
            this.Log().Info("Total files {0}. Total skipped {1}. Total downloaded = {2}.", totalFiles, totalSkippedFiles, totalDownloadedFiles);


        }


        public static byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                var readBuffer = new byte[4096];

                var totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead != readBuffer.Length) continue;

                    var nextByte = stream.ReadByte();

                    if (nextByte == -1) continue;

                    var temp = new byte[readBuffer.Length * 2];
                    Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                    Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                    readBuffer = temp;
                    totalBytesRead++;
                }

                var buffer = readBuffer;

                if (readBuffer.Length == totalBytesRead) return buffer;

                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}
