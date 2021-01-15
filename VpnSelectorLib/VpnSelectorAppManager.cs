using FrwSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dionext
{
    public class VpnSelectorAppManager: AppManager
    {

        override public void InitApplication()
        {
            base.InitApplication();

            VpnSelectorDm.MyLocalIP = BaseNetworkUtils.GetHostIp();
            if ("127.0.0.1".Equals(VpnSelectorDm.MyLocalIP) == false)
            {
                VpnSelectorDm.MyMac = BaseNetworkUtils.GetMacByIP(VpnSelectorDm.MyLocalIP);
                VpnSelectorDm.DnsIP = BaseNetworkUtils.GetDnsAddressForIP(VpnSelectorDm.MyLocalIP);
                if (VpnSelectorDm.DnsIP != null)
                {
                    VpnSelectorDm.DnsMac = BaseNetworkUtils.GetMacByIP(VpnSelectorDm.DnsIP);
                }
            }

            bool _isActiveConnectionPresent = VpnConnUtils.IsActiveConnectionPresent();
            if (_isActiveConnectionPresent)
            {
                VpnConnUtils.CurrentVPNServer = JVPNServer.FindFromNames(VpnConnUtils.GetActiveConnectionsNames());
            }


            Console.WriteLine("IP: " + VpnSelectorDm.MyLocalIP);
            Console.WriteLine("MAC: " + VpnSelectorDm.MyMac);
            Console.WriteLine("DNS: " + VpnSelectorDm.DnsIP);
            Console.WriteLine("DNS MAC: " + VpnSelectorDm.DnsMac);
            Console.WriteLine("Current VPN Server: " + VpnConnUtils.CurrentVPNServer);

            BaseNetworkUtils.ShowNetworkInterfaces();
        }

        //override protected BaseMainAppForm GetMainForm()
        // {
        //    return new VpnSelectorMainForm();
        //}
        //override protected IListProcessor GetListWindowForType(Type type)
        //{
        //    IListProcessor w = null;
         //   if (AttrHelper.IsSameOrSubclass(typeof(JVPNServer), type)) w = new JVPNServerListWindow();
        //    else w = base.GetListWindowForType(type);
        //    return w;
        //}
    }
}
