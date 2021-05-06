using dllRapportVisites;
using System.Net;
using System.Windows;

namespace GsbRapports
{
    /// <summary>
    /// Interaction logic for DetailsRapport.xaml
    /// </summary>
    public partial class DetailsRapport : Window
    {
        private readonly WebClient _wb;
        private readonly Secretaire _secretaire;
        private readonly string _site;
        private readonly Rapport _rapport;

        public DetailsRapport(WebClient wb, Secretaire secretaire, string site, Rapport rapport)
        {
            InitializeComponent();
            _wb = wb;
            _secretaire = secretaire;
            _site = site;
            _rapport = rapport;

            nomVisiteur.Content = _rapport.nomVisiteur;
            prenomVisiteur.Content = _rapport.prenomVisiteur;
            nomMedecin.Content = _rapport.nomMedecin;
            prenomMedecin.Content = _rapport.prenomMedecin;
            Date.Content = _rapport.date.ToString("dd-MM-yyyy");
            Motif.Text = _rapport.motif;
            Bilan.Text = _rapport.bilan;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateRapport_Click(object sender, RoutedEventArgs e)
        {
            if (Motif.Text != string.Empty)
            {
                if (Bilan.Text != string.Empty)
                {
                    string url = _site + "gsbRapports/rapport/" + _rapport;
                }
                else
                {
                    MessageBox.Show("Vous devez saisir un motif");
                }
            }
            else
            {
                MessageBox.Show("Vous devez saisir un motif");
            }
        }
    }
}
