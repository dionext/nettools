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
    [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider")]
    public class BaseProxyProvider
    {
        public static Type CurrentType;

        public const string ProxyProviderSecType = "ProxyProviderSecType";

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider_JProxyProviderId")]
        [JPrimaryKey, JAutoIncrement]
        public string JProxyProviderId { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider_Name")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider_URL")]
        public string Url { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider_Login")]
        public string Login { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider_Password")]
        public string Password { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider_VPNLogin")]
        public string VPNLogin { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider_VPNPassword")]
        public string VPNPassword { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider_UserPresharedKey")]
        public string UserPresharedKey { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider_SecLevel")]
        [JDictProp(DictNames.SecLevel, false, DisplyPropertyStyle.TextOnly)]
        public int SecLevel { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider_Info")]
        [JText]
        public string Info { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseProxyProvider_Attachments")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }
    }
}
