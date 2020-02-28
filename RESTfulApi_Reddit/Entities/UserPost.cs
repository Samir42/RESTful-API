namespace RESTfulApi_Reddit.Entities {
    public class UserPost : Post {
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
