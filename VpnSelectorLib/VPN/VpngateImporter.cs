/**********************************************************************************
 *   Dionext network tools   https://github.com/dionext/nettools
 *   The open source tools for access to network
 *   MIT License Copyright (c) 2017 Dionext Software
 *   
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using FrwSoftware;

namespace Dionext
{
    public class VpngateImporter
    {
        static public void DowloadAndParse()
        {
            string url = "http://www.vpngate.net/en/";
            WebClient webclient = new WebClient();
            string page = webclient.DownloadString(url);
            page = page.Replace('\n', ' ');
            page = page.Replace('\r', ' ');
            page = page.Replace("</td>  </td>", "</td>");
            page = page.Replace("</td></td>", "</td>");
            ParseDirect(page);
        }
        private static string ClearTag(string tag)
        {
            if (tag == null) return null;
            else return tag.Replace('\r', ' ').Replace('\n', ' ').Trim();
        }
        //todo log
        //todo check vpn provider id 
        //todo final message
        private static void ParseDirect(string s)
        {
            List<BaseProxyServer> tmpList = new List<BaseProxyServer>();
            IList cl = Dm.Instance.FindAll(typeof(JCountry));
            TextReader reader = new StringReader(s);

            HtmlDocument doc = new HtmlDocument();
            doc.Load(reader);
            if (doc.DocumentNode != null)
            {
                HtmlNode span = doc.DocumentNode.SelectSingleNode("//span[@id='Label_Table']");
                HtmlNode table = span.SelectSingleNode("table[@id='vg_hosts_table_id']");

                HtmlNodeCollection trs = table.SelectNodes("tr");
                if (trs != null)
                {
                    //BaseProxyServer p = null;
                    bool firstTr = true;
                    foreach (HtmlNode tr in trs)
                    {
                        if (firstTr)//header 
                        {
                            firstTr = false;
                            continue;
                        }
                        if (tr.SelectNodes("td[@class='vg_table_header']") != null && tr.SelectNodes("td[@class='vg_table_header']").Count > 0)
                        {
                            continue;
                        }

                        HtmlNodeCollection tds = tr.SelectNodes("td");
                        if (tds != null)
                        {
                            bool valid = false;
                            int tdnum = 0;
                            foreach (HtmlNode td in tds)
                            {
                                if (tdnum == 5)
                                {
                                    string l2tp = ClearTag(td.InnerText);
                                    if (string.IsNullOrEmpty(l2tp) == false && l2tp.Contains("L2TP"))
                                    {
                                        valid = true;
                                        break;
                                    }
                                }
                                tdnum++;
                            }
                            if (valid)
                            {
                                BaseProxyServer p = (BaseProxyServer)Activator.CreateInstance(BaseProxyServer.CurrentType);
                                tdnum = 0;
                                foreach (HtmlNode td in tds)
                                {
                                    if (tdnum == 0)//country <img src='../images/flags/JP.png' width='32' height='32' /><br>Japan</td>
                                    {
                                        string p_JCountry = ClearTag(td.InnerText);
                                        if (p_JCountry != null)
                                        {
                                            foreach (JCountry c in cl)
                                            {
                                                if (c.Name.Equals(p_JCountry)
                                                    || (c.Official_name_en != null && c.Official_name_en.Equals(p_JCountry))
                                                    || (c.Official_name_fr != null && c.Official_name_fr.Equals(p_JCountry))
                                                    )
                                                {
                                                    p.JCountry = c;
                                                    break;
                                                }
                                            }
                                        }
                                        if (p.JCountry == null)
                                        {
                                            HtmlNode image = td.SelectSingleNode("img");
                                            if (image != null)
                                            {
                                                string src = image.GetAttributeValue("src", null);
                                                if (src != null)
                                                {
                                                    int indexDot = src.IndexOf(".png");
                                                    int indexSplash = src.LastIndexOf("/");
                                                    if (indexDot > -1 && indexSplash > -1 && indexDot > indexSplash)
                                                    {
                                                        string countryCode = src.Substring(indexSplash + 1, (indexDot - indexSplash) - 1);
                                                        countryCode = countryCode.ToLower();
                                                        foreach (JCountry c in cl)
                                                        {
                                                            if (c.JCountryId.Equals(countryCode))
                                                            {
                                                                p.JCountry = c;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (p.JCountry == null)
                                        {
                                            Console.WriteLine("==================================================");
                                            Console.WriteLine("Country not found: " + td.InnerHtml);
                                            Console.WriteLine("==================================================");
                                        }
                                    }
                                    if (tdnum == 1)//url  <b><span style='font-size: 9pt;'>word-wind-vpn.opengw.net</span></b><br><span style='font-size: 10pt;'>218.229.183.122</span><br><span style='font-size: 7pt;'>(nttkyo1188122.tkyo.nt.ngn.ppp.infoweb.ne.jp)</span>
                                    {
                                        HtmlNode b = td.SelectSingleNode("b/span");
                                        string urlsStr = ClearTag(b.InnerText);
                                        p.Url = urlsStr;
                                    }
                                    tdnum++;
                                }
                                tmpList.Add(p);
                            }//valid
                        }//tds
                    }
                }
            }

            List<BaseProxyServer> newList = new List<BaseProxyServer>();
            List<BaseProxyServer> sameList = new List<BaseProxyServer>();
            List<BaseProxyServer> restoredList = new List<BaseProxyServer>();
            List<BaseProxyServer> archivedList = new List<BaseProxyServer>();

            BaseProxyProvider pp = (BaseProxyProvider)Dm.Instance.Find(BaseProxyProvider.CurrentType, "5");
            foreach (var p0 in tmpList)
            {
                //Console.WriteLine(ModelHelper.ModelPropertyList(p0, "\n", null, null));
                //Console.WriteLine("==================================================");

                BaseProxyServer foundSame = null;
                IList oldList = Dm.Instance.FindAll(BaseProxyServer.CurrentType);
                foreach (var o in oldList)
                {
                    BaseProxyServer oldP = (BaseProxyServer)o;
                    if (oldP.GetProxyProvider() != null && oldP.GetProxyProvider().Equals(pp) && oldP.Url.Equals(p0.Url))
                    {
                        foundSame = oldP;
                        if (oldP.IsArchive == true)
                        {
                            oldP.IsArchive = false;
                            restoredList.Add(oldP);
                            Dm.Instance.SaveObject(oldP);
                        }
                        break;
                    }
                }
                if (foundSame == null)
                {
               
                    BaseProxyServer p = (BaseProxyServer)Dm.Instance.EmptyObject(BaseProxyServer.CurrentType, null);
                    p.SetProxyProvider(pp);
                    p.Url = p0.Url;
                    p.JCountry = p0.JCountry;
                    p.AvailableProtocols = new List<string>();
                    p.AvailableProtocols.Add(ProxyProtocolTypeEnum.L2TP.ToString());
                    p.EncryptionType = ProxyEncryptionTypeEnum.Require.ToString();

                    Dm.Instance.SaveObject(p);
                    newList.Add(p);
                   
                }
                else
                {
                    sameList.Add(foundSame);
                }
            }
            //reverse search 
            IList oldList2 = Dm.Instance.FindAll(BaseProxyServer.CurrentType);
            foreach (var o in oldList2)
            {
                BaseProxyServer oldP = (BaseProxyServer)o;
                if (oldP.GetProxyProvider() != null && oldP.GetProxyProvider().Equals(pp))
                {
                    bool found = false;
                    foreach (var p0 in tmpList)
                    {
                        if (oldP.Url.Equals(p0.Url))
                        {
                            found = true;
                        }
                    }
                    if (!found && oldP.IsArchive == false)
                    {
                        oldP.IsArchive = true;
                        archivedList.Add(oldP);
                    }
                }
            }
            Log.ProcessDebug("================ same ================ " + sameList.Count);
            foreach (var v in sameList)
            {
                Log.ProcessDebug(v.Url);
            }
            Log.ProcessDebug("================ new ================ " + newList.Count);
            foreach (var v in newList)
            {
                Log.ProcessDebug(v.Url);
            }
            Log.ProcessDebug("================ restored ================ " + restoredList.Count);
            foreach (var v in restoredList)
            {
                Log.ProcessDebug(v.Url);
            }
            Log.ProcessDebug("================ archive ================ " + archivedList.Count);
            foreach (var v in archivedList)
            {
                Log.ProcessDebug(v.Url);
            }
        }
    }
}
