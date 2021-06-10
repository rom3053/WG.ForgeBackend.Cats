using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;
using WebApp7.Cats.DALL.Helper;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApp7.Cats.DALL
{
    public partial class wg_forge_dbContext : DbContext
    {
        public const string connString = "Host=localhost;Port=5432;Database=wg_forge_db;Username=wg_forge;Password=a42";
        static wg_forge_dbContext()
                =>NpgsqlConnection.GlobalTypeMapper.MapEnum<CatColor>();
        public wg_forge_dbContext()
        {
        }

        public wg_forge_dbContext(DbContextOptions<wg_forge_dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CatColorsInfo> CatColorsInfo { get; set; }
        public virtual DbSet<Cats> Cats { get; set; }
        public virtual DbSet<CatsStat> CatsStat { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=wg_forge_db;Username=wg_forge;Password=a42")
                                .UseSnakeCaseNamingConvention();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<int[], string>(
                v => string.Join(";", v),
                v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => int.Parse(val)).ToArray());

            modelBuilder.HasPostgresEnum(null, "cat_color", new[] { "black", "white", "black & white", "red", "red & white", "red & black & white" });
            modelBuilder.Entity<CatColorsInfo>().HasIndex(u => u.Color).IsUnique();
            modelBuilder.Entity<CatColorsInfo>(entity =>
            {
                
                entity.HasKey(e => e.Color)
                    .HasName("cat_colors_info_color_key");
                    
                entity.ToTable("cat_colors_info");

                entity.Property(e => e.Count).HasColumnName("count");
                entity.Property(e => e.Color).HasColumnName("color");
            });

            modelBuilder.Entity<Cats>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("cats_pkey");

                entity.ToTable("cats");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.TailLength).HasColumnName("tail_length");

                entity.Property(e => e.WhiskersLength).HasColumnName("whiskers_length");
                entity.Property(e => e.Color)
                    .HasColumnName("color");
                    //.HasConversion(
                    //    v=>ConvertEnumToString.ConvertToString(v),
                    //    v=>ConvertEnumToString.ConvertToEnum(v));
            });

            modelBuilder.Entity<CatsStat>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("cats_stat");

                entity.Property(e => e.TailLengthMean)
                    .HasColumnName("tail_length_mean")
                    .HasColumnType("numeric");

                entity.Property(e => e.TailLengthMedian)
                    .HasColumnName("tail_length_median")
                    .HasColumnType("numeric");

                entity.Property(e => e.TailLengthMode)
                    .HasColumnName("tail_length_mode")
                    .HasConversion(converter); ;

                entity.Property(e => e.WhiskersLengthMean)
                    .HasColumnName("whiskers_length_mean")
                    .HasColumnType("numeric");

                entity.Property(e => e.WhiskersLengthMedian)
                    .HasColumnName("whiskers_length_median")
                    .HasColumnType("numeric");

                entity.Property(e => e.WhiskersLengthMode)
                    .HasColumnName("whiskers_length_mode")
                    .HasConversion(converter);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
