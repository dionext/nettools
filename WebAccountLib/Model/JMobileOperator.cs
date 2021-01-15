using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FrwSoftware
{
    [JDisplayName("Мобильный оператор")]
    [JEntity]
    public class JMobileOperator
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JMobileOperatorId { get; set; }

        [JDisplayName("Название")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName("Страна")]
        [JManyToOne]
        [JImageRef(DisplyPropertyStyle.TextAndImage)]
        public JCountry JCountry { get; set; }

        [JDisplayName("Адрес сайта")]
        [JUrl]
        public string Url { get; set; }

        [JDisplayName("Справочные сведения"), JHeaderImage("book_open")]
        [JText]
        public string Description { get; set; }

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
                w.Url = Url;
                return w;
            }
        }

    }
}
