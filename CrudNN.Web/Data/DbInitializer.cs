using CrudNN.Web.Data;
using CrudNN.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudNN.Web.Data;

public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context)
    {
        try
        {
            // Garantir que o banco está pronto
            await context.Database.EnsureCreatedAsync();

            // Verificar se já existe algum admin
            var adminExistente = await context.Admins.FirstOrDefaultAsync(a => a.Username == "admin");
            
            if (adminExistente != null)
            {
                // Atualizar hash da senha se necessário (para compatibilidade)
                if (adminExistente.PasswordHash.Length < 50) // Hash BCrypt tem ~60 caracteres
                {
                    adminExistente.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
                    await context.SaveChangesAsync();
                    Console.WriteLine("✅ Senha do admin atualizada!");
                }
                return; // Admin já existe
            }

            // Criar admin padrão
            var adminPadrao = new Admin
            {
                Username = "admin",
                NomeCompleto = "Administrador do Sistema",
                Email = "admin@plataformaead.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            context.Admins.Add(adminPadrao);
            await context.SaveChangesAsync();

            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("✅ Admin padrão criado com sucesso!");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("   Usuário: admin");
            Console.WriteLine("   Senha: admin123");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("   ⚠️  IMPORTANTE: Altere a senha após o primeiro login!");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao inicializar admin: {ex.Message}");
        }
    }
}
