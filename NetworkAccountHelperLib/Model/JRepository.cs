using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dionext;
using Newtonsoft.Json;

namespace FrwSoftware
{

    [JDisplayName("Репозиторий")]
    [JEntity(ImageName = "fr_webaccount", Resource = typeof(NetworkAccountHelperLibRes))]
    public class JRepository
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JRepositiryId { get; set; }


        [JDisplayName("Наименование")]
        [JNameProperty]
        public string Name
        {
            get;
            set;
        }
        [JDisplayName("Короткое наименование")]
        [JShortNameProperty, JReadOnly, JsonIgnore]
        public string ShortName
        {
            get
            {
                return Name;
            }
        }


        [JDisplayName("Web аккаунт")]
        [JManyToOne]
        public JWebAccount JWebAccount { get; set; }


        [JDisplayName("Url")]
        public string Url { get; set; }

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
                else w = new WebEntryInfo();
                if (Url != null) w.Url = Url;
                return w;
            }
        }
    }
}
