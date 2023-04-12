using Microsoft.EntityFrameworkCore;
using MyCellar.API.Context;
using MyCellar.API.Wrappers;
using MyCellar.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCellar.API.Repository.Impl
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly ModelDbContext _context;

        public ProductRepository(ModelDbContext context)
        {
            _context = context;
        }

        public async Task<Product> Add(Product o)
        {
            _context.Products.Add(o);
            await _context.SaveChangesAsync();
            return o;
        }

        public async Task<int> Count()
        {
            return await _context.Products.CountAsync();
        }

        public async Task Delete(Product o)
        {
            _context.Products.Remove(o);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAll()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<PageResult<Product>> GetAllPaginate(int? page, int pagesize, string search)
        {
            var query = string.IsNullOrEmpty(search) ? await _context.Products.ToListAsync()
                                                     : await _context.Products.Where(e => e.Title.ToLower().Contains(search.ToLower())).ToListAsync();

            int total = query.Count();
            PageResult<Product> result = new PageResult<Product>
            {
                Count = total,
                PageIndex = page ?? 1,
                PageSize = 10,
                Items = query.Skip((page - 1 ?? 00) * pagesize).Take(pagesize).ToList()
            };
            return result;
        }

        public async Task<Product> GetById(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Product> Update(Product o)
        {
            _context.Entry(o).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return o;
        }
    }
}
