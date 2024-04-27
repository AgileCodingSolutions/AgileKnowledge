using System.Linq.Expressions;
using AgileKnowledge.Service.Domain.BaseEntity;
using AgileKnowledge.Service.Domain.Enities;
using AgileKnowledge.Service.Helper;

using Microsoft.EntityFrameworkCore;

namespace AgileKnowledge.Service.Domain
{
	public class KnowledgeDbContext : DbContext
	{
		private readonly JwtTokenProvider _jwtTokenProvider;
		public KnowledgeDbContext(DbContextOptions<KnowledgeDbContext> options, JwtTokenProvider jwtTokenProvider) : base(options)
		{
			_jwtTokenProvider = jwtTokenProvider;
		}

		public DbSet<User> Users { get; set; }
		public DbSet<FileStorage> FileStorages { get; set; }


		public DbSet<KnowledgeBase> KnowledgeBases { get; set; }
		public DbSet<KnowledgeBaseDetails> KnowledgeBaseCategories { get; set; }


		public DbSet<ChatApplication> ChatApplications { get; set; }
		public DbSet<ChatDialog> ChatDialogs { get; set; }
		public DbSet<ChatDialogHistory> ChatDialogHistorys { get; set; }



		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(entity =>
			{
				entity.ToTable("knowledge-users");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).ValueGeneratedOnAdd();

				entity.Property(e => e.Account).HasMaxLength(128).IsRequired();
				entity.Property(e => e.Name).HasMaxLength(128).IsRequired();
				entity.Property(e => e.Password).HasMaxLength(128).IsRequired();
				entity.Property(e => e.Salt).HasMaxLength(64).IsRequired();
				entity.Property(e => e.Avatar).HasMaxLength(2048).IsRequired(false);
				entity.Property(e => e.Email).HasMaxLength(64).IsRequired(false);
				entity.Property(e => e.Phone).IsRequired(false);
				entity.Property(e => e.IsDisable).IsRequired().HasDefaultValue(false);
				entity.Property(e => e.Role).IsRequired();;
			});

