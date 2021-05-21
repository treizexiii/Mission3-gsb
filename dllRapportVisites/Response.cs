using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dllRapportVisites
{
    public class ResponseMedecins
    {
        public List<Medecin> Medecins { get; set; }
        public string ticket { get; set; }
    }
    public class ResponseRapports
    {
        public List<Rapport> rapports { get; set; }
        public string ticket { get; set; }
    }
    public class ResponseVisiteurs
    {
        public List<Visiteur> Visiteurs { get; set; }
        public string ticket { get; set; }
    }
}
