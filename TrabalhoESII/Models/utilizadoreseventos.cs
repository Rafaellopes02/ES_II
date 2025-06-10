using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrabalhoESII.Models
{
    public class utilizadoreseventos
    {
        public int idutilizador { get; set; }
        public int idevento { get; set; }
        public int? idingresso { get; set; }
        public string estado { get; set; }
        public DateTime? datainscricao { get; set; }

        [ForeignKey("idutilizador")]
        public utilizadores utilizador { get; set; }

        [ForeignKey("idevento")]
        public eventos evento { get; set; }
    }
}

