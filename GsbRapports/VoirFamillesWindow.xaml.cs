using dllRapportVisites;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Windows;

namespace GsbRapports
{
    /// <summary>
    /// Logique d'interaction pour VoirFamillesWindow.xaml
    /// </summary>
    public partial class VoirFamillesWindow : Window
    {
        private readonly WebClient _wb;
        private readonly Secretaire _laSecretaire;
        private readonly string _site;

        public VoirFamillesWindow(WebClient wb, Secretaire s, string site)
        {
            InitializeComponent();
            _wb = wb;
            _laSecretaire = s;
            _site = site;

            string url = _site + "familles?ticket=" + _laSecretaire.getHashTicketMdp();
            string reponse = _wb.DownloadString(url);
            dynamic d = JsonConvert.DeserializeObject(reponse);
            string familles = d.familles.ToString();
            string ticket = d.ticket;
            List<Famille> f = JsonConvert.DeserializeObject<List<Famille>>(familles);
            _laSecretaire.ticket = ticket;
            dtg_famille.ItemsSource = f;
        }
    }
}
