using System.ComponentModel.DataAnnotations.Schema;

namespace TrabalhoESII.Models
{
    public class atividade
    {
        public int idatividade { get; set; }
        public string nome { get; set; }
        public int quantidademaxima { get; set; }
        public DateTime data { get; set; }
        public TimeSpan? hora { get; set; }
        public int idevento { get; set; }
    }
}
