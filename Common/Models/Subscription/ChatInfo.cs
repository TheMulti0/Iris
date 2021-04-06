namespace Common
{
    public record ChatInfo
    {
        public long Id { get; init; }
        
        public ChatType Type { get; init; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }

        public string Username { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public bool AllMembersAreAdministrators { get; set; }
       
        public string InviteLink { get; set; }
    }
}