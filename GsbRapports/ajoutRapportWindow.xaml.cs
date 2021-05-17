using dllRapportVisites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GsbRapports
{
    /// <summary>
    /// Logique d'interaction pour ajoutRapportWindow.xaml
    /// </summary>
    public partial class ajoutRapportWindow : Window
    {
        private Secretaire laSecretaire;
        private WebClient wb;
        private string site;
        private List<Offre> offres = new List<Offre>();
        private Visiteur leVisiteur;
        private Medecin leMedecin;

        public ajoutRapportWindow(
            WebClient wb,
            Secretaire s,
            string site,
            Visiteur leVisiteur,
            Medecin leMedecin)
        {
            InitializeComponent();
            this.laSecretaire = s;
            this.wb = wb;
            this.site = site;
            this.leVisiteur = leVisiteur;
            this.leMedecin = leMedecin;

            this.nomVisiteur.Content = leVisiteur.nom;
           

            //Liste des familles de médicaments
            string url1 = this.site + "familles?ticket=" + this.laSecretaire.getHashTicketMdp();
            string reponse2 = this.wb.DownloadString(url1);
            dynamic d2 = JsonConvert.DeserializeObject(reponse2);
            string familles = d2.familles.ToString();
            string ticket2 = d2.ticket;
            List<Famille> f2 = JsonConvert.DeserializeObject<List<Famille>>(familles);
            this.lstMedicaments.ItemsSource = f2;
            this.lstMedicaments.DisplayMemberPath = "libelle";
            this.laSecretaire.ticket = ticket2;

            //Liste Qte medicaments
            for (int i = 1; i < 15; i++)
            {
                this.lstQte.Items.Add(i);
            }
        }

      


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Famille famille = (Famille)this.lstMedicaments.SelectedItem;
            string id = famille.id.ToString();
            string url = this.site + "medicaments?ticket=" + this.laSecretaire.getHashTicketMdp() + "&idFamille=" + id;
            string reponse = this.wb.DownloadString(url);
            dynamic d = JsonConvert.DeserializeObject(reponse);
            string medicament = d.medicaments.ToString();
            string ticket = d.ticket;
            List<Medicament> f = JsonConvert.DeserializeObject<List<Medicament>>(medicament);
            this.lstNomMedic.ItemsSource = f;
            this.lstNomMedic.DisplayMemberPath = "nomCommercial";
            this.laSecretaire.ticket = ticket;
            this.lstNomMedic.Focus();
        }

        private void buttonAjoutMedic_Click(object sender, RoutedEventArgs e)
        {

            Medicament medicament = (Medicament)this.lstNomMedic.SelectedItem;
            string idMedic = medicament.id.ToString();
            string qte = this.lstQte.SelectedValue.ToString();
            Offre offre = new Offre(idMedic, qte);
            offres.Add(offre);
            this.dtgRecap.Items.Add(offre);
        }

        private void buttonSupMedic_Click(object sender, RoutedEventArgs e)
        {
            this.dtgRecap.Items.Clear();
        }

       

        private void Button_ValiderRapport(object sender, RoutedEventArgs e)
        {
            string ticket = this.laSecretaire.getHashTicketMdp();
            
            string visiteur = this.leVisiteur.id.ToString();

            string motif = this.motif.Text;

            string bilan = this.bilan.Text;

            string date = ((DateTime)this.date.SelectedDate).ToString("yyyy-MM-dd");
           
            string medecin = leMedecin.id.ToString();

            string url = this.site + "rapports";
            NameValueCollection parametres = new NameValueCollection();
            //Dictionary<string, string> parametres = new Dictionary<string, string>();
            parametres.Add("ticket", ticket);
            parametres.Add("motif", motif);
            parametres.Add("bilan", bilan);
            parametres.Add("date", date);
            parametres.Add("idMedecin", medecin);
            parametres.Add("idVisiteur", visiteur);
            NameValueCollection parametresMedicament = new NameValueCollection();
            foreach (var offre in offres)
            {
                parametresMedicament.Add("medicaments[" + offre.id + "]", offre.qte);

            }
            parametres.Add(parametresMedicament);



            byte[] tabByte = wb.UploadValues(url, "POST", parametres);
            string reponse1 = UnicodeEncoding.UTF8.GetString(tabByte);
        }

        

       


    }
}
