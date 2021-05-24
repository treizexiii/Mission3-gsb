using dllRapportVisites;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Windows;

namespace GsbRapports
{
    /// <summary>
    /// Interaction logic for VoirMedecins.xaml
    /// </summary>
    public partial class VoirMedecins : Window
    {
        private readonly WebClient _wb;
        private readonly string _site;
        private readonly Secretaire _secretaire;

        public VoirMedecins(WebClient wb, String site, Secretaire secretaire)
        {
            InitializeComponent();
            _wb = wb;
            _site = site;
            _secretaire = secretaire;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string hashedToken = _secretaire.getHashTicketMdp();
            string url = _site + "medecins?ticket=" + hashedToken + "&nom=" + RechercherMedecin.Text.ToLower();
            string raw = _wb.DownloadString(url);
            var response = JsonConvert.DeserializeObject<ResponseMedecins>(raw);
            _secretaire.ticket = response.ticket;

            MedecinsList.ItemsSource = null;
            MedecinsList.ItemsSource = response.Medecins;
            RechercherMedecin.Text = String.Empty;
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (MedecinsList.SelectedItem != null)
            {
                DetailsMedecin voir = new DetailsMedecin(_wb, _site, _secretaire, (Medecin)MedecinsList.SelectedItem);
                voir.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Vous devez selectionner un médecin");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
