using DapperExtensions.Mapper;
using Flunt.Notifications;
using Flunt.Validations;
using System.Data.SqlClient;

namespace ConsoleDapperTest;

public static class Program
{
    static void Main(string[] args)
    {
        string caminhoBanco = Path.Combine(Environment.CurrentDirectory,"dados", "TesteDapper.mdf");
        caminhoBanco = caminhoBanco.Replace("\\bin\\Debug\\net6.0", "");


        string connStr =
            $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={caminhoBanco};Integrated Security=True;Connect Timeout=30";

        
        
        
        using (SqlConnection conn = new SqlConnection(connStr))
        {

            for (int i = 0; i < 20; i++)
            {
                var categoria = new Categoria($"Categoria {Guid.NewGuid().ToString("N").Substring(0,7)}", "S");
                DapperExtensions.DapperExtensions.Insert<Categoria>(conn,categoria); // Verificar que este inser usado deve dapperExtension.Insert pois existe o Dapper.Contrib.Extension
            }
        }

        Console.WriteLine("Fim de processamento");
        Console.ReadKey();
    }
}


/// <summary>
/// Quando se criar o ClassMapper o próprio extension ignora as propriedades setadas ok. Porém vc terá de usar sempre o ClassMapper, pois ele precisa dele para saber os campos ok. 
/// </summary>
public class CategoriaMapper : ClassMapper<Categoria>
{
    public CategoriaMapper()
    {

        Table("Categoria");
        Map(c => c.Ativo).Column("ativo");
        Map(c => c.Nome).Column("nome");
        Map(c => c.Notifications).Ignore();
        Map(c => c.Valid).Ignore();
        Map(c => c.Invalid).Ignore();
    }
}

public class Categoria : Notifiable
{
    public Categoria()
    {

    }
    public Categoria(string nome, string ativo)
    {
        Nome = nome;
        Ativo = ativo;

        new Contract()
            .IsNullOrEmpty(nome, "Nome", "O nome deve ser informado.")
            .HasMinLen(nome, 3, "Nome", "A nome deve conter no mínimo 3 caracteres.")
            .HasMaxLen(nome, 100, "Nome", "A nome deve conter no máximo 100 caracteres.");
    }
    public string Nome { get; private set; }
    public string Ativo { get; private set; }
}