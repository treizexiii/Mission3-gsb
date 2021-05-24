using dllRapportVisites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Windows;

namespace GsbRapports
{
    /// <summary>
    /// Interaction logic for DetailsMedecin.xaml
    /// </summary>
    public partial class DetailsMedecin : Window
    {
        private readonly WebClient _wb;
        private readonly string _site;
        private readonly Secretaire _secretaire;
        private readonly Medecin _medecin;
        private readonly List<Rapport> _rapports;

        public DetailsMedecin(WebClient wb, string site, Secretaire secretaire, Medecin medecin)
        {
            InitializeComponent();
            _wb = wb;
            _site = site;
            _secretaire = secretaire;
            _medecin = medecin;

            IdMedecin.Content = medecin.id;
            NomMedecin.Content = medecin.nom;
            PrenomMedecin.Content = medecin.prenom;
            Adresse.Text = medecin.adresse;
            Departement.Text = medecin.departement;
            Tel.Text = medecin.tel;
            Specialite.Text = medecin.specialiteComplementaire;

            _rapports = FeedRapportList(medecin.id.ToString().ToLower());
            RapportList.ItemsSource = _rapports;
        }

        private List<Rapport> FeedRapportList(string id)
        {
            string hashedToken = _secretaire.getHashTicketMdp();
            string url = _site + "rapports?ticket=" + hashedToken + "&idMedecin=" + id;
            string raw = _wb.DownloadString(url);
            var response = JsonConvert.DeserializeObject<ResponseRapports>(raw);
            _secretaire.ticket = response.ticket;
            return response.rapports;
        }

        private void UpdateMedecin_Click(object sender, RoutedEventArgs e)
        {
            if (Adresse.Text != string.Empty && Departement.Text != string.Empty && Tel.Text != string.Empty)
            {
                try
                {
                    Convert.ToInt32(Departement.Text);
                    try
                    {
                        string url = _site + "medecin/" + _medecin.id;
                        NameValueCollection parameters = new NameValueCollection();
                        parameters.Add("ticket", _secretaire.getHashTicketMdp());
                        parameters.Add("adresse", Adresse.Text);
                        parameters.Add("departement", Departement.Text);
                        parameters.Add("tel", Tel.Text);
                        parameters.Add("specialite", Specialite.Text);
                        byte[] tabByte = _wb.UploadValues(url, "POST", parameters);
                        string reponse1 = UnicodeEncoding.UTF8.GetString(tabByte);
                        _secretaire.ticket = reponse1;
                        MessageBox.Show($"Le medecin {_medecin.id} a bien été modifié.");
                        VoirMedecins voir = new VoirMedecins(_wb, _site, _secretaire);
                        voir.Show();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Département: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("L'ensemble des champs doivent être renseignés.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            VoirMedecins voir = new VoirMedecins(_wb, _site, _secretaire);
            voir.Show();
            this.Close();
        }

        private void GenereteXml_Click(object sender, RoutedEventArgs e)
        {
            var generate = Services.GenerateXml(_rapports, "Rapport");
            if (generate)
            {
                MessageBox.Show($"Génération du fichier XML, {Services._path}");
            }
            else
            {
                MessageBox.Show($"Problème dans la création du fichier XML");
            }
        }
    }
}
