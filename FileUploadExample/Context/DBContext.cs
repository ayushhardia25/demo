using FileUploadExample.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploadExample.Context
{
    public class DBContext:DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }
       
    }
}
