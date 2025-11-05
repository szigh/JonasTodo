using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JonasTodo.Models;

public partial class LearningDbContext : DbContext
{
    private readonly IOptions<DALSettings> _dalSettings;

    public LearningDbContext(IOptions<DALSettings> dalSettings)
    {
        _dalSettings = dalSettings;
    }

    public LearningDbContext(IOptions<DALSettings> dalSettings, DbContextOptions<LearningDbContext> options)
        : base(options)
    {
        _dalSettings = dalSettings;
    }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<Subtopic> Subtopics { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(_dalSettings.Value.ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Subtopic>(entity =>
        {
            entity.Property(e => e.Description).IsFixedLength();
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.Property(e => e.Description).IsFixedLength();
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
