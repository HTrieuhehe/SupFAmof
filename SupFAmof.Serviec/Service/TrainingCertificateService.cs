using AutoMapper;
using AutoMapper.QueryableExtensions;
using LAK.Sdk.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Service.Commons;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Utilities;
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
        private readonly IMapper _mapper;
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
                                    .DynamicSort(paging.Sort, paging.Order)
                                    .Where(x => x.IsActive == true)
                                    .PagingQueryable(paging.Page, paging.PageSize);

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
                var trainingCertificate = await _unitOfWork.Repository<TrainingCertificate>().GetAll()
                                      .FirstOrDefaultAsync(x => x.Id == trainingCertificateId && x.IsActive == true);

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

        public async Task<BaseResponseViewModel<TrainingCertificateResponse>> CreateTrainingCertificate(int accountId, CreateTrainingCertificateRequest request)
        {
            try
            {
                //check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                if (request.TrainingTypeId == null || request.TrainingTypeId == "")
                {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE,
                                        TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE.GetDisplayName());
                }

                var tranningCertificate = await _unitOfWork.Repository<TrainingCertificate>()
                                           .FindAsync(x => x.TrainingTypeId.Contains(request.TrainingTypeId) && x.IsActive == true);

                if (tranningCertificate != null)
                {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED,
                                        TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED.GetDisplayName());
                }

                var result = _mapper.Map<CreateTrainingCertificateRequest, TrainingCertificate>(request);

                result.TrainingTypeId = result.TrainingTypeId.ToUpper();
                result.CreateAt = Ultils.GetCurrentDatetime();

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

        public async Task<BaseResponseViewModel<TrainingCertificateResponse>> UpdateTrainingCertificate(int accountId, int trainingCertificateId, UpdateTrainingCertificateRequest request)
        {
            try
            {
                //check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                if (request.TrainingTypeId == null || request.TrainingTypeId == "")
                {
                    throw new ErrorResponse(400, (int)TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE,
                                        TrainingCertificateErrorEnum.INVALID_TRAINING_CERTIFICATE_TYPE.GetDisplayName());
                }

                var checkCertificate = await _unitOfWork.Repository<TrainingCertificate>()
                                                        .FindAsync(x => x.TrainingTypeId == request.TrainingTypeId.ToUpper() && x.IsActive == true);

                if (checkCertificate == null)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED,
                                             TrainingCertificateErrorEnum.TRAINING_CERTIFICATE_EXISTED.GetDisplayName());
                }

                var tranningCertificate = await _unitOfWork.Repository<TrainingCertificate>()
                                                        .FindAsync(x => x.Id == trainingCertificateId && x.IsActive == true);

                if (tranningCertificate == null)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                             TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                var updateTrainingCertificate = _mapper.Map<UpdateTrainingCertificateRequest, TrainingCertificate>(request, tranningCertificate);

                updateTrainingCertificate.TrainingTypeId = updateTrainingCertificate.TrainingTypeId.ToUpper();
                updateTrainingCertificate.UpdateAt = Ultils.GetCurrentDatetime();

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

        public async Task<BaseResponsePagingViewModel<TrainingCertificateResponse>> SearchTrainingCertificate(string search, PagingRequest paging)
        {
            //Search by Name or Type
            try
            {

                if (search == null || search.Length == 0)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                        TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                var certificate = _unitOfWork.Repository<TrainingCertificate>().GetAll()
                                    .ProjectTo<TrainingCertificateResponse>(_mapper.ConfigurationProvider)
                                    .Where(x => x.CertificateName.Contains(search) || x.TrainingTypeId.Contains(search.ToUpper()))
                                    .PagingQueryable(paging.Page, paging.PageSize);

                if (!certificate.Item2.Any())
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                        TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                return new BaseResponsePagingViewModel<TrainingCertificateResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = certificate.Item1
                    },
                    Data = certificate.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<bool>> DisableTrainingCertificate(int accountId, int trainingCertificateId)
        {
            try
            {
                //check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                var trainingCertificate = await _unitOfWork.Repository<TrainingCertificate>()
                                            .FindAsync(x => x.Id == trainingCertificateId && x.IsActive == true);

                if (trainingCertificate == null)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                             TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                trainingCertificate.IsActive = false;
                trainingCertificate.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<TrainingCertificate>().UpdateDetached(trainingCertificate);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<bool>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Delete Training Certificate Successfully",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
