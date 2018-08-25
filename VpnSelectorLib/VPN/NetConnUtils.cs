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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotRas;
using FrwSoftware;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Net;
using Newtonsoft.Json;
using FrwSoftware;

namespace Dionext
{
    public class NetworkChekComplatedEventArgs : EventArgs
    {
    }
    public delegate void NetworkChekComplatedEventHandler(object sender, NetworkChekComplatedEventArgs e);

    public class VpnDisconnectedEventArgs : EventArgs    {    }
    public delegate void VpnDisconnectedEventHandler(object sender, VpnDisconnectedEventArgs e);
    public class VpnConnectedEventArgs : EventArgs { }
    public delegate void VpnConnectedEventHandler(object sender, VpnConnectedEventArgs e);
    public class VpnDialerErrorEventArgs : EventArgs {
        public Exception Exception { get; set; }
    }
    public delegate void VpnDialerErrorEventHandler(object sender, VpnDialerErrorEventArgs e);


    //Note: we only work with PPTP and L2TP connections 
    //Note: most methods are static (we can have only one vpn connection at the same time)
    public class NetConnUtils
    {
        static public string MyExternalIP { get; set; }
        static public JIPAddressInfo MyExtIPAddressInfo { get; set; }
        static public BaseProxyServer CurrentProxyServer { get; set; }

        static public event NetworkChekComplatedEventHandler OnNetworkChekComplatedEvent;

        //static private WebClient webclient = new WebClient();
        // to improve the performance of repeated requests, you need to isolate it into a static variable
        // however, it is possible to issue a warning to the log "WebClient does not support simultaneous I/O operations"
        static private RasDialer dialer = null;
        static private RasHandle handle = null;
        public static bool IsReconnect { get; set; }
        public static VpnConnectedEventHandler VpnConnectedEvent = null;
        public static VpnDisconnectedEventHandler VpnDisconnectedEvent = null;
        public static VpnDialerErrorEventHandler VpnDialerErrorEvent = null;
        static public string DEFAULT_JPROXY_SERVER = "MainApp.DefaultProxyServer";
        static public string SETTING_CHECK_IP_ON_STARTUP = "Vpn.CheckIPOnStarup";
        static public BaseProxyServer BaseProxyServerDefault = null;
        static private RasConnectionWatcher watcher = new RasConnectionWatcher();// for the watcher to run all the events it must be run when the application is loaded

        static public bool IsSynchMode { get; set; }
        
        //must be call after Dm init
        public static void InitSettings()
        {

            JSetting setting = FrwConfig.Instance.CreatePropertyIfNotExist(new JSetting()
            {
                Name = "Vpn.Reconnect",
                Description = VpnSelectorLibRes.Reconnect_to_VPN,
                Help = VpnSelectorLibRes.Reconnect_to_current_VPN_server_when_connection_lost,
                Group = "VPN",
                Value = true,
                IsUser = true
            });
            NetConnUtils.IsReconnect = FrwConfig.Instance.GetPropertyValueAsBool(setting.Name, true);
            setting = FrwConfig.Instance.CreatePropertyIfNotExist(new JSetting()
            {
                Name = NetConnUtils.DEFAULT_JPROXY_SERVER,
                Description = VpnSelectorLibRes.VPN_server_by_default,
                Group = "VPN",
                Value = null,
                ValueType = BaseProxyServer.CurrentType,
                IsUser = true
            });
            setting = FrwConfig.Instance.CreatePropertyIfNotExist(new JSetting()
            {
                Name = NetConnUtils.SETTING_CHECK_IP_ON_STARTUP,
                Description = VpnSelectorLibRes.Check_IP_address_on_startup,
                Group = "VPN",
                Value = true,
                IsUser = true
            });

        }

