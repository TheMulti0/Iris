using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using DashboardBackend.Models;

namespace DashboardBackend.Data
{
    public interface IUpdatesRepository
    {
        Task<int> CountAsync();
        
        Task<List<UpdateEntity>> Get(PageSearchParams searchParams);

        Task AddAsync(UpdateEntity update);

        Task DeleteAsync(Guid id);
    }
}