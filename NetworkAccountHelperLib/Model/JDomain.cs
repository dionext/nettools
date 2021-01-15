using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dionext;
using Newtonsoft.Json;

namespace FrwSoftware
{
    [JDisplayName("Доменное имя")]
    [JEntity]
    public class JDomain
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JDomainId { get; set; }

        /// <summary>
        /// Внимание: При регистрации в системах web аналитики может иметь значение форам указания имени http://mysite.com www.mysite.com  mysite.com
        /// </summary>
        [JDisplayName("Имя домена")]
        [JNameProperty, JRequired, JUnique]
        public string Url { get; set; }


        [JDisplayName("Web аккаунт")]
        [JManyToOne]
        public JWebAccount JWebAccount { get; set; }

        [JDisplayName("Главный домен (для зеркала)")]
        [JManyToOne]
        public JDomain MainJDomain { get; set; }


        [JDisplayName("Информация")]
        [JText]
        public string Info { get; set; }

        [JDisplayName("Прикрепленные документы"), JHeaderImage("attachment")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }

        [JDisplayName("Последняя реновация")]
        [JExpired("Expired")]
        public DateTime LastRenewDate { get; set; }

        [JDisplayName("Период регистрации")]
        [JDictProp(DictNames.Period, false, DisplyPropertyStyle.TextOnly)]
        [JExpired("Expired")]
        public string ExpiredPeriod { get; set; }

        [JDisplayName("Просрочен")]
        [JReadOnly, JsonIgnore, JIgnore, JExpired]
        public string Expired {
            get
            {
                return BaseTasksUtils.Expired(LastRenewDate, ExpiredPeriod).ToString();
            }
        }


        [JDisplayName("Аккаунт Google аналитики")]
        [JManyToOne]
        public JWebAccount GoogleJWebAccount { get; set; }


        [JDisplayName("Идентификатор Google аналитики")]
        public string GoogleAnaliticsId { get; set; }

        [JDisplayName("Report Home Google аналитики")]
        public string GoogleReportHome { get; set; }

        [JDisplayName("Аккаунт Yandex аналитики")]
        [JManyToOne]
        public JWebAccount YandexJWebAccount { get; set; }


        ////////////
        [JDisplayName("Доменное имя")]
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = null;
                if (Url != null)
                {
                    w = new WebEntryInfo() { Url = this.Url };
                    if (w.Url.StartsWith("http://") == false && w.Url.StartsWith("https://") == false)
                    {
                        w.Url = "http://" + w.Url;
                    }
                }
                return w;
            }
        }
        
        [JDisplayName("Регистратор домена")]
        [JIgnore, JsonIgnore]
        public WebEntryInfo RegistratorWebEntryInfo
        {
            get
            {
                WebEntryInfo w = null;
                if (JWebAccount != null)
                {
                    w = JWebAccount.WebEntryInfo;
                }
                return w;
            }
        }

        [JDisplayName("Google аналитика")]
        [JIgnore, JsonIgnore]
        public WebEntryInfo GoogleWebEntryInfo
        {
            get
            {
                WebEntryInfo w = null;
                if (GoogleJWebAccount != null)
                {
                    w = GoogleJWebAccount.WebEntryInfo;
                    //w = new WebEntryInfo();
                    if (GoogleReportHome != null)
                    {
                        w.Url = "https://analytics.google.com/analytics/web/#/report-home/" + GoogleReportHome;
                    }
                    else
                    {
                        w.Url = "https://analytics.google.com/analytics/web/";
                    }
                    w.Login = GoogleJWebAccount.Login;
                    w.Password = GoogleJWebAccount.Password;
                }
                return w;
            }
        }

        [JDisplayName("Google search console")]
        [JIgnore, JsonIgnore]
        public WebEntryInfo GoogleSearchWebEntryInfo
        {
            get
            {
                WebEntryInfo w = null;
                if (GoogleJWebAccount != null)
                {
                    w = new WebEntryInfo();
                    w.Url = "https://search.google.com/search-console?resource_id=" + Url;
                    //w.Url = "https://search.google.com/search-console?resource_id=sc-domain:" + Url;
                    //Примечание: если предварительно войти в google аккаунт, то может происходить редирект на аналитику другого домена в случае, если на данный аккаунт зарегистрировано несколько доменов
                    w.Login = GoogleJWebAccount.Login;
                    w.Password = GoogleJWebAccount.Password;
                }
                return w;
            }
        }
        
            
        [JDisplayName("Yandex аналитика")]
        [JIgnore, JsonIgnore]
        public WebEntryInfo YandexWebEntryInfo
        {
            get
            {
                WebEntryInfo w = null;
                if (YandexJWebAccount != null)
                {
                    w = YandexJWebAccount.WebEntryInfo;
                    //w = new WebEntryInfo();
                    w.Url = "https://webmaster.yandex.ru/sites/";
                    w.Login = YandexJWebAccount.Login;
                    w.Password = YandexJWebAccount.Password;
                }
                return w;
            }
        }
    }
}
