using dllRapportVisites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace GsbRapports
{
    /// <summary>
    /// Interaction logic for VoirVisiteWindow.xaml
    /// </summary>
    public partial class VoirVisiteWindow : Window
    {
        private const string NameFileXml = "Rapports";
        private readonly Secretaire secretaire;
        private WebClient wb { get; }
        private string site { get; }
        private bool hasDownloadRapport { get; set; }
        public Medecin selectedMedecin { get; set; }
        public Visiteur selectedVisiteur { get; set; }
        public List<Medecin> medecins { get; set; }
        public DateTime? startDateSelect { get; set; }
        public DateTime? endDateSelect { get; set; }
        public List<Visiteur> lesVisiteurs { get; set; }
        public List<Rapport> lesRapports { get; set; }


        public VoirVisiteWindow(WebClient wb, string site, Secretaire secretaire)
        {
            InitializeComponent();
            this.wb = wb;
            this.site = site;
            this.secretaire = secretaire;
            medecins = new List<Medecin>();
            lesVisiteurs = new List<Visiteur>();
            startDateSelect = null;
            endDateSelect = null;
            lesRapports = new List<Rapport>();
            hasDownloadRapport = false;
            getVisiteurs();
        }

        private void searchMedecins_Click(object sender, RoutedEventArgs e)
        {
            string name = nameMedecin.Text.ToLower();
            getMedecins(name);
        }

        private void getMedecins(string name)
        {
            string hash = this.secretaire.getHashTicketMdp();
            string url = this.site + "medecins?ticket=" + hash + "&nom=" + name;
            string response = this.wb.DownloadString(url);
            var receivedMedecins = JsonConvert.DeserializeObject<LesMedecins>(response);

            setNewTicketToSecretary(receivedMedecins.ticket);
            medecins.Clear();
            medecinsDataList.ItemsSource = null;
            displayMedecinName.Text = string.Empty;
            selectedMedecin = null;

            foreach (Medecin medecin in receivedMedecins.Medecins)
            {
                medecins.Add(medecin);
            }
            medecinsDataList.ItemsSource = medecins;
            nameMedecin.Text = string.Empty;
        }

        private void medecinsDataList_Selected(object sender, SelectedCellsChangedEventArgs e)
        {
            selectedMedecin = medecinsDataList.SelectedItem as Medecin;
            displayMedecinName.Text = string.Empty;
            if (selectedMedecin != null)
            {
                displaySelectedMedecin();
            }
        }

        private void displaySelectedMedecin()
        {
            displayMedecinName.Text = "Medecin: " + selectedMedecin.prenom + " " + selectedMedecin.nom;
        }

        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDateSelect = (DateTime)(sender as DatePicker).SelectedDate;
        }

        private void endDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDateSelect = (DateTime)(sender as DatePicker).SelectedDate;
        }

        private void previewDataToXml_Click(object sender, RoutedEventArgs e)
        {
            lesRapports.Clear();
            rapportsDataList.ItemsSource = null;
            bool asJustOneDateSelected = startDateSelect != null && endDateSelect == null || startDateSelect == null && endDateSelect != null;
            if (asJustOneDateSelected)
            {
                MessageBox.Show("Merci de selectionner les deux dates");
            }
            else
            {
                RapportToDisplay();
            }
        }

        private void RapportToDisplay()
        {
            hasDownloadRapport = true;
            List<Rapport> listRapports;
            rapportsDataList.ItemsSource = null;
            bool asDatesSelected = startDateSelect != null && endDateSelect != null;
            if (selectedMedecin != null)
            {
                listRapports = getRapportByMedecinId();

                if (listRapports.Count > 0 && selectedVisiteur != null && asDatesSelected == false)
                {
                    listRapports.RemoveAll(rapport => rapport.nomVisiteur != selectedVisiteur.nom);
                    addElementToListRapport(listRapports);
                }
                else if (listRapports.Count > 0 && selectedVisiteur != null && asDatesSelected == true)
                {

                    listRapports.RemoveAll(rapport => rapport.nomVisiteur != selectedVisiteur.nom);
                    listRapports.RemoveAll(rapport => startDateSelect > rapport.date || endDateSelect < rapport.date);

                    addElementToListRapport(listRapports);
                }
                else if (listRapports.Count > 0 && selectedVisiteur == null && asDatesSelected == true)
                {
                    listRapports.RemoveAll(rapport => startDateSelect > rapport.date || endDateSelect < rapport.date);
                    addElementToListRapport(listRapports);
                }
                else if (listRapports.Count > 0 && selectedVisiteur == null && asDatesSelected == false)
                {
                    listRapports.RemoveAll(rapport => startDateSelect > rapport.date || endDateSelect < rapport.date);
                    addElementToListRapport(listRapports);
                }

                if (listRapports.Count == 0)
                {
                    MessageBox.Show("Aucun rapport lié à ce medecin");
                    ClearInputVisiteur();
                }
            }
            else
            {
                if (selectedVisiteur != null)
                {
                    if (asDatesSelected == false)
                    {
                        listRapports = getRapportByVisiteurId();
                        if (listRapports.Count > 0)
                        {
                            addElementToListRapport(listRapports);
                        }
                        if (listRapports.Count == 0)
                        {
                            MessageBox.Show("Aucun rapport lié à ce visiteur");
                            ClearInputVisiteur();
                        }
                    }
                    else
                    {
                        listRapports = getRapportByDates();
                        listRapports.RemoveAll(rapport => startDateSelect > rapport.date || endDateSelect < rapport.date);
                        if (listRapports.Count > 0)
                        {
                            addElementToListRapport(listRapports);
                        }
                        if (listRapports.Count == 0)
                        {
                            MessageBox.Show("Aucun rapport lié à ce visiteur, avec ces dates");
                            ClearInputVisiteur();
                        }
                    }
                }
                else
                {
                    if (asDatesSelected == true)
                    {
                        listRapports = getRapportByDates();
                        listRapports.RemoveAll(rapport => startDateSelect > rapport.date || endDateSelect < rapport.date);
                        if (listRapports.Count > 0)
                        {
                            addElementToListRapport(listRapports);
                        }
                        if (listRapports.Count == 0)
                        {
                            MessageBox.Show("Aucun rapport lié à ce visiteur");
                            ClearInputVisiteur();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Aucune selection, la liste de rapport ne peut pas être obtenue");
                    }
                }

            }

            rapportsDataList.ItemsSource = lesRapports;
        }

        private void ClearInputVisiteur()
        {
            selectedVisiteur = null;
            comboLesVisiteurs.SelectedItem = null;
            rapportsDataList.ItemsSource = null;
        }

        private void addElementToListRapport(List<Rapport> listRapports)
        {
            foreach (var rapport in listRapports)
            {
                {
                    lesRapports.Add(rapport);
                }
            }
        }

        private List<Rapport> getRapportByVisiteurId()
        {
            List<Rapport> listRapportByVisiteurId = new List<Rapport>();
            string hash = secretaire.getHashTicketMdp();
            string url = site + "rapports?ticket=" + hash + "&idVisiteur=" + selectedVisiteur.id;
            string response = wb.DownloadString(url);
            var rapportsVisiteur = JsonConvert.DeserializeObject<LesRapports>(response);
            foreach (Rapport rapport in rapportsVisiteur.rapports)
            {
                listRapportByVisiteurId.Add(rapport);
            }
            setNewTicketToSecretary(rapportsVisiteur.ticket);
            return listRapportByVisiteurId;
        }

        private List<Rapport> getRapportByDates()
        {
            List<Rapport> listRapportByDates = new List<Rapport>();
            string hash = secretaire.getHashTicketMdp();
            string url = site + "rapports?ticket=" + hash + "&dateDebut=" + startDate + "&dateFin=" + endDate;
            string response = wb.DownloadString(url);
            var rapportsDates = JsonConvert.DeserializeObject<LesRapports>(response);
            foreach (Rapport rapport in rapportsDates.rapports)
            {
                listRapportByDates.Add(rapport);
            }
            setNewTicketToSecretary(rapportsDates.ticket);
            return listRapportByDates;
        }

        private List<Rapport> getRapportByMedecinId()
        {
            List<Rapport> listRapportByMedecinName = new List<Rapport>();
            string hash = secretaire.getHashTicketMdp();
            string url = site + "rapports?ticket=" + hash + "&idMedecin=" + selectedMedecin.id;
            string response = wb.DownloadString(url);
            var rapportsMedecin = JsonConvert.DeserializeObject<LesRapports>(response);
            foreach (Rapport rapport in rapportsMedecin.rapports)
            {
                listRapportByMedecinName.Add(rapport);
            }
            setNewTicketToSecretary(rapportsMedecin.ticket);
            return listRapportByMedecinName;
        }

        private void setNewTicketToSecretary(string newTicket)
        {
            secretaire.ticket = newTicket;
        }

        private void getVisiteurs()
        {
            lesVisiteurs.Clear();
            string hash = secretaire.getHashTicketMdp();
            string url = site + "visiteurs?ticket=" + hash;
            string response = wb.DownloadString(url);
            var visiteurs = JsonConvert.DeserializeObject<LesVisiteurs>(response);
            setNewTicketToSecretary(visiteurs.ticket);

            foreach (Visiteur visiteur in visiteurs.Visiteurs)
            {
                lesVisiteurs.Add(visiteur);
            }
            comboLesVisiteurs.ItemsSource = lesVisiteurs;
        }

        private void comboLesVisiteurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedVisiteur = (Visiteur)(sender as ComboBox).SelectedItem;
            if (hasDownloadRapport)
            {
                RapportToDisplay();
            }
        }

        private void clearVisiteur_Click(object sender, RoutedEventArgs e)
        {
            ClearInputVisiteur();
            if (hasDownloadRapport)
            {
                RapportToDisplay();
            }
        }

        private void resetFilter_Click(object sender, RoutedEventArgs e)
        {
            ClearInputVisiteur();
            ClearInputMedecin();
            ClearInputDates();

            ClearRapportsList();

        }

        private void ClearRapportsList()
        {
            lesRapports.Clear();
            rapportsDataList.ItemsSource = null;
        }

        private void ClearInputDates()
        {
            startDateSelect = null;
            endDateSelect = null;
        }

        private void ClearInputMedecin()
        {
            selectedMedecin = null;
            displayMedecinName.Text = string.Empty;
            medecinsDataList.ItemsSource = null;
        }

        private void generateXml_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";
            if (lesRapports.Count > 0)
            {
                try
                {
                    var xEle = new XElement(NameFileXml,
                                from rapport in lesRapports
                                select new XElement("Rapport",
                                               new XElement("Date", rapport.date),
                                               new XElement("NomMedecin", rapport.nomMedecin),
                                               new XElement("PrenomMedecin", rapport.prenomMedecin),
                                               new XElement("NomVisiteur", rapport.nomVisiteur),
                                               new XElement("PrenomVisiteur", rapport.prenomVisiteur),
                                               new XElement("Motif", rapport.motif)
                                           ));

                    xEle.Save(path + NameFileXml + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + ".xml");
                    MessageBox.Show($"Génération du fichier XML, {path}");

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Problème dans la création du fichier XML, {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Vous devez afficher une liste de rapport avant la génération d'un fichier xml");
            }
        }

        private void ShowRapport_Click(object sender, RoutedEventArgs e)
        {
            //on vérifie que le rapport est bien selectionné
            if (rapportsDataList.SelectedItem != null)
            {
                var rapport = (Rapport)rapportsDataList.SelectedItem;

                // On vérifie que le rapport contient bien les infos du visiteur sinon elles sont ajoutées
                if (rapport.idVisiteur == null && rapport.nomVisiteur == null && rapport.prenomVisiteur == null)
                {
                    var visiteur = (Visiteur)comboLesVisiteurs.SelectedItem;
                    rapport.idVisiteur = visiteur.id;
                    rapport.nomVisiteur = visiteur.nom;
                    rapport.prenomVisiteur = visiteur.prenom;
                }

                // On génére la vue détails
                DetailsRapport detailsRapport = new DetailsRapport(wb, secretaire, site, rapport);
                detailsRapport.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Vous devez selectionner un rapport");
            }
        }

        private void CreateRapport_Click(object sender, RoutedEventArgs e)
        {
            if (comboLesVisiteurs.SelectedItem != null && medecinsDataList.SelectedItem != null)
            {
                Visiteur leVisiteur = (Visiteur)comboLesVisiteurs.SelectedItem;
                Medecin leMedecin = (Medecin)medecinsDataList.SelectedItem;
                ajoutRapportWindow addRapport = new ajoutRapportWindow(wb, secretaire, site, leVisiteur, leMedecin);
                addRapport.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Vous devez selectionner un visiteur et un médecin pour continuer");
            }
        }
    }

    public class LesMedecins
    {
        public List<Medecin> Medecins { get; set; }
        public string ticket { get; set; }
    }
    public class LesRapports
    {
        public List<Rapport> rapports { get; set; }
        public string ticket { get; set; }
    }
    public class LesVisiteurs
    {
        public List<Visiteur> Visiteurs { get; set; }
        public string ticket { get; set; }
    }
}
