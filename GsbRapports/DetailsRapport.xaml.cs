using dllRapportVisites;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
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

            IdRapport.Content = _rapport.id;
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
            VoirVisiteWindow voir = new VoirVisiteWindow(_wb, _site, _secretaire);
            voir.Show();
            this.Close();
        }

        private void UpdateRapport_Click(object sender, RoutedEventArgs e)
        {
            if (Motif.Text != string.Empty)
            {
                if (Bilan.Text != string.Empty)
                {
                    try
                    {
                        string url = _site + "rapport/" + _rapport.id;
                        NameValueCollection parameters = new NameValueCollection();
                        parameters.Add("ticket", _secretaire.getHashTicketMdp());
                        parameters.Add("motif", Motif.Text);
                        parameters.Add("bilan", Bilan.Text);
                        byte[] tabByte = _wb.UploadValues(url, "POST", parameters);
                        string reponse1 = UnicodeEncoding.UTF8.GetString(tabByte);
                        _secretaire.ticket = reponse1;
                        MessageBox.Show($"Le rapport {_rapport.id} a bien été modifié.");
                        VoirVisiteWindow voir = new VoirVisiteWindow(_wb, _site, _secretaire);
                        voir.Show();
                        this.Close();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez saisir un bilan");
                }
            }
            else
            {
                MessageBox.Show("Vous devez saisir un motif");
            }
        }
    }
}
