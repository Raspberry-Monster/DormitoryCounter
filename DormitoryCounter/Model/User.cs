using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DormitoryCounter.Model
{
    public class User
    {
        [JsonPropertyName("strFlag1")]
        public string Flag { get; set; } = "teacher";
        [JsonPropertyName("strUserName1")]
        public required string UserName { get; set; }
        [JsonPropertyName("strPassword1")]
        public required string Password { get; set; }
        [JsonPropertyName("strGrade1")]
        public string Grade { get; set; } = "0";
        [JsonPropertyName("strClass1")]
        public string Class { get; set; } = string.Empty;
        [JsonPropertyName("userRole1")]
        public string Role { get; set; } = "班主任";
    }
}
