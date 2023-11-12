using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface ITrainingCertificateService
    {
        Task<BaseResponsePagingViewModel<TrainingCertificateResponse>> GetTrainingCertificates(TrainingCertificateResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<TrainingCertificateResponse>> GetTrainingCertificateById(int trainingCertificateId);
        Task<BaseResponseViewModel<TrainingCertificateResponse>> CreateTrainingCertificate(int accountId, CreateTrainingCertificateRequest request);
        Task<BaseResponseViewModel<TrainingCertificateResponse>> UpdateTrainingCertificate(int accountId, int trainingCertificateId, UpdateTrainingCertificateRequest request);
        Task<BaseResponseViewModel<bool>> DisableTrainingCertificate(int accountId, int trainingCertificateId);
        Task<BaseResponsePagingViewModel<TrainingCertificateResponse>> SearchTrainingCertificate(string search, PagingRequest paging);
    }
}
