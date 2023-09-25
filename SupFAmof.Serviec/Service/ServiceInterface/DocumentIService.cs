using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IDocumentService
    {
        Task<BaseResponsePagingViewModel<AdmissionDocumentResponse>> GetDocuments(PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionDocumentResponse>> CreateDocument(DocumentRequest request);
        Task<BaseResponseViewModel<AdmissionDocumentResponse>> UpdateDocument(int documentId, DocumentUpdateRequest request);
        Task<BaseResponseViewModel<AdmissionDocumentResponse>> DisableDocument(int documentId);
    }
}
