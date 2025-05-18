//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MovieBuzz.Core.Security
//{
//    public class JwtSettings
//    {
//        public string Key { get; set; }
//        public string Issuer { get; set; }
//        public string Audience { get; set; }
//        public int ExpiryInMinutes { get; set; }
//    }
//}

namespace MovieBuzz.Core.Security
{
    public class JwtSettings
    {
        public required string Key { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int ExpiryInMinutes { get; set; }
    }
}
