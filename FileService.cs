using System;
using System.Collections.Generic;
using System.IO;

namespace macro
{
  internal class FileService : IFile
  {
    public List<string> findFile(string directory, string ext)
    {
      List<string> file1 = new List<string>();
      foreach (FileInfo file2 in new DirectoryInfo(directory).GetFiles())
      {
        if (file2.Extension.ToLower().CompareTo("." + ext) == 0)
          file1.Add(file2.FullName);
      }
      return file1;
    }

    public string logMsg(string msg) => string.Format("[{0}] {1}\n", (object) DateTime.Now, (object) msg);

    public void logSave(string msg)
    {
      string path = Environment.CurrentDirectory + "\\Log";
      string str = path + "\\Log_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
      DirectoryInfo directoryInfo = new DirectoryInfo(path);
      FileInfo fileInfo = new FileInfo(str);
      try
      {
        if (!directoryInfo.Exists)
          Directory.CreateDirectory(path);
        if (!fileInfo.Exists)
        {
          using (StreamWriter streamWriter = new StreamWriter(str))
          {
            streamWriter.WriteLine(msg);
            streamWriter.Close();
          }
        }
        else
        {
          using (StreamWriter streamWriter = File.AppendText(str))
          {
            streamWriter.WriteLine(msg);
            streamWriter.Close();
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("log save fail : " + ex?.ToString());
      }
    }

    public void userSave(string msg)
    {
      string path = Environment.CurrentDirectory + "\\Log";
      string str = path + "\\Log_" + DateTime.Today.ToString("yyyyMMdd") + "_usersave.log";
      DirectoryInfo directoryInfo = new DirectoryInfo(path);
      FileInfo fileInfo = new FileInfo(str);
      try
      {
        if (!directoryInfo.Exists)
          Directory.CreateDirectory(path);
        if (!fileInfo.Exists)
        {
          using (StreamWriter streamWriter = new StreamWriter(str))
          {
            streamWriter.WriteLine(msg);
            streamWriter.Close();
          }
        }
        else
        {
          using (StreamWriter streamWriter = File.AppendText(str))
          {
            streamWriter.WriteLine(msg);
            streamWriter.Close();
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("log save fail : " + ex?.ToString());
      }
    }
  }
}
