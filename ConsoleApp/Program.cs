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
            MiniFtp miniFtp = new MiniFtp("ftpuser", "ftpuser$");
            Uri uri = new Uri("ftp://192.168.100.118/OS/Microsoft");
            miniFtp.GetFileList(uri);
            Console.ReadLine();

        }
    }
}
