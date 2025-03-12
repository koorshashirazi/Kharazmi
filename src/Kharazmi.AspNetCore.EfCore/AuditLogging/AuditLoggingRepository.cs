using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.AuditLogging;
using Kharazmi.AspNetCore.Core.Common;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;

namespace Kharazmi.AspNetCore.EFCore.AuditLogging
{
    public interface IAuditLoggingRepository<TDbContext, TAuditLog>
        where TDbContext : DbContext, IUnitOfWork<TDbContext>, IAuditLoggingDbContext<TAuditLog>
        where TAuditLog : AuditLog
    {
        Task<PagedList<TAuditLog>> GetAsync(string @event,
            string source,
            string category,
            DateTime? created,
            string subjectIdentifier,
            string subjectName,
            int page = 1,
            int pageSize = 10);

        Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan);

        Task SaveAsync(TAuditLog auditLog);

        Task<PagedList<TAuditLog>> GetAsync(int page = 1, int pageSize = 10);

        Task<PagedList<TAuditLog>> GetAsync(
            string subjectIdentifier,
            string subjectName,
            string category,
            int page = 1,
            int pageSize = 10);
    }

    public class AuditLoggingRepository<TDbContext, TAuditLog> : IAuditLoggingRepository<TDbContext, TAuditLog>
        where TDbContext : DbContext, IUnitOfWork<TDbContext>, IAuditLoggingDbContext<TAuditLog>
        where TAuditLog : AuditLog
    {
        protected IUnitOfWork<TDbContext> DbContext;

        public AuditLoggingRepository(IUnitOfWork<TDbContext> dbContext)
        {
            DbContext = dbContext;
            DbContext.CheckArgumentIsNull(nameof(DbContext));
        }

        public async Task<PagedList<TAuditLog>> GetAsync(
            string @event,
            string source,
            string category,
            DateTime? created,
            string subjectIdentifier,
            string subjectName,
            int page = 1,
            int pageSize = 10)
        {
            var pagedList = new PagedList<TAuditLog>();

            var auditLogs = await DbContext.Set<TAuditLog>()
                .WhereIf(subjectIdentifier.IsNotEmpty(),
                    log => log.SubjectIdentifier.Contains(subjectIdentifier))
                .WhereIf(subjectName.IsNotEmpty(), log => log.SubjectName.Contains(subjectName))
                .WhereIf(@event.IsNotEmpty(), log => log.Event.Contains(@event))
                .WhereIf(source.IsNotEmpty(), log => log.Source.Contains(source))
                .WhereIf(category.IsNotEmpty(), log => log.Category.Contains(category))
                .WhereIf(created.HasValue, log => log.Created.Date == created.Value.Date)
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync().ConfigureAwait(false);

            pagedList.AddRange(auditLogs);
            pagedList.PageSize = pageSize;
            pagedList.TotalCount = await DbContext.Set<TAuditLog>()
                .WhereIf(subjectIdentifier.IsNotEmpty(),
                    log => log.SubjectIdentifier.Contains(subjectIdentifier))
                .WhereIf(subjectName.IsNotEmpty(), log => log.SubjectName.Contains(subjectName))
                .WhereIf(@event.IsNotEmpty(), log => log.Event.Contains(@event))
                .WhereIf(source.IsNotEmpty(), log => log.Source.Contains(source))
                .WhereIf(category.IsNotEmpty(), log => log.Category.Contains(category))
                .WhereIf(created.HasValue, log => log.Created.Date == created.Value.Date)
                .CountAsync().ConfigureAwait(false);

            return pagedList;
        }

        public virtual async Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan)
        {
            var logsToDelete = await DbContext.Set<TAuditLog>().Where(x => x.Created.Date < deleteOlderThan.Date).ToListAsync().ConfigureAwait(false);

            if (logsToDelete.Count == 0) return;

            DbContext.Set<TAuditLog>().RemoveRange(logsToDelete);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }


        public virtual async Task<PagedList<TAuditLog>> GetAsync(int page = 1, int pageSize = 10)
        {
            PagedList<TAuditLog> pagedList1 = new PagedList<TAuditLog>();
            DbSet<TAuditLog> auditLog = DbContext.Set<TAuditLog>();
            Expression<Func<TAuditLog, int>> orderBy = x => x.Id;
            int page1 = page;
            int pageSize1 = pageSize;
            pagedList1.AddRange(await auditLog.PageBy(orderBy, page1, pageSize1, true).ToListAsync().ConfigureAwait(false));
            pagedList1.PageSize = pageSize;
            PagedList<TAuditLog> pagedList2 = pagedList1;
            pagedList2.TotalCount = await DbContext.Set<TAuditLog>().CountAsync().ConfigureAwait(false);
            return pagedList1;
        }

        public virtual async Task<PagedList<TAuditLog>> GetAsync(
            string subjectIdentifier,
            string subjectName,
            string category,
            int page = 1,
            int pageSize = 10)
        {
            PagedList<TAuditLog> pagedList1 = new PagedList<TAuditLog>();
            IQueryable<TAuditLog> query = DbContext.Set<TAuditLog>()
                .WhereIf((subjectIdentifier.IsNotEmpty() ? 1 : 0) != 0,
                    x => x.SubjectIdentifier == subjectIdentifier)
                .WhereIf((subjectName.IsNotEmpty() ? 1 : 0) != 0,
                    x => x.SubjectName == subjectName).WhereIf<TAuditLog>(
                    (category.IsNotEmpty() ? 1 : 0) != 0,
                    x => x.Category == category);
            Expression<Func<TAuditLog, int>> orderBy = x => x.Id;
            int page1 = page;
            int pageSize1 = pageSize;
            pagedList1.AddRange(await query.PageBy(orderBy, page1, pageSize1, true).ToListAsync().ConfigureAwait(false));
            pagedList1.PageSize = pageSize;
            PagedList<TAuditLog> pagedList2 = pagedList1;
            pagedList2.TotalCount = await DbContext.Set<TAuditLog>().CountAsync().ConfigureAwait(false);
            return pagedList1;
        }

        public virtual async Task SaveAsync(TAuditLog auditLog)
        {
            var entityEntry = await this.DbContext.Set<TAuditLog>().AddAsync(auditLog).ConfigureAwait(false);
            int num = await this.DbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}