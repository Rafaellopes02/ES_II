using System.ComponentModel.DataAnnotations.Schema;

namespace TrabalhoESII.Models
{
    public class utilizadoresatividade
    {
        public int idutilizador { get; set; }
        public int idatividade { get; set; }
        public int idevento { get; set; }
    }
}
