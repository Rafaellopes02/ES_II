using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class utilizadoreseventos
{
    [Key]
    public int idutilizador { get; set; }
    public int idevento { get; set; }
    public int idingresso { get; set; }
    public string estado { get; set; }
    public DateTime datainscricao { get; set; }
}
