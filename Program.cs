using System;
using System.Windows.Forms;

namespace macro
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      new Market().Show();
      Application.Run();
    }
  }
}
