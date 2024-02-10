using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Application.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string NewToken { get; set; }
        public UserInfoResult UserInfo { get; set; }
    }
}
