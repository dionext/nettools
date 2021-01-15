using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrwSoftware.Kitty;
using System.Windows.Forms;

using Dionext;
using System.Collections;
using System.IO;

namespace FrwSoftware
{
    public class NAAppManager : WebAccountLibAppManager
    {
        override public void InitApplication()
        {
            base.InitApplication();

            if (VpnSelectorDm.MyMac != null)
            {
                NADm.MyCompDevice = Dm.Instance.FindAll<JCompDevice>().FirstOrDefault<JCompDevice>(c => BaseNetworkUtils.CompareMACs(c.MACAddress, VpnSelectorDm.MyMac));
            }
            if (VpnSelectorDm.DnsMac != null)
            {
                //detect network for MAC  stage 1
                NADm.MyCurrentCompDeviceNetwork = Dm.Instance.FindAll<JCompDeviceNetwork>().FirstOrDefault<JCompDeviceNetwork>(c => BaseNetworkUtils.CompareMACs(c.RouterMacAddress, VpnSelectorDm.DnsMac));
                //detect network for MAC  stage 2
                if (NADm.MyCurrentCompDeviceNetwork == null)
                {
                    JCompDevice router = Dm.Instance.FindAll<JCompDevice>().FirstOrDefault<JCompDevice>(c => BaseNetworkUtils.CompareMACs(c.MACAddress, VpnSelectorDm.DnsMac));
                    if (router != null)
                    {
                        NADm.MyCurrentCompDeviceNetwork = router.JCompDeviceNetworkParentLevel;
                    }
                }

                if (NADm.MyCurrentCompDeviceNetwork == null)
                {
                    DialogResult res = DialogResult.OK;
                    if (VpnConnUtils.CurrentVPNServer != null && JCompDeviceNetwork.FindByVPN(VpnConnUtils.CurrentVPNServer as JVPNServer) != null)
                    {
                        res = MessageBox.Show("Не удалось определить текущую сеть. Но есть сеть у текущего прокси сервера. Продолжить? Нажмите ОК, чтобы продолжить и установить сеть потом вручную. Нажмите Cancel для выхода из программы.",
                            FrwConstants.WARNING, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        res = MessageBox.Show("Не удалось определить текущую сеть. Продолжить? Нажмите ОК, чтобы продолжить и установить сеть потом вручную. Нажмите Cancel для выхода из программы.",
                            FrwConstants.WARNING, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    }
                    if (res != DialogResult.OK)
                    {
                        Application.Exit();
                    }
                }
                //else
                //{
                    //if (NADm.MyCurrentCompDeviceNetwork.IsInternal == false)
                    //{
                    //    NADm.MyCurrentCompDeviceNetwork.IsInternal = true;
                    //    Dm.Instance.SaveObject(NADm.MyCurrentCompDeviceNetwork);
                    //}
                //}

            }
            Console.WriteLine("My comp: " + (NADm.MyCompDevice != null ? NADm.MyCompDevice.Name : ""));
            Console.WriteLine("My current comp network: " + (NADm.MyCurrentCompDeviceNetwork != null ? NADm.MyCurrentCompDeviceNetwork.Name : ""));
            Console.WriteLine("My current VPN: " + (VpnConnUtils.CurrentVPNServer != null ? VpnConnUtils.CurrentVPNServer.GetConnectionName() : ""));

        }
        override public void DestroyApp()
        {
            try
            {
                WinSCPUtils.CloseAllRemoteSession();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
            base.DestroyApp();
        }

    override protected void OpenOtherTools(WebEntryInfo webEntryInfo, List<ToolStripItem> subs)
        {
            ToolStripMenuItem menuItem = null;
            //Открыть в Kitty
            string kittyPath = FrwConfig.Instance.GetPropertyValueAsString(AppLocator.SETTING_KittyPortablePath, null);
            if (string.IsNullOrEmpty(kittyPath) == false && File.Exists(kittyPath))
            {
                string pathForCmd = KittyUtils.MakeKittyPathForCmd(webEntryInfo);
                if (pathForCmd != null )
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = "Открыть в Kitty";
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            //AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;

                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            ProcessUtils.ExecuteProgram(kittyPath, pathForCmd);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs.Add(menuItem);
                }
            }
            string puttyPath = FrwConfig.Instance.GetPropertyValueAsString(AppLocator.SETTING_PuttyPortablePath, null);
            if (string.IsNullOrEmpty(puttyPath) == false && File.Exists(puttyPath)) { 
                string puttyPathForCmd = KittyUtils.MakePuttyPathForCmd(webEntryInfo);
                if (puttyPathForCmd != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = "Открыть в Putty";
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            //AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;

                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            ProcessUtils.ExecuteProgram(puttyPath, puttyPathForCmd);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs.Add(menuItem);
                }
            }
            //Открыть в WinSCP
            if (AppLocator.WinSCPPath != null)
            {
                string pathForCmd = WinSCPUtils.MakePathForCmd(webEntryInfo);
                if (pathForCmd != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = "Открыть в WinSCP";
                    //if (recоmendedViewType == ViewType.WORD) menuItem.Font = new Font(menuItem.Font, FontStyle.Bold);
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            //AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.ExecuteProgram(AppLocator.WinSCPPath, pathForCmd);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs.Add(menuItem);
                }
            }
            //Открыть в RDP
            if (AppLocator.RDPPath != null)
            {
                string pathForCmd = RDPUtils.MakePathForCmd(webEntryInfo);
                if (pathForCmd != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = "RDP";
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            if (CheckIfWeNeedVPN(webEntryInfo) == false) return;
                            AppManager.Instance.CurrentWebEntryInfo = webEntryInfo;
                            ProcessUtils.ExecuteProgram(AppLocator.RDPPath, pathForCmd);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    subs.Add(menuItem);
                }
            }


        }

    }
}
