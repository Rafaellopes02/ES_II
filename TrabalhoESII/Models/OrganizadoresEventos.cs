using System.ComponentModel.DataAnnotations.Schema;

namespace TrabalhoESII.Models
{
    public class OrganizadoresEventos
    {
        public int idutilizador { get; set; }
        public utilizadores utilizadores { get; set; }

        public int idevento { get; set; }
        public eventos eventos { get; set; }
    }
}