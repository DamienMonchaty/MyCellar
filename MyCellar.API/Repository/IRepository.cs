using MyCellar.API.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCellar.API.Repository
{
    public interface IRepository<T> where T : class, new()
    {
        Task<List<T>> GetAll();
        Task<T> Add(T o);
        Task<T> GetById(int id);
        Task<T> Update(T o);
        Task Delete(T o);
        Task<int> Count();
        Task<PageResult<T>> GetAllPaginate(int? page, int pagesize, string search);

    }
}
