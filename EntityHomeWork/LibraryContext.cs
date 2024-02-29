using System;
using System.Collections.Generic;
using EntityHomeWork.DBModels;
using Microsoft.EntityFrameworkCore;

namespace EntityHomeWork;

public partial class LibraryContext : DbContext
{
    public LibraryContext()
    {
    }

    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookOnHand> BookOnHands { get; set; }

    public virtual DbSet<Librarian> Librarians { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    public virtual DbSet<TypeOfDocument> TypeOfDocuments { get; set; }

    public virtual DbSet<TypeOfPublishingCode> TypeOfPublishingCodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=DESKTOP-ACEF3H9;Database=Library;Integrated Security=True; Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Author__70DAFC149C2A6FED");

            entity.ToTable("Author");

            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.SecondName).HasMaxLength(100);

            entity.HasMany(d => d.Books).WithMany(p => p.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "AuthorBook",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AuthorBoo__BookI__47DBAE45"),
                    l => l.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__AuthorBoo__Autho__46E78A0C"),
                    j =>
                    {
                        j.HasKey("AuthorId", "BookId").HasName("PK__AuthorBo__1304F036F3B563A6");
                        j.ToTable("AuthorBook");
                        j.IndexerProperty<int>("AuthorId").HasColumnName("AuthorID");
                        j.IndexerProperty<int>("BookId").HasColumnName("BookID");
                    });
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Book__3DE0C227F6570ECC");

            entity.ToTable("Book");

            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.PublishingCodeTypeId).HasColumnName("PublishingCodeTypeID");
            entity.Property(e => e.Year).HasColumnType("datetime");

            entity.HasOne(d => d.PublishingCodeType).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublishingCodeTypeId)
                .HasConstraintName("FK__Book__Publishing__440B1D61");
        });

        modelBuilder.Entity<BookOnHand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookOnHa__3214EC27CD7EE43D");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.CheckoutDate).HasColumnType("datetime");
            entity.Property(e => e.DaysLeft).HasComputedColumnSql("(datediff(day,getdate(),[DueDate]))", false);
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.TakenBy)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Book).WithMany(p => p.BookOnHands)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__BookOnHan__BookI__5CD6CB2B");

            entity.HasOne(d => d.TakenByNavigation).WithMany(p => p.BookOnHands)
                .HasForeignKey(d => d.TakenBy)
                .HasConstraintName("FK__BookOnHan__Taken__5DCAEF64");
        });

        modelBuilder.Entity<Librarian>(entity =>
        {
            entity.HasKey(e => e.Login).HasName("PK__Libraria__5E55825A60CFCF4B");

            entity.ToTable("Librarian");

            entity.HasIndex(e => e.Email, "UQ__Libraria__A9D1053433D48A99").IsUnique();

            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)            
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.HasKey(e => e.Login).HasName("PK__Reader__5E55825A9A39801C");

            entity.ToTable("Reader");

            entity.HasIndex(e => e.Email, "UQ__Reader__A9D10534C2C6AFAB").IsUnique();

            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DocumentTypeId).HasColumnName("DocumentTypeID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.DocumentType).WithMany(p => p.Readers)
                .HasForeignKey(d => d.DocumentTypeId)
                .HasConstraintName("FK__Reader__Document__3F466844");
        });

        modelBuilder.Entity<TypeOfDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentTypeId).HasName("PK__TypeOfDo__DBA390C15C0B68C1");

            entity.ToTable("TypeOfDocument");

            entity.Property(e => e.DocumentTypeId).HasColumnName("DocumentTypeID");
            entity.Property(e => e.TypeName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TypeOfPublishingCode>(entity =>
        {
            entity.HasKey(e => e.PublishingCodeTypeId).HasName("PK__TypeOfPu__F8082F4CA620040A");

            entity.ToTable("TypeOfPublishingCode");

            entity.Property(e => e.PublishingCodeTypeId).HasColumnName("PublishingCodeTypeID");
            entity.Property(e => e.TypeName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
