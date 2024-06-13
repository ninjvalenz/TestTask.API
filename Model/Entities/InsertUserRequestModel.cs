namespace TestTask.API.Model.Entities
{
    public class InsertUserRequestModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsAdmin { get; set; }
        public int CompanyId { get; set; }
    }
}