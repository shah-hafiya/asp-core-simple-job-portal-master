using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal.ViewModels.Home
{
    public sealed class AccessToken
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}
