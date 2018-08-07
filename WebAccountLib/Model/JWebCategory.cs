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
    [JDisplayName(typeof(WebAccountLibRes), "JWebCategory_JWebCategory")]
    [JEntity(ImageName = "fr_webfolder", Resource = typeof(WebAccountLibRes))]
    public class JWebCategory
    {
        [JDisplayName(typeof(WebAccountLibRes), "JWebCategory_JWebCategoryId")]
        [JPrimaryKey, JAutoIncrement]
        public string JWebCategoryId { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JWebCategory_Name")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JWebCategory_SrcId")]
        public string SrcId { get; set; }


        [JDisplayName(typeof(WebAccountLibRes), "JWebCategory_Parent")]
        [JManyToOne]
        public JWebCategory Parent { get; set; }

    }
}
