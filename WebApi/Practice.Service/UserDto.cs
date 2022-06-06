namespace Practice.Service
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int? Coins { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string About { get; set; }
        public string PhotoPath { get; set; }
        public LocationDto Location { get; set; }
    }
}
