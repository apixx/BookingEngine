using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Data.Configurations
{
    internal abstract class AbstractEntityConfiguration<TEntity> : AbstractConfiguration<TEntity> where TEntity : class
    {
    }
}
