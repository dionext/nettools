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
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using FrwSoftware;

namespace Dionext
{
    public partial class VpnSelectorMainForm : BaseVpnSelectorAppForm
    {
        public VpnSelectorMainForm()
        {
            InitializeComponent();
            useMainTrayIconForVPN = true;
            CreateVPNSupport();
            this.Name = VpnSelectorLibRes.VPN_selector;
            this.Text = this.Name;
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
            //file
            CreateFileMenuItems(menuItemFile);
            //tools
            CreateToolsMenuItems(menuItemTools, toolBar, statusBar);
            //view
            CreateViewMenuItems(menuItemView, toolBar, statusBar);
            //help
            CreateHelpMenuItems(menuItemHelp, this);
        }
    }
}
