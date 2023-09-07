using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Producer.Exceptions;
using System.Reflection;

namespace Producer
{
    public class ProducerContext : DbContext
    {
        private readonly Assembly _dbContextAssembly;

        public ProducerContext(DbContextOptions<ProducerContext> options)
            : base(options)
        {
            _dbContextAssembly = GetType().Assembly;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(_dbContextAssembly);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }

        public override int SaveChanges()
        {
            return InternalSaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return InternalSaveChanges(acceptAllChangesOnSuccess);
        }

        private int InternalSaveChanges(bool acceptAllChangesOnSuccess = true)
        {
            try
            {
                return base.SaveChanges(acceptAllChangesOnSuccess);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InfrastructureFailureException("Db update concurrency exception.");
            }
            catch (DbUpdateException)
            {
                throw new InfrastructureFailureException("Db update exception.");
            }
            catch (SqlException sqlException)
            {
                if (sqlException.InnerException is null)
                {
                    throw;
                }

                if (sqlException.InnerException is not SqlException sourceException)
                {
                    throw;
                }

                switch (sourceException.ErrorCode)
                {
                    case 11:
                        // Timeout
                        throw new InfrastructureFailureException("Sql database unavailable.");
                    case 1205:
                        // Deadlock
                        throw new InfrastructureFailureException("Deadlock occurred");
                    default:
                        throw;
                }
            }
        }

        // MassTransit uses this method when using the Outbox
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return await InternalSaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await InternalSaveChangesAsync(true, cancellationToken);
        }

        private async Task<int> InternalSaveChangesAsync(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InfrastructureFailureException("Db update concurrency exception.");
            }
            catch (DbUpdateException)
            {
                throw new InfrastructureFailureException("Db update exception.");
            }
            catch (SqlException sqlException)
            {
                if (sqlException.InnerException is null)
                {
                    throw;
                }

                if (sqlException.InnerException is not SqlException sourceException)
                {
                    throw;
                }

                switch (sourceException.Number)
                {
                    case 11:
                        // Timeout
                        throw new InfrastructureFailureException("Sql database unavailable.");
                    case 1205:
                        // Deadlock
                        throw new InfrastructureFailureException("Deadlock occurred");
                    default:
                        throw;
                }
            }
        }
    }
}
