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
using Dionext;
using FrwSoftware;

namespace FrwSoftware
{

    public enum VPNEncryptionTypeEnum
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

    public enum VPNProtocolTypeEnum
    {
        PPTP,
        L2TP
    }

    [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer")]
    [JEntity]
    public class JVPNServer
    {
        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_JVPNServerId")]
        [JPrimaryKey, JAutoIncrement]
        public string JVPNServerId { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_Url")]
        [JNameProperty, JRequired, JUnique]
        public string Url { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "FVPNProvider_JVPNProvider")]
        [JManyToOne]
        [JRequired]
        public JVPNProvider JVPNProvider { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_JCountry")]
        [JManyToOne]
        [JImageRef(DisplyPropertyStyle.TextAndImage)]
        public JCountry JCountry { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_JCountryDeclared")]
        [JManyToOne]
        [JImageRef(DisplyPropertyStyle.TextAndImage)]
        public JCountry JCountryDeclared { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_Town")]
        public string Town { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_TownDeclared")]
        public string TownDeclared { get; set; }


        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_AvailableProtocols")]
        [JDictProp(VpnDictNames.VpnProtocol, true, DisplyPropertyStyle.TextOnly)]
        public IList<string> AvailableProtocols { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_EncryptionType")]
        [JDictProp(VpnDictNames.RasEncryptionType, false, DisplyPropertyStyle.TextOnly)]
        public string EncryptionType { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_Favorite")]
        public bool Favorite { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_ErrorCount")]
        public int ErrorCount { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_SuccessCount")]
        public int SuccessCount { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_LastErrorDate")]
        public DateTimeOffset LastErrorDate { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_LastSuccessDate")]
        public DateTimeOffset LastSuccessDate { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_IsArchived")]
        public bool IsArchived { get; set; }

        [JIgnore]
        [JManyToOne]
        public JRunningJob JRunningJob { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNServer_Stage")]
        [JDictProp(DictNames.RunningJobStage, false, DisplyPropertyStyle.ImageOnly)]
        public string Stage
        {
            get
            {
                if (JRunningJob != null) return JRunningJob.Stage;
                else return null;
            }
        }
        
        public string GetConnectionName()
        {
            return JVPNProvider.Name + "_" + (JCountry != null ? JCountry.Name : "") + "_" + JVPNServerId;
        }

        static public JVPNServer FindFromNames(IList<string> names)
        {
            foreach (var name in names)
            {
                string[] n = name.Split('_');
                if (n.Length > 2)
                {
                    object p = Dm.Instance.Find(typeof(JVPNServer), n[2]);
                    if (p != null) return (JVPNServer)p;
                    else continue;
                }
            }
            return null;
        }

        public bool IsProtocolAvailable(VPNProtocolTypeEnum prot)
        {
            return ModelHelper.IsContainsDict(AvailableProtocols, prot);
        }

    }
}
