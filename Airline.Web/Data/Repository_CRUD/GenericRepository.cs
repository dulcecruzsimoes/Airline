using Airline.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly DataContext _context;



        public GenericRepository(DataContext context)
        {
            _context = context;
        }



        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveAllAsync();
        }

        private async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() >0 ; // Retorna true se tiver realizado alguma mudança
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await SaveAllAsync();
        }


        public Task<bool> ExistsAsync(int id)
        {
            return _context.Set<T>().AnyAsync(e => e.Id == id);
        }


        public IQueryable<T> GetAll()
        {
            //Usar o método set para realizar as queries
            return _context.Set<T>().AsNoTracking(); // Coloco os AsNoTracking para não guardar os registos em memória
        }


        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }


        public async Task UpDateAsync(T entity)
        {
             _context.Set<T>().Update(entity);
            await SaveAllAsync();

        }
    }
}
