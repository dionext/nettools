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
    [JDisplayName(typeof(WebAccountLibRes), "JWeb_JWeb")]
    [JEntity(ImageName = "fr_web", Resource = typeof(WebAccountLibRes))]
    public class JWeb
    {
        [JDisplayName(typeof(WebAccountLibRes), "JWeb_JWebId")]
        [JPrimaryKey, JAutoIncrement]
        public string JWebId { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JWeb_Name")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JWeb_Url")]
        public string Url { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JWeb_Info")]
        [JText]
        public string Info { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JWeb_IsArchived")]
        public bool IsArchived
        {
            get; set;
        }

        [JDisplayName(typeof(WebAccountLibRes), "JWeb_JWebCategory")]
        [JManyToOne]
        public JWebCategory JWebCategory { get; set; }

        [JDisplayName("IMAP host")]
        public string IMAPHost { get; set; }

        [JDisplayName("SMTP host")]
        public string SMTPHost { get; set; }

        [JDisplayName("POP3 host")]
        public string POP3Host { get; set; }


        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = null;
                IList <JWebAccount> aList = Dm.Instance.ResolveOneToManyRelation<JWebAccount>(this);
                if (aList != null && aList.Count == 1)
                {
                    JWebAccount a = aList.FirstOrDefault();
                    if (a != null)
                    {
                        w = a.WebEntryInfo;
                        //w.Login = a.Login;
                        //w.Password = a.Password;
                    }
                }
                if (w == null)
                {
                    w = new WebEntryInfo() { Url = Url };
                }

                return w;
            }
        }
    }
}
