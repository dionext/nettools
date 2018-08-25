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
    public partial class BaseVpnSelectorAppForm : AdvancedMainAppForm
    {
        protected ToolStripSplitButton vpnButton;
        protected ToolStripStatusLabel ipAddressLabel;
        protected NotifyIcon vpnNotifyIcon;
        
        
        protected bool useMainTrayIconForVPN = false;
        protected Icon vpnOnIcon = VpnSelectorLibRes.vpn_on;  
        protected Icon vpnOffIcon = VpnSelectorLibRes.vpn_off; 

        public BaseVpnSelectorAppForm()
        {
            InitializeComponent();
        }

        protected void CreateVPNSupport()
        {
            NetConnUtils.InitSettings();

            // 
            // vpnButton
            // 
            this.vpnButton = new System.Windows.Forms.ToolStripSplitButton();
            this.vpnButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.vpnButton.Enabled = true;
            this.vpnButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.vpnButton.Name = "vpnButton";
            this.vpnButton.Size = new System.Drawing.Size(19, 23);
            this.vpnButton.Text = "VPN";
            this.vpnButton.ToolTipText = "VPN";
            vpnButton.Image = VpnSelectorLibRes.network_off;
            //vpnButton.DoubleClick += VpnButton_DoubleClick; - do not working

            // 
            // ipAddressLabel
            // 
            this.ipAddressLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ipAddressLabel.AutoToolTip = true;
            this.ipAddressLabel.Enabled = false;
            this.ipAddressLabel.Name = "ipAddressLabel";
            this.ipAddressLabel.Size = new System.Drawing.Size(66, 20);
            this.ipAddressLabel.Text = "127.0.0.1";
            this.ipAddressLabel.ToolTipText = "127.0.0.1";

            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.eventWarrningButton, this.vpnButton, this.ipAddressLabel});

            if (useMainTrayIconForVPN && BaseApplicationContext.NotifyIcon != null)
            {
                vpnNotifyIcon = BaseApplicationContext.NotifyIcon;
            }
            else
            {
                vpnNotifyIcon = new NotifyIcon(components)
                {
                    ContextMenuStrip = new ContextMenuStrip(),
                    Icon = vpnOffIcon,
                    Text = "VPN",
                    Visible = true
                };
            }

            vpnButton.MouseDown += VpnButton_MouseDown;
            vpnNotifyIcon.ContextMenuStrip.Opening += VpnNotifyIconContextMenuStrip_Opening;
            vpnNotifyIcon.DoubleClick += VpnNotifyIcon_DoubleClick;

            NetConnUtils.OnNetworkChekComplatedEvent += JustNetworkUtils_OnNetworkChekComplatedEvent;
            NetConnUtils.VpnDisconnectedEvent += VpnSelector_OnVpnDisconnectedEvent;
            NetConnUtils.VpnConnectedEvent += VpnSelector_OnVpnConnectedEvent;
            NetConnUtils.VpnDialerErrorEvent += VpnSelector_OnVpnDialerErrorEvent;

            NetConnUtils.CreateDialerAndBeginWatch();
            bool _isActiveConnectionPresent = NetConnUtils.IsActiveConnectionPresent();
            if (_isActiveConnectionPresent)
            {
                NetConnUtils.CurrentProxyServer = BaseProxyServer.FindFromNames(NetConnUtils.GetActiveConnectionsNames());
                ProcessVpnConnectedEvent();
                //DotRasUtils.CurrentProxyServer = DotRasUtils.CurrentProxyServer;
            }
            else
            {
                if ((bool)FrwConfig.Instance.GetPropertyValue(NetConnUtils.SETTING_CHECK_IP_ON_STARTUP, true) == true)
                {
                    NetConnUtils.ConfirmIpAddressAsync();
                }
            }
        }


        private void JustAppMainAppForm_Load(object sender, EventArgs e)
        {
        }
        private void JustNetworkUtils_OnNetworkChekComplatedEvent(object sender, NetworkChekComplatedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate () { ProcessNetworkChekComplatedEvent(); });
                }
                else
                {
                    ProcessNetworkChekComplatedEvent();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
        }
        private void ProcessNetworkChekComplatedEvent()
        {
            if (NetConnUtils.MyExtIPAddressInfo != null)
            {
                ipAddressLabel.Enabled = true;
                ipAddressLabel.Text = " IP: " + NetConnUtils.MyExtIPAddressInfo.Ip;
                ipAddressLabel.ToolTipText = Log.PropertyList(NetConnUtils.MyExtIPAddressInfo);
            }
            else
            {
                ipAddressLabel.Enabled = false;
                ipAddressLabel.Text = VpnSelectorLibRes.Error_Get_IP;
            }
        }

        private void VpnNotifyIconContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                e.Cancel = false;
                //vpnNotifyIcon.ContextMenuStrip.Items.Clear();
                vpnNotifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

                List<ToolStripItem> subitems = new List<ToolStripItem>();
                NetConnUtils.MakeDefaultContextMenu(subitems);
                NetConnUtils.MakeFavoriteContextMenu(subitems);
                NetConnUtils.MakeContextMenuForAllBaseProxyServers(subitems);
                vpnNotifyIcon.ContextMenuStrip.Items.AddRange(subitems.ToArray());
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        private void VpnNotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                NetConnUtils.ConnectOrDisconnectDefautBaseProxyServerAsync();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }



        private void VpnSelector_OnVpnConnectedEvent(object sender, VpnConnectedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate () { ProcessVpnConnectedEvent(); });
                }
                else
                {
                    ProcessVpnConnectedEvent();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
        }
        private void ProcessVpnConnectedEvent()
        {
            this.vpnButton.Enabled = true;
            if (NetConnUtils.CurrentProxyServer != null && NetConnUtils.CurrentProxyServer.JCountry != null
            && NetConnUtils.CurrentProxyServer.JCountry.Image != null)
            {
                string imageName = NetConnUtils.CurrentProxyServer.JCountry.Image;
                Image smallImage = (Image)VpnSelectorLibRes.ResourceManager.GetObject(imageName);
                //if not found in current assembly do advanced search 
                if (smallImage == null) smallImage = TypeHelper.FindImageInAllDiskStorages(imageName);
                
                if (smallImage != null)
                {
                    Image smallImage1 = smallImage.Clone() as Image;
                    vpnButton.Image = smallImage1;
                    //vpnButton.Image = Properties.Resources.network_on;
                    IntPtr icH = ((Bitmap)smallImage).GetHicon();
                    Icon ico = Icon.FromHandle(icH);
                    smallImage.Dispose();

                    vpnNotifyIcon.Icon = ico;
                }
                else
                {
                    vpnButton.Image = VpnSelectorLibRes.network_on;
                    vpnNotifyIcon.Icon = vpnOnIcon;
                }
            }
            else
            {
                vpnButton.Image = VpnSelectorLibRes.network_on;
                vpnNotifyIcon.Icon = vpnOnIcon;
            }
        }
        private void ProcessVpnDisconnectedEvent()
        {
            //AppManager.Instance.ProcessNotification(VpnSelectorLibRes.VPN_disconnected);
            vpnButton.Image = VpnSelectorLibRes.network_off;
            vpnNotifyIcon.Icon = vpnOffIcon;
        }

        private void VpnSelector_OnVpnDisconnectedEvent(object sender, VpnDisconnectedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate () { ProcessVpnDisconnectedEvent(); });
                }
                else
                {
                    ProcessVpnDisconnectedEvent();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
        }
        private void VpnSelector_OnVpnDialerErrorEvent(object sender, VpnDialerErrorEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    MessageBox.Show(null, VpnSelectorLibRes.VPN_dialer_failed__ + e.Exception, VpnSelectorLibRes.Warrning,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(null, VpnSelectorLibRes.VPN_dialer_failed__ + e.Exception, VpnSelectorLibRes.Warrning,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }
        }
        private void VpnButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks > 1)
            {
                try
                {
                    NetConnUtils.ConnectOrDisconnectDefautBaseProxyServerAsync();
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }

            }
        }

    }
}
