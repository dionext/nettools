using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FrwSoftware
{
    [JDisplayName("Мировая валюта")]
    [JEntity]
    public class JWorldCurrency
    {
        [JDisplayName("Идентификатор")]
        [JPrimaryKey, JAutoIncrement]
        public string JWorldCurrencyId { get; set; }

        [JDisplayName("Код")]
        [JNameProperty, JRequired, JUnique]
        public string Code { get; set; }

        [JDisplayName("Название")]
        public string Name { get; set; }

        [JDisplayName("Страна")]
        [JManyToOne]
        [JImageRef(DisplyPropertyStyle.TextAndImage)]
        public JCountry JCountry { get; set; }

    }
}
