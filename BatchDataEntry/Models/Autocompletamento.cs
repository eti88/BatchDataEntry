
namespace BatchDataEntry.Models
{
    public class Autocompletamento
    {
        public int Id { get; set; }
        public int Colonna { get; set; }
        public string Valore { get; set; }

        public Autocompletamento() {
            this.Id = 0;
            this.Colonna = 0;
            this.Valore = string.Empty;
        }

        public Autocompletamento(int Col, string val)
        {
            this.Id = 0;
            this.Colonna = Col;
            this.Valore = val;
        }
    }
}
