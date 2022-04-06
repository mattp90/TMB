using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Nop.Plugin.TMB.Helpers
{
    public class FTPManager
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RequestFolder { get; set; }
        public string ResponseFolder { get; set; }
        public string ProcessedFolder { get; set; }
        public string PdfFolder { get; set; }

        public FTPManager(string host, int port, string username, string password, string requestFolder, string responseFolder, string processedFolder, string pdfFolder)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            RequestFolder = requestFolder;
            ResponseFolder = responseFolder;
            ProcessedFolder = processedFolder;
            PdfFolder = pdfFolder;
        }
        
        public void UploadFile(string filename, string content)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{Host}:{Port}/{RequestFolder}/{filename}");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.EnableSsl = true;
            
            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(Username, Password);

            byte[] bytes = Encoding.ASCII.GetBytes(content);
            // Write the bytes into the request stream.
            request.ContentLength = bytes.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
	        
            // FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            // Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
        }
        
        public List<string> GetResponseFiles()
        {
            var result = new List<string>();
            
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{Host}:{Port}/{ResponseFolder}");
            request.Credentials = new NetworkCredential(Username, Password);
            request.EnableSsl = true;
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            using (var res = (FtpWebResponse) request.GetResponse())
            {
                StreamReader streamReader = new StreamReader(res.GetResponseStream());
                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    if(line.EndsWith(".json"))
                        result.Add(line);
                    
                    line = streamReader.ReadLine();
                }

                streamReader.Close();
            }

            return result;
        }

        public string DownloadFile(string filename, bool isPdf = false)
        {
            var result = string.Empty;

            var uriDownload = (isPdf) ? $"{Host}:{Port}/{PdfFolder}/{filename}" : $"{Host}:{Port}/{ResponseFolder}/{filename}";
            
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uriDownload);
            request.Credentials = new NetworkCredential(Username, Password);
            request.EnableSsl = true;
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            using (var res = (FtpWebResponse) request.GetResponse())
            {
                Stream responseStream = res.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                result = reader.ReadToEnd();
                reader.Close();
            }

            return result;
        }
        
        public byte[] DownloadInvoicePdf(string filename)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{Host}:{Port}/{PdfFolder}/{filename}");
            request.Credentials = new NetworkCredential(Username, Password);
            request.EnableSsl = true;
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            using (var res = (FtpWebResponse) request.GetResponse())
            {
                Stream responseStream = res.GetResponseStream();
                using (MemoryStream ms = new MemoryStream())
                {
                    responseStream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
        
        public void MoveFileToProcessed(string filename)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{Host}:{Port}/{ResponseFolder}/{filename}");
            request.Method = WebRequestMethods.Ftp.Rename;
            request.EnableSsl = true;
            request.Credentials = new NetworkCredential(Username, Password);
            request.RenameTo = $"./{ProcessedFolder}/{filename}";

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        }
    }
}