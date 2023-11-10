using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IDocumentService
    {
        Task<BaseResponsePagingViewModel<AdmissionDocumentResponse>> GetDocuments(PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionDocumentResponse>> CreateDocument(int accountId,DocumentRequest request);
        Task<BaseResponseViewModel<AdmissionDocumentResponse>> UpdateDocument(int accountId, int documentId, DocumentUpdateRequest request);
        Task<BaseResponseViewModel<AdmissionDocumentResponse>> DisableDocument(int accountId, int documentId);
        Task<BaseResponsePagingViewModel<AdmissionDocumentResponse>> SearchDocument(string search, PagingRequest paging);
    }
}
