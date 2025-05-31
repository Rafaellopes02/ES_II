using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrabalhoESII.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<utilizadoreseventos> ingressosDTO { get; set; }
        public DbSet<utilizadoreseventos> utilizadoreseventos { get; set; }
        public DbSet<tiposutilizadores> tiposutilizadores { get; set; }
        public DbSet<tipospagamentos> tipospagamentos { get; set; }
        public DbSet<categorias> categorias { get; set; }
        public DbSet<tiposingressos> tiposingressos { get; set; }
        public DbSet<estadospagamentos> estadospagamentos { get; set; }
        public DbSet<utilizadores> utilizadores { get; set; }
        public DbSet<eventos> eventos { get; set; }
        public DbSet<feedbacks> feedbacks { get; set; }
        public DbSet<ingressos> ingressos { get; set; }
        public DbSet<pagamentos> pagamentos { get; set; }
        public DbSet<organizadoreseventos> organizadoreseventos { get; set; }
        public DbSet<atividades> atividades { get; set; }
        public DbSet<utilizadoresatividades> utilizadoresatividades { get; set; }
        public DbSet<notificacoes> notificacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<organizadoreseventos>()
                .ToTable("organizadoreseventos")
                .HasKey(oe => new { oe.idutilizador, oe.idevento });

            modelBuilder.Entity<organizadoreseventos>()
                .HasOne(oe => oe.utilizadores)
                .WithMany()
                .HasForeignKey(oe => oe.idutilizador)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<organizadoreseventos>()
                .HasOne(oe => oe.eventos)
                .WithMany()
                .HasForeignKey(oe => oe.idevento)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<utilizadoresatividades>()
                .HasKey(ua => new { ua.idutilizador, ua.idatividade });
            
            modelBuilder.Entity<atividades>()
                .HasOne(a => a.eventos)
                .WithMany()
                .HasForeignKey(a => a.idevento)
                .OnDelete(DeleteBehavior.Cascade);

                 modelBuilder.Entity<utilizadoreseventos>()
                .HasKey(u => new { u.idutilizador, u.idevento, u.idingresso });

            base.OnModelCreating(modelBuilder);
        }
    }

    public class tiposutilizadores
    {
        [Key]
        public int idtipoutilizador { get; set; }
        [Required, StringLength(50)]
        public string nome { get; set; }
    }

    public class tipospagamentos
    {
        [Key]
        public int idtipopagamento { get; set; }
        [Required, StringLength(50)]
        public string nome { get; set; }
    }

    public class categorias
    {
        [Key]
        public int idcategoria { get; set; }
        [Required, StringLength(50)]
        public string nome { get; set; }
    }

    public class tiposingressos
    {
        [Key]
        public int idtipoingresso { get; set; }
        [Required, StringLength(50)]
        public string nome { get; set; }
    }

    public class estadospagamentos
    {
        [Key]
        public int idestado { get; set; }
        [Required, StringLength(255)]
        public string descricao { get; set; }
    }

    public class utilizadores
    {
        [Key]
        public int idutilizador { get; set; }
        [Required, StringLength(100)]
        public string nome { get; set; }
        [Required]
        public int idade { get; set; }
        [Required, StringLength(100), EmailAddress]
        public string email { get; set; }
        [Required, StringLength(15)]
        public string telefone { get; set; }
        [Required, StringLength(50)]
        public string nacionalidade { get; set; }
        [Required, StringLength(50)]
        public string nomeutilizador { get; set; }
        [Required, StringLength(100)]
        public string senha { get; set; }

        [Required]
        public int idtipoutilizador { get; set; }
        [ForeignKey("idtipoutilizador")]
        public tiposutilizadores tiposutilizadores { get; set; }
    }

    public class eventos
    {
        [Key]
        public int idevento { get; set; }
        [Required, StringLength(100)]
        public string nome { get; set; }
        [Required]
        public DateTime data { get; set; }
        [Required]
        public TimeSpan hora { get; set; }
        [StringLength(100)]
        public string local { get; set; }
        [StringLength(255)]
        public string descricao { get; set; }
        [Required]
        public int capacidade { get; set; }

        [Required]
        public int idcategoria { get; set; }
        [ForeignKey("idcategoria")]
        public categorias categoria { get; set; }
    }

    public class feedbacks
    {
        [Key]
        public int idfeedback { get; set; }
        [Required, Range(1, 5)]
        public int avaliacao { get; set; }
        [StringLength(255)]
        public string comentario { get; set; }
        [Required]
        public DateTime data { get; set; }

        [Required]
        public int idutilizador { get; set; }
        [ForeignKey("idutilizador")]
        public utilizadores utilizadores { get; set; }

        [Required]
        public int idevento { get; set; }
        [ForeignKey("idevento")]
        public eventos eventos { get; set; }
    }

    public class ingressos
    {
        [Key]
        public int idingresso { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal preco { get; set; }

        [Required]
        public int quantidadeatual { get; set; }

        [Required]
        public int quantidadedefinida { get; set; }

        [Required]
        public int idevento { get; set; }

        [ForeignKey("idevento")]
        public eventos eventos { get; set; }

        [Required]
        public int idtipoingresso { get; set; }

        [ForeignKey("idtipoingresso")]
        public tiposingressos tiposingressos { get; set; }

        [Required]
        public string nomeingresso { get; set; }
    }
    
    public class pagamentos
    {
        [Key]
        public int idpagamento { get; set; }
        [Required]
        public DateTime datahora { get; set; }
        [StringLength(255)]
        public string descricao { get; set; }

        [Required]
        public int idtipopagamento { get; set; }
        [ForeignKey("idtipopagamento")]
        public tipospagamentos tipospagamentos { get; set; }

        [Required]
        public int idutilizador { get; set; }
        [ForeignKey("idutilizador")]
        public utilizadores utilizadores { get; set; }

        [Required]
        public int idingresso { get; set; }
        [ForeignKey("idingresso")]
        public ingressos ingressos { get; set; }

        [Required]
        public int idestado { get; set; }
        [ForeignKey("idestado")]
        public estadospagamentos estadospagamentos { get; set; }
    }
    

    public class atividades
    {
        [Key]
        public int idatividade { get; set; }
        [Required, StringLength(100)]
        public string nome { get; set; }
        public int? quantidademaxima { get; set; }
        [Required]
        public DateTime data { get; set; }
        [Required]
        public TimeSpan hora { get; set; }

        [Required]
        public int idevento { get; set; }
        [ForeignKey("idevento")]
        public eventos eventos { get; set; }
    }

    public class utilizadoresatividades
    {
        public int idutilizador { get; set; }
        [ForeignKey("idutilizador")]
        public utilizadores utilizadores { get; set; }

        public int idatividade { get; set; }
        [ForeignKey("idatividade")]
        public atividades atividades { get; set; }
        
        public int idevento { get; set; }
        [ForeignKey("idevento")]
        public eventos eventos { get; set; }
    }
    
    public class notificacoes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idnotificacao { get; set; }
    
        [Required]
        public int idutilizador { get; set; }
    
        [Required, MaxLength]
        public string mensagem { get; set; }
    
        [ForeignKey("idutilizador")]
        public virtual utilizadores utilizador { get; set; }
    }
}
