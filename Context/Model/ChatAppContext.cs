using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ChatApi.Context.Model
{
    public partial class ChatAppContext : IdentityDbContext<User>
    {
        public ChatAppContext()
        {
        }

        public ChatAppContext(DbContextOptions<ChatAppContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TbEstadoMensaje> TbEstadoMensajes { get; set; }
        public virtual DbSet<TbMensaje> TbMensajes { get; set; }
        public virtual DbSet<TbConexion> TbConexions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TbEstadoMensaje>(entity =>
            {
                entity.ToTable("TbEstadoMensaje");

                entity.Property(e => e.Codigo)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<TbMensaje>(entity =>
            {
                entity.ToTable("TbMensaje");

                entity.Property(e => e.Contenido).IsRequired();

                entity.Property(e => e.EmisorUserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ReceptorUserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.EmisorUser)
                    .WithMany(p => p.Enviados)
                    .HasForeignKey(d => d.EmisorUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TbMensaje_AspNetUsers");

                entity.HasOne(d => d.IdEstadoMensajeNavigation)
                    .WithMany(p => p.TbMensajes)
                    .HasForeignKey(d => d.IdEstadoMensaje)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TbMensaje_TbEstadoMensaje");

                entity.HasOne(d => d.ReceptorUser)
                    .WithMany(p => p.Recibidos)
                    .HasForeignKey(d => d.ReceptorUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TbMensaje_AspNetUsers1");
            });
            modelBuilder.Entity<TbConexion>(entity =>
            {
                entity.ToTable("TbConexion");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany( p => p.Conexiones)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TbConexion_AspNetUser");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }

    public class User: IdentityUser
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }

        public virtual ICollection<TbMensaje> Enviados { get; set; }

        public virtual ICollection<TbMensaje> Recibidos { get; set; }

        public virtual ICollection<TbConexion> Conexiones { get; set; }
    }
}
