using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dionext;
using Newtonsoft.Json;

namespace FrwSoftware
{
    [JDisplayName("Актер")]
    [JEntity(ImageName = "fr_actor", Resource = typeof(Dionext.WebAccountLibRes))]
    public class JActor
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JActorId { get; set; }

        [JDisplayName("Полное имя")]
        [JNameProperty]
        public string FullName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                if (LastName != null) s.Append(LastName);
                if (FirstName != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    if (s.Length > 0 && FirstName.Length > 1) s.Append(FirstName.Substring(0, 1).ToUpper() + ".");
                    else s.Append(FirstName);
                }
                if (MiddleName != null)
                {
                    if (s.Length > 0) s.Append(" ");
                    if (s.Length > 0 && MiddleName.Length > 1) s.Append(MiddleName.Substring(0, 1).ToUpper() + ".");
                    else s.Append(MiddleName);
                }
                return s.ToString();
            }
        }

        [JDisplayName("Имя")]
        public string FirstName { get; set; }

        [JDisplayName("Фамилия")]
        public string LastName { get; set; }

        [JDisplayName("Отчество")]
        public string MiddleName { get; set; }

        [JDisplayName("Дата рождения")]
        public string Birthday { get; set; }

        [JDisplayName("Страна")]
        [JManyToOne]
        [JImageRef(DisplyPropertyStyle.TextAndImage)]
        public JCountry JCountry { get; set; }

        [JDisplayName("Почтовый код")]
        public string ZipCode { get; set; }

        [JDisplayName("Регигон. Область, штат.")]
        public string Region { get; set; }

        [JDisplayName("Город")]
        public string Town { get; set; }

        [JDisplayName("Адрес. Улица, дом и т.д.")]
        public string Address { get; set; }

        [JDisplayName("Основной Email")]
        [JManyToOne]
        public JMailAccount JMailAccount { get; set; }

        [JDisplayName("Основной телефон")]
        [JManyToOne]
        public JPhoneNumber JPhoneNumber { get; set; }

        [JDisplayName("Документ")]
        public string Document { get; set; }

        [JDisplayName("Справочные сведения"), JHeaderImage("book_open")]
        [JText]
        public string Description { get; set; }

        [JDisplayName("Прикрепленные документы"), JHeaderImage("attachment")]
        [JAttachments]
        public List<JAttachment> Attachments { get; set; }

        [JDisplayName("Архивный")]
        public bool IsArchived { get; set; }

        [JDisplayName("Требуемое VPN подключение. Заполняется, если для данной сущнности рекомендовано выходить в интернет через определенное VPN подключение")]
        [JManyToOne]
        public JVPNServer AllowedVPNServer { get; set; }

    }
}
