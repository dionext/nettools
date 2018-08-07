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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrwSoftware;

namespace Dionext
{

    public enum ProxyEncryptionTypeEnum
    {
        //
        // Summary:
        //     No encryption type specified. 
        //  (DataEncryption=0)
        None = 0,
        //
        // Summary:
        //     Require encryption.
        // (DataEncryption=8)
        Require = 1,
        //
        // Summary:
        //     Require maximum encryption.
        // (DataEncryption=256 (not works с Keenetic Router))
        RequireMax = 2,
        //
        // Summary:
        //     Use encryption if available.
        // (DataEncryption=512 (not works с Keenetic Router))
        Optional = 3
    }

    public enum ProxyProtocolTypeEnum
    {
        PPTP,
        L2TP
    }

    [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer")]
    public class BaseProxyServer
    {
        public static Type CurrentType;

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_JProxyServerId")]
        [JPrimaryKey, JAutoIncrement]
        public string JProxyServerId { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_Url")]
        [JNameProperty, JRequired, JUnique]
        public string Url { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_JCountry")]
        [JManyToOne]
        [JImageRef(DisplyPropertyStyle.TextAndImage)]
        public JCountry JCountry { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_JCountryDeclared")]
        [JManyToOne]
        [JImageRef(DisplyPropertyStyle.TextAndImage)]
        public JCountry JCountryDeclared { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_Town")]
        public string Town { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_TownDeclared")]
        public string TownDeclared { get; set; }


        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_AvailableProtocols")]
        [JDictProp(VpnDictNames.VpnProtocol, true, DisplyPropertyStyle.TextOnly)]
        public IList<string> AvailableProtocols { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_EncryptionType")]
        [JDictProp(VpnDictNames.RasEncryptionType, false, DisplyPropertyStyle.TextOnly)]
        public string EncryptionType { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_Favorite")]
        public bool Favorite { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_ErrorCount")]
        public int ErrorCount { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_SuccessCount")]
        public int SuccessCount { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_LastErrorDate")]
        public DateTimeOffset LastErrorDate { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_LastSuccessDate")]
        public DateTimeOffset LastSuccessDate { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_IsArchive")]
        public bool IsArchive { get; set; }

        [JIgnore]
        [JManyToOne]
        public JRunningJob JRunningJob { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyServer_Stage")]
        [JDictProp(DictNames.RunningJobStage, false, DisplyPropertyStyle.ImageOnly)]
        public string Stage
        {
            get
            {
                if (JRunningJob != null) return JRunningJob.Stage;
                else return null;
            }
        }

        virtual public string GetConnectionName()
        {
            throw new NotImplementedException();
        }
        virtual public BaseProxyProvider GetProxyProvider()
        {
            throw new NotImplementedException();
        }
        virtual public void SetProxyProvider(BaseProxyProvider provider)
        {
            throw new NotImplementedException();
        }

        static public BaseProxyServer FindFromNames(IList<string> names)
        {
            foreach (var name in names)
            {
                string[] n = name.Split('_');
                if (n.Length > 2)
                {
                    object p = Dm.Instance.Find(BaseProxyServer.CurrentType, n[2]);
                    if (p != null) return (BaseProxyServer)p;
                    else continue;
                }
            }
            return null;
        }

        public bool IsProtocolAvailable(ProxyProtocolTypeEnum prot)
        {
            return ModelHelper.IsContainsDict(AvailableProtocols, prot);
        }

    }
}
