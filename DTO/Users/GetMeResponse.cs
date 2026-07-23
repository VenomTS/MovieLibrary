namespace DTO.Users
{
    public class GetMeResponse
    {
        public Guid Id { get; set; }
        public IList<string> Roles { get; set; } = [];
    }
}
