using Rahpele.Models.Data;
using Rahpele.Services.Interfaces;

namespace Rahpele.Services
{
    public class ProductManager : IProductManager
    {
        private readonly ApplicationDbContext _context;
        public ProductManager(ApplicationDbContext context)
        {
            _context = context;
        }





    }
}
