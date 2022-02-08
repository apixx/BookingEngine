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
    internal class SearchRequestHotelConfiguration : AbstractEntityConfiguration<SearchRequestHotel>
    {
        public override void Configure(EntityTypeBuilder<SearchRequestHotel> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.SearchRequestHotelId);

            builder.HasIndex(x => x.HotelId);
            builder.HasIndex(x => x.SearchRequestId);

            builder.Property(x => x.Available).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.PriceTotal).IsRequired(false).HasDefaultValue(null);
            builder.Property(x => x.PriceCurrency).IsRequired(false).HasDefaultValue(null);
            builder.Property(x => x.Distance).IsRequired(false).HasDefaultValue(null);

            builder.HasOne(srh => srh.SearchRequest)
                .WithMany(sr => sr.SearchRequestHotels)
                .HasForeignKey(x => x.SearchRequestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(srh => srh.Hotel)
                .WithMany(sr => sr.SearchRequestHotels)
                .HasForeignKey(srh => srh.HotelId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
