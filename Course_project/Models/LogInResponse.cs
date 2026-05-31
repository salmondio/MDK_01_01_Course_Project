using Course_project_wpf.Models.FullModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_project_wpf.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime ValidTo { get; set; }
        public User User { get; set; }
    }
}