			modelBuilder.Entity<FileStorage>(entity =>
			{
				entity.ToTable("knowledge-file-storages");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).ValueGeneratedOnAdd();

				entity.Property(e => e.Name).HasMaxLength(128);
				entity.Property(e => e.FullName).HasMaxLength(2048);
				entity.Property(e => e.Path).HasMaxLength(2048);
				entity.Property(e => e.Size).IsRequired();
				entity.Property(e => e.IsCompression).IsRequired();
			});

			modelBuilder.Entity<KnowledgeBase>(entity =>
			{
				entity.ToTable("knowledge-knowledge-base");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).ValueGeneratedOnAdd();

				entity.Property(e => e.Name).HasMaxLength(128).IsRequired();
				entity.Property(e => e.Icon).HasMaxLength(2048).IsRequired();
				entity.Property(e => e.Model).IsRequired(false);
				entity.Property(e => e.EmbeddingModel).IsRequired(true);

				entity.HasMany<KnowledgeBaseDetails>(x => x.KnowledgeBaseDetails).WithOne(x=>x.KnowledgeBase).IsRequired(false);
			});

			modelBuilder.Entity<KnowledgeBaseDetails>(entity =>
			{
				entity.ToTable("knowledge-knowledge-base-details");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).ValueGeneratedOnAdd();

				entity.Property(e => e.State).IsRequired();
				entity.Property(e => e.MaxTokensPerParagraph).IsRequired();
				entity.Property(e => e.MaxTokensPerLine).IsRequired();
				entity.Property(e => e.OverlappingTokens).IsRequired();
				entity.Property(e => e.TrainingPattern).IsRequired();
				entity.Property(e => e.QAPromptTemplate).HasMaxLength(10240).IsRequired(false);
				entity.Property(e => e.DataCount).IsRequired();

				entity.HasOne<KnowledgeBase>(x => x.KnowledgeBase).WithMany(x => x.KnowledgeBaseDetails).IsRequired();
				entity.HasOne<FileStorage>(x => x.File).WithMany();
			});

			modelBuilder.Entity<ChatApplication>(entity =>
			{
				entity.ToTable("knowledge-chat-application");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).ValueGeneratedOnAdd();

				entity.Property(e => e.Name).HasMaxLength(128).IsRequired();
				entity.Property(e => e.Prompt).HasMaxLength(2048).IsRequired();
				entity.Property(e => e.ChatModel).HasMaxLength(128).IsRequired();
				entity.Property(e => e.Temperature).HasColumnType("decimal(3,2)").IsRequired();
				entity.Property(e => e.MaxResponseToken).IsRequired();
				entity.Property(e => e.Template).HasMaxLength(2048).IsRequired();
				entity.Property(e => e.Opener).HasMaxLength(2048).IsRequired();

				entity.HasMany<KnowledgeBase>(x => x.KnowledgeBases).WithMany();
				entity.HasMany<ChatDialog>(x => x.ChatDialogs).WithOne(x=>x.ChatApplication).IsRequired(false);

			});

			modelBuilder.Entity<ChatDialog>(entity =>
			{
				entity.ToTable("knowledge-chat-dialog");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).ValueGeneratedOnAdd();

				entity.Property(e => e.Name).HasMaxLength(128).IsRequired();
				entity.Property(e => e.Description).HasMaxLength(2048).IsRequired();

				entity.HasOne<ChatApplication>(x => x.ChatApplication).WithMany(x => x.ChatDialogs).IsRequired();
				entity.HasMany<ChatDialogHistory>(x => x.ChatDialogHistorys).WithOne(x=>x.ChatDialog).IsRequired(false);
			});

			modelBuilder.Entity<ChatDialogHistory>(entity =>
			{
				entity.ToTable("knowledge-chat-dialog-history");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).ValueGeneratedOnAdd();

				entity.Property(e => e.Content).HasMaxLength(128).IsRequired();
				entity.Property(e => e.TokenConsumption).IsRequired();
				entity.Property(e => e.Type).IsRequired();

				entity.HasOne<ChatDialog>(x => x.ChatDialog).WithMany(x => x.ChatDialogHistorys).IsRequired();
			});


			base.OnModelCreating(modelBuilder);

			
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				if (typeof(FullAuditedEntity).IsAssignableFrom(entityType.ClrType))
				{
					modelBuilder.Entity(entityType.ClrType).HasQueryFilter(GetIsNotDeletedFilterExpression(entityType.ClrType));
				}
			}
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			AuditedChange();
			return base.SaveChangesAsync(cancellationToken);
		}

		public void AuditedChange()
		{
			var addedEntities = ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Added && e.Entity is FullAuditedEntity);

			foreach (var entityEntry in addedEntities)
			{
				((FullAuditedEntity)entityEntry.Entity).CreationTime = DateTime.Now;
				((FullAuditedEntity)entityEntry.Entity).CreatorId = _jwtTokenProvider.GetUserId();
			}

			var modifiedEntities = ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Modified && e.Entity is FullAuditedEntity);

			foreach (var entityEntry in modifiedEntities)
			{
				((FullAuditedEntity)entityEntry.Entity).LastModificationTime = DateTime.Now;
				((FullAuditedEntity)entityEntry.Entity).LastModifierId = _jwtTokenProvider.GetUserId();
			}

			var deleteEntities = ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Deleted && e.Entity is FullAuditedEntity);

			foreach (var entityEntry in deleteEntities)
			{
				entityEntry.State = EntityState.Modified;

				((FullAuditedEntity)entityEntry.Entity).DeletionTime = DateTime.Now;
				((FullAuditedEntity)entityEntry.Entity).DeleterUserId = _jwtTokenProvider.GetUserId();
				((FullAuditedEntity)entityEntry.Entity).IsDeleted = true;
			}
		}
		private LambdaExpression GetIsNotDeletedFilterExpression(Type entityType)
		{
			var parameter = Expression.Parameter(entityType, "e");
			var property = Expression.Property(parameter, "IsDeleted");
			var condition = Expression.Equal(property, Expression.Constant(false));
			var lambda = Expression.Lambda(condition, parameter);
			return lambda;
		}

	}
}
