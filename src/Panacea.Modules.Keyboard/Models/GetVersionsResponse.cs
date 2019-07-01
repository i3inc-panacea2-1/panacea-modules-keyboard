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
