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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ModelDbContext _context;

        public CategoryRepository(ModelDbContext context)
        {
            _context = context;
        }

        public async Task<Category> Add(Category o)
        {
            _context.Categories.Add(o);
            await _context.SaveChangesAsync();
            return o;
        }

        public async Task<int> Count()
        {
            return await _context.Categories.CountAsync();
        }

        public async Task Delete(Category o)
        {
            _context.Categories.Remove(o);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAll()
        {
            return await _context.Categories.Include(x => x.Products).ToListAsync();
        }

        public async Task<PageResult<Category>> GetAllPaginate(int? page, int pagesize, string search)
        {
            var query = string.IsNullOrEmpty(search) ? await _context.Categories.ToListAsync()
                                                      : await _context.Categories.Where(e => e.Title.ToLower().Contains(search.ToLower())).ToListAsync();

            int total = query.Count();
            PageResult<Category> result = new PageResult<Category>
            {
                Count = total,
                PageIndex = page ?? 1,
                PageSize = 10,
                Items = query.Skip((page - 1 ?? 00) * pagesize).Take(pagesize).ToList()
            };
            return result;
        }

        public async Task<Category> Update(Category o)
        {
            _context.Entry(o).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return o;
        }

        public async Task<Category> GetById(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Category>> GetProductsByCategoryId(int categoryId)
        {
            return await _context.Categories.Include("Products").Where(p => p.Id == categoryId).ToListAsync();
        }

        public async Task<Product> GetOneProductByCategoryId(int categoryId, int productId)
        {
            var category = await _context.Categories.Include("Products").SingleOrDefaultAsync(o => o.Id == categoryId);
            var product = category.Products.FirstOrDefault(ol => ol.Id == productId);
            return product;
        }

        public async Task<Category> SaveOneProductByCategoryId(int categoryId, Product product)
        {
            var category = await _context.Categories.Include("Products").SingleOrDefaultAsync(o => o.Id == categoryId);
            category.Products.Add(product);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> EditOneProductByCategoryId(int categoryId, int productId, Product product)
        {
            var category = await _context.Categories.Include("Products").SingleOrDefaultAsync(o => o.Id == categoryId);
            var productToUpdate = _context.Products.FirstOrDefault(ol => ol.Id == productId);
            
            productToUpdate.Title = product.Title;
            productToUpdate.Description = product.Description;
            productToUpdate.Quantity = product.Quantity;
            productToUpdate.ImgUrl = product.ImgUrl;

            _context.Entry(productToUpdate).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> DeleteOneProductByCategoryId(int categoryId, int productId)
        {
            var category = await _context.Categories.Include("Products").SingleOrDefaultAsync(o => o.Id == categoryId);
            var productToDelete = category.Products.FirstOrDefault(ol => ol.Id == productId);
            if (productToDelete != null) {
                category.Products.Remove(productToDelete);
            }
            // _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();
            return category;
        }

    }
}
