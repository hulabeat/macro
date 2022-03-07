using System.Collections.Generic;

namespace macro
{
  public interface IFile
  {
    List<string> findFile(string directory, string ext);

    string logMsg(string msg);

    void logSave(string msg);

    void userSave(string msg);
  }
}
