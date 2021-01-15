using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dionext;

namespace FrwSoftware
{
 


    public class NADictNames : WADictNames
    {
        public const string CompDeviceType = "CompDeviceType";
    }
    public class NADm : WebAccountDm
    {

        static public JCompDevice MyCompDevice { get; set; }
        static public JCompDeviceNetwork MyCurrentCompDeviceNetwork { get; set; }
        //temp for proxy mode 
        static public JCompDeviceNetwork NoneVPNMyCurrentCompDeviceNetwork { get; set; }

        override protected void InitDictionaries()
        {
            base.InitDictionaries();
            JDictionary dict = null;

            dict = new JDictionary() { Id = NADictNames.CompDeviceType };

            dictionaries.Add(dict);
            dict.Items.Add(new JDictItem() { Key = CompDeviceTypeEnum.Phone.ToString(), Text = "Телефон" });
            dict.Items.Add(new JDictItem() { Key = CompDeviceTypeEnum.Smartphone.ToString(), Text = "Смартфон" });
            dict.Items.Add(new JDictItem() { Key = CompDeviceTypeEnum.Tablet.ToString(), Text = "Планшет" });
            dict.Items.Add(new JDictItem() { Key = CompDeviceTypeEnum.Noutbook.ToString(), Text = "Ноутбук" });
            dict.Items.Add(new JDictItem() { Key = CompDeviceTypeEnum.Desktop.ToString(), Text = "Компьютер" });
            dict.Items.Add(new JDictItem() { Key = CompDeviceTypeEnum.Virtual.ToString(), Text = "Виртуальное устройство" });
            dict.Items.Add(new JDictItem() { Key = CompDeviceTypeEnum.Router.ToString(), Text = "Роутер" });
            dict.Items.Add(new JDictItem() { Key = CompDeviceTypeEnum.Modem.ToString(), Text = "Модем" });
            dict.Items.Add(new JDictItem() { Key = CompDeviceTypeEnum.IPRelay.ToString(), Text = "IP реле" });
            dict.Items.Add(new JDictItem() { Key = CompDeviceTypeEnum.IPCam.ToString(), Text = "IP камера" });
            dict.Items.Add(new JDictItem() { Key = CompDeviceTypeEnum.Other.ToString(), Text = "Другое" });

        }

    }
}
