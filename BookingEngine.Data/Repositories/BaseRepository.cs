using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Data.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly DatabaseContext _dbContext;

        public BaseRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
