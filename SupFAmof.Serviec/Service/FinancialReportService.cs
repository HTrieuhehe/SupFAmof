using System.IO;
using AutoMapper;
using ServiceStack;
using OfficeOpenXml;
using Service.Commons;
using System.Drawing.Text;
using SupFAmof.Data.Entity;
using LAK.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Helpers;
using System.Security.Principal;
using SupFAmof.Service.Utilities;
using System.Collections.Generic;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using System.Net.NetworkInformation;
using AutoMapper.QueryableExtensions;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;

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

        public async Task<byte[]> GenerateAccountExcel()
        {
            try
            {
            var list = _unitOfWork.Repository<Account>().GetAll().Where(x => x.Email.EndsWith("fpt.edu.vn"));
            var data = await AccountReportGenerator(list);
                return data;

            }catch(Exception ex)
            {
                throw new Exceptions.ErrorResponse(500, (int)AccountReportErrorEnum.MISSING_INFORMATION, AccountReportErrorEnum.MISSING_INFORMATION.GetDisplayName());
            }



        }
        private async Task<byte[]> AccountReportGenerator(IQueryable<Account> accounts)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ExcelPackage xlPackage = new ExcelPackage(memoryStream))
                {
                    // Do work with the Excel package, populate the worksheet if needed
                    int row = 1;
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Data CTV");
                    worksheet.Cells["A1"].Value = "STT";
                    worksheet.Cells["B1"].Value = "Họ tên";
                    worksheet.Cells["C1"].Value = "MSSV";
                    worksheet.Cells["D1"].Value = "CMND";
                    worksheet.Cells["E1"].Value = "MST";
                    worksheet.Cells["F1"].Value = "Người thụ hưởng";
                    worksheet.Cells["G1"].Value = "STK";
                    worksheet.Cells["H1"].Value = "NH";
                    worksheet.Cells["I1"].Value = "CN";
                    worksheet.Cells["J1"].Value = "Tổng";
                    foreach (var account in accounts)
                    {
                        row++;
                        worksheet.Cells[row, 1].Value = account.Id;
                        worksheet.Cells[row, 2].Value = account.Name;
                        worksheet.Cells[row, 3].Value = account.AccountInformation?.IdStudent;
                        worksheet.Cells[row, 4].Value = account.AccountInformation?.IdentityNumber;
                        worksheet.Cells[row, 5].Value = account.AccountInformation?.TaxNumber;
                        worksheet.Cells[row, 6].Value = account.AccountBankings.First().Beneficiary;
                        worksheet.Cells[row, 7].Value = account.AccountBankings.First().AccountNumber;
                        worksheet.Cells[row, 8].Value = account.AccountBankings.First().BankName;
                        worksheet.Cells[row, 9].Value = account.AccountBankings.First().Branch;



                    }
                    // Save the Excel package to create the new file
                    await xlPackage.SaveAsync();
                }
                byte[] array  = memoryStream.ToArray();
                return array;
            }
        }

        public BaseResponsePagingViewModel<CollabReportResponse> AccountReportList(PagingRequest paging)
        {
            try
            {
                var accounts = _unitOfWork.Repository<Account>().GetAll()
                                          .Where(x => x.Email.EndsWith("fpt.edu.vn"))
                                          .OrderByDescending(x => x.Id)
                                          .ProjectTo<CollabReportResponse>(_mapper.ConfigurationProvider)
                                          .PagingQueryable(paging.Page, paging.PageSize,
                                                           Constants.LimitPaging, Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<CollabReportResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accounts.Item1
                    },
                    Data = accounts.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
