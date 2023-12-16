using SupFAmof.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IAdmissionCredentialService
    {
        Task<BaseResponseViewModel<AccountResponse>> CreateAdmissionCredential(int administratorId, int accountId);
        Task<BaseResponseViewModel<AccountResponse>> DisableAdmissionCredential(int administratorId, int accountId);
    }
}
