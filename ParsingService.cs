using HtmlAgilityPack;

namespace macro
{
  public class ParsingService
  {
    public HtmlDocument getHtml(string link)
    {
      HtmlDocument htmlDocument = new HtmlDocument();
      return new HtmlWeb().Load(link);
    }

    public string mobile2PCLink(string link) => link.Replace("m.", "").Replace("/m", "");

    public string pC2MobileLink(string link) => link.Replace("www", "m").Replace("products/", "products/m/");
  }
}
