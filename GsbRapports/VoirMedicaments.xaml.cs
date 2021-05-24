using dllRapportVisites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;

namespace GsbRapports
{
    /// <summary>
    /// Interaction logic for VoirMedicaments.xaml
    /// </summary>
    public partial class VoirMedicaments : Window
    {
        private readonly WebClient _wb;
        private readonly string _site;
        private readonly Secretaire _secretaire;
        private List<Medicament> _offerts;

        public VoirMedicaments(WebClient wb, string site, Secretaire secretaire)
        {
            InitializeComponent();
            _wb = wb;
            _site = site;
            _secretaire = secretaire;

            ListFamille.ItemsSource = FeedFamilyList();
            ListFamille.DisplayMemberPath = "libelle";
            ListMedicaments.ItemsSource = GetMedicaments();
            _offerts = null;
        }

        private List<Famille> FeedFamilyList()
        {
            string hashedToken = _secretaire.getHashTicketMdp();
            string url = _site + "familles?ticket=" + hashedToken;
            string raw = _wb.DownloadString(url);
            var response = JsonConvert.DeserializeObject<ResponseFamily>(raw);
            _secretaire.ticket = response.ticket;
            return response.Familles;
        }

        private List<Medicament> GetMedicaments(string id = null)
        {
            List<Medicament> listMedicaments = null;
            if (id != null)
            {
                string url = _site + "medicaments?ticket=" + _secretaire.getHashTicketMdp() + "&idFamille=" + id;
                string raw = _wb.DownloadString(url);
                var response = JsonConvert.DeserializeObject<ResponseMedicaments>(raw);
                _secretaire.ticket = response.ticket;
                listMedicaments = response.Medicaments;
            }
            else
            {
                string url = _site + "medicaments?ticket=" + _secretaire.getHashTicketMdp();
                string raw = _wb.DownloadString(url);
                var response = JsonConvert.DeserializeObject<ResponseMedicaments>(raw);
                _secretaire.ticket = response.ticket;
                listMedicaments = response.Medicaments;
            }

            return listMedicaments;
        }

        private List<Medicament> GetMedicaments(DateTime dateStart, DateTime dateEnd)
        {
            string url = _site + "medicaments?ticket=" + _secretaire.getHashTicketMdp() + "&dateDebut=" + dateStart.ToString("yyyy-MM-dd") + "&dateFin=" + dateEnd.ToString("yyyy-MM-dd");
            string raw = _wb.DownloadString(url);
            var response = JsonConvert.DeserializeObject<ResponseMedicaments>(raw);
            _secretaire.ticket = response.ticket;
            return response.Medicaments;
        }

        private void Quitter_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Rechercher_Click(object sender, RoutedEventArgs e)
        {
            DateStart.SelectedDate = null;
            DateEnd.SelectedDate = null;
            ListMedicaments.ItemsSource = null;
            var selectdeFamily = (Famille)ListFamille.SelectedItem;
            if (selectdeFamily != null)
            {
                ListMedicaments.ItemsSource = GetMedicaments(selectdeFamily.id.ToLower());
            }
            else
            {
                ListMedicaments.ItemsSource = GetMedicaments();
            }
            GenerateXml.IsEnabled = false;
        }

        private void VoirMedicament_Click(object sender, RoutedEventArgs e)
        {
            if (ListMedicaments.SelectedItem != null)
            {
                DetailsMedicament voir = new DetailsMedicament(_wb, _secretaire, _site, (Medicament)ListMedicaments.SelectedItem);
                voir.Show();
                this.Close();
            }
        }

        private void FilterByDate_Click(object sender, RoutedEventArgs e)
        {
            if(DateStart.SelectedDate != null && DateEnd.SelectedDate != null)
            {
                if (DateStart.SelectedDate < DateEnd.SelectedDate)
                {
                    ListMedicaments.ItemsSource = null;
                    _offerts = null;
                    _offerts = GetMedicaments((DateTime)DateStart.SelectedDate, (DateTime)DateEnd.SelectedDate);
                    
                    if(_offerts != null)
                    {
                        ListMedicaments.ItemsSource = _offerts;
                        GenerateXml.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Pas de médicament offert sur cette période.");
                    }
                }
                else
                {
                    MessageBox.Show("Les dates choisies ne sont pas correcte.");
                }
            }
            else
            {
                MessageBox.Show("La période doit être renseignée.");
            }
        }

        private void GenerateXml_Click(object sender, RoutedEventArgs e)
        {
            if (_offerts != null)
            {
                var generate = Services.GenerateXml(_offerts, "MedicamentsOfferts", DateStart.SelectedDate, DateEnd.SelectedDate);

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
}
