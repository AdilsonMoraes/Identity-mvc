using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.ViewModel
{
    public class LoginViewModel
    {
        public string Token { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Id { get; set; }

        public string DisplayName { get; set; }

        public bool Active { get; set; }

        public ICollection<string> Roles { get; set; }
    }
}
