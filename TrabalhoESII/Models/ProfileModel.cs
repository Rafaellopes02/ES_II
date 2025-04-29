namespace TrabalhoESII.Models;

public class ProfileModel
{
    public int Id { get;}
    public string Nome { get; set; }
    public string Email { get; set; }
    public int Idade { get; set; }
    public string Telefone { get; set; }
    public string Nacionalidade { get; set; }
    public string NomeUtilizador { get; set; }
    public string Senha { get; set; }
    public int IdTipoUtilizador { get; set; }
}