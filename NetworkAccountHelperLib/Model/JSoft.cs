using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FrwSoftware
{
    [JDisplayName("ПО")]
    [JEntity(ImageName = "fr_soft", Resource = typeof(NetworkAccountHelperLibRes))]
    public class JSoft
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JSoftId { get; set; }

        [JDisplayName("Наименование")]
        [JNameProperty, JRequired]
        public string Name { get; set; }

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
   
        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfoHelp
        {
            get
            {
                WebEntryInfo w = new WebEntryInfo();
                {
                    foreach (var a in Attachments)
                    {
                        w.Url = JHelp.MakeFromJAttachment(a, this).Path;
                        break;
                    }
                }
                return w;
            }
        }
             */
    }
}
