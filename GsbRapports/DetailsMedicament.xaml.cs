using dllRapportVisites;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GsbRapports
{
    /// <summary>
    /// Interaction logic for DetailsMedicament.xaml
    /// </summary>
    public partial class DetailsMedicament : Window
    {
        private readonly WebClient _wb;
        private readonly Secretaire _secretaire;
        private readonly string _site;
        private readonly Medicament _medicament;

        public DetailsMedicament(WebClient wb, Secretaire secretaire, string site, Medicament medicament)
        {
            InitializeComponent();
            _wb = wb;
            _secretaire = secretaire;
            _site = site;
            _medicament = medicament;

            IdMedicament.Content = medicament.id;
            NomCommercial.Content = medicament.nomCommercial;
            IdFamille.Content = medicament.idFamille;
            composition.Text = _medicament.composition;
            contreIndications.Text = _medicament.contreIndications;
            effets.Text = _medicament.effets;
        }

        private void UpdateMedicament_Click(object sender, RoutedEventArgs e)
        {
            if (_medicament.id != null && composition.Text != string.Empty)
            {
                try
                {
                    string url = _site + "medicament/" + _medicament.id;
                    NameValueCollection parameters = new NameValueCollection();
                    parameters.Add("ticket", _secretaire.getHashTicketMdp());
                    parameters.Add("idMedicament", _medicament.id);
                    parameters.Add("effets", effets.Text);
                    parameters.Add("contreIndications", contreIndications.Text);
                    parameters.Add("composition", composition.Text);
                    byte[] tabByte = _wb.UploadValues(url, "POST", parameters);
                    string reponse1 = UnicodeEncoding.UTF8.GetString(tabByte);
                    _secretaire.ticket = reponse1;
                    MessageBox.Show($"Le médicament {_medicament.id} a bien été modifié.");
                    VoirMedicaments voir = new VoirMedicaments(_wb, _site, _secretaire);
                    voir.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("La composition du médicament doit être renseigné.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            VoirMedicaments voir = new VoirMedicaments(_wb, _site, _secretaire);
            voir.Show();
            this.Close();
        }
    }
}
