using BookingEngine.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Data.Configurations
{
    internal class SearchRequestConfiguration : AbstractEntityConfiguration<SearchRequest>
    {
        public override void Configure(EntityTypeBuilder<SearchRequest> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.SearchRequestId);

            builder.HasIndex(x => x.CityCode);
            builder.HasIndex(x => x.CheckInDate);
            builder.HasIndex(x => x.CheckOutDate);

            builder.HasIndex(x => new { x.CityCode, x.CheckInDate, x.CheckOutDate });

            builder.Property(x => x.CityCode).IsRequired().HasMaxLength(30);
            builder.Property(x => x.CheckInDate).IsRequired();
            builder.Property(x => x.CheckOutDate).IsRequired();
            builder.Property(x => x.ValidUntil).IsRequired().HasDefaultValue(new DateTime(2022, 1, 1, 0, 0, 0));
            builder.Property(x => x.Adults).HasDefaultValue(1);
            builder.Property(x => x.NextItemsLink).IsRequired(false).HasDefaultValue(null);

        }
    }
}
