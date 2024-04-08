using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.DataAccess.Entities
{
    public class BookSearchHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public DateTime CreatedDataTime { get; set; }
        public string SearchQuery { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public string SearchResult { get; set; }
    }
}
