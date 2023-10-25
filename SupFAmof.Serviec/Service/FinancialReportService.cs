using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service
{
    public class FinancialReportService : IFinancialReportService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public FinancialReportService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponseViewModel<CollabInfoReportResponse>> GetAdmissionFinancialReport(int accountId)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>()
                                         .GetAll()
                                         .Include(a => a.AccountInformation)
                                         .Include(b => b.PostAttendees)
                                            .ThenInclude(c => c.Position)
                                         .Include(d => d.PostAttendees)
                                            .ThenInclude(e => e.Post)
                                            .ThenInclude(f => f.PostCategory)
                                        .Where(x => x.Posts.Any(x => x.CreateAt >= Ultils.GetCurrentDatetime().AddMonths(-1) && x.AccountId == accountId));


                return new BaseResponseViewModel<CollabInfoReportResponse>()
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = null
                    //Data = _mapper.Map<CollabInfoReportResponse>(null)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
