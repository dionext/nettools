﻿/**********************************************************************************
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
    public class VpnDictNames : DictNames
    {
        public const string RasEncryptionType = "RasEncryptionType";
        public const string VpnProtocol = "VpnProtocol";
    }
    public class VpnSelectorDm : Dm
    {
        static public string MyLocalIP { get; set; }
        //static public JIPAddress MyExtJIPAddress { get; set; }
        static public string MyMac { get; set; }
        static public string DnsIP { get; set; }
        static public string DnsMac { get; set; }

        override protected void InitDictionaries()
        {
            base.InitDictionaries();


            JDictionary dict = null;
            dict = new JDictionary() { Id = VpnDictNames.RasEncryptionType };
             dictionaries.Add(dict);
            dict.Items.Add(new JDictItem() { Key = VPNEncryptionTypeEnum.None.ToString(), Text = "None" });
            dict.Items.Add(new JDictItem() { Key = VPNEncryptionTypeEnum.Optional.ToString(), Text = "Optional" });
            dict.Items.Add(new JDictItem() { Key = VPNEncryptionTypeEnum.Require.ToString(), Text = "Require" });
            dict.Items.Add(new JDictItem() { Key = VPNEncryptionTypeEnum.RequireMax.ToString(), Text = "RequireMax" });

            dict = new JDictionary() { Id = VpnDictNames.VpnProtocol };
            dict.Items.Add(new JDictItem() { Key = VPNProtocolTypeEnum.PPTP.ToString() });
            dict.Items.Add(new JDictItem() { Key = VPNProtocolTypeEnum.L2TP.ToString() });
            Dm.Instance.AddDictionary(dict);

        }
    }
}
