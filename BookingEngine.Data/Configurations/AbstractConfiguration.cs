using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Data.Configurations
{
    internal abstract class AbstractConfiguration<T> : IEntityTypeConfiguration<T> where T : class
    {
        protected virtual string Name => typeof(T).Name;

        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.ToTable(Name);
        }
    }
}
