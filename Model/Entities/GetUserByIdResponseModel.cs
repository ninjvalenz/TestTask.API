namespace TestTask.API.Model.Entities
{
    public class GetUserByIdResponseModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }   
        public int CompanyId { get; set;  }
    }
}