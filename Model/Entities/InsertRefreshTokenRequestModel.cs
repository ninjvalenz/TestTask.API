namespace TestTask.API.Model.Entities
{
    public class InsertRefreshTokenRequestModel
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByIp { get; set; }
    }
}