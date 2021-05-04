using dllRapportVisites;
using Newtonsoft.Json;
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
        public ajoutRapportWindow(WebClient wb, Secretaire s, string site)
        {
            InitializeComponent();
            this.laSecretaire = s;
            this.wb = wb;
            this.site = site;
            // liste des visiteurs par nom
            string url = this.site + "visiteurs?ticket=" + this.laSecretaire.getHashTicketMdp();
            string reponse = this.wb.DownloadString(url);
            dynamic visiteursObjects = JsonConvert.DeserializeObject(reponse);
            string visiteursConvertToString = visiteursObjects.visiteurs.ToString();
            string ticket = visiteursObjects.ticket;
            List<Visiteur> f = JsonConvert.DeserializeObject<List<Visiteur>>(visiteursConvertToString);
            this.lstVisiteurs.ItemsSource = f;
            this.lstVisiteurs.DisplayMemberPath = "nom";
            this.laSecretaire.ticket = ticket;


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
            for (int i = 0; i < 15; i++)
            {
                this.lstQte.Items.Add(i);
            }




        }

        //bouton pour generer la liste de medecins
        private void buttonRechercher_Click(object sender, RoutedEventArgs e)
        {
            string substring = this.saisieMedecins.Text.Substring(0, 1);
            string url = this.site + "medecins?ticket=" + this.laSecretaire.getHashTicketMdp() + "&nom=" + substring;
            string reponse1 = this.wb.DownloadString(url);
            dynamic d1 = JsonConvert.DeserializeObject(reponse1);
            string medecins = d1.medecins.ToString();
            string ticket1 = d1.ticket;
            List<Medecin> f1 = JsonConvert.DeserializeObject<List<Medecin>>(medecins);
            this.lstMedecins.ItemsSource = f1;
            this.lstMedecins.DisplayMemberPath = "name";
            this.laSecretaire.ticket = ticket1;

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


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Medecin medecin = (Medecin)this.lstMedecins.SelectedItem;


        }

        private void Button_ValiderRapport(object sender, RoutedEventArgs e)
        {
            string ticket = this.laSecretaire.getHashTicketMdp();
            Visiteur leVisiteur = (Visiteur)this.lstVisiteurs.SelectedItem;
            string visiteur = leVisiteur.id.ToString();

            string motif = this.motif.Text;

            string bilan = this.bilan.Text;

            string date = this.date.SelectedDate.ToString();

            Medecin lemedecin = (Medecin)this.lstMedecins.SelectedItem;
            string medecin = lemedecin.id.ToString();






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

        private void lstMedecins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Medecin medecin = (Medecin)this.lstMedecins.SelectedItem;

        }


    }
}
