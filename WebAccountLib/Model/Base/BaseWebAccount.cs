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
using Newtonsoft.Json;
using FrwSoftware;

namespace Dionext
{
    [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount")]
    public class BaseWebAccount
    {
        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_JAccountId")]
        [JPrimaryKey, JAutoIncrement]
        public string JAccountId { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_Login")]
        [JRequired]
        public string Login { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_FullName")]
        [JNameProperty, JReadOnly, JsonIgnore]
        virtual public string FullName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                /*
                if (JActor != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(JActor.FullName);
                }
                else { */
                    if (s.Length > 0) s.Append(" ");
                    s.Append(Login);
                //}
                return s.ToString();
            }
        }
        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_ShortName")]
        [JShortNameProperty, JReadOnly, JsonIgnore]
        virtual public string ShortName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                /*
                if (JActor != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(JActor.FullName);
                }
                else
                {*/
                    if (s.Length > 0) s.Append(" ");
                    s.Append(Login);
                //}
                return s.ToString();
            }
        }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_Password")]
        public string Password { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_JWeb")]
        [JManyToOne]
        public JWeb JWeb { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_Info")]
        [JText]
        public string Info { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_IsArchive")]
        public bool IsArchive
        {
            get; set;
        }


        [JIgnore, JsonIgnore]
        virtual public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = new WebEntryInfo() ;
                if (JWeb != null)
                {
                    w.Url = JWeb.Url;
                }
                w.Login = Login;
                w.Password = Password;
                /*
                if (AllowedNetworks != null && AllowedNetworks.Count > 0)
                {
                    w.AllowedNetworks = new List<object>();
                    foreach (var n in AllowedNetworks) w.AllowedNetworks.Add(n);
                }
                if (JActor != null)
                {
                    w.SecLevel = JActor.SecLevel;
                }
                */
                return w;
            }
        }
    }
}
