namespace WebAPI;

public static class Seeder
{
    public static void Seed(this PessoaContext pessoasContext)
    {
        if (pessoasContext.Pessoas.Any()) return;

        List<Pessoa> pessoas = new()
            {
                new Pessoa ("Ada Lovelace"),
                new Pessoa ("Grace Hopper"),
                new Pessoa ("Margaret Hamilton"),
                new Pessoa("Alan Turing"),
                new Pessoa("Jean Baptiste Joseph Fourier"),
                new Pessoa("Pierre-Simon Laplace")
            };

        pessoasContext.AddRange(pessoas);
        pessoasContext.SaveChanges();
    }
}
