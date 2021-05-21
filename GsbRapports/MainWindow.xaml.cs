using dllRapportVisites;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Windows;

namespace GsbRapports
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebClient wb;
        private string site;
        private string ticket;
        private Secretaire laSecretaire;
        public MainWindow()
        {

            InitializeComponent();
            this.wb = new WebClient();
            this.site = ConfigurationManager.AppSettings.Get("srvLocal");
            this.laSecretaire = new Secretaire();

            this.DckMenu.Visibility = Visibility.Hidden;
            this.imgLogo.Visibility = Visibility.Hidden;
            this.txtBonjour.Visibility = Visibility.Hidden;

        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string mdp = this.txtMdp.Password;
                string login = this.txtLogin.Text;
                string reponse; // la réponse retournée  par le serveur
                /* Création de la requête*/
                string url = this.site + "login?login=" + login;
                /*Appel à l'objet wb pour récupérer le résultat de la requête*/
                reponse = this.wb.DownloadString(url);
                /* récupération, après désérialisation et conversion*/
                this.ticket = (string)JsonConvert.DeserializeObject(reponse);
                if (this.ticket == null)
                {
                    MessageBox.Show("erreur de Login");
                    this.txtLogin.Text = "";
                }
                else
                {
                    this.laSecretaire.ticket = this.ticket;
                    this.laSecretaire.mdp = mdp;
                    /* on appelle la fonction de la classe secrétaire qui va hashe ticket+mdp */
                    string hash = this.laSecretaire.getHashTicketMdp();
                    /*On crée la requête*/
                    url = this.site + "connexion?login=" + login + "&mdp=" + hash;
                    /* On récupère la réponse du serveur de type json */
                    reponse = this.wb.DownloadString(url);
                    /*On transforme la réponse json en objet Secrétaire!!*/
                    Secretaire s = JsonConvert.DeserializeObject<Secretaire>(reponse);
                    if (s == null)
                        MessageBox.Show("erreur de mot de passe!!");
                    else
                    {
                        /* On renseigne le champ de la secrétaire pour la passer aux formulaires*/
                        this.laSecretaire.nom = s.nom;
                        this.laSecretaire.prenom = s.prenom;
                        this.laSecretaire.mdp = this.txtMdp.Password;
                        this.laSecretaire.ticket = s.ticket;
                        this.txtBonjour.Visibility = Visibility.Visible;
                        this.txtBonjour.Text = "Bonjour " + this.laSecretaire.prenom + " " + this.laSecretaire.nom;
                        this.DckMenu.Visibility = Visibility.Visible;
                        this.imgLogo.Visibility = Visibility.Visible;
                        this.stPanel.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                    MessageBox.Show(((HttpWebResponse)ex.Response).StatusCode.ToString());

            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //VoirFamillesWindow w = new VoirFamillesWindow();
            //w.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            majFamilleWindow w = new majFamilleWindow();
            w.Show();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            ajoutFamilleWindow w = new ajoutFamilleWindow();
            w.Show();
        }

        private void VoirVisite_Click(object sender, RoutedEventArgs e)
        {
            VoirVisiteWindow visite = new VoirVisiteWindow(wb, site, laSecretaire);
            visite.Show();
        }

        private void ModifierVisite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AjouterVisite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void VoirMedecins_Click(object sender, RoutedEventArgs e)
        {
            VoirMedecins medecins = new VoirMedecins(wb, site, laSecretaire);
            medecins.Show();
        }
    }
}
