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


namespace Dionext
{
    public class CheckExternalIPAddressComplatedEventArgs : EventArgs
    {
    }
    public delegate void CheckExternalIPAddressComplatedEventHandler(object sender, CheckExternalIPAddressComplatedEventArgs e);

    public class VpnDisconnectedEventArgs : EventArgs    {    }
    public delegate void VpnDisconnectedEventHandler(object sender, VpnDisconnectedEventArgs e);
    public class VpnConnectedEventArgs : EventArgs { }
    public delegate void VpnConnectedEventHandler(object sender, VpnConnectedEventArgs e);
    public class VpnDialerErrorEventArgs : EventArgs {
        public Exception Exception { get; set; }
    }
    public delegate void VpnDialerErrorEventHandler(object sender, VpnDialerErrorEventArgs e);
    public class CurVPNServerChangedEventArgs : EventArgs { }
    public delegate void CurVPNServerChangedEventHandler(object sender, CurVPNServerChangedEventArgs e);


    //Note: we only work with PPTP and L2TP connections 
    //Note: most methods are static (we can have only one vpn connection at the same time)
    public class VpnConnUtils
    {
        static public string MyExternalIP { get; set; }
        static public string MyExternalIPWithoutVPN { get; set; }
        static public JIPAddressInfo MyExtIPAddressInfo { get; set; }
        static public JVPNServer CurrentVPNServer { get; set; }

        static public event CheckExternalIPAddressComplatedEventHandler OnCheckExternalIPAddressComplatedEvent;

        //static private WebClient webclient = new WebClient();
        // to improve the performance of repeated requests, you need to isolate it into a static variable
        // however, it is possible to issue a warning to the log "WebClient does not support simultaneous I/O operations"
        static private RasDialer dialer = null;
        static private RasHandle handle = null;
        public static bool IsReconnect { get; set; }
        public static VpnConnectedEventHandler VpnConnectedEvent = null;
        public static VpnDisconnectedEventHandler VpnDisconnectedEvent = null;
        public static VpnDialerErrorEventHandler VpnDialerErrorEvent = null;
        //not in use 
        public static CurVPNServerChangedEventHandler CurVPNServerChangedEvent = null;
        static public string DEFAULT_JVPN_SERVER = "MainApp.DefaultVPNServer";
        static public string SETTING_CHECK_IP_ON_STARTUP = "Vpn.CheckIPOnStarup";
        static public JVPNServer BaseVPNServerDefault = null;
        static private RasConnectionWatcher watcher = new RasConnectionWatcher();// for the watcher to run all the events it must be run when the application is loaded

