using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.Keyboard.Models
{
    [DataContract]
    public class GetVersionsResponse
    {

        [DataMember(Name = "inputLanguages")]
        public List<Language> InputLanguages { get; set; }

        [DataMember(Name = "languages")]
        public List<Language> Languages { get; set; }

        [DataMember(Name = "translations")]
        public Dictionary<string, Dictionary<string, List<Translation>>> Translations { get; set; }
    }

    [DataContract]
    public class Translation
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "trans")]
        public string Trans { get; set; }

    }


    [DataContract]
    public class Language
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "rtl")]
        public String IsRtl { get; set; }

        [DataMember(Name = "code")]
        public String Code { get; set; }

    }
}
