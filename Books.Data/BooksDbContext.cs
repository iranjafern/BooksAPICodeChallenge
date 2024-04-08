using Books.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Books.DataAccess
{
    public class BooksDbContext : DbContext
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options)
        {

        }

        public DbSet<BookSearchHistory> BookSearchHistory { get; set; }        
    }
}