        static private void BeginWatch()
        {
            watcher.Connected += new EventHandler<RasConnectionEventArgs>(watcher_Connected);
            watcher.Disconnected += new EventHandler<RasConnectionEventArgs>(watcher_Disconnected);
            watcher.EnableRaisingEvents = true;
        }
        static private void watcher_Connected(object sender, RasConnectionEventArgs e)
        {
            // A connection has successfully connected.
            Log.ProcessDebug("Event " +  VpnSelectorLibRes.A_connection_has_successfully_connected_ + e.Connection.EntryName);

            BaseProxyServer oldProxy = NetConnUtils.CurrentProxyServer;
            if (IsSynchMode == false)
            {
                NetConnUtils.CurrentProxyServer = BaseProxyServer.FindFromNames(NetConnUtils.GetActiveConnectionsNames());
            }
            if (VpnConnectedEvent != null) VpnConnectedEvent(null, new VpnConnectedEventArgs());
            if (IsSynchMode == false)
            {
                if ((oldProxy == null && NetConnUtils.CurrentProxyServer != null)
                    || (oldProxy != null && oldProxy != NetConnUtils.CurrentProxyServer))
                {
                    //proxy changed
                    NetConnUtils.ConfirmIpAddressAsync();
                }

            }
        }
        static private void watcher_Disconnected(object sender, RasConnectionEventArgs e)
        {
            // A connection has disconnected successfully.
            Log.ProcessDebug("Event " + VpnSelectorLibRes.A_connection_has_disconnected_successfully_ + e.Connection.EntryName);
          
            if (VpnDisconnectedEvent != null) VpnDisconnectedEvent(null, new VpnDisconnectedEventArgs());
            if (IsSynchMode == false)
            {
                AppManager.Instance.ProcessNotification(VpnSelectorLibRes.VPN_disconnected);
                BaseProxyServer oldProxy = NetConnUtils.CurrentProxyServer;
                NetConnUtils.CurrentProxyServer = null;
                if (oldProxy != null)
                {
                    //proxy changed
                    NetConnUtils.ConfirmIpAddressAsync();
                }
                //reconnect
                //https://stackoverflow.com/questions/23950702/reconnect-vpn-windows-service
                if (NetConnUtils.CurrentProxyServer != null && IsReconnect == true)
                {
                    OpenConnectLocal(NetConnUtils.CurrentProxyServer, true);
                }
            }

        }


        static public void CreateConnectionEntry(BaseProxyServer ps, ProxyProtocolTypeEnum protocolType)
        {
            if (!ps.IsProtocolAvailable(protocolType)) throw new ArgumentException("Protocol " + protocolType.ToString() + " not avilable");

            //http://stackoverflow.com/questions/36213393/get-connection-status-vpn-using-dotras
            // File.WriteAllText("your rasphone.pbk  path","")//Add
            RasPhoneBook rasPhoneBook1 = new RasPhoneBook();
            string rasPhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);//alt RasPhoneBookType.AllUsers
            rasPhoneBook1.Open(rasPhoneBookPath);

            string deviceTypeStr = "(" + protocolType.ToString() + ")";
            RasVpnStrategy strategy = (protocolType == ProxyProtocolTypeEnum.L2TP) ? RasVpnStrategy.L2tpOnly : RasVpnStrategy.PptpOnly;
            //alt 
            //RasVpnStrategy strategy = RasVpnStrategy.Default;
            RasEntry entry = RasEntry.CreateVpnEntry(ps.GetConnectionName(), ps.Url,
                strategy,
                RasDevice.GetDeviceByName(deviceTypeStr, RasDeviceType.Vpn, false));
            entry.EncryptionType = ps.EncryptionType.ToEnum<RasEncryptionType>();
            if (protocolType == ProxyProtocolTypeEnum.L2TP && !string.IsNullOrEmpty(ps.GetProxyProvider().UserPresharedKey)) 
            {
                entry.Options.UsePreSharedKey = true;
            }
            rasPhoneBook1.Entries.Add(entry);
            if (protocolType == ProxyProtocolTypeEnum.L2TP && !string.IsNullOrEmpty(ps.GetProxyProvider().UserPresharedKey))
            {
                entry.UpdateCredentials(RasPreSharedKey.Client, ps.GetProxyProvider().UserPresharedKey);
            }

