using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MiniFtp miniFtp = new MiniFtp("UserName", "Password");
            Uri uri = new Uri("ftp://host/dir/temp.txt");

            Console.WriteLine("IsFileExist: "+miniFtp.IsFileExist(uri));

            uri = new Uri("ftp://host/dir");
            Console.WriteLine("IsDirectoryExist: " + miniFtp.IsDirectoryExist(uri));

            //uri = new Uri("ftp://host/dir");
            //List<FolderFile> folderFiles = miniFtp.GetFileList(uri);

            //foreach(var folderFile in folderFiles)
            //{
            //    Console.WriteLine(folderFile.FilePath);
            //}

            //uri = new Uri("ftp://host/dir/temp.txt");
            //miniFtp.DownloadFile(uri, @"c:\temp\temp.txt");
            //Console.WriteLine("File Downloaded!! ");

            //uri = new Uri("ftp://host/dir/temp.txt");
            //miniFtp.UploadFile(uri, @"c:\temp\temp.txt");
            //Console.WriteLine("File Uploaded!! ");

            //uri = new Uri("ftp://host/dir/temp.txt");
            //miniFtp.DeleteFile(uri);
            //Console.WriteLine("File Deleted!! ");

            //uri = new Uri("ftp://host/dir/temp.txt");
            //miniFtp.RenameFile(uri,"newFile.txt");
            //Console.WriteLine("File Renamed!! ");

            Console.WriteLine();
            Console.WriteLine("Press Enter to Finish !!");
            Console.ReadLine();

        }
    }
}
