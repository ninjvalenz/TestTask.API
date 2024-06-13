using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestTask.API.Model
{
    public class RefreshToken
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByIp { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
        public bool IsActive => !IsExpired;
    }
}