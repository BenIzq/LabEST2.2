using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labestu2.Objetos;

namespace Labestu2.Objetos
{
    public class participante
    {
        public string name { get; set; }
        public string dpi { get; set; }
        public string dateBirth { get; set; }
        public string address { get; set; }
        public List<string> companies { get; set; }    
    }
}
