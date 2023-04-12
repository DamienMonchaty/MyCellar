using MyCellar.API.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCellar.API.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUserNameAndPassword(string userName, string password);
        Task<User> GetByUserName(string userName);
        Task<string> GetEmailByUserId(int id);
        Task<User> AssignOneProductToCurrentUser(int userId, int productId);
        Task<User> DeleteOneProductFromCurrentUser(int userId, int productId);
        Task<List<Product>> GetAllProductsFromCurrentUser(int userId);
    }
}
