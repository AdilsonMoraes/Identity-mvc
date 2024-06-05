using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.ViewModel
{
    public class ResetTokenViewModel
    {
        public string Token { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
    }
}
