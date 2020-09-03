using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public interface IGenericRepository <T> where T: class // Repositório Genérico de uma classe qualquer T
    {
        // IQueryable Equivalente ao genérico IEnumerable ou list - É o mais genérico que há
        // Ir buscar todos os registos
        IQueryable<T> GetAll();


        //Ir buscar apenas um registo
        Task<T> GetByIdAsync(int id);


        //Inserir um novo registo
        Task CreateAsync(T entity);


        //Actualizar um registo
        Task UpDateAsync(T entity);


        //Apagar um registo
        Task DeleteAsync(T entity);


        Task<bool> ExistsAsync(int id);

    }
}
