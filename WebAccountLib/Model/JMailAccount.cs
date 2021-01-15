using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;
using Dionext;

namespace FrwSoftware
{

    [JDisplayName("E-mail аккаунт")]
    [JEntity(ImageName = "fr_mailaccount", Resource = typeof(Dionext.WebAccountLibRes))]
    public class JMailAccount : IValidatableObject
    {
        public JMailAccount()
        {
        }
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JMailAccountId { get; set; }

        [RegularExpression(@".+\@.+\..+", ErrorMessage = "Not a valid email")]
        [JDisplayName("Адрес")]
        [JNameProperty, JRequired, JUnique]
        public string Address { get; set; }

        [JDisplayName("Аккаунт")]
        [JManyToOne]
        public JWebAccount JWebAccount { get; set; }

        [JDisplayName("Email восстановления. При регистрации у провайдера указан как адрес для восстановления доступа к аккаунту")]
        [JManyToOne]
        public JMailAccount RecoveryEmail { get; set; }

        [JDisplayName("Email пересылки")]
        [JManyToOne]
        public JMailAccount RedirectEmail { get; set; }

        [JDisplayName("Конечный Email пересылки")]
        [JManyToOne, JReadOnly, JsonIgnore]
        public JMailAccount EndRedirectEmail
        {
            get
            {
                return GetEndRedirectAccount();
            }
            set
            {

            }
        }
        
        [JDisplayName("Email для тестирования")]
        [JManyToOne]
        public JMailAccount TestEmail { get; set; }

        [JDisplayName(typeof(WebAccountLibRes), "BaseWebAccount_JWeb")]
        [JManyToOne, JReadOnly, JsonIgnore]
        public JWeb JWeb
        {
            get
            {
                return (JWebAccount != null) ? JWebAccount.JWeb : null;
            }
        }

        [JDisplayName("Телефон подтверждения")]
        [JManyToOne, JReadOnly, JsonIgnore]
        public JPhoneNumber JPhoneNumber
        {
            get
            {
                return (JWebAccount != null) ? JWebAccount.JPhoneNumber : null;
            }
        }

        [JDisplayName("Информация")]
        [JText]
        public string Info { get; set; }
        
        [JDisplayName("Архивный")]
        public bool IsArchived { get; set; }

        [JDisplayName("Проверять почту")]
        public bool IsChecking { get; set; }
        
        //[JDisplayName("Периодичность проверки (мин)")]
        //[Range(0, 2)]
        //public int CheckInterval { get; set; }

        
 
        ////////////
        [JIgnore, JsonIgnore]
        public WebEntryInfo WebEntryInfo
        {
            get
            {
                WebEntryInfo w = new WebEntryInfo();
                if (JMailProvider() != null)
                {
                    w.Url = JMailProvider().Url;
                }
                w.Login = Login();
                w.Password = Password();
                return w;
            }
        }
        ////////////

        [JIgnore]
        [JManyToOne]
        public JRunningJob JRunningJob { get; set; }
        [JDisplayName("Стадия задания")]
        [JDictProp(DictNames.RunningJobStage, false, DisplyPropertyStyle.ImageOnly)]
        public string Stage {
            get
            {
                if (JRunningJob != null) return JRunningJob.Stage;
                else return null;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //
            var results = new List<ValidationResult>();
            if (Address != null && Address.StartsWith("*"))
            {
                results.Add(new ValidationResult("Address can not start with *"));
            }
            if (RedirectEmail != null && RedirectEmail.Address.IndexOf("mail.ru") > -1)
            {
                results.Add(new ValidationResult("Can not reditect to mail.ru"));
            }
            /*
                Validator.TryValidateProperty(this.Prop1,
                    new ValidationContext(this, null, null) { MemberName = "Prop1" },
                    results);
            }
            */
            return results;

        }
        public JMailAccount GetEndRedirectAccount()
        {
            JMailAccount test = this.RedirectEmail;
            while (true) {
                if (test == null || test.RedirectEmail == null)
                {
                    break;
                }
                else
                {
                    test = test.RedirectEmail;
                }
            }
            return test;
        }

        //
        public string Login()
        {
                return (JWebAccount != null) ? JWebAccount.Login : null;
        }


        public string Password()
        {
                return (JWebAccount != null) ? JWebAccount.Password : null;
        }


        public string AutoPassword()
        {
                return (JWebAccount != null) ? JWebAccount.AutoPassword : null;
        }
        public JWeb JMailProvider() { 
               return (JWebAccount != null) ? JWebAccount.JWeb : null;
        }

        public JMailAccount VerEmail() { 
                return (JWebAccount != null) ? JWebAccount.JMailAccount : null;
        }


        /*
        [JValidate]
        public void Validate(ValidationResult result)
        {
            if (CheckInterval > 1) result.ValidationErrors.Add(new ValidationError() {PropertyName = "CheckInterval", Message = "CheckInterval > 1" });
        }
        */


    }
    [JEntityPlugin(typeof(JMailAccount))]
    public class JMailAccountBasePlugin : IFormsEntityPlugin
    {
        public void MakeContextMenu(IListProcessor list, List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            JMailAccount item = selectedObject as JMailAccount;
        }
    }
}
