using HtmlAgilityPack;

namespace macro
{
  public interface IAPI
  {
    HtmlDocument getHtml(string link);

    string mobile2PCLink(string link);
  }
}
