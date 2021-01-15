using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FrwSoftware
{
    [JDisplayName("Сайт")]
    [JEntity]
    public class JSite
    {
        [JPrimaryKey, JAutoIncrement, JDisplayName("Идентификатор")]
        public string JSiteId { get; set; }

        [JDisplayName("Наименование")]
        public string Name { get; set; }

        [JDisplayName("Полное наименование")]
        [JReadOnly, JsonIgnore, JNameProperty]
        public string FullName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                if (JDomain != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(JDomain.Url);
                }
                if (Name != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(Name);
                }
                return s.ToString();
            }
        }

        [JDisplayName("Комп. устройство (хостинг)")]
        [JManyToOne]
        public JCompDevice JCompDevice { get; set; }

        [JDisplayName("Основной сайт")]
        public bool IsMain { get; set; }

        [JDisplayName("Домен")]
        [JManyToOne]
        public JDomain JDomain { get; set; }

        //[JDisplayName("Суб. домен")]
        //public string Subdomain { get; set; }

        //[JDisplayName("Путь")]
        //public string Path { get; set; }

        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = new WebEntryInfo();
                if (JCompDevice != null)
                {
                    w = JCompDevice.WebEntryInfo;
                }
                //w.Path = Path;
                if (JDomain != null)
                {
                    w.Url = JDomain.Url;
                    if (w.Url != null && w.Url.StartsWith("http") == false && w.Url.StartsWith("https") == false)
                        w.Url = "http://" + w.Url;
                }
                return w;
            }
        }
    }
}
