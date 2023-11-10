using AutoMapper;
using ServiceStack;
using Service.Commons;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Runtime.CompilerServices;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;
using ErrorResponse = SupFAmof.Service.Exceptions.ErrorResponse;
using SupFAmof.Service.Utilities;
using LAK.Sdk.Core.Utilities;

namespace SupFAmof.Service.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DocumentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponsePagingViewModel<AdmissionDocumentResponse>> GetDocuments(PagingRequest paging)
        {
            var listDocument = _unitOfWork.Repository<DocumentTemplate>().GetAll()
                                          .ProjectTo<AdmissionDocumentResponse>(_mapper.ConfigurationProvider)
                                          .Where(x => x.IsActive == true)
                                          .PagingQueryable(paging.Page, paging.PageSize);

            return new BaseResponsePagingViewModel<AdmissionDocumentResponse>()
            {
                Metadata = new PagingsMetadata()
                {
                    Page = paging.Page,
                    Size = paging.PageSize,
                    Total = listDocument.Item1
                },
                Data = listDocument.Item2.ToList()
            };
        }

        public async Task<BaseResponseViewModel<AdmissionDocumentResponse>> CreateDocument(int accountId, DocumentRequest request)
        {
            try
            {
                //check account post Permission
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (account == null || account.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                if (request == null)
                {
                    throw new ErrorResponse(400,(int)DocumentErrorEnum.INVALID_DOCUMENT,
                                                     DocumentErrorEnum.INVALID_DOCUMENT.GetDisplayName());
                }

                var document = _mapper.Map<DocumentTemplate>(request);
                document.IsActive = true;

                await _unitOfWork.Repository<DocumentTemplate>().InsertAsync(document);
                await _unitOfWork.CommitAsync();
                
                return new BaseResponseViewModel<AdmissionDocumentResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionDocumentResponse>(document)
                };
            }
            catch(Exception ex)
            {
                throw ;
            }

        }

        public async Task<BaseResponseViewModel<AdmissionDocumentResponse>> UpdateDocument(int accountId, int documentId, DocumentUpdateRequest request)
        {
            try
            {
                //check account post Permission
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (account == null || account.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                if (request == null)
                {
                    throw new ErrorResponse(400, (int)DocumentErrorEnum.INVALID_DOCUMENT, 
                                                      DocumentErrorEnum.INVALID_DOCUMENT.GetDisplayName());
                }

                var checkDocument = await _unitOfWork.Repository<DocumentTemplate>()
                                                     .FindAsync(x => x.Id == documentId && x.IsActive == true);

                if (checkDocument == null)
                {
                    throw new ErrorResponse(400, (int)DocumentErrorEnum.NOT_FOUND_DOCUMENT, 
                                                      DocumentErrorEnum.NOT_FOUND_DOCUMENT.GetDisplayName());

                }

                var documentMapping = _mapper.Map<DocumentUpdateRequest, DocumentTemplate>(request, checkDocument);
                
                await _unitOfWork.Repository<DocumentTemplate>().UpdateDetached(documentMapping);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionDocumentResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionDocumentResponse>(documentMapping)
                };
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<BaseResponseViewModel<AdmissionDocumentResponse>> DisableDocument(int accountId, int documentId)
        {
            try
            {

                //check account post Permission
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (account == null || account.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                var document = await _unitOfWork.Repository<DocumentTemplate>()
                                                               .FindAsync(x => x.Id == documentId);

                if (document == null)
                {
                    throw new ErrorResponse(400, (int)DocumentErrorEnum.NOT_FOUND_DOCUMENT, DocumentErrorEnum.NOT_FOUND_DOCUMENT.GetDisplayName());
                }

                if (!document.IsActive)
                {
                    throw new ErrorResponse(400, (int)DocumentErrorEnum.ALREADY_DISABLED, DocumentErrorEnum.ALREADY_DISABLED.GetDisplayName());
                }

                document.IsActive = false;

                await _unitOfWork.Repository<DocumentTemplate>().UpdateDetached(document);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionDocumentResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionDocumentResponse>(document)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AdmissionDocumentResponse>> SearchDocument(string search, PagingRequest paging)
        {
            //Search by Name
            try
            {

                if (search == null || search.Length == 0)
                {
                    throw new ErrorResponse(404, (int)DocumentErrorEnum.NOT_FOUND_DOCUMENT,
                                        DocumentErrorEnum.NOT_FOUND_DOCUMENT.GetDisplayName());
                }

                var document = _unitOfWork.Repository<DocumentTemplate>().GetAll()
                                    .ProjectTo<AdmissionDocumentResponse>(_mapper.ConfigurationProvider)
                                    .Where(x => x.DocName.Contains(search))
                                    .PagingQueryable(paging.Page, paging.PageSize);

                if (!document.Item2.Any())
                {
                    throw new ErrorResponse(404, (int)DocumentErrorEnum.NOT_FOUND_DOCUMENT,
                                        DocumentErrorEnum.NOT_FOUND_DOCUMENT.GetDisplayName());
                }

                return new BaseResponsePagingViewModel<AdmissionDocumentResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = document.Item1
                    },
                    Data = document.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
