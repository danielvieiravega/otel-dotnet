namespace WebAPI;

public class Pessoa
{
    public Guid Id{ get; private set; }
    public string Nome { get; private set; }

    public Pessoa(string nome)
    {
        Id = Guid.NewGuid();
        Nome = nome;
    }
}
