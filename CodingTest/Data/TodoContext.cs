using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CodingTest.Models;

namespace CodingTest.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext (DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<CodingTest.Models.Todo> Todo { get; set; }
    }
}
