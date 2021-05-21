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

        public static bool GenerateXml(dynamic obj, string name)
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
                                                   new XElement("Date", medicament.nomCommercial),
                                                   new XElement("NomMedecin", medicament.composition),
                                                   new XElement("PrenomMedecin", medicament.contreIndications),
                                                   new XElement("NomVisiteur", medicament.effets),
                                                   new XElement("PrenomVisiteur", medicament.idFamille)));
                    }
                    xEle.Save(_path + name + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + ".xml");
                }
                return true;
            }

            return false;
        }
    }
}