            if (!string.IsNullOrEmpty(ps.GetProxyProvider().VPNLogin))
            {
                //entry.UpdateCredentials(new System.Net.NetworkCredential(ps.JProxyProvider.VPNLogin, ps.JProxyProvider.VPNPassword), false);
            }

        }
        static public bool IsConnectionEntryExist(BaseProxyServer ps)
        {
            string path = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);
            if (path != null && File.Exists(path))
            {
                string name = ps.GetConnectionName();
                return RasEntry.Exists(name, path);
            }
            else return false;
        }
        static public void RemoveConnectionEntry(BaseProxyServer ps)
        {
            string name = ps.GetConnectionName();
            RasPhoneBook rasPhoneBook1 = new RasPhoneBook();
            string rasPhoneBookPath = null;
            rasPhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);
            rasPhoneBook1.Open(rasPhoneBookPath); 
            RasEntry entryFound = null;
            foreach (RasEntry entry1 in rasPhoneBook1.Entries)
            {
                if (entry1.Name.Equals(name))
                {
                    entryFound = entry1;
                    break;
                }
            }
            if (entryFound != null) entryFound.Remove();

        }
        static public void RemoveAllConnectionEntry()
        {
            RasPhoneBook rasPhoneBook1 = new RasPhoneBook();
            string rasPhoneBookPath = null;
            rasPhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);
            rasPhoneBook1.Open(rasPhoneBookPath); 
            List<RasEntry> list = new List<RasEntry>();
            foreach (RasEntry entry1 in rasPhoneBook1.Entries)
            {
                list.Add(entry1);
            }
            foreach (RasEntry entry1 in list)
            {
                entry1.Remove();
            }
        }

        static public string ShowConnectionEntries()
        {
            string br = "\n\r";
            StringBuilder str = new StringBuilder();
            RasPhoneBook rasPhoneBook1 = new RasPhoneBook();
            string rasPhoneBookPath = null;
            try
            {
                rasPhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);
                str.Append(VpnSelectorLibRes.User_Phonebook_path__ + rasPhoneBookPath);
                str.Append(br);
                str.Append(br);
                rasPhoneBook1.Open(rasPhoneBookPath);
                foreach (RasEntry entry1 in rasPhoneBook1.Entries)
                {
                    str.Append(VpnSelectorLibRes.Name__ + entry1.Name + VpnSelectorLibRes._Server__ + entry1.PhoneNumber);
                    str.Append(br);
                }
            }
            catch (Exception ex)
            {
                str.Append("Error: " + ex.ToString());
                str.Append(br);
            }
            /* gets error in windows 10 for security reason 
            try
            {
                rasPhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
                str.Append(VpnSelectorLibRes.All_User_Phonebook_path__ + rasPhoneBookPath);
                str.Append(br);
                rasPhoneBook1.Open(rasPhoneBookPath);// в примере открытие без параметров 
                foreach (RasEntry entry1 in rasPhoneBook1.Entries)
                {
                    str.Append(entry1.Name);
                    str.Append(br);
                }
            }
            catch (Exception ex)
            {
                str.Append("Error: " + ex.Message);
                str.Append(br);
            }
            */
            return str.ToString();
        }

        static public bool IsActiveConnectionPresent()
        {
            return (RasConnection.GetActiveConnections().Count > 0);
        }

        static public IList<string> GetActiveConnectionsNames()
        {
            List<string> list = new List<string>();
            foreach (var s in RasConnection.GetActiveConnections())
            {
                list.Add(s.EntryName);
            }
            return list;
        }


        static public string ShowActiveConnections()
        {
            string br = "\n\r";
            StringBuilder str = new StringBuilder();
            str.Append(VpnSelectorLibRes.Active_Connection_list);
            str.Append(br);
            foreach (RasConnection connection in RasConnection.GetActiveConnections())
            {
                RasIPInfo ipAddresses = (RasIPInfo)connection.GetProjectionInfo(RasProjectionType.IP);

                str.Append(VpnSelectorLibRes.Connection__ + connection.EntryName + "  ID: " + connection.EntryId);
                str.Append(br);
                if (ipAddresses != null)
                {
                    str.Append(VpnSelectorLibRes.IP__ + ipAddresses.IPAddress.ToString());
                    str.Append(br);
                    str.Append(VpnSelectorLibRes.Server_IP_Address__ + ipAddresses.ServerIPAddress.ToString());
                    str.Append(br);
                }
            }
            return str.ToString();
        }
        static public void OpenConnect(BaseProxyServer ps)
        {
            
            OpenConnectLocal(ps, true);
        }

        // for the watcher to run all the events it must be run when the application is loaded
        static public void CreateDialerAndBeginWatch()
        {
            if (dialer == null)
            {
                dialer = new RasDialer();
                dialer.StateChanged += Dialer_StateChanged;
                dialer.DialCompleted += Dialer_DialCompleted;
                dialer.Error += Dialer_Error;

                System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += new System.Net.NetworkInformation.NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
                Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
                //
                BeginWatch();
            }
        }

        static private void OpenConnectLocal(BaseProxyServer ps, bool async)
        {
            CreateDialerAndBeginWatch();//if not created yet
            //http://www.dotnetobject.com/Thread-Connecting-VPN-using-C

            dialer.EntryName = ps.GetConnectionName();
            dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);
            try
            {
                try
                {
                    RasConnection conn = RasConnection.GetActiveConnectionByName(ps.GetConnectionName(), dialer.PhoneBookPath);
                    if (conn != null) conn.HangUp();
                }
                catch (Exception)
                {
                    //no connection present
                }
                if (!string.IsNullOrEmpty(ps.GetProxyProvider().VPNLogin))
                {
                    System.Net.NetworkCredential cred = new System.Net.NetworkCredential(ps.GetProxyProvider().VPNLogin, ps.GetProxyProvider().VPNPassword);
                    dialer.Credentials = cred;
                }
                //dialer.Dial();//alt DialAsync()  
                if (async)
                    handle = dialer.DialAsync();
                else
                {
                    handle = dialer.Dial();
                    NetConnUtils.CurrentProxyServer = ps;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        static public void CloseConnect(BaseProxyServer ps, bool async)
        {
            if (ps == null)
            {
                if (dialer != null)
                {
                    RasConnection conn = RasConnection.GetActiveConnectionByName(dialer.EntryName, dialer.PhoneBookPath);
                    if (conn != null)
                    {
                        conn.HangUp();
                    }
                }
            }
            else
            {
                RasConnection conn = RasConnection.GetActiveConnectionByName(ps.GetConnectionName(), RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User));
                if (conn != null)
                {
                    conn.HangUp();
                }
            }
            if (!async) NetConnUtils.CurrentProxyServer = null;
        }

        static public void CloseAllActiveConnections(bool async)
        {
            foreach (RasConnection conn in RasConnection.GetActiveConnections())
            {
                if (conn != null)
                {
                    conn.HangUp();
                }
            }
            if (!async) NetConnUtils.CurrentProxyServer = null;
        }


        static public bool IsConnected(BaseProxyServer ps)
        {
            RasConnection conn = null;
            if (ps == null)
            {
                if (dialer != null)
                {
                    conn = RasConnection.GetActiveConnectionByName(dialer.EntryName, dialer.PhoneBookPath);
                }
            }
            else
            {
                conn = RasConnection.GetActiveConnectionByName(ps.GetConnectionName(), RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User));
            }

            if (conn != null)
            {
                RasConnectionStatus st = conn.GetConnectionStatus();
                if (st.ConnectionState == RasConnectionState.Connected) return true;
                else return false;
            }
            else return false;
        }

        private static void Dialer_Error(object sender, System.IO.ErrorEventArgs e)
        {
            Log.LogError("Event Dialer_Error: " + e.ToString(), e.GetException());
        }

        private static void Dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            //Note: events are not generated when manipulating with a connection from outside
            if (e.Cancelled)
            {
                Log.ProcessDebug("Event Dialer Cancelled");
            }
            else if (e.TimedOut)
            {
                Log.ProcessDebug("Event Dialer Timeout");
            }
            else if (e.Connected)
            {
                Log.ProcessDebug("Event Dialer Connection successful");
            }
            else if (e.Error != null)
            {
                Log.LogError("Event Dialer Error " + e.Error.ToString(), e.Error);
                try
                {
                    if (dialer.EntryName != null)
                    {
                        BaseProxyServer currentProxyServer = BaseProxyServer.FindFromNames(new List<string>() { dialer.EntryName });
                        if (currentProxyServer != null)
                        {
                            currentProxyServer.ErrorCount = currentProxyServer.ErrorCount + 1;
                            currentProxyServer.LastErrorDate = DateTimeOffset.Now;
                            Dm.Instance.SaveObject(currentProxyServer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.ProcessDebug("Event Error find BaseProxyServer " + ex);
                }

                if (IsSynchMode == false)
                {
                    if (VpnDialerErrorEvent != null)
                    {
                        VpnDialerErrorEvent(null, new VpnDialerErrorEventArgs() { Exception = e.Error });
                    }
                }
            }
            if (!e.Connected)
            {
                Log.ProcessDebug("Dialer not connected");
            }
        }

        private static void Dialer_StateChanged(object sender, StateChangedEventArgs e)
        {
            Log.ProcessDebug("Event Dialer_StateChanged: " + e.State.ToString());
        }

        private static void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            Log.ProcessDebug("Event SystemEvents_PowerModeChanged " + e.Mode);
            //if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            //if (e.Mode == Microsoft.Win32.PowerModes.Suspend)
        }

        private static void NetworkChange_NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            Log.ProcessDebug("Event NetworkChange_NetworkAvailabilityChanged e.IsAvailable: " + e.IsAvailable);
        }

        static public BaseProxyServer GetDefaulBaseProxyServer()
        {
            if (BaseProxyServerDefault == null)
            {
                JSetting setting = FrwConfig.Instance.GetProperty(DEFAULT_JPROXY_SERVER);
                if (setting != null)
                {
                    BaseProxyServerDefault = setting.Value as BaseProxyServer;
                }
            }
            return BaseProxyServerDefault; 
        }
        static public bool ConnectOrDisconnectDefautBaseProxyServerAsync()
        {
            bool statusChanged = false;
            if (NetConnUtils.IsActiveConnectionPresent())
            {
                NetConnUtils.CloseAllActiveConnections(true);
                statusChanged = true;
            }
            else
            {
                BaseProxyServer item = GetDefaulBaseProxyServer();
                if (item != null)
                {
                    statusChanged = CreateAndConnectToProxyAsync(item);
                }
                else
                {
                    MessageBox.Show(VpnSelectorLibRes.Double_click_on_this_icon_creates_VPN_connection_to_default_VPN_server_which_not_set__No_connection_created_, VpnSelectorLibRes.Warning_, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            return statusChanged;
        }
        static private bool CreateAndConnectToProxyAsync(BaseProxyServer item)
        {
            bool statusChanged = false;
            if (!NetConnUtils.IsConnected(item))
            {
                if (NetConnUtils.IsActiveConnectionPresent())
                {
                    NetConnUtils.CloseAllActiveConnections(false);
                }
                if (NetConnUtils.IsConnectionEntryExist(item) == false)
                {
                    if (item.IsProtocolAvailable(ProxyProtocolTypeEnum.PPTP))
                        NetConnUtils.CreateConnectionEntry(item, ProxyProtocolTypeEnum.PPTP);
                    else if (item.IsProtocolAvailable(ProxyProtocolTypeEnum.L2TP))
                        NetConnUtils.CreateConnectionEntry(item, ProxyProtocolTypeEnum.L2TP);
                    else
                    {
                        throw new ArgumentException(VpnSelectorLibRes.Non_PPTP_no_L2TP_protocols_available_for_this_vpn_entry);
                    }
                }
                NetConnUtils.OpenConnect(item);
                //DotRasUtils.CurrentProxyServer = item;
                statusChanged = true;
            }
            return statusChanged;
        }


        static public void MakeDefaultContextMenu(List<ToolStripItem> items)
        {
            BaseProxyServer item = GetDefaulBaseProxyServer();
            if (item != null)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Text = VpnSelectorLibRes.Deafault_connection;
                items.Add(menuItem);

                List<ToolStripItem> subitems = new List<ToolStripItem>();
                MakeContextMenuForBaseProxyServer(subitems, item);
                menuItem.DropDownItems.AddRange(subitems.ToArray());
            }
        }

        static public void MakeFavoriteContextMenu(List<ToolStripItem> items)
        {
            BaseProxyServer defaultItem = GetDefaulBaseProxyServer();
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Favorit_connections;
            items.Add(menuItem);

            IList list = Dm.Instance.FindAll(BaseProxyServer.CurrentType);
            foreach (var o in list)
            {
                BaseProxyServer item = (BaseProxyServer)o;
                if (item.Favorite == true)
                {
                    ToolStripMenuItem subMenuItem = new ToolStripMenuItem();
                    subMenuItem.Text = item.GetConnectionName();
                    if (item == defaultItem) subMenuItem.Font = new Font(subMenuItem.Font, FontStyle.Bold);
                    menuItem.DropDownItems.Add(subMenuItem);

                    List<ToolStripItem> subitems = new List<ToolStripItem>();
                    MakeContextMenuForBaseProxyServer(subitems, item);
                    subMenuItem.DropDownItems.AddRange(subitems.ToArray());
                }
            }
        }

        static public void MakeContextMenuForBaseProxyServer(List<ToolStripItem> items, BaseProxyServer item)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Create_VPN_entry_ + " PPTP "+ item.GetConnectionName();
            if (NetConnUtils.IsConnectionEntryExist(item) || item.IsProtocolAvailable(ProxyProtocolTypeEnum.PPTP) == false)
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    NetConnUtils.CreateConnectionEntry(item, ProxyProtocolTypeEnum.PPTP);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Create_VPN_entry_ + " L2TP " + item.GetConnectionName();
            if (NetConnUtils.IsConnectionEntryExist(item) || item.IsProtocolAvailable(ProxyProtocolTypeEnum.L2TP) == false)
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    NetConnUtils.CreateConnectionEntry(item, ProxyProtocolTypeEnum.L2TP);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Connect_to_VPN_server;
            if (NetConnUtils.IsConnected(item))
            {
                menuItem.Enabled = false;
            }
            if (NetConnUtils.IsConnectionEntryExist(item))
            {
                menuItem.Text = menuItem.Text + VpnSelectorLibRes.__created_;
            }
            menuItem.Text = menuItem.Text + " " + item.GetConnectionName();
            menuItem.Click += (s, em) =>
            {
                try
                {
                    DialogResult res = DialogResult.Cancel;
                    if (NetConnUtils.IsActiveConnectionPresent())
                    {
                        res = MessageBox.Show(VpnSelectorLibRes.Active_VPN_connection_found____
                            + NetConnUtils.ShowConnectionEntries() + VpnSelectorLibRes.__Press_OK_to_close_it_and_connect_to_selected_VPN_server_, VpnSelectorLibRes.Warning_, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        if (res == DialogResult.OK)
                        {
                            NetConnUtils.CloseAllActiveConnections(false);
                        }
                    }
                    else res = DialogResult.OK;

                    if (res == DialogResult.OK)
                    {
                        bool statusChanged = CreateAndConnectToProxyAsync(item);
                    }
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);


            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Disonnect_from_VPN_server_ +  item.GetConnectionName(); 
            if (!NetConnUtils.IsConnected(item))
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    NetConnUtils.CloseConnect(item, true);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Set_as_default_VPN_connection_ + item.GetConnectionName();
            if (NetConnUtils.GetDefaulBaseProxyServer() == item)
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    BaseProxyServerDefault = item;
                    JSetting setting = FrwConfig.Instance.GetProperty(DEFAULT_JPROXY_SERVER);
                    setting.Value = item;
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);
        }


        static public void MakeContextMenuForAllBaseProxyServers(List<ToolStripItem> items)
        {

            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Disconnect_from_active_VPN_connection;
            if (!NetConnUtils.IsActiveConnectionPresent())
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    NetConnUtils.CloseAllActiveConnections(true);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);


            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Show_created_VPN_entries_info;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    MessageBox.Show(NetConnUtils.ShowConnectionEntries());
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);
            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Show_active_VPN_connection_info;
            if (!NetConnUtils.IsActiveConnectionPresent())
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    MessageBox.Show(NetConnUtils.ShowActiveConnections());
                    
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);


            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Delete_all_VPN_entries;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    DialogResult res = MessageBox.Show(null, VpnSelectorLibRes.Delete_VPN_entries__Press__Yes__to_delete_,
                               FrwCRUDRes.WARNING, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (res == DialogResult.Yes)
                    {
                        if (NetConnUtils.IsActiveConnectionPresent())
                        {
                            NetConnUtils.CloseAllActiveConnections(true);
                        }
                        NetConnUtils.RemoveAllConnectionEntry();
                    }
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);
        }

        #region IP 

        static public void ConfirmIpAddressAsync()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (wSender, doWorkEvent) =>
            {
                try
                {
                    NetConnUtils.ConfirmIPAddress(NetConnUtils.CurrentProxyServer, null, new JobLog());
                }
                catch (Exception ex)
                {
                    Log.LogError(ex);
                }
            };
            worker.RunWorkerCompleted += (wSender, runWorkerCompletedEvent) =>
            {
                try
                {
                    if (runWorkerCompletedEvent.Cancelled == false)
                    {
                        if (OnNetworkChekComplatedEvent != null)
                        {
                            NetworkChekComplatedEventArgs e = new NetworkChekComplatedEventArgs();
                            OnNetworkChekComplatedEvent(null, e);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }

            };
            worker.RunWorkerAsync();

        }



        static public string GetMyExternalIP()
        {
            WebClient webclient = new WebClient();
            string ipServerTestAddress = "http://icanhazip.com";
            //alt plain text format
            //http://ipinfo.io/ip 
            //https://icanhazip.com/
            //http://checkip.amazonaws.com/
            //https://wtfismyip.com/text
            //https://api.ipify.org slow
            //alt html format 
            //http://checkip.dyndns.org
            try
            {
                string externalip = webclient.DownloadString(ipServerTestAddress);
                externalip = externalip.Trim().Replace("\n", "");
                if (ipServerTestAddress.Contains("dyndns.org"))
                {
                    //html format <html><head><title>Current IP Check</title></head><body>Current IP Address: 127.0.0.1</body></html>
                    string[] a = externalip.Split(':');
                    string a2 = a[1].Substring(1);
                    string[] a3 = a2.Split('<');
                    string a4 = a3[0];
                    return a4;
                }
                else
                {
                    return externalip;
                }
            }
            catch (Exception e)
            {
                ProcessWebException(e);
            }
            return null;

        }
        static private void ProcessWebException(Exception ex)
        {
            WebException e = ex as WebException;
            if (e != null)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                }
                else if (e.Status == WebExceptionStatus.Timeout)
                {
                    Console.WriteLine("Web request timeot");
                }
                else
                {
                    Console.WriteLine("Web exception " + e.Status);
                }
            }
            else if (ex != null)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static public JIPAddressInfo GetIPAddressInfo(string ipAddress)
        {
            WebClient webclient = new WebClient();

            string ipServerTestAddress = "http://ip-api.com/json/";//Our system will automatically ban any IP addresses doing over 150 requests per minute. To unban your IP click here.
            //alt http://ipinfodb.com/ip_location_api.php req login and password
            //alt http://www.telize.com/ not free 

            if (!string.IsNullOrEmpty(ipAddress)) ipServerTestAddress = ipServerTestAddress + ipAddress;
            //else test my ip 
            try
            {
                string ipInfoStr = webclient.DownloadString(ipServerTestAddress);

                dynamic ipInfo = JsonConvert.DeserializeObject(ipInfoStr);

                if (ipInfo.status == "success")
                {
                    JIPAddressInfo ip = new JIPAddressInfo();
                    ip.As = ipInfo["as"];
                    //ip.As = ipInfo.as;                   
                    ip.City = ipInfo.city;
                    ip.Country = ipInfo.country;
                    ip.CountryCode = ipInfo.countryCode;
                    ip.Ip = ipInfo.query;
                    ip.Isp = ipInfo.isp;
                    ip.Lat = ipInfo.lat;
                    ip.Lon = ipInfo.lon;
                    ip.Mobile = ipInfo.mobile;
                    ip.Org = ipInfo.org;
                    ip.Proxy = ipInfo.proxy;
                    ip.Region = ipInfo.region;
                    ip.RegionName = ipInfo.regionName;
                    ip.Reverse = ipInfo.reverse;
                    ip.Timezone = ipInfo.timezone;
                    ip.Zip = ipInfo.zip;
                    return ip;
                }
                else if (ipInfo.status == "fail")
                {
                    Console.WriteLine("Fail response from IP info server: " + ipInfo.message);
                }
                else
                {
                    Console.WriteLine("Unknown response from IP info server");
                }
            }
            catch (Exception e)
            {
                ProcessWebException(e);
            }
            return null;
        }

        #endregion


        static public bool ConnectWithConfirmation(BaseProxyServer item, string homeIP, JobLog jobLog)
        {
            IsSynchMode = true;
            string oldExternalIP = NetConnUtils.MyExternalIP;
            JIPAddressInfo oldExtIPAddressInfo = NetConnUtils.MyExtIPAddressInfo;

            bool createdNew = false;
            bool res = false;
            try
            {
                res = ConnectWithConfirmationLocal(item, homeIP, out createdNew, jobLog);
            }
            finally{
                DisconnectWithConfirmation(item, homeIP, createdNew, jobLog);
                NetConnUtils.MyExternalIP = oldExternalIP;
                NetConnUtils.MyExtIPAddressInfo = oldExtIPAddressInfo;
                IsSynchMode = false;
            }
            return res;
        }
        static public bool DisconnectWithConfirmation(BaseProxyServer item, string homeIP, bool createdNew, JobLog jobLog)
        {
            if (NetConnUtils.IsActiveConnectionPresent())
            {
                NetConnUtils.CloseAllActiveConnections(false);
                Thread.Sleep(2 * 1000);
            }
            if (createdNew)
            {
                NetConnUtils.RemoveConnectionEntry(item);
            }
            return true;
        }
        static public bool ConnectWithConfirmationLocal(BaseProxyServer item, string homeIP, out bool createdNew, JobLog jobLog)
        {

            bool result = false;
            jobLog.Debug("Start VpnServerConnectWithFullTestLocal for item " + item.JProxyServerId);
            createdNew = false;
            int closeAttemptCount = 0;
            while (NetConnUtils.IsActiveConnectionPresent())
            {
                if (closeAttemptCount > 10) throw new Exception("Unable to close previous active connection");
                jobLog.Debug("Found previous active connection. Going to close it");
                NetConnUtils.CloseAllActiveConnections(false);
                Thread.Sleep(2 * 1000);//!!!
                closeAttemptCount++;
            }
            ///////test 
            if (homeIP != null)
            {

                string externalIP = NetConnUtils.GetMyExternalIP();
                if (externalIP == null) throw new Exception("Ip of default connection is null");
                jobLog.Info("Default ExternalIP: " + externalIP);
                if (!homeIP.Equals(externalIP))
                {
                    throw new Exception("Ip address of default connection not equals home ip address.");
                }
            }
            //////////
            if (NetConnUtils.IsConnectionEntryExist(item) == false)
            {
                if (item.IsProtocolAvailable(ProxyProtocolTypeEnum.PPTP))
                {
                    NetConnUtils.CreateConnectionEntry(item, ProxyProtocolTypeEnum.PPTP);
                    createdNew = true;
                }
                else if (item.IsProtocolAvailable(ProxyProtocolTypeEnum.L2TP))
                {
                    NetConnUtils.CreateConnectionEntry(item, ProxyProtocolTypeEnum.L2TP);
                    createdNew = true;
                }
                else
                {
                    throw new ArgumentException(VpnSelectorLibRes.Non_PPTP_no_L2TP_protocols_available_for_this_vpn_entry);
                }
            }
            try
            {
                NetConnUtils.OpenConnectLocal(item, false);//sync

                Thread.Sleep(2 * 1000);//!!!
                //for (int i = 0; i < 60; i++)//~ 1 min
                //{
                if (NetConnUtils.IsConnected(item))
                {
                    ConfirmIPAddress(item, homeIP, jobLog);
                    result = true;
                    item.SuccessCount = item.SuccessCount + 1;
                    item.LastSuccessDate = DateTimeOffset.Now;
                    Dm.Instance.SaveObject(item);

                }
                else
                {
                    jobLog.Info("Not connected for item " + item.JProxyServerId);
                }
                //Thread.Sleep(1 * 1000);
                //}
                //todo error event
            }
            finally {
                //change label
                if (OnNetworkChekComplatedEvent != null)
                {
                    NetworkChekComplatedEventArgs e = new NetworkChekComplatedEventArgs();
                    OnNetworkChekComplatedEvent(null, e);
                }
            }
            return result;
        }

        static public void ConfirmIPAddress(BaseProxyServer item, string homeIP, JobLog jobLog)
        {
            string externalIP = NetConnUtils.GetMyExternalIP();
            if (externalIP == null) throw new Exception("Ip of vpn connection is null");
            jobLog.Info("ExternalIP: " + externalIP);
            if (homeIP != null)
            {
                if (homeIP.Equals(externalIP))
                {
                    throw new Exception("Ip address of vpn connection not changed. It equals home ip address.");
                }
            }
            JIPAddressInfo extIPAddressInfo = NetConnUtils.GetIPAddressInfo(externalIP);
            if (extIPAddressInfo == null) throw new Exception("IPAddressInfo of vpn connection is null");
            jobLog.Info("New IP address Info: " + Log.PropertyList(extIPAddressInfo));
            if (item != null)
            {
                if (item.JCountry != null)
                {
                    if (!string.IsNullOrEmpty(extIPAddressInfo.CountryCode))
                    {
                        if (extIPAddressInfo.CountryCode.ToLower().Equals(item.JCountry.JCountryId) == false)
                        {
                            //throw new Exception
                            jobLog.Warn("Country code of vpn connection ip address (" +
                                extIPAddressInfo.CountryCode.ToLower() + ") not equals to contry code of VPN server ("
                                + item.JCountry.JCountryId + ")");

                            JCountry newContry = Dm.Instance.Find<JCountry>(extIPAddressInfo.CountryCode.ToLower());
                            if (newContry != null)
                            {
                                if (item.JCountryDeclared == null) item.JCountryDeclared = item.JCountry;
                                item.JCountry = newContry;
                                Dm.Instance.SaveObject(item);
                            }
                            else
                            {
                                throw new Exception("Country code of vpn connection ip address (" +
                                    extIPAddressInfo.CountryCode.ToLower() + ") not a valid country code");
                            }
                        }
                    }
                    else throw new Exception("Country code of vpn connection is empty");
                    if (!string.IsNullOrEmpty(extIPAddressInfo.City))
                    {
                        if (item.Town != null)
                        {
                            if (extIPAddressInfo.City.ToLower().Equals(item.Town.ToLower()) == false)
                            {
                                jobLog.Warn("City vpn connection ip address (" +
                                    extIPAddressInfo.City + ") not equals to town of VPN server ("
                                    + item.Town + "). New City value was set");
                                if (item.TownDeclared == null) item.TownDeclared = item.Town;
                                item.Town = extIPAddressInfo.City;
                                Dm.Instance.SaveObject(item);
                            }
                        }
                        else
                        {
                            item.Town = extIPAddressInfo.City;
                            Dm.Instance.SaveObject(item);
                        }
                    }
                }
                else
                {
                    //todo 
                }
                jobLog.Info("Test OK for item " + item.JProxyServerId);
            }
            //ok

            //todo
            NetConnUtils.MyExternalIP = externalIP;
            NetConnUtils.MyExtIPAddressInfo = extIPAddressInfo;
   
        }
    }
}
