using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupFAmof.Service.DTO.Request
{
    public class MailSettings
    {
        public string? Mail { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
    }

    public class MailPaths
    {
        public Dictionary<string, string>? Paths { get; set; }
    }

    public class MailRequest
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Subject { get; set; }

        [Required]
        public string? Type { get; set; }

        public int Code { get; set; }

        public string? ContestTestField { get; set; }
    }
}
