namespace BookStore.Data
{
    public class GalleryImage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
