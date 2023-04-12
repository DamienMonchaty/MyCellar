using MyCellar.API.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCellar.API.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> GetProductsByCategoryId(int categoryId);
        Task<Category> SaveOneProductByCategoryId(int categoryId, Product product);
        Task<Product> GetOneProductByCategoryId(int categoryId, int productId);
        Task<Category> EditOneProductByCategoryId(int categoryId, int productId, Product product);
        Task<Category> DeleteOneProductByCategoryId(int categoryId, int productId);
    }
}
