using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashboardBackend.Models;
using UpdatesConsumer;

namespace DashboardBackend.Data
{
    public interface IUpdatesRepository
    {
        Task<int> CountAsync();
        
        Task<List<Update>> Get(PageSearchParams searchParams);

        Task AddAsync(Update update);

        Task DeleteAsync(Guid id);
    }
}