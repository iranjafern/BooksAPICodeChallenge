using Books.DataAccess;
using Books.DataAccess.Entities;
using Books.API.Services.IRepositories;

namespace Books.API.Services.Repositories
{
    public class BookSearchHistoryRepository : IBookSearchHistoryRepository
    {
        private readonly BooksDbContext dbContext;

        public BookSearchHistoryRepository(BooksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Write the book search response to the DB for the analytics
        /// </summary>
        /// <param name="bookSearchHistory"></param>
        /// <returns></returns>
        public async Task CreateAsync(BookSearchHistory bookSearchHistory)
        {
            dbContext.BookSearchHistory.Add(bookSearchHistory);
            await dbContext.SaveChangesAsync();
        }
    }
}
