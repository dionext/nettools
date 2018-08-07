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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrwSoftware;

namespace Dionext
{
    public partial class WebAccountMainForm : BaseVpnSelectorAppForm
    {

        public WebAccountMainForm()
        {
            InitializeComponent();
            useMainTrayIconForVPN = true;
            this.Name = WebAccountLibRes.Web_accounts_management;
            this.Text = this.Name;
            CreateVPNSupport();
            CreateMainMenuItems();
        }


        private void CreateMainMenuItems()
        {
            ToolStripMenuItem groupItem = null;
            //dict
            groupItem = new ToolStripMenuItem(VpnSelectorLibRes.VPN);
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(FProxyProvider));
            CreateMainMenuItemForEntityType(groupItem, typeof(FProxyServer));
            groupItem = new ToolStripMenuItem(VpnSelectorLibRes.Service);
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(JCountry));
            groupItem = new ToolStripMenuItem(WebAccountLibRes.Web);
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForWindowType(groupItem, WebAccountLibRes.Web__tree_, typeof(FWebTreeWindow));
            CreateMainMenuItemForEntityType(groupItem, typeof(JWeb));
            CreateMainMenuItemForEntityType(groupItem, typeof(FWebAccount));
            groupItem = new ToolStripMenuItem(VpnSelectorLibRes.Service);
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(JCountry));
            CreateMainMenuItemForEntityType(groupItem, typeof(JUserAgent));
            //tools
            CreateToolsMenuItems(menuItemTools, toolBar, statusBar);
            //file
            CreateFileMenuItems(menuItemFile);
            //view
            CreateViewMenuItems(menuItemView, toolBar, statusBar);
            //help
            CreateHelpMenuItems(menuItemHelp, this);

        }
    }
}
