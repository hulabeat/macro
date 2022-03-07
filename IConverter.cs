using System.Drawing;

namespace macro
{
  internal interface IConverter
  {
    byte[] bitmap2ByteArray(Bitmap bitmap);
  }
}
