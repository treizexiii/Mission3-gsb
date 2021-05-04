namespace dllRapportVisites
{
    public class Offre
    {
        public string id { get; set; }
        public string qte { get; set; }

        public Offre(string id, string qte)
        {
            this.id = id;
            this.qte = qte;
        }
    }
}