        static public bool IsSynchMode { get; set; }
        static public bool IsManualDisconnect { get; set; }

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
            VpnConnUtils.IsReconnect = FrwConfig.Instance.GetPropertyValueAsBool(setting.Name, true);
            setting = FrwConfig.Instance.CreatePropertyIfNotExist(new JSetting()
            {
                Name = VpnConnUtils.DEFAULT_JVPN_SERVER,
                Description = VpnSelectorLibRes.VPN_server_by_default,
                Group = "VPN",
                Value = null,
                ValueType = typeof(JVPNServer),
                IsUser = true
            });
            setting = FrwConfig.Instance.CreatePropertyIfNotExist(new JSetting()
            {
                Name = VpnConnUtils.SETTING_CHECK_IP_ON_STARTUP,
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
            Log.ProcessDebug("Event " +  VpnSelectorLibRes.A_connection_has_successfully_connected_ + e.Connection.EntryName + " IsSynchMode = " + IsSynchMode);
            if (IsSynchMode == false)
            {
                IsManualDisconnect = false;
                JVPNServer oldVPN = VpnConnUtils.CurrentVPNServer;
                VpnConnUtils.CurrentVPNServer = JVPNServer.FindFromNames(VpnConnUtils.GetActiveConnectionsNames());

                if (VpnConnectedEvent != null) VpnConnectedEvent(null, new VpnConnectedEventArgs());

                if ((oldVPN == null && VpnConnUtils.CurrentVPNServer != null)
                    || (oldVPN != null && oldVPN != VpnConnUtils.CurrentVPNServer))
                {
                    //proxy changed
                    if (CurVPNServerChangedEvent != null) CurVPNServerChangedEvent(null, new CurVPNServerChangedEventArgs());
                    VpnConnUtils.ConfirmIpAddressAsync();
                }

            }
        }
        static private void watcher_Disconnected(object sender, RasConnectionEventArgs e)
        {
            // A connection has disconnected successfully.
            Log.ProcessDebug("Event " + VpnSelectorLibRes.A_connection_has_disconnected_successfully_ + e.Connection.EntryName + " IsSynchMode = " + IsSynchMode);
            if (IsSynchMode == false)
            {
                JVPNServer oldVPN = VpnConnUtils.CurrentVPNServer;
                VpnConnUtils.CurrentVPNServer = null;
                if (VpnDisconnectedEvent != null) VpnDisconnectedEvent(null, new VpnDisconnectedEventArgs());
                if (!IsManualDisconnect) AppManager.Instance.ProcessNotification(VpnSelectorLibRes.VPN_disconnected);
                if (oldVPN != null)
                {
                    //VPN changed
                    if (CurVPNServerChangedEvent != null) CurVPNServerChangedEvent(null, new CurVPNServerChangedEventArgs());
                    VpnConnUtils.ConfirmIpAddressAsync();
                }
                //reconnect
                //https://stackoverflow.com/questions/23950702/reconnect-vpn-windows-service
                if (VpnConnUtils.CurrentVPNServer != null && IsReconnect == true)
                {
                    OpenConnectLocal(VpnConnUtils.CurrentVPNServer, true);
                }
            }

        }


        static public void CreateConnectionEntry(JVPNServer ps, VPNProtocolTypeEnum protocolType)
        {
            if (!ps.IsProtocolAvailable(protocolType)) throw new ArgumentException("Protocol " + protocolType.ToString() + " not avilable");

            //http://stackoverflow.com/questions/36213393/get-connection-status-vpn-using-dotras
            // File.WriteAllText("your rasphone.pbk  path","")//Add
            RasPhoneBook rasPhoneBook1 = new RasPhoneBook();
            string rasPhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);//alt RasPhoneBookType.AllUsers
            rasPhoneBook1.Open(rasPhoneBookPath);

            string deviceTypeStr = "(" + protocolType.ToString() + ")";
            RasVpnStrategy strategy = (protocolType == VPNProtocolTypeEnum.L2TP) ? RasVpnStrategy.L2tpOnly : RasVpnStrategy.PptpOnly;
            //alt 
            //RasVpnStrategy strategy = RasVpnStrategy.Default;
            RasEntry entry = RasEntry.CreateVpnEntry(ps.GetConnectionName(), ps.Url,
                strategy,
                RasDevice.GetDeviceByName(deviceTypeStr, RasDeviceType.Vpn, false));
            entry.EncryptionType = ps.EncryptionType.ToEnum<RasEncryptionType>();
            if (protocolType == VPNProtocolTypeEnum.L2TP && !string.IsNullOrEmpty(ps.JVPNProvider.UserPresharedKey)) 
            {
                entry.Options.UsePreSharedKey = true;
            }
            rasPhoneBook1.Entries.Add(entry);
            if (protocolType == VPNProtocolTypeEnum.L2TP && !string.IsNullOrEmpty(ps.JVPNProvider.UserPresharedKey))
            {
                entry.UpdateCredentials(RasPreSharedKey.Client, ps.JVPNProvider.UserPresharedKey);
            }

            if (!string.IsNullOrEmpty(ps.JVPNProvider.VPNLogin))
            {
                //entry.UpdateCredentials(new System.Net.NetworkCredential(ps.JVPNProvider.VPNLogin, ps.JVPNProvider.VPNPassword), false);
            }

        }
        static public bool IsConnectionEntryExist(JVPNServer ps)
        {
            string path = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);
            if (path != null && File.Exists(path))
            {
                string name = ps.GetConnectionName();
                return RasEntry.Exists(name, path);
            }
            else return false;
        }
        static public void RemoveConnectionEntry(JVPNServer ps)
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
        static public void OpenConnect(JVPNServer ps)
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

