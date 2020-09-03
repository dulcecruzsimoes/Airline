using Airline.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public class Repository : IRepository
    {
        private readonly DataContext _context;

        public Repository(DataContext context) // Repositório precisa sempre de aceder ao dataContext
        {
            _context = context;
        }

        // Ideia é ter um método que retorna todos os detinos e ordenados por nome (será utilizado no controlador, no indice)

        public IEnumerable<Destination> GetDestinations()
        {
            return _context.Destinations.OrderBy(d => d.Airport);

        }

        //Método que retorna um destino especifico
        public Destination GetDestination(int id)
        {
            return _context.Destinations.Find(id);

        }

        //Método que adiciona um destino especifico
        public void AddDestination(Destination destination)
        {

            _context.Destinations.Add(destination);
        }

        //Método que actualiza um destino especifico
        public void UpdateDestination(Destination destination)
        {

            _context.Destinations.Update(destination);
        }

        //Método que apaga um destino especifico
        public void RemoveDestination(Destination destination)
        {
            _context.Destinations.Remove(destination);

        }

        //Método que grava todas as mudanças
        public async Task<bool> SaveAllAsync() // Grava todas as mudanças que pode ser mais do que uma. Se não gravar nenhuma retorna zero
        {

            return await _context.SaveChangesAsync() > 0;
        }

        //Método que verifica a existência de um destino especifico
        public bool DestinationExists(int id)
        {

            return _context.Destinations.Any(d => d.Id == id);
        }
    }
}
