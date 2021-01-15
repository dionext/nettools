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
using Newtonsoft.Json;


namespace FrwSoftware
{

    [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount")]
    [JEntity(ImageName = "fr_webaccount", Resource = typeof(WebAccountLibRes))]
    public class JWebAccount
    {
        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_JAccountId")]
        [JPrimaryKey, JAutoIncrement]
        public string JAccountId { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_Login")]
        [JRequired]
        public string Login { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_Password")]
        [JPassword]
        public string Password { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_ControlQ")]
        public string ControlQ { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_ControlAns")]
        public string ControlAn { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_JWeb")]
        [JManyToOne]
        public JWeb JWeb { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_Info")]
        [JText]
        public string Info { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_IsArchived")]
        public bool IsArchived
        {
            get; set;
        }

        //////////////////////////////
        ///
        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_FullName")]
        [JNameProperty, JReadOnly, JsonIgnore]
        public string FullName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                if (JActor != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(JActor.FullName);
                }
                else
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(Login);
                }
                return s.ToString();
            }
        }
        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_ShortName")]
        [JShortNameProperty, JReadOnly, JsonIgnore]
        public string ShortName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                if (JActor != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(JActor.FullName);
                }
                else
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(Login);
                }
                return s.ToString();
            }
        }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_JMailAccount")]
        [JManyToOne]
        public JMailAccount JMailAccount { get; set; }


        //todo
        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_AppPassword")]
        public string AppPassword { get; set; }
        [JIgnore, JsonIgnore, JReadOnly]
        public string AutoPassword
        {
            get
            {
                if (AppPassword != null) return AppPassword;
                else return Password;
            }
        }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_OAuthWebAccount")]
        [JManyToOne]
        public JWebAccount OAuthWebAccount { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_JPhoneNumber")]
        [JManyToOne]
        public JPhoneNumber JPhoneNumber { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_JActor")]
        [JManyToOne]
        public JActor JActor { get; set; }


        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_JMoneyBankAcc")]
        [JManyToOne]
        public JMoneyBankAcc JMoneyBankAcc { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_AddJMoneyBankAcc")]
        [JManyToMany]
        public IList<JMoneyBankAcc> AddJMoneyBankAcc { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_LastRenewDate")]
        [JExpired("Expired")]
        public DateTime LastRenewDate { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_ExpiredPeriod")]
        [JDictProp(WADictNames.Period, false, DisplyPropertyStyle.TextOnly)]
        [JExpired("Expired")]
        public string ExpiredPeriod { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_Expired")]
        [JReadOnly, JsonIgnore, JIgnore, JExpired]
        public string Expired
        {
            get
            {
                return BaseTasksUtils.Expired(LastRenewDate, ExpiredPeriod).ToString();
            }
        }

        [JDisplayName(typeof(WebAccountLibRes), "AllowedVPNServer")]
        [JManyToOne]
        public JVPNServer AllowedVPNServer { get; set; }


        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                //do not use  JWeb.WebEntryInfo !!! recurce
                WebEntryInfo w = new WebEntryInfo();
                if (JWeb != null)
                {
                    w.Url = JWeb.Url;
                }
                if (OAuthWebAccount != null)
                {
                    w.Login = OAuthWebAccount.Login;
                    w.Password = OAuthWebAccount.Password;
                }
                else
                {
                    w.Login = Login;
                    w.Password = Password;
                }

                if (JActor != null)
                {
                    //w.AllowedVPNCountrу = JActor.AllowedVPNCountrу;
                    if (JActor.AllowedVPNServer != null) 
                        w.AllowedVPNServerId = JActor.AllowedVPNServer.JVPNServerId;
                    //w.AllowedVPNTown = JActor.AllowedVPNTown;
                }
                //if (AllowedVPNCountrу != null) w.AllowedVPNCountrу = AllowedVPNCountrу;
                if (AllowedVPNServer != null) w.AllowedVPNServerId = AllowedVPNServer.JVPNServerId;
                //if (AllowedVPNTown != null) w.AllowedVPNTown = AllowedVPNTown;
                return w;
            }
        }
    }
}
