using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dionext;
using Newtonsoft.Json;

namespace FrwSoftware
{
    public enum CompDeviceTypeEnum
    {
        Phone,
        Smartphone,
        Tablet,
        Noutbook,
        Desktop,
        Virtual,
        Modem,
        Router,
        IPRelay,
        IPCam,
        Other
    }

    [JDisplayName("Компьюретное устройство")]
    [JEntity(ImageName = "fr_computer", Resource = typeof(NetworkAccountHelperLibRes))]
    public class JCompDevice
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JCompDeviceId { get; set; }

        [JDisplayName("Наименование")]
        [JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName("Полное наименование")]
        [JNameProperty, JReadOnly, JsonIgnore]
        public string FullName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                s.Append(ShortName);
                if (JCompDeviceNetworkParentLevel != null)
                {
                    if (s.Length > 0)
                    {
                        s.Append(" (");
                        s.Append(JCompDeviceNetworkParentLevel.Name);
                        s.Append(")");
                    }
                    else s.Append(JCompDeviceNetworkParentLevel.Name);
                }
                return s.ToString();
            }
        }
        [JDisplayName("Короткое наименование")]
        [JShortNameProperty, JReadOnly, JsonIgnore]
        public string ShortName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                if (Name != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(Name);
                }
                else if (GetInternalAddress() != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(GetInternalAddress());
                }
                else
                {
                    s.Append(JCompDeviceId);
                }
                return s.ToString();
            }
        }


        [JDisplayName("Тип устройства")]
        [JDictProp(NADictNames.CompDeviceType, false, DisplyPropertyStyle.TextOnly)]
        public string Stage { get; set; }

        [JDisplayName("Тех. характеристики")]
        public string Specifications { get; set; }

        [JDisplayName("Подсеть")]
        [JManyToOne]
        public JCompDeviceNetwork JCompDeviceNetwork { get; set; }

        [JIgnore, JsonIgnore, JReadOnly]
        public JCompDeviceNetwork JCompDeviceNetworkParentLevel
        {
            get
            {
                JCompDevice d = this;
                while (d != null)
                {
                    if (d.JCompDeviceNetwork != null) return d.JCompDeviceNetwork;
                    d = d.JCompDeviceHost;
                }
                return null;

            }
            set
            {

            }
        }

        [JDisplayName("Хостовое устройство")]
        [JManyToOne]
        public JCompDevice JCompDeviceHost { get; set; }

        [JDisplayName("Внешние адреса")]
        public IList<string> ExternalAddresses { get; set; }

        public string GetExternalAddress()
        {
            return (ExternalAddresses != null) ? (ExternalAddresses.FirstOrDefault()) : null;
        }

        [JDisplayName("Вннутренние адреса")]
        public IList<string> InternalAddresses { get; set; }
        
        public string GetInternalAddress()
        {
            return (InternalAddresses != null) ? (InternalAddresses.FirstOrDefault()) : null;
        } 

        [JDisplayName("IMEI")]
        public string Imei { get; set; }

        [JDisplayName("MAC адрес")]
        public string MACAddress { get; set; }

        [JDisplayName("Крактое описание")]
        public string Description { get; set; }

        [JDisplayName("Информация")]
        [JText]
        public string Info { get; set; }

        [JDisplayName("Логин")]
        public string Login { get; set; }
        
        [JDisplayName("Пароль")]
        [JPassword]
        public string Password { get; set; }

        [JDisplayName("Порты доступа")]
        public IList<JPort> AccessPorts { get; set; }

        [JDisplayName("Ssh Host Key Fingerprint")]
        public string SshHostKeyFingerprint { get; set; }

        [JDisplayName("Архивный")]
        public bool Archive { get; set; }

        [JDisplayName("Прикрепленные документы"), JHeaderImage("attachment")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }

        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = new WebEntryInfo();

                w.Login = Login;
                w.Password = Password;
                w.InternalAddress = GetInternalAddress();
                if (JCompDeviceNetworkParentLevel != null)
                {
                    if (NADm.MyCurrentCompDeviceNetwork == JCompDeviceNetworkParentLevel ||
                       (VpnConnUtils.CurrentVPNServer != null
                       && JCompDeviceNetworkParentLevel.VPNServer != null
                       && JCompDeviceNetworkParentLevel.VPNServer == VpnConnUtils.CurrentVPNServer))
                    {
                        w.IsInInternalNetwork = true;
                    }
                    else w.IsInInternalNetwork = false;

                    if (GetExternalAddress() != null) w.ExternalAddress = GetExternalAddress();
                    else w.ExternalAddress = JCompDeviceNetworkParentLevel.GetExternalAddress();
                }
                if (AccessPorts != null)
                {
                    foreach (var p in AccessPorts)
                    {
                        w.AccessPorts.Add(p);
                    }
                }

                return w;
            }
        }
        //public object RemoteSession;

        static public void CloneDependencies(object cloningObject, object targetObject)
        {
            //Console.WriteLine("---- -------- CloneDependencies");
            IList<JSoftInstance> siList = Dm.Instance.ResolveOneToManyRelation<JSoftInstance>(cloningObject);
            foreach (var si in siList)
            {
                JSoftInstance siClone = (JSoftInstance)Dm.CloneObject(si, CloneObjectType.ForNew);
                siClone.JCompDevice = targetObject as JCompDevice;
                Dm.Instance.SaveObject(siClone);

                //cloning user accounts
                IList<JUserAccount> uaList = Dm.Instance.ResolveOneToManyRelation<JUserAccount>(si);
                foreach (var ua in uaList)
                {
                    JUserAccount uaClone = (JUserAccount)Dm.CloneObject(ua, CloneObjectType.ForNew);
                    uaClone.JSoftInstance = siClone;
                    Dm.Instance.SaveObject(uaClone);
                }
            }
            IList<JCompDeviceStorage> sdsList = Dm.Instance.ResolveOneToManyRelation<JCompDeviceStorage>(cloningObject);
            foreach (var sds in sdsList)
            {
                JCompDeviceStorage sdsClone = (JCompDeviceStorage)Dm.CloneObject(sds, CloneObjectType.ForNew);
                sdsClone.JCompDevice = targetObject as JCompDevice;
                Dm.Instance.SaveObject(sdsClone);
            }
        }
    }



    [JEntityPlugin(typeof(JCompDevice))]
    public class JCompDeviceBasePlugin : IFormsEntityPlugin
    {
        public void MakeContextMenu(IListProcessor list, List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            menuItemList.Add(new ToolStripSeparator());
            JCompDevice item = (JCompDevice)selectedObject;
            if (item != null)
            {
                ToolStripItem menuItem = null;
                if (WinSCPUtils.IsRemoteSessionOpened(item))
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = "Закрыть удаленную сессию подключения WinSCP";
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            WinSCPUtils.CloseRemoteSession(item);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    menuItemList.Add(menuItem);
                }
            }
        }

    }
}
