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

        [JDisplayName(typeof(WebAccountLibRes), "JWeb_SrcId")]
        public string SrcId { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JWeb_Info")]
        [JText]
        public string Info { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JWeb_IsArchive")]
        public bool IsArchive
        {
            get; set;
        }

        [JDisplayName(typeof(WebAccountLibRes), "JWeb_JWebCategory")]
        [JManyToOne]
        public JWebCategory JWebCategory { get; set; }

        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = new WebEntryInfo() { Url = Url };
                /*
                if (JAccounts != null && JAccounts.Count == 1)
                {
                    w.Login = JAccounts[0].Login;
                    w.Password = JAccounts[0].Password;
                }*/
                return w;
            }
        }
    }
}