        static private void OpenConnectLocal(JVPNServer ps, bool async)
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
                if (!string.IsNullOrEmpty(ps.JVPNProvider.VPNLogin))
                {
                    System.Net.NetworkCredential cred = new System.Net.NetworkCredential(ps.JVPNProvider.VPNLogin, ps.JVPNProvider.VPNPassword);
                    dialer.Credentials = cred;
                }
                //dialer.Dial();//alt DialAsync()  
                if (async)
                    handle = dialer.DialAsync();
                else
                {
                    handle = dialer.Dial();
                    VpnConnUtils.CurrentVPNServer = ps;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        static public void CloseConnect(JVPNServer ps)
        {
            if (ps == null)
            {
                if (dialer != null)
                {
                    RasConnection conn = RasConnection.GetActiveConnectionByName(dialer.EntryName, dialer.PhoneBookPath);
                    if (conn != null)
                    {
                        IsManualDisconnect = true;
                        conn.HangUp();
                    }
                }
            }
            else
            {
                RasConnection conn = RasConnection.GetActiveConnectionByName(ps.GetConnectionName(), RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User));
                if (conn != null)
                {
                    IsManualDisconnect = true;
                    conn.HangUp();
                }
            }
            //if (!async) 
                //VpnConnUtils.CurrentVPNServer = null;
        }

        static public void CloseAllActiveConnections()
        {
            foreach (RasConnection conn in RasConnection.GetActiveConnections())
            {
                if (conn != null)
                {
                    IsManualDisconnect = true;
                    conn.HangUp();
                }
            }
            //if (!async) 
            //VpnConnUtils.CurrentVPNServer = null;
        }


