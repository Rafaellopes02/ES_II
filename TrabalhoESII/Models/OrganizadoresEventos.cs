using System.ComponentModel.DataAnnotations.Schema;

namespace TrabalhoESII.Models
{
    public class organizadoreseventos
    {
        public int idutilizador { get; set; }
        public utilizadores utilizadores { get; set; }

        public int idevento { get; set; }
        public eventos eventos { get; set; }
        public bool eorganizador { get; set; }
    }
}