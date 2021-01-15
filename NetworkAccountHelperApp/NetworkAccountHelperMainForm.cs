using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dionext;


namespace FrwSoftware
{
    public partial class NetworkAccountHelperMainForm : BaseVpnSelectorAppForm
    {
        public NetworkAccountHelperMainForm()
        {
            InitializeComponent();
            useMainTrayIconForVPN = true;
            this.Name = "DevOps helper";// WebAccountLibRes.Web_accounts_management;
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
            CreateMainMenuItemForEntityType(groupItem, typeof(JVPNProvider));
            CreateMainMenuItemForEntityType(groupItem, typeof(JVPNServer));
            groupItem = new ToolStripMenuItem(WebAccountLibRes.Web);
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForWindowType(groupItem, WebAccountLibRes.Web__tree_, typeof(JWebTreeWindow));
            CreateMainMenuItemForEntityType(groupItem, typeof(JWeb));
            CreateMainMenuItemForEntityType(groupItem, typeof(JWebAccount));
            groupItem = new ToolStripMenuItem(VpnSelectorLibRes.Service);
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(JCountry));
            CreateMainMenuItemForEntityType(groupItem, typeof(JWorldCurrency));
            CreateMainMenuItemForEntityType(groupItem, typeof(JUserAgent));
            groupItem = new ToolStripMenuItem(WebAccountLibRes.Phones);
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(JMobileOperator));
            CreateMainMenuItemForEntityType(groupItem, typeof(JPhoneNumber));

            groupItem = new ToolStripMenuItem(WebAccountLibRes.Mail_and_messangers);
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(JMailAccount));
            groupItem.DropDownItems.Add(new ToolStripSeparator());
            groupItem = new ToolStripMenuItem(NetworkAccountHelperLibRes.Hosting);
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(JDomain));
            CreateMainMenuItemForEntityType(groupItem, typeof(JSite));
            CreateMainMenuItemForEntityType(groupItem, typeof(JUserAccount));
            groupItem = new ToolStripMenuItem(NetworkAccountHelperLibRes.Infrastructure);
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForWindowType(groupItem, NetworkAccountHelperLibRes.Infrastructure__tree_, typeof(BaseCompResourcesTreeWindow));
            CreateMainMenuItemForEntityType(groupItem, typeof(JCompDeviceNetwork));
            CreateMainMenuItemForEntityType(groupItem, typeof(JCompDevice));
            CreateMainMenuItemForEntityType(groupItem, typeof(JCompDeviceStorage));
            CreateMainMenuItemForEntityType(groupItem, typeof(JDomain));
            CreateMainMenuItemForEntityType(groupItem, typeof(JSoft));
            CreateMainMenuItemForEntityType(groupItem, typeof(JSoftInstance));
            CreateMainMenuItemForEntityType(groupItem, typeof(JUserAccount));
            CreateMainMenuItemForEntityType(groupItem, typeof(JActor));
            groupItem = new ToolStripMenuItem(WebAccountLibRes.Finance);
            menuItemDict.DropDownItems.Add(groupItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(JMoneyBankAcc));
            //tools
            CreateToolsMenuItems(menuItemTools, toolBar, statusBar);
            CreateMainMenuItemForWindowType(menuItemTools, NetworkAccountHelperLibRes.Enhanced_Console, typeof(ApplicationConsoleWindow));
            CreateMainMenuItemForWindowType(menuItemTools, NetworkAccountHelperLibRes.Error_and_notification_console, typeof(NotifAndErrorConsoleWindow));
            //file
            CreateFileMenuItems(menuItemFile);
            //view
            CreateViewMenuItems(menuItemView, toolBar, statusBar);
            //help
            CreateHelpMenuItems(menuItemHelp, this);

        }
    }
}
