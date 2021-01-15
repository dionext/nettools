using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FrwSoftware
{
    [JDisplayName("Инсталляция ПО")]
    [JEntity(ImageName = "fr_soft_instance", Resource = typeof(NetworkAccountHelperLibRes))]
    public class JSoftInstance
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JSoftInstanceId { get; set; }

        [JDisplayName("Наименование")]
        public string Name { get; set; }

        [JDisplayName("Полное наименование")]
        [JNameProperty, JsonIgnore, JReadOnly]
        public string FullName {
            get
            {
                StringBuilder s = new StringBuilder();
                s.Append(ShortName);
                if (JSite != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(JSite.FullName);
                }
                if (JCompDevice != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(JCompDevice.FullName);
                }
                return s.ToString();
            }
        }
        [JDisplayName("Короткое наименование")]
        [JShortNameProperty, JsonIgnore, JReadOnly]
        public string ShortName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                if (Name != null) s.Append(Name);
                if (JSoft != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(JSoft.Name);
                }
                return s.ToString();
            }
        }

        [JDisplayName("ПО")]
        [JManyToOne]
        public JSoft JSoft { get; set; }

        [JDisplayName("Сайт")]
        [JManyToOne]
        public JSite JSite { get; set; }

        [JDisplayName("Путь")]
        public string Path { get; set; }

        [JDisplayName("Docker")]
        public bool IsDocker { get; set; }

        [JDisplayName("BasicAuth логин")]
        public string BasicAuthLogin { get; set; }
        [JDisplayName("BasicAuth пароль")]
        [JPassword]
        public string BasicAuthPassword { get; set; }

        [JDisplayName("Порты доступа")]
        public IList<JPort> AccessPorts { get; set; }

        [JDisplayName("Компьютер")]
        [JManyToOne]
        public JCompDevice JCompDevice { get; set; }
        [JDisplayName("Прикрепленные документы"), JHeaderImage("attachment")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }

        ////////////
        /*
        [JHelps]
        [JIgnore, JsonIgnore]
        public List<JHelp> Helps
        {
            get
            {
                List<JHelp> helps = new List<JHelp>();
                if (JSoft != null) helps.AddRange(JSoft.Helps);
                if (IsDocker)
                {
                    helps.Add(new JHelp() { Path = "file:///R:/doc/dio_documents/docker.htm" });//todo 
                }
                if (Attachments != null)
                {
                    foreach (var a in Attachments)
                    {
                        helps.Add(JHelp.MakeFromJAttachment(a, this));
                    }
                }
                return helps;
            }
        }
        */
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
                if (JSite != null && JSite.JDomain != null && JSite.JDomain.Url != null)
                {
                    w.ExternalAddress = JSite.JDomain.Url;
                }

                if (string.IsNullOrEmpty(Path) == false && Path.StartsWith("http") == false)
                {
                    if (w.Path != null)
                        w.Path =  w.Path + ((!(w.Path.EndsWith("/") || Path.StartsWith("/"))) ? "/" : "") + Path;
                    else w.Path = Path;
                }

                if (AccessPorts != null)
                {
                    foreach (var p in AccessPorts)
                    {
                        w.AccessPorts.Add(p);
                    }
                }
                if (BasicAuthLogin != null) w.BasicAuthLogin = BasicAuthLogin;
                if (BasicAuthPassword != null) w.BasicAuthPassword = BasicAuthPassword;
                return w;
            }
        }


    }
}