        static public bool IsConnected(JVPNServer ps)
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
                        JVPNServer currentVPNServer = JVPNServer.FindFromNames(new List<string>() { dialer.EntryName });
                        if (currentVPNServer != null)
                        {
                            currentVPNServer.ErrorCount = currentVPNServer.ErrorCount + 1;
                            currentVPNServer.LastErrorDate = DateTimeOffset.Now;
                            Dm.Instance.SaveObject(currentVPNServer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.ProcessDebug("Event Error find BaseVPNServer " + ex);
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

        static public JVPNServer GetDefaulBaseVPNServer()
        {
            if (BaseVPNServerDefault == null)
            {
                JSetting setting = FrwConfig.Instance.GetProperty(DEFAULT_JVPN_SERVER);
                if (setting != null)
                {
                    BaseVPNServerDefault = setting.Value as JVPNServer;
                }
            }
            return BaseVPNServerDefault; 
        }
        static public bool ConnectOrDisconnectDefautBaseVPNServerAsync()
        {
            bool statusChanged = false;
            if (VpnConnUtils.IsActiveConnectionPresent())
            {
                VpnConnUtils.CloseAllActiveConnections();
                statusChanged = true;
            }
            else
            {
                JVPNServer item = GetDefaulBaseVPNServer();
                if (item != null)
                {
                    statusChanged = CreateAndConnectToVPNAsync(item);
                }
                else
                {
                    MessageBox.Show(VpnSelectorLibRes.Double_click_on_this_icon_creates_VPN_connection_to_default_VPN_server_which_not_set__No_connection_created_, VpnSelectorLibRes.Warning_, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            return statusChanged;
        }
        static private bool CreateAndConnectToVPNAsync(JVPNServer item)
        {
            bool statusChanged = false;
            if (!VpnConnUtils.IsConnected(item))
            {
                if (VpnConnUtils.IsActiveConnectionPresent())
                {
                    VpnConnUtils.CloseAllActiveConnections();
                }
                if (VpnConnUtils.IsConnectionEntryExist(item) == false)
                {
                    if (item.IsProtocolAvailable(VPNProtocolTypeEnum.PPTP))
                        VpnConnUtils.CreateConnectionEntry(item, VPNProtocolTypeEnum.PPTP);
                    else if (item.IsProtocolAvailable(VPNProtocolTypeEnum.L2TP))
                        VpnConnUtils.CreateConnectionEntry(item, VPNProtocolTypeEnum.L2TP);
                    else
                    {
                        throw new ArgumentException(VpnSelectorLibRes.Non_PPTP_no_L2TP_protocols_available_for_this_vpn_entry);
                    }
                }
                VpnConnUtils.OpenConnect(item);
                //DotRasUtils.CurrentVPNServer = item;
                statusChanged = true;
            }
            return statusChanged;
        }


        static public void MakeDefaultContextMenu(List<ToolStripItem> items)
        {
            JVPNServer item = GetDefaulBaseVPNServer();
            if (item != null)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Text = VpnSelectorLibRes.Deafault_connection;
                items.Add(menuItem);

                List<ToolStripItem> subitems = new List<ToolStripItem>();
                if (item != null) MakeContextMenuForBaseVPNServer(subitems, item);
                menuItem.DropDownItems.AddRange(subitems.ToArray());
            }
        }

        static public void MakeFavoriteContextMenu(List<ToolStripItem> items)
        {
            JVPNServer defaultItem = GetDefaulBaseVPNServer();
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Favorit_connections;
            items.Add(menuItem);

            IList list = Dm.Instance.FindAll(typeof(JVPNServer));
            foreach (var o in list)
            {
                JVPNServer item = (JVPNServer)o;
                if (item.Favorite == true)
                {
                    ToolStripMenuItem subMenuItem = new ToolStripMenuItem();
                    subMenuItem.Text = item.GetConnectionName();
                    if (item == defaultItem) subMenuItem.Font = new Font(subMenuItem.Font, FontStyle.Bold);
                    menuItem.DropDownItems.Add(subMenuItem);

                    List<ToolStripItem> subitems = new List<ToolStripItem>();
                    if (item != null) MakeContextMenuForBaseVPNServer(subitems, item);
                    subMenuItem.DropDownItems.AddRange(subitems.ToArray());
                }
            }
        }

        static public void MakeContextMenuForBaseVPNServer(List<ToolStripItem> items, JVPNServer item)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Create_VPN_entry_ + " PPTP "+ item.GetConnectionName();
            if (VpnConnUtils.IsConnectionEntryExist(item) || item.IsProtocolAvailable(VPNProtocolTypeEnum.PPTP) == false)
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    VpnConnUtils.CreateConnectionEntry(item, VPNProtocolTypeEnum.PPTP);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Create_VPN_entry_ + " L2TP " + item.GetConnectionName();
            if (VpnConnUtils.IsConnectionEntryExist(item) || item.IsProtocolAvailable(VPNProtocolTypeEnum.L2TP) == false)
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    VpnConnUtils.CreateConnectionEntry(item, VPNProtocolTypeEnum.L2TP);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Connect_to_VPN_server;
            if (VpnConnUtils.IsConnected(item))
            {
                menuItem.Enabled = false;
            }
            if (VpnConnUtils.IsConnectionEntryExist(item))
            {
                menuItem.Text = menuItem.Text + VpnSelectorLibRes.__created_;
            }
            menuItem.Text = menuItem.Text + " " + item.GetConnectionName();
            menuItem.Click += (s, em) =>
            {
                try
                {
                    DialogResult res = DialogResult.Cancel;
                    if (VpnConnUtils.IsActiveConnectionPresent())
                    {
                        res = MessageBox.Show(VpnSelectorLibRes.Active_VPN_connection_found____
                            + VpnConnUtils.ShowActiveConnections() + VpnSelectorLibRes.__Press_OK_to_close_it_and_connect_to_selected_VPN_server_, VpnSelectorLibRes.Warning_, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        if (res == DialogResult.OK)
                        {
                            VpnConnUtils.CloseAllActiveConnections();
                        }
                    }
                    else res = DialogResult.OK;

                    if (res == DialogResult.OK)
                    {
                        bool statusChanged = CreateAndConnectToVPNAsync(item);
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
            if (!VpnConnUtils.IsConnected(item))
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    VpnConnUtils.CloseConnect(item);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Set_as_default_VPN_connection_ + item.GetConnectionName();
            if (VpnConnUtils.GetDefaulBaseVPNServer() == item)
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    BaseVPNServerDefault = item;
                    JSetting setting = FrwConfig.Instance.GetProperty(DEFAULT_JVPN_SERVER);
                    setting.Value = item;
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);
        }


        static public void MakeContextMenuForAllBaseVPNServers(List<ToolStripItem> items)
        {

            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Disconnect_from_active_VPN_connection;
            if (!VpnConnUtils.IsActiveConnectionPresent())
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    VpnConnUtils.CloseAllActiveConnections();
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
                    MessageBox.Show(VpnConnUtils.ShowConnectionEntries());
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            items.Add(menuItem);
            menuItem = new ToolStripMenuItem();
            menuItem.Text = VpnSelectorLibRes.Show_active_VPN_connection_info;
            if (!VpnConnUtils.IsActiveConnectionPresent())
            {
                menuItem.Enabled = false;
            }
            menuItem.Click += (s, em) =>
            {
                try
                {
                    MessageBox.Show(VpnConnUtils.ShowActiveConnections());
                    
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
                        if (VpnConnUtils.IsActiveConnectionPresent())
                        {
                            VpnConnUtils.CloseAllActiveConnections();
                        }
                        VpnConnUtils.RemoveAllConnectionEntry();
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
                    VpnConnUtils.CheckExternalIPAddress(VpnConnUtils.CurrentVPNServer, true, new JobLog());
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
                        if (OnCheckExternalIPAddressComplatedEvent != null)
                        {
                            CheckExternalIPAddressComplatedEventArgs e = new CheckExternalIPAddressComplatedEventArgs();
                            OnCheckExternalIPAddressComplatedEvent(null, e);
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
            Console.WriteLine("GetMyExternalIP() start: " + DateTime.Now);
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
            string externalip = null;
            try
            {

                //Task<string> t = webclient.DownloadStringTaskAsync(ipServerTestAddress);

                externalip = webclient.DownloadString(ipServerTestAddress);
                Console.WriteLine("GetMyExternalIP() end: " + DateTime.Now);
                externalip = externalip.Trim().Replace("\n", "");
                if (ipServerTestAddress.Contains("dyndns.org"))
                {
                    //html format <html><head><title>Current IP Check</title></head><body>Current IP Address: 127.0.0.1</body></html>
                    string[] a = externalip.Split(':');
                    string a2 = a[1].Substring(1);
                    string[] a3 = a2.Split('<');
                    string a4 = a3[0];
                    externalip = a4;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetMyExternalIP() ex: " + DateTime.Now);
                ProcessWebException(e);
            }
            finally
            {
                webclient.Dispose();
            }
            return externalip;

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
            Console.WriteLine("GetIPAddressInfo start: " + DateTime.Now);
            WebClient webclient = new WebClient();
            //WebClientAdv webclient = new WebClientAdv();
            //webclient.Timeout = 1 * 60 * 1000;
            string ipServerTestAddress = "http://ip-api.com/json/";  //"https://www.google.com/";// 
            //This endpoint is limited to 45 requests per minute from an IP address.
            //If you go over the limit your requests will be throttled(HTTP 429) until your rate limit window is reset.If you constantly go over the limit your IP address will be banned for 1 hour.
            //The returned HTTP header X - Rl contains the number of requests remaining in the current rate limit window.X - Ttl contains the seconds until the limit is reset.
            //Your implementation should always check the value of the X - Rl header, and if its is 0 you must not send any more requests for the duration of X - Ttl in seconds.
            //alt http://ipinfodb.com/ip_location_api.php req login and password
            //alt http://www.telize.com/ not free 

            if (!string.IsNullOrEmpty(ipAddress)) ipServerTestAddress = ipServerTestAddress + ipAddress;
            //else test my ip 

            JIPAddressInfo ip = null;
            try
            {
                string ipInfoStr = webclient.DownloadString(ipServerTestAddress);
                Console.WriteLine("GetIPAddressInfo end: " + DateTime.Now);
                Console.WriteLine("ipInfoStr: " + ipInfoStr);
                
                dynamic ipInfo = JsonConvert.DeserializeObject(ipInfoStr);

                if (ipInfo.status == "success")
                {
                    ip = new JIPAddressInfo();
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
                Console.WriteLine("GetIPAddressInfo ex: " + DateTime.Now);
                ProcessWebException(e);
            }
            finally
            {
                webclient.Dispose();
            }
            return ip;
        }

        #endregion


        static public bool TestConnectionWithConfirmation(JVPNServer item, JobLog jobLog)
        {
            IsSynchMode = true;
            string oldExternalIP = VpnConnUtils.MyExternalIP;
            JIPAddressInfo oldExtIPAddressInfo = VpnConnUtils.MyExtIPAddressInfo;

            bool createdNew = false;
            bool res = false;
            try
            {
                res = ConnectWithConfirmationLocal(item, out createdNew, false, jobLog);
            }
            finally{
                DisconnectWithConfirmation(jobLog);
                if (createdNew)
                {
                    VpnConnUtils.RemoveConnectionEntry(item);
                }
                VpnConnUtils.MyExternalIP = oldExternalIP;
                VpnConnUtils.MyExtIPAddressInfo = oldExtIPAddressInfo;
                VpnConnUtils.CurrentVPNServer = null;
                IsSynchMode = false;
            }
            return res;
        }
        static public bool ConnectWithConfirmation(JVPNServer item)
        {
            IsSynchMode = true;
            bool createdNew = false;
            bool res = false;
            try
            {
                JobLog jobLog = new JobLog();
                res = ConnectWithConfirmationLocal(item, out createdNew, true, jobLog);
            }
            finally
            {
                IsSynchMode = false;
            }
            return res;
        }
        static public bool DisconnectWithConfirmation(JobLog jobLog)
        {
            int closeAttemptCount = 0;
            while (VpnConnUtils.IsActiveConnectionPresent())
            {
                if (closeAttemptCount > 10) throw new Exception("Unable to close previous active connection");
                jobLog.Debug("Found previous active connection. Going to close it");
                VpnConnUtils.CloseAllActiveConnections();
                Thread.Sleep(2 * 1000);//!!!
                closeAttemptCount++;
            }
            return true;
        }
        static private bool ConnectWithConfirmationLocal(JVPNServer item, out bool createdNew, bool manualMode, JobLog jobLog)
        {
            bool result = false;
            jobLog.Debug("Start ConnectWithConfirmationLocal for item " + item.JVPNServerId);
            createdNew = false;
            
            DisconnectWithConfirmation(jobLog);
            if (IsSynchMode)
            {
                if (VpnDisconnectedEvent != null) VpnDisconnectedEvent(null, new VpnDisconnectedEventArgs());
            }

            if (VpnConnUtils.MyExternalIPWithoutVPN == null)
            {
                VpnConnUtils.MyExternalIPWithoutVPN = VpnConnUtils.GetMyExternalIP();
                if (VpnConnUtils.MyExternalIPWithoutVPN == null) throw new Exception("Ip of connection without VPN is null");
                jobLog.Info("VpnConnUtils.MyExternalIPWithoutVPN: " + VpnConnUtils.MyExternalIPWithoutVPN);
                VpnConnUtils.MyExternalIP = VpnConnUtils.MyExternalIPWithoutVPN;
            }
            //else
            //{
            //    if (!VpnConnUtils.MyExternalIPWithoutVPN.Equals(VpnConnUtils.MyExternalIP))
            //    {
            //        throw new Exception("Ip address of default connection not equals ip address without VPN.");
            //    }
            //}
            if (VpnConnUtils.IsConnectionEntryExist(item) == false)
            {
                if (item.IsProtocolAvailable(VPNProtocolTypeEnum.PPTP))
                {
                    VpnConnUtils.CreateConnectionEntry(item, VPNProtocolTypeEnum.PPTP);
                    createdNew = true;
                }
                else if (item.IsProtocolAvailable(VPNProtocolTypeEnum.L2TP))
                {
                    VpnConnUtils.CreateConnectionEntry(item, VPNProtocolTypeEnum.L2TP);
                    createdNew = true;
                }
                else
                {
                    throw new ArgumentException(VpnSelectorLibRes.Non_PPTP_no_L2TP_protocols_available_for_this_vpn_entry);
                }
            }
            try
            {
                VpnConnUtils.OpenConnectLocal(item, false);//sync

                Thread.Sleep(2 * 1000);//!!!

                if (VpnConnUtils.IsConnected(item))
                {
                    if (VpnConnUtils.IsActiveConnectionPresent())
                    {
                        if (VpnConnUtils.CurrentVPNServer == null) VpnConnUtils.CurrentVPNServer = JVPNServer.FindFromNames(VpnConnUtils.GetActiveConnectionsNames());
                        if (IsSynchMode)
                        {
                            if (VpnConnectedEvent != null) VpnConnectedEvent(null, new VpnConnectedEventArgs());
                        }
                        bool confirmResult = CheckExternalIPAddress(item, manualMode, jobLog);
                        if (!confirmResult)
                        {
                            return false;
                        }
                    }
                    result = true;
                    item.SuccessCount = item.SuccessCount + 1;
                    item.LastSuccessDate = DateTimeOffset.Now;
                    Dm.Instance.SaveObject(item);

                }
                else
                {
                    jobLog.Info("Not connected for item " + item.JVPNServerId);
                }
            }
            finally {
                //change label
                if (OnCheckExternalIPAddressComplatedEvent != null)
                {
                    CheckExternalIPAddressComplatedEventArgs e = new CheckExternalIPAddressComplatedEventArgs();
                    OnCheckExternalIPAddressComplatedEvent(null, e);
                }
            }
            return result;
        }
        static public bool CheckExternalIPAddress(JVPNServer item, bool manualMode, JobLog jobLog)
        {
            jobLog.Info("ConfirmIPAddress ");
            string externalIP = VpnConnUtils.GetMyExternalIP();
            if (externalIP == null)
            {

                throw new Exception("Ip of vpn connection is null");
            }
            jobLog.Info("ExternalIP: " + externalIP);
            if (VpnConnUtils.MyExternalIPWithoutVPN != null)
            {
                if (VpnConnUtils.MyExternalIPWithoutVPN.Equals(externalIP))
                {
                    throw new Exception("Ip address of vpn connection not changed. It equals ip address without VPN.");
                }
            }
            //Thread.Sleep(2 * 1000);//!!!

            JIPAddressInfo extIPAddressInfo = null;
            int confirmAttemptCount = 0;
            while (extIPAddressInfo == null)
            {
                if (confirmAttemptCount > 1 && !manualMode) break; // throw new Exception("Unable to confirm ip. Attempt " + confirmAttemptCount);

                extIPAddressInfo = VpnConnUtils.GetIPAddressInfo(externalIP);
                if (extIPAddressInfo != null)
                {
                    break;
                }
                else
                {
                    jobLog.Warn("IPAddressInfo of vpn connection is null.  Attempt " + confirmAttemptCount);
                    if (manualMode)
                    {
                        DialogResult res = MessageBox.Show("Соединение установлено, но не удалось получить информацию о новом IP адресе:  " + externalIP + " Если вы хотите продолжить, нажмите \"ОК\"",
                            FrwConstants.WARNING, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (!(res == DialogResult.OK))
                        {
                            break;
                        }
                    }
                }
                Thread.Sleep(5 * 1000);
                confirmAttemptCount++;
            }
           
            if (extIPAddressInfo != null) { 
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
                    jobLog.Info("ConfirmIPAddress OK for item " + item.JVPNServerId);
                }
                //ok
                VpnConnUtils.MyExternalIP = externalIP;
                VpnConnUtils.MyExtIPAddressInfo = extIPAddressInfo;
                return true;
            }
            else return false;
        }
    }
}
