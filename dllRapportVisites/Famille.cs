namespace dllRapportVisites
{
    public class Famille
    {
        public string id { get; set; }
        public string libelle { get; set; }
        public Famille(string id, string lib)
        {
            this.id = id;
            this.libelle = lib;
        }
    }
}
