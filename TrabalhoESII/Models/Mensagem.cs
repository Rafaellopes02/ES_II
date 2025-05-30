using System;
using System.ComponentModel.DataAnnotations;

namespace TrabalhoESII.Models
{
    public class Mensagem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Conteudo { get; set; }

        [Required]
        public int EventoId { get; set; }

        [Required]
        public int DestinatarioId { get; set; }

        public DateTime DataEnvio { get; set; } = DateTime.Now;
    }
}
