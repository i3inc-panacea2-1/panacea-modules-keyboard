using Panacea.Modules.Keyboard.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Panacea.Modules.Keyboard
{
    public class MappingsSerializer
    {
        public static string Serialize(Mappings mappings)
        {
            string retval = "";
            var x = new XmlSerializer(typeof(Mappings));
            using (var sw = new StringWriter())
            {
                x.Serialize(sw, mappings);
                retval = sw.ToString();
                sw.Close();
            }
            return retval;
        }

        public static Mappings Deserialize(string uri)
        {
            if (!File.Exists(uri))
            {
                uri = Path.GetDirectoryName(uri) + "\\en-CA.xml";
            }
            Mappings retval = null;
            var x = new XmlSerializer(typeof(Mappings), "");
            using (var tr = new XmlTextReader(uri))
            {
                try
                {
                    retval = (Mappings)x.Deserialize(tr);
                }
                catch (Exception)
                {
                }
                tr.Close();
            }
            return retval;
        }

    }
}
