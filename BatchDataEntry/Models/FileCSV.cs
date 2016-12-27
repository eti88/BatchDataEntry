namespace BatchDataEntry.Models
{

    public class FileCSV
    {

        public long Id { get; set; }
        public string Path { get; set; }
        public char Separatore { get; set; }

        public virtual Modello Modello { get; set; }

        public FileCSV()
        {
        }

        public FileCSV(string p, char s)
        {
            this.Path = p;
            this.Separatore = s;
        }

        public FileCSV(long id, string p, char s)
        {
            this.Id = id;
            this.Path = p;
            this.Separatore = s;
        }
    }
}
