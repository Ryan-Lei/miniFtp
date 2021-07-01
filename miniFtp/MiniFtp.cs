using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

public class MiniFtp
{

    private bool _isUsePassive = true;
    private bool _isUseBinary = true; 

    /// <summary>
    /// init. instance
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    public MiniFtp(string userName,string password)
    {
        FtpNetworkCredential = new NetworkCredential();
        FtpNetworkCredential.UserName = userName;
        FtpNetworkCredential.Password = password;
    }

    /// <summary>
    /// init. instance
    /// </summary>
    /// <param name="networkCredential"></param>
    public MiniFtp(NetworkCredential networkCredential)
    {
        FtpNetworkCredential = networkCredential;
    }

    /// <summary>
    /// IsFileExist
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public bool IsFileExist(Uri uri)
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

    /// <summary>
    /// IsDirectoryExist
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public bool IsDirectoryExist(Uri uri)
    {
        var request = (FtpWebRequest)WebRequest.Create(uri);
        request.Credentials = FtpNetworkCredential;
        request.Method = WebRequestMethods.Ftp.ListDirectory;
        bool isExist = false;

        try
        {
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            isExist = true;
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

    /// <summary>
    /// GetFileList
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public List<FolderFile> GetFileList(Uri uri)
    {  
 
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
        request.UsePassive = _isUsePassive;
        request.Method = WebRequestMethods.Ftp.ListDirectoryDetails; 
        request.Credentials = FtpNetworkCredential;

        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        Stream responseStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(responseStream);
        List<FolderFile> folderFiles = new List<FolderFile>();

        try
        {
            string[] stringSeparators = new string[] { "\r\n" };
            string[] rawFolderFileList = reader.ReadToEnd().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            
            for (int i=0; i< rawFolderFileList.Length;i++)
            {
                var rawFolderFile = rawFolderFileList[i].Split(new string[] { "    " }, StringSplitOptions.RemoveEmptyEntries); 
                folderFiles.Add(GetFolderFile(uri, rawFolderFile));

            }

            reader.Close();
            response.Close();
        }
        catch (Exception ex)
        {
            throw ex; 
        } 

        return folderFiles;
    }

    /// <summary>
    /// GetFolderFile
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="rawFolderFile"></param>
    /// <returns></returns>
    private FolderFile GetFolderFile(Uri uri,string[] rawFolderFile)
    {
        FolderFile folderFile = new FolderFile();
        int folderFileIndex = rawFolderFile.Length - 1;
        int folderFileNameIndex = rawFolderFile[folderFileIndex].Trim().IndexOf(' ')+1;

        foreach (var line in rawFolderFile)
        {
            folderFile.IsFolder = line.Contains("<DIR>") ? true : false;

            if (folderFile.IsFolder)
                break;
        }
         
        folderFile.FilePath = uri.LocalPath+"/"+ rawFolderFile[folderFileIndex].Trim().Substring(folderFileNameIndex);
        folderFile.FolderName = folderFile.IsFolder ? rawFolderFile[folderFileIndex].Trim().Substring(folderFileNameIndex) : "";
        folderFile.FileName = folderFile.IsFolder ? "" : rawFolderFile[folderFileIndex].Trim().Substring(folderFileNameIndex);

        return folderFile;
    }

    /// <summary>
    /// FtpNetworkCredential
    /// </summary>
    public NetworkCredential FtpNetworkCredential { get; set; }

    /// <summary>
    /// IsUsePassive
    /// </summary>
    public bool IsUsePassive
    {
        get { return _isUsePassive; }
        set { _isUsePassive = value; }
    }

    /// <summary>
    /// IsUseBinary
    /// </summary>
    public bool IsUseBinary
    {
        get { return _isUseBinary; }
        set { _isUseBinary = value; }
    }

}

public class FolderFile
{
    /// <summary>
    /// FilePath
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// FolderName
    /// </summary>
    public string FolderName { get; set; }

    /// <summary>
    /// FileName
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// IsFolder
    /// </summary>
    public bool IsFolder { get; set; }
}
