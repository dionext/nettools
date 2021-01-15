using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dionext;
using Newtonsoft.Json;

namespace FrwSoftware
{
    public enum NetworkSecTypeEnum
    {
        Normal = 0,
        Official = 1,
        Private = 2,
        Personal = 3
    }

    [JDisplayName("Компьютерная подсеть")]
    [JEntity(ImageName = "fr_network", Resource = typeof(NetworkAccountHelperLibRes))]
    public class JCompDeviceNetwork
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JCompDeviceNetworkId { get; set; }

        [JDisplayName("Название")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName("Внешние адреса")]
        public IList<string> ExternalAddresses { get; set; }
        public string GetExternalAddress()
        {
            return (ExternalAddresses != null) ? (ExternalAddresses.FirstOrDefault()) : null;
        }

        [JDisplayName("Web аккаунт")]
        [JManyToOne]
        public JWebAccount JWebAccount { get; set; }

        [JDisplayName("Прокси")]
        [JManyToOne]
        public JVPNServer VPNServer { get; set; }
        
        [JDisplayName("Роутер MAC")]
        public string RouterMacAddress { get; set; }

        //[JDisplayName("Внутренняя сеть")]
        //public bool IsInternal { get; set; }

        [JDisplayName("Информация")]
        [JText]
        public string Info { get; set; }

        [JDisplayName("Прикрепленные документы"), JHeaderImage("attachment")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }

        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = null;
                if (JWebAccount != null)
                {
                    w = JWebAccount.WebEntryInfo;
                }
                else { 
                    w = new WebEntryInfo();
                }
                if (GetExternalAddress() != null)
                {
                    w.Url = null;
                    w.ExternalAddress = GetExternalAddress();
                }
                return w;
            }
        }
        public static JCompDeviceNetwork FindByVPN(JVPNServer p)
        {
            return Dm.Instance.FindAll<JCompDeviceNetwork>().FirstOrDefault(c => c.VPNServer == p);
        }
    }
}
