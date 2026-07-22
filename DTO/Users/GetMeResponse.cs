using Models.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Users
{
    public class GetMeResponse
    {
        public Guid Id { get; set; }
        public Roles Role { get; set; }
    }
}
