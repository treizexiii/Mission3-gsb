using dllRapportVisites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;

namespace GsbRapports
{
    /// <summary>
    /// Interaction logic for TestMotifs.xaml
    /// </summary>
    public partial class TestMotifs : Window
    {
        private readonly WebClient _wb;
        private readonly string _site;
        private readonly Secretaire _secretaire;
        private readonly List<Rapport> _lesRapports;
        private readonly List<string> _lesMotifs;

        public TestMotifs(WebClient wb, string site, Secretaire secretaire)
        {
            InitializeComponent();
            _wb = wb;
            _site = site;
            _secretaire = secretaire;
            _lesRapports = GetLesRapports();
            
            if (_lesRapports.Count > 0)
            {
                _lesMotifs = GetMotifs();

                if (_lesMotifs.Count > 0)
                {
                    foreach (var element in _lesMotifs)
                    {
                        ListMotif.Items.Add(element);
                    }
                }
            }
        }

        private List<Rapport> GetLesRapports()
        {
            var url = _site + "rapports?ticket=" + _secretaire.getHashTicketMdp() + "&dateDebut=2016-01-01&dateFin=" + DateTime.Now.ToString("yyy-MM-dd");
            var raw = _wb.DownloadString(url);
            var response = JsonConvert.DeserializeObject<ResponseRapports>(raw);
            _secretaire.ticket = response.ticket;
            return response.rapports;
        }

        private List<string> GetMotifs()
        {
            List<string> motif = new List<string>();
            foreach (var rapport in _lesRapports)
            {
                var isExist = motif.Contains(rapport.motif);
                if (!isExist)
                {
                    motif.Add(rapport.motif);
                }
            }

            return motif;
        }
    }
}
