using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dllRapportVisites
{
    public class Offre
    {
        public string id { get; set; }
        public string qte { get; set; }

        public Offre(string id, string qte)
        {
            this.id = id;
            this.qte = qte;
        }
    }
}
