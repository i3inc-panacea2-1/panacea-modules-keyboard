using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Panacea.Modules.Keyboard.Models
{
    [XmlRoot("Mappings")]
    public class Mappings
    {

        public Mappings()
        {
            AllMappings = new List<Map>();
        }

        [XmlElement("Map")]
        public List<Map> AllMappings { get; set; }

        [XmlAttribute("SupportsAltGr")]
        public bool SupportsAltGr { get; set; }

        internal string GetDisplayValue(string key)
        {
            return AllMappings.Where(m => m.Key == key).FirstOrDefault().Value;
        }

        internal string GetShiftValue(string key)
        {
            return AllMappings.Where(m => m.Key == key).FirstOrDefault().ShiftValue;
        }

        internal string GetAltGrValue(string key)
        {
            return AllMappings.Where(m => m.Key == key).FirstOrDefault().AltGrValue;
        }
    }

    public class Map
    {
        [XmlAttribute("Key")]
        public string Key { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; }

        [XmlAttribute("ShiftValue")]
        public string ShiftValue { get; set; }

        [XmlAttribute("AltGrValue")]
        public string AltGrValue { get; set; }

        [XmlAttribute("CapsLikeShift")]
        public bool CapsLikeShift { get; set; }

        [XmlAttribute("FakeToggle")]
        public bool FakeToggle { get; set; } = false;
    }
}
