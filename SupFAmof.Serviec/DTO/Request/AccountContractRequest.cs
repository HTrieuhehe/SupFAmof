using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request
{
    public class CreateAccountContractRequest
    {
        [Required(ErrorMessage = "ContractId is required")]
        [Range(0, int.MaxValue, ErrorMessage = "ContractId must be a non-negative number")]
        public int ContractId { get; set; }

        [Required(ErrorMessage = "AccountId is required")]
        [Range(0, int.MaxValue, ErrorMessage = "AccountId must be a non-negative number")]
        public int AccountId { get; set; }

        //[Required]
        public byte[]? SubmittedFile { get; set; }
    }
}
