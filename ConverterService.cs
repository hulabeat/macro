using System;
using System.Drawing;
using System.IO;

namespace macro
{
  internal class ConverterService : IConverter
  {
    public byte[] bitmap2ByteArray(Bitmap bitmap)
    {
      byte[] numArray = (byte[]) null;
      if (bitmap != null)
      {
        MemoryStream memoryStream = new MemoryStream();
        bitmap.Save((Stream) memoryStream, bitmap.RawFormat);
        numArray = memoryStream.ToArray();
      }
      else
        Console.WriteLine("Bitmap is null.");
      return numArray;
    }
  }
}
