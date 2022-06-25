using System;
using System.Collections.Generic;
using System.Text;

namespace MROCoatching.DataObjects
{
    public class ClientRefreshTokenRequest
    {
        public string ClientId { get; set; }
        public string RefreshToken { get; set; }
    }
}
