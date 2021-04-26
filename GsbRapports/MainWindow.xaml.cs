using System.Configuration;
using System.Net;
using System.Windows;
using dllRapportVisites;
using Newtonsoft.Json;

namespace GsbRapports
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WebClient _wb;
        private readonly string _site;
        private string _ticket;
        private readonly Secretaire _laSecretaire;

        public MainWindow()
        {
            InitializeComponent();
            _wb = new WebClient();
            _site = ConfigurationManager.AppSettings.Get("srvLocal");
            _laSecretaire = new Secretaire();

            DckMenu.Visibility = Visibility.Hidden;
            imgLogo.Visibility = Visibility.Hidden;
            txtBonjour.Visibility = Visibility.Hidden;
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string mdp = txtMdp.Password;
                string login = txtLogin.Text;
                string reponse; // la réponse retournée  par le serveur

                /* Création de la requête*/
                string url = _site + "login?login=" + login;

                /*Appel à l'objet wb pour récupérer le résultat de la requête*/
                reponse = _wb.DownloadString(url);

                /* récupération, après désérialisation et conversion*/
                _ticket = (string)JsonConvert.DeserializeObject(reponse);

                if (_ticket == null)
                {
                    MessageBox.Show("erreur de Login");
                    txtLogin.Text = "";
                }
                else
                {
                    _laSecretaire.ticket = _ticket;
                    _laSecretaire.mdp = mdp;

                    /* on appelle la fonction de la classe secrétaire qui va hashe ticket+mdp */
                    string hash = _laSecretaire.getHashTicketMdp();

                    /*On crée la requête*/
                    url = _site + "connexion?login=" + login + "&mdp=" + hash;

                    /* On récupère la réponse du serveur de type json */
                    reponse = _wb.DownloadString(url);

                    /*On transforme la réponse json en objet Secrétaire!!*/
                    Secretaire s = JsonConvert.DeserializeObject<Secretaire>(reponse);

                    if (s == null)
                        MessageBox.Show("erreur de mot de passe!!");
                    else
                    {
                        /* On renseigne le champ de la secrétaire pour la passer aux formulaires*/
                        _laSecretaire.nom = s.nom;
                        _laSecretaire.prenom = s.prenom;
                        _laSecretaire.mdp = txtMdp.Password;
                        _laSecretaire.ticket = s.ticket;
                        txtBonjour.Visibility = Visibility.Visible;
                        txtBonjour.Text = "Bonjour " + _laSecretaire.prenom + " " + _laSecretaire.nom;
                        DckMenu.Visibility = Visibility.Visible;
                        imgLogo.Visibility = Visibility.Visible;
                        stPanel.Visibility = Visibility.Hidden;
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
            VoirFamillesWindow w = new VoirFamillesWindow(_wb, _laSecretaire, _site);
            w.Show();
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
    }
}
