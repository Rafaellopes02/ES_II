using System.Runtime.InteropServices.JavaScript;

namespace TrabalhoESII.Models
{
    public class EventosRegisterModel
    {
        public string nome { get; set; }
        public DateTime data { get; set; }
        public TimeSpan hora { get; set; }
        public string local { get; set; }
        public string descricao { get; set; }
        public int capacidade { get; set; }
        public int idCategoria { get; set; }

       

    }
}