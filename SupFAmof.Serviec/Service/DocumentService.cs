using AutoMapper;
using ServiceStack;
using Service.Commons;
using SupFAmof.Data.Entity;
using NTQ.Sdk.Core.Utilities;
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

        public async Task<DTO.Response.BaseResponsePagingViewModel<AdmissionDocumentResponse>> GetDocuments(PagingRequest paging)
        {
            var listDocument = _unitOfWork.Repository<DocumentTemplate>().GetAll()
                                          .ProjectTo<AdmissionDocumentResponse>(_mapper.ConfigurationProvider)
                                          .PagingQueryable(paging.Page, paging.PageSize,
                                           Constants.LimitPaging, Constants.DefaultPaging);
            if (listDocument.Item2 == null)
            {
                return new BaseResponsePagingViewModel<AdmissionDocumentResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = listDocument.Item1
                    },
                    Data = new List<AdmissionDocumentResponse>()
                };
            }
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
        public async Task<BaseResponseViewModel<AdmissionDocumentResponse>> CreateDocument(DocumentRequest request)
        {
            try
            {
                if(request == null)
                {
                    throw new ErrorResponse(400,(int)DocumentErrorEnum.INVALID_DOCUMENT,DocumentErrorEnum.INVALID_DOCUMENT.GetDisplayName());
                }
                var document = _mapper.Map<DocumentTemplate>(request);
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
        public async Task<BaseResponseViewModel<AdmissionDocumentResponse>> UpdateDocument(int documentId,DocumentUpdateRequest request)
        {
            try
            {
                if (request == null)
                {
                    throw new ErrorResponse(400, (int)DocumentErrorEnum.INVALID_DOCUMENT, DocumentErrorEnum.INVALID_DOCUMENT.GetDisplayName());
                }
                var document = _mapper.Map<DocumentTemplate>(request);
                var documentNeedToBeUpdated = await _unitOfWork.Repository<DocumentTemplate>().GetAll().SingleOrDefaultAsync(x=>x.Id == documentId);
                if (documentNeedToBeUpdated == null)
                {
                    throw new ErrorResponse(400, (int)DocumentErrorEnum.NOT_FOUND_DOCUMENT, DocumentErrorEnum.NOT_FOUND_DOCUMENT.GetDisplayName());

                }
                if (!string.IsNullOrWhiteSpace(request.DocName))
                {
                    documentNeedToBeUpdated.DocName = request.DocName;
                }

                // Update only if the DocUrl is provided in the request
                if (!string.IsNullOrWhiteSpace(request.DocUrl))
                {
                    documentNeedToBeUpdated.DocUrl = request.DocUrl;
                }
                await _unitOfWork.Repository<DocumentTemplate>().Update(documentNeedToBeUpdated,documentNeedToBeUpdated.Id);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<AdmissionDocumentResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionDocumentResponse>(documentNeedToBeUpdated)
                };
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<BaseResponseViewModel<AdmissionDocumentResponse>> DisableDocument(int documentId)
        {
            try
            {
             
                var documentNeedToBeUpdated = await _unitOfWork.Repository<DocumentTemplate>().GetAll().SingleOrDefaultAsync(x => x.Id == documentId);
                if (documentNeedToBeUpdated == null)
                {
                    throw new ErrorResponse(400, (int)DocumentErrorEnum.NOT_FOUND_DOCUMENT, DocumentErrorEnum.NOT_FOUND_DOCUMENT.GetDisplayName());
                }
                if (!documentNeedToBeUpdated.IsActive)
                {
                    throw new ErrorResponse(400, (int)DocumentErrorEnum.ALREADY_DISABLED, DocumentErrorEnum.ALREADY_DISABLED.GetDisplayName());
                }

                await _unitOfWork.Repository<DocumentTemplate>().Update(documentNeedToBeUpdated, documentNeedToBeUpdated.Id);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionDocumentResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionDocumentResponse>(documentNeedToBeUpdated)
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
                                    .PagingQueryable(paging.Page, paging.PageSize,
                                                        Constants.LimitPaging, Constants.DefaultPaging);

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
