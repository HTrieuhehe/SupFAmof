using AutoMapper;
using AutoMapper.QueryableExtensions;
using NTQ.Sdk.Core.Utilities;
using Service.Commons;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class TrainingCertificateService : ITrainingCertificateService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TrainingCertificateService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponsePagingViewModel<TrainingCertificateResponse>> GetTrainingCertificates(TrainingCertificateResponse filter, PagingRequest paging)
        {
            try
            {
                var role = _unitOfWork.Repository<TrainingCertificate>().GetAll()
                                    .ProjectTo<TrainingCertificateResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(filter)
                                    .PagingQueryable(paging.Page, paging.PageSize,
                                    Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<TrainingCertificateResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = role.Item1
                    },
                    Data = role.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<TrainingCertificateResponse>> GetTrainingCertificateById(int trainingCertificateId)
        {
            try
            {
                var trainingCertificate = _unitOfWork.Repository<TrainingCertificate>().GetAll()
                                      .FirstOrDefault(x => x.Id == trainingCertificateId);

                if (trainingCertificate == null)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                         TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                return new BaseResponseViewModel<TrainingCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<TrainingCertificateResponse>(trainingCertificate)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponseViewModel<TrainingCertificateResponse>> CreateTrainingCertificate(CreateTrainingCertificateRequest request)
        {
            try
            {
                if (request.TrainingTypeId == null || request.TrainingTypeId == "")
                {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE,
                                        TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE.GetDisplayName());
                }

                var tranningCertificate = _unitOfWork.Repository<TrainingCertificate>()
                                           .Find(x => x.TrainingTypeId.Contains(request.TrainingTypeId));

                if (tranningCertificate != null)
                {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED,
                                        TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED.GetDisplayName());
                }
                var result = _mapper.Map<CreateTrainingCertificateRequest, TrainingCertificate>(request);

                result.TrainingTypeId = result.TrainingTypeId.ToUpper();
                result.CreateAt = DateTime.Now;

                await _unitOfWork.Repository<TrainingCertificate>().InsertAsync(result);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<TrainingCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<TrainingCertificateResponse>(result)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<TrainingCertificateResponse>> UpdateTrainingCertificate(int trainingCertificateId, UpdateTrainingCertificateRequest request)
        {
            try
            {
                var tranningCertificate = _unitOfWork.Repository<TrainingCertificate>().Find(x => x.Id == trainingCertificateId);

                if (tranningCertificate == null)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                             TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                if (request.TrainingTypeId == null || request.TrainingTypeId == "")
                {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE,
                                        TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE.GetDisplayName());
                }

                var updateTrainingCertificate = _mapper.Map<UpdateTrainingCertificateRequest, TrainingCertificate>(request, tranningCertificate);

                updateTrainingCertificate.TrainingTypeId = updateTrainingCertificate.TrainingTypeId.ToUpper();
                updateTrainingCertificate.UpdateAt = DateTime.Now;

                await _unitOfWork.Repository<TrainingCertificate>().UpdateDetached(updateTrainingCertificate);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<TrainingCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<TrainingCertificateResponse>(updateTrainingCertificate)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
