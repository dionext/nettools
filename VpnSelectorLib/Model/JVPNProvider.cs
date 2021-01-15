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
using Dionext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FrwSoftware
{
    [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNProvider")]
    [JEntity]
    public class JVPNProvider
    {
        public const string VPNProviderSecType = "VPNProviderSecType";

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNProvider_JVPNProviderId")]
        [JPrimaryKey, JAutoIncrement]
        public string JVPNProviderId { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNProvider_Name")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNProvider_URL")]
        public string Url { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNProvider_Login")]
        public string Login { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNProvider_Password")]
        [JPassword]
        public string Password { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNProvider_VPNLogin")]
        public string VPNLogin { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNProvider_VPNPassword")]
        [JPassword]
        public string VPNPassword { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNProvider_UserPresharedKey")]
        public string UserPresharedKey { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNProvider_Info")]
        [JText]
        public string Info { get; set; }

        [JDisplayName(typeof(VpnSelectorLibRes), "BaseVPNProvider_Attachments")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }
    }
}
