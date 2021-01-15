using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dionext;
using Newtonsoft.Json;

namespace FrwSoftware
{
    [JDisplayName("Телефоннный номер")]
    [JEntity]
    public class JPhoneNumber
    {
        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_JPhoneNumberId")]
        [JPrimaryKey, JAutoIncrement]
        public string JPhoneNumberId { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_Number")]
        [JNameProperty, JRequired, JUnique]
        public string Number { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_JMobileOperator")]
        [JManyToOne]
        public JMobileOperator JMobileOperator { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_Country")]
        [JReadOnly, JsonIgnore]
        public string Country {
            get
            {
                return (JMobileOperator != null && JMobileOperator.JCountry != null) ? JMobileOperator.JCountry.Name : null;
            }
        }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_Owner")]
        public string Owner { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_JActor")]
        [JManyToOne]
        public JActor JActor { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_Purpose")]
        public string Purpose { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_Balance")]
        public double Balance { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_IsArchived")]
        public bool IsArchived { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_Login")]
        public string Login { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_Password")]
        public string Password { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_Info")]
        [JText]
        public string Info { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "JPhoneNumber_Attachments")]
        [JHeaderImage("attachment")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }

        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = new WebEntryInfo();
                if (JMobileOperator != null)
                {
                    w.Url = JMobileOperator.Url;
                }
                if (Login != null) w.Login = Login;
                else w.Login = Number;
                w.Password = Password;

                return w;
            }
        }

    }
}
