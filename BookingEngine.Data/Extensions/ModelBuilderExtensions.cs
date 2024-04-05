using BookingEngine.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BookingEngine.Data
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus
                {
                    OrderStatusId = 1,
                    StatusValue = "CONFIRMED"
                },
                new OrderStatus
                {
                    OrderStatusId = 2,
                    StatusValue = "PENDING"
                },
                new OrderStatus
                {
                    OrderStatusId = 3,
                    StatusValue = "CANCELED"
                }
            );
        }
    }
}
