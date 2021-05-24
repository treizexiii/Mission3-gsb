using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dllRapportVisites
{
    public static class Services
    {
        public static readonly string _path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";

        public static bool GenerateXml(dynamic obj, string name, DateTime? dateStart = null, DateTime? dateEnd = null)
        {
            //Xml pour rapports
            if (obj is List<Rapport>)
            {
                if (obj.Count > 0)
                {
                    var xEle = new XElement(name);
                    foreach (Rapport rapport in obj)
                    {
                        xEle.Add(new XElement("Rapport",
                                                   new XElement("Date", rapport.date),
                                                   new XElement("NomMedecin", rapport.nomMedecin),
                                                   new XElement("PrenomMedecin", rapport.prenomMedecin),
                                                   new XElement("NomVisiteur", rapport.nomVisiteur),
                                                   new XElement("PrenomVisiteur", rapport.prenomVisiteur),
                                                   new XElement("Motif", rapport.motif)));
                    }
                    xEle.Save(_path + name + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + ".xml");
                }
                return true;
            }

            //Xml pour médicament
            if (obj.Equals(typeof(List<Medicament>)))
            {
                if (obj.Count > 0)
                {
                    var xEle = new XElement(name);
                    foreach (Medicament medicament in obj)
                    {
                        xEle.Add(new XElement("Rapport",
                                                   new XElement("NomCommercial", medicament.nomCommercial),
                                                   new XElement("Composition", medicament.composition),
                                                   new XElement("ContreIindication", medicament.contreIndications),
                                                   new XElement("Effets", medicament.effets),
                                                   new XElement("IdFamille", medicament.idFamille)));
                    }
                    if (dateStart == null && dateEnd == null)
                    {
                        xEle.Save(_path + name + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + ".xml");
                    }
                    else
                    {
                        xEle.Save(_path + name + dateStart.ToString() + "to" + dateEnd.ToString() + ".xml");
                    }
                }
                return true;
            }

            return false;
        }
    }
}
