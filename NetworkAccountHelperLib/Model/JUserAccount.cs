using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FrwSoftware
{
    [JDisplayName("Пользовательский аккаунт")]
    [JEntity(ImageName = "fr_webaccount", Resource = typeof(NetworkAccountHelperLibRes))]
    public class JUserAccount
    {
        [JPrimaryKey, JAutoIncrement, JDisplayName("Идентификатор")]
        public string JUserAccountId { get; set; }


        [JDisplayName("Полное наименование")]
        [JReadOnly, JsonIgnore, JNameProperty]
        public string FullName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                s.Append(ShortName);
                if (JSoftInstance != null && JSoftInstance.JSoft != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    s.Append(JSoftInstance.FullName);
                }
                //if (JHosting != null)
                //{
                //    if (s.Length > 0) s.Append(" ");
                 //   s.Append(JHosting.FullName);
                //}
                return s.ToString();
            }
        }
        [JDisplayName("Короткое наименование")]
        [JReadOnly, JsonIgnore, JShortNameProperty]
        public string ShortName
        {
            get
            {
                StringBuilder s = new StringBuilder();
               if (Login != null) s.Append(Login);
               else s.Append(JUserAccountId);
               return s.ToString();
            }
        }

        //[JDisplayName("Хостинг")]
        //[JManyToOne]
        //public JHosting JHosting { get; set; }

        [JDisplayName("Инсталляция ПО")]
        [JManyToOne]
        public JSoftInstance JSoftInstance { get; set; }

        [JDisplayName("Логин")]
        public string Login { get; set; }
        [JDisplayName("Пароль")]
        [JPassword]
        public string Password { get; set; }

        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = null;
                if (JSoftInstance != null)
                {
                    w = JSoftInstance.WebEntryInfo;
                }
                //else if (JHosting != null)
                //{
                 //   w = JHosting.WebEntryInfo;
                //}
                else w = new WebEntryInfo();
                w.Login = Login;
                w.Password = Password;
                return w;
            }
        }

    }
}
