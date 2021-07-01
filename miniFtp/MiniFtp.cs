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
    /// DownloadFile
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="filePath"></param>
    public void DownloadFile( Uri uri, string filePath)
    {
        try
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.KeepAlive = true;
            request.UsePassive = IsUsePassive;
            request.UseBinary = _isUseBinary;
            request.Credentials = FtpNetworkCredential;
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                byte[] buffer = new byte[10240];
                int ReadCount = responseStream.Read(buffer, 0, buffer.Length);
                while (ReadCount > 0)
                {
                    fs.Write(buffer, 0, ReadCount);
                    ReadCount = responseStream.Read(buffer, 0, buffer.Length);
                }
            } 
 
            response.Close();
            response.Dispose();
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    /// <summary>
    /// UploadFile
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="localFilePath"></param>
    public void UploadFile(Uri uri, string localFilePath)
    {
        try
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.KeepAlive = true;
            request.UsePassive = IsUsePassive;
            request.UseBinary = IsUseBinary; 
            request.Credentials = FtpNetworkCredential;
            
            using (Stream source = File.OpenRead(localFilePath))
            {
                using (Stream dest = request.GetRequestStream())
                {
                    source.CopyTo(dest);
                }
            }  
        }
        catch(Exception ex)
        {
            throw ex;
        }
         
    }

    /// <summary>
    /// DeleteFile
    /// </summary>
    /// <param name="uri"></param>
    public void DeleteFile(Uri uri)
    { 
        try
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Credentials = FtpNetworkCredential;
            request.UsePassive = IsUsePassive;
            request.UseBinary = IsUseBinary;
            request.Method = WebRequestMethods.Ftp.DeleteFile; 

            FtpWebResponse response = (FtpWebResponse)request.GetResponse(); 
            response.Close(); 
        }
        catch (WebException webException)
        {
            throw webException;
        } 

    }

    /// <summary>
    /// RenameFile
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="newFileName"></param>
    public void RenameFile( Uri uri, string newFileName)
    {
        try
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Credentials = FtpNetworkCredential; 
            request.RenameTo = newFileName;
            request.Method = WebRequestMethods.Ftp.Rename;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();
        }
        catch(Exception ex)
        {
            throw ex;
        } 
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
         
        folderFile.FilePath = uri.LocalPath.TrimEnd('/')+"/"+ rawFolderFile[folderFileIndex].Trim().Substring(folderFileNameIndex);
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
