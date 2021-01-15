using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dionext;
using Newtonsoft.Json;

namespace FrwSoftware
{
    [JDisplayName("Банковский счет (кошелек)")]
    [JEntity]
    public class JMoneyBankAcc
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JMoneyBankAccId { get; set; }

        [JDisplayName("Наименование")]
        [JNameProperty, JsonIgnore, JReadOnly]
        public string FullName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                if (Name != null) s.Append(Name);
                if (Type != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(Type);
                }
                if (JWebAccount != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(JWebAccount.ShortName);
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
                if (Type != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(Type);
                }
                return s.ToString();
            }
        }

        [JDisplayName("Номер счета")]
        public string Name { get; set; }

        [JDisplayName("Type")]
        public string Type { get; set; }

        [JDisplayName("Валюта")]
        [JManyToOne]
        public JWorldCurrency JWorldCurrency { get; set; }

        [JDisplayName("Владелец. Как записано на Вашей платежной карте.")]
        public string Owner { get; set; }

        [JDisplayName("Действителен до")]
        public string Expired { get; set; }

        [JDisplayName("CVV. CVV/CVC код (Card Verification Value/Code) находится на задней стороне Вашей платежной карты")]
        public string Cvv { get; set; }

        [JDisplayName("Пин код")]
        public string Pin { get; set; }

        [JDisplayName("Web аккаунт")]
        [JManyToOne]
        public JWebAccount JWebAccount { get; set; }

        [JDisplayName("Описание")]
        public string Description { get; set; }

        [JDisplayName("Информация")]
        [JText]
        public string Info { get; set; }

        [JDisplayName("Прикрепленные документы"), JHeaderImage("attachment")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }

        [JDisplayName("Архивный")]
        public bool IsArchived { get; set; }
        
        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = new WebEntryInfo();
                if (JWebAccount != null)
                {
                    if (JWebAccount.JWeb != null)
                    {
                        w.Url = JWebAccount.JWeb.Url;
                    }
                    w.Login = JWebAccount.Login;
                    w.Password = JWebAccount.Password;
                }

                return w;
            }
        }
    }
}
