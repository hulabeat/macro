using HtmlAgilityPack;
using System.Collections.Generic;
using System.Xml;

namespace macro
{
  internal interface IProductInfo
  {
    string getPrdNo(string link);

    string getOptionStcNo(HtmlDocument doc);

    List<List<string>> productOptionCase1(string api, string productNo);

    string productOptionCase0(string api, string productNo);

    bool productOptionClassification(XmlDocument xml);

    string getProductShortLink(string prdNo, string optionStckNo, int optionStock);
  }
}
