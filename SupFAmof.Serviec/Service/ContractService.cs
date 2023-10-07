using AutoMapper;
using AutoMapper.QueryableExtensions;
using NTQ.Sdk.Core.Utilities;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContractService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<BaseResponseViewModel<AdmissionContractResponse>> CreateAdmissionContract(int accountId, CreateAdmissionContractRequest request)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<BaseResponseViewModel<AdmissionContractResponse>> UpdateAdmissionContract(int accountId, UpdateAdmissionContractRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponseViewModel<AdmissionContractResponse>> GetAdmissionContractById(int accountId, int contractId)
        {
            try
            {
                var contract = await _unitOfWork.Repository<Contract>().FindAsync(x => x.Id == contractId && x.CreatePersonId == accountId));
                if (contract == null)
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }
                return new BaseResponseViewModel<AdmissionContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionContractResponse>(contract)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AdmissionContractResponse>> GetAdmissionContracts(int accountId, AdmissionContractResponse filter, PagingRequest paging)
        {
            try
            {
                var contract = _unitOfWork.Repository<Contract>().GetAll()
                                                .Where(x => x.CreatePersonId == accountId)
                                                .ProjectTo<AdmissionContractResponse>(_mapper.ConfigurationProvider)
                                                .DynamicFilter(filter)
                                                .DynamicSort(filter)
                                                .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);
                
                return new BaseResponsePagingViewModel<AdmissionContractResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = contract.Item1
                    },
                    Data = contract.Item2.ToList(),
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<ContractResponse>> GetContracts(ContractResponse filter, PagingRequest paging)
        {
            try
            {
                var contract = _unitOfWork.Repository<Contract>().GetAll()
                                                .ProjectTo<ContractResponse>(_mapper.ConfigurationProvider)
                                                .DynamicFilter(filter)
                                                .DynamicSort(filter)
                                                .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<ContractResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = contract.Item1
                    },
                    Data = contract.Item2.ToList(),
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<ContractResponse>> GetContractsById(int contractId)
        {
            try
            {
                var contract = await _unitOfWork.Repository<Contract>().FindAsync(x => x.Id == contractId));
                if (contract == null)
                {
                    throw new ErrorResponse(404, (int)ContractErrorEnum.NOT_FOUND_CONTRACT,
                                        ContractErrorEnum.NOT_FOUND_CONTRACT.GetDisplayName());
                }
                return new BaseResponseViewModel<ContractResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<ContractResponse>(contract)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
