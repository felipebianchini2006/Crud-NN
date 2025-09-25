using CrudNN.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CrudNN.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Matricula>().HasKey(m => new { m.AlunoId, m.CursoId });

        builder.Entity<Matricula>()
            .HasOne(m => m.Aluno)
            .WithMany(a => a.Matriculas)
            .HasForeignKey(m => m.AlunoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Matricula>()
            .HasOne(m => m.Curso)
            .WithMany(c => c.Matriculas)
            .HasForeignKey(m => m.CursoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Curso>()
            .Property(c => c.PrecoBase)
            .HasPrecision(18, 2);

        builder.Entity<Matricula>()
            .Property(m => m.PrecoPago)
            .HasPrecision(18, 2);

        builder.Entity<Matricula>()
            .Property(m => m.NotaFinal)
            .HasPrecision(4, 2);

        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        builder.Entity<Matricula>()
            .Property(m => m.Data)
            .HasColumnType("timestamp with time zone")
            .HasConversion(utcConverter);
    }
}
