using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Xml;

namespace macro
{
  internal class ProductInfoService
  {
    public static string ROOTURL = "https://buy.m.11st.co.kr//MW/Order/orderBasicFirstStep.tmall?&";
    public static string INCOMMINGCODE = "incommingCode=01";
    public static string PRDAPPMTDBDLBCD = "prdAppmtDbDlvCd=01";
    public static string MOBILEYN = "mobileYn=N";
    public static string BCKTEXYN = "bcktExYn=N";
    private static string OPTIONPRC = "optionPrc=0";
    public static string optionStock = "";
    private XmlDocument xml;

    public string getOptionStcNo(HtmlDocument doc)
    {
      try
      {
        doc.DocumentNode.SelectSingleNode("//body");
        return doc.DocumentNode.SelectSingleNode("//*[@id='layBodyWrap']/div/div[1]/div[2]/div/div[2]/form[2]/input[27]").GetAttributeValue("value", "");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return (string) null;
      }
    }

    public List<string> getOptionStcNoList(HtmlDocument doc)
    {
      List<string> optionStcNoList = new List<string>();
      try
      {
        foreach (HtmlNode selectNode in (IEnumerable<HtmlNode>) doc.DocumentNode.SelectNodes("//*[@id='optlst_0']/li"))
          optionStcNoList.Add(selectNode.SelectSingleNode("a").GetAttributeValue("data-stockno", ""));
        return optionStcNoList;
      }
      catch (Exception ex)
      {
        Console.WriteLine("error : " + ex?.ToString());
        return (List<string>) null;
      }
    }

    public List<List<string>> productOptionCase1(string api, string productNo)
    {
      List<List<string>> stringListList = new List<List<string>>();
      this.xml.Load("http://openapi.11st.co.kr/openapi/OpenApiService.tmall?key=" + api + "&apiCode=ProductInfo&productCode=" + productNo + "&option=PdOption");
      try
      {
        XmlNodeList elementsByTagName = this.xml.GetElementsByTagName("Value");
        for (int index = 0; index < elementsByTagName.Count; ++index)
        {
          stringListList[index].Add(elementsByTagName[index].SelectSingleNode("Order").InnerText);
          stringListList[index].Add(elementsByTagName[index].SelectSingleNode("ValueName").InnerText);
        }
        return stringListList;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return (List<List<string>>) null;
      }
    }

    public string productOptionCase0(string api, string productNo)
    {
      this.xml.Load("http://openapi.11st.co.kr/openapi/OpenApiService.tmall?key=" + api + "&apiCode=ProductInfo&productCode=" + productNo + "&option=PdOption");
      try
      {
        return this.xml.SelectSingleNode("ProductName").InnerText;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return (string) null;
      }
    }

    public bool productBuyState(HtmlDocument doc)
    {
      try
      {
        return doc.DocumentNode.SelectSingleNode("/html/body").InnerText.Contains("구매하기");
      }
      catch (Exception ex)
      {
        Console.WriteLine("productBuyState err : " + ex?.ToString());
        return false;
      }
    }

    public string productBuyPrice(HtmlDocument doc)
    {
      try
      {
        return doc.DocumentNode.SelectSingleNode("//*[@id='layBodyWrap']/div/div[1]/div/div/div[1]/div[2]/div[2]/div[4]/div[1]/ul/li/dl[1]/dd/strong/span[1]").InnerText.Replace(",", "");
      }
      catch (Exception ex)
      {
        Console.WriteLine("productBuyState err : " + ex?.ToString());
        return (string) null;
      }
    }

    public string getProductShortLink(string prdNo, string optionStckNo, int optionStock) => ProductInfoService.ROOTURL + "prdNo=" + prdNo + "&" + ProductInfoService.INCOMMINGCODE + "&" + ProductInfoService.PRDAPPMTDBDLBCD + "&" + ProductInfoService.MOBILEYN + "&" + ProductInfoService.BCKTEXYN + "&optionStckNo=" + optionStckNo + "&" + ProductInfoService.OPTIONPRC + "&optionStock=" + optionStock.ToString();
  }
}
