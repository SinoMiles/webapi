using Newtonsoft.Json;
namespace DTO
{
    public class BookDto
    {

        //[JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }

    }
}
