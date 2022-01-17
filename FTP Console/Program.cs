using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace FTP_Console
{


    class Program
    {

        static (bool,string) Selectfile(string filePath)
        {


            Dispatcher.CurrentDispatcher.Invoke(((Action)(() =>
            {

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {

                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {

                        filePath = openFileDialog.FileName;

                    }

                }

            })), null);

                  
            return (true, filePath);
        }



    
        static (bool, string) Uploadfile(string filePath)
        {
            Console.Write(" --------");


            Console.Write("\n   |");
            Console.Write("\n   ----> Enter username: ");

            string username = Console.ReadLine();

            Console.Write("\n   |");
            Console.Write("\n   ----> Enter password: ");

            string password = Console.ReadLine();

       

            try
            {
                string path = Path.GetFileName(filePath);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://ftpupload.net/htdocs/{path}");
                request.Credentials = new NetworkCredential($"{username}", $"{password}");
                request.Method = WebRequestMethods.Ftp.UploadFile;

                using (Stream fileStream = File.OpenRead($"{filePath}"))
                using (Stream ftpStream = request.GetRequestStream())
                {
                    byte[] buffer = new byte[102400000];
                    int read;
                    while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ftpStream.Write(buffer, 0, read);
                        Console.WriteLine(" \n Uploaded {0} bytes", fileStream.Position);
                    }
                }

             


            }
            catch (Exception ex)
            {

                MessageBox.Show($"{ex}");

            }
            return (true, filePath);
        }

        static (bool, string) Downloadfile(string filePath)
        {


            Console.Write(" --------");


            Console.Write("\n   |");
            Console.Write("\n   ----> Enter username: ");

            string username = Console.ReadLine();

            Console.Write("\n   |");
            Console.Write("\n   ----> Enter password: ");

            string password = Console.ReadLine();

            try
            {



                string path = Path.GetFileName(filePath);

                string path2 = Path.GetFileNameWithoutExtension(filePath);

                FileInfo fileInfo = new FileInfo(path);

                string ex = fileInfo.Extension;


                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://ftpupload.net/htdocs/{path}");
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                using (Stream fileStream = File.Create($"{filePath} {Guid.NewGuid()} {ex}"))
                {
                    byte[] buffer = new byte[102400000];
                    int read;
                    while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, read);
                        Console.WriteLine("Downloaded {0} bytes", fileStream.Position);
                    }
                }

     

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            return (true, filePath);
        }

        [STAThread]
        static void Main(string[] args)
        {

            Console.WriteLine(
                "\n 1 <- Select file \n" +
                " 2 <- Upload file \n" +
                " 3 <- Download file \n");

            var filePath = string.Empty;

            bool ch_1 = false;
            bool ch_2 = false;
            bool ch_3 = false;

            Console.Title = "FTP Console";
            while (true)
            {

                Console.Write("\n Enter: ");

                string s = Console.ReadLine();

                if (s == "1")
                {



                    (ch_1, filePath) = Selectfile(filePath);

                    Console.WriteLine($" {(ch_1, filePath)}");

                }

                if (s == "2")
                {

                    if (ch_1 == true && !string.IsNullOrEmpty(filePath))
                    {

                        (ch_2, filePath) = Uploadfile(filePath);

                    }
                    else
                    {
                        Console.WriteLine("\n Select file first");
                        (ch_1, filePath) = Selectfile(filePath);

                        Console.WriteLine($"{(ch_1, filePath)}");
                    }

                }

                if (s == "3")
                {

                    if (ch_1 == false && string.IsNullOrEmpty(filePath))
                    {

                        Console.WriteLine("\n Select file first");
                        (ch_1, filePath) = Selectfile(filePath);

                        Console.WriteLine($" {(ch_1, filePath)}");

                    }
                    if (ch_2 == true && !string.IsNullOrEmpty(filePath))
                    {

                        (ch_3, filePath) = Downloadfile(filePath);
                    }
                    else
                    {
                        Console.WriteLine("\n Upload first");
                        (ch_2, filePath)= Uploadfile(filePath);
                    }



                }



            }



            Console.ReadKey();
        }
    }
}
