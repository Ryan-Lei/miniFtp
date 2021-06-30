using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

public class MiniFtp
{ 

    public MiniFtp(string userName,string password)
    {
        FtpNetworkCredential = new NetworkCredential();
        FtpNetworkCredential.UserName = userName;
        FtpNetworkCredential.Password = password;
    }

    public MiniFtp(NetworkCredential networkCredential)
    {
        FtpNetworkCredential = networkCredential;
    }

    public  bool IsFileExist(Uri uri)
    {
        var request = (FtpWebRequest)WebRequest.Create(uri);
        request.Credentials = FtpNetworkCredential;
        request.Method = WebRequestMethods.Ftp.GetFileSize;
        bool isExist = false;

        try
        {
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            isExist= true;
        }
        catch (WebException webException)
        {
            FtpWebResponse response = (FtpWebResponse)webException.Response;

            if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                isExist = false;
            } 
            else
            {
                throw webException;
            }  
        }
        return isExist;
    }

    public NetworkCredential FtpNetworkCredential { get; set; } 

}
