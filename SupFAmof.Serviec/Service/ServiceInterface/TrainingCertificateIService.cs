﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response.Admission;

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
        Task<BaseResponseViewModel<dynamic>> CreateDaysForCertificateInterview(int accountId, EventDaysCertificate request);
        Task<BaseResponseViewModel<dynamic>> TrainingCertificateRegistration(int accountId, TrainingCertificateRegistration request);
        Task<BaseResponseViewModel<dynamic>> AssignDayToRegistration(int accountId, List<AssignEventDayToAccount> requests);
        Task<BaseResponsePagingViewModel<ViewCollabInterviewClassResponse>> GetCollabInClass(int accountId, int eventDayId, PagingRequest paging);
        Task<BaseResponseViewModel<dynamic>> UpdateDaysForCertificateInterview(int accountId, int evenDayId, UpdateDaysCertifcate request);
    }
}
