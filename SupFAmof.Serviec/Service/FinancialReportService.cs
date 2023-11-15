﻿using AutoMapper;
using System.Net;
using ServiceStack;
using OfficeOpenXml;
using System.Drawing;
using Service.Commons;
using OfficeOpenXml.Style;
using SupFAmof.Data.Entity;
using OfficeOpenXml.Drawing;
using LAK.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Utilities;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Spreadsheet;
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


        public async Task<byte[]> GenerateAccountExcel(int accountId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (account.PostPermission == false)
                {
                    throw new Exceptions.ErrorResponse(401, (int)AccountReportErrorEnum.UNAUTHORIZED, AccountReportErrorEnum.UNAUTHORIZED.GetDisplayName());
                }


                var list = _unitOfWork.Repository<Account>().GetAll().Where(x => x.Email.EndsWith("fpt.edu.vn"));
            var data = await AccountReportGenerator(list);
                return data;

            }catch(Exception ex)
            {
                throw;
            }

        }
        public async Task<byte[]> GenerateAccountWithIdentityExcel(int accountId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new Exceptions.ErrorResponse(401, (int)AccountReportErrorEnum.UNAUTHORIZED, AccountReportErrorEnum.UNAUTHORIZED.GetDisplayName());
                }


                var list = _unitOfWork.Repository<Account>().GetAll().Where(x => x.Email.EndsWith("fpt.edu.vn"));
                var data = await AccountReportWithIdentityGenerator(list);
                return data;

            }
            catch (Exception ex)
            {
                throw;
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

                        // Use null-conditional operator to access AccountBankings properties safely
                        worksheet.Cells[row, 6].Value = account.AccountBankings?.FirstOrDefault()?.Beneficiary ?? "N/A";
                        worksheet.Cells[row, 7].Value = account.AccountBankings?.FirstOrDefault()?.AccountNumber ?? "N/A";
                        worksheet.Cells[row, 8].Value = account.AccountBankings?.FirstOrDefault()?.BankName ?? "N/A";
                        worksheet.Cells[row, 9].Value = account.AccountBankings?.FirstOrDefault()?.Branch ?? "N/A";
                    }

                    // Save the Excel package to create the new file
                    await xlPackage.SaveAsync();
                }
                byte[] array  = memoryStream.ToArray();
                return array;
            }
        }
        private async Task<byte[]> AccountReportWithIdentityGenerator(IQueryable<Account> accounts)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ExcelPackage xlPackage = new ExcelPackage(memoryStream))
                {
                    // Do work with the Excel package, populate the worksheet if needed
                    int row = 2;
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Data CTV");
                    worksheet.Cells["A1"].Value = "STT";
                    worksheet.Cells["B1"].Value = "CMND";
                    worksheet.Cells["C1"].Value = "MST";
                    worksheet.Cells["D1"].Value = "Họ tên";
                    worksheet.Cells["E1"].Value = "Nơi Cấp";
                    worksheet.Cells["F1"].Value = "Địa chỉ";
                    worksheet.Cells["G1"].Value = "Ngày Cấp";
                    worksheet.Cells["H1"].Value = "Mặt trước";
                    worksheet.Cells["I1"].Value = "Mặt sau";
                    foreach (var account in accounts)
                    {
                        var existingMerge = worksheet.MergedCells[row, 1];
                        if (existingMerge == null)
                        {
                            worksheet.Cells[row, 1, row + 3, 1].Merge = true;
                            worksheet.Cells[row, 1].Value = account.Id;
                            worksheet.Cells[row, 2, row + 3, 2].Merge = true;
                            worksheet.Cells[row, 2].Value = account.AccountInformation?.IdentityNumber;
                            worksheet.Cells[row, 3, row + 3, 3].Merge = true;
                            worksheet.Cells[row, 3].Value = account.AccountInformation?.TaxNumber;
                            worksheet.Cells[row, 4, row + 3, 4].Merge = true;
                            worksheet.Cells[row, 4].Value = account.Name;
                            worksheet.Cells[row, 5, row + 3, 5].Merge = true;
                            worksheet.Cells[row, 5].Value = account.AccountInformation?.PlaceOfIssue ?? "N/A";
                            worksheet.Cells[row, 6, row + 3, 6].Merge = true;
                            worksheet.Cells[row, 6].Value = account.AccountInformation?.Address ?? "N/A";
                            worksheet.Cells[row, 7, row + 3, 7].Merge = true;
                            worksheet.Cells[row, 7].Value = account.AccountInformation?.IdentityIssueDate?.ToString("dd/MM/yyyy") ?? "N/A";
                            worksheet.Cells[row, 8, row + 3, 8].Merge = true;
                            if (!string.IsNullOrEmpty(account.AccountInformation?.IdentityFrontImg) && Uri.TryCreate(account.AccountInformation?.IdentityFrontImg, UriKind.Absolute, out _))
                            {
                                using (var webClient = new WebClient())
                                {
                                    byte[] frontImageData = webClient.DownloadData(account.AccountInformation?.IdentityFrontImg);

                                    // Use SixLabors.ImageSharp to resize the front image
                                    using (var frontImage = SixLabors.ImageSharp.Image.Load(frontImageData))
                                    {
                                        frontImage.Mutate(x => x.Resize(new ResizeOptions
                                        {
                                            Size = new SixLabors.ImageSharp.Size(80, 80),
                                            Mode = ResizeMode.Max
                                        }));

                                        // Save the resized front image to a temporary file with a specific extension
                                        string tempFrontFileName = Path.ChangeExtension(Path.GetTempFileName(), "png");
                                        frontImage.Save(tempFrontFileName);

                                        // Add the front image to the worksheet using the temporary file
                                        FileInfo frontImageFileInfo = new FileInfo(tempFrontFileName);
                                        ExcelPicture frontPicture = worksheet.Drawings.AddPicture($"FrontImg_{row}", frontImageFileInfo);
                                        frontPicture.From.Column = 7;
                                        frontPicture.From.Row = row-1;
                                        frontPicture.SetSize(100, 100);
                                    }
                                }
                            }
                            else
                            {
                                worksheet.Cells[row, 8].Value = "N/A";
                            }
                            worksheet.Cells[row, 9, row + 3, 9].Merge = true;
                            if (!string.IsNullOrEmpty(account.AccountInformation?.IdentityBackImg) && Uri.TryCreate(account.AccountInformation?.IdentityBackImg, UriKind.Absolute, out _))
                            {
                                using (var webClient = new WebClient())
                                {
                                    byte[] backImageData = webClient.DownloadData(account.AccountInformation?.IdentityBackImg);

                                    // Use SixLabors.ImageSharp to resize the back image
                                    using (var backImage = SixLabors.ImageSharp.Image.Load(backImageData))
                                    {
                                        backImage.Mutate(x => x.Resize(new ResizeOptions
                                        {
                                            Size = new SixLabors.ImageSharp.Size(80, 80),
                                            Mode = ResizeMode.Max
                                        }));

                                        // Save the resized back image to a temporary file with a specific extension
                                        string tempBackFileName = Path.ChangeExtension(Path.GetTempFileName(), "png");
                                        backImage.Save(tempBackFileName);

                                        // Add the back image to the worksheet using the temporary file
                                        FileInfo backImageFileInfo = new FileInfo(tempBackFileName);
                                        ExcelPicture backPicture = worksheet.Drawings.AddPicture($"BackImg_{row}", backImageFileInfo);
                                        backPicture.From.Column = 8;
                                        backPicture.From.Row = row-1;
                                        backPicture.SetSize(100, 100);
                                    }
                                }
                            }
                            else
                            {
                                worksheet.Cells[row, 9].Value = "N/A";
                            }
                        }
                        row = row + 4;
                    }

                    // Save the Excel package to create the new file
                    await xlPackage.SaveAsync();
                }
                byte[] array = memoryStream.ToArray();
                return array;
            }
        }

        public async Task<BaseResponsePagingViewModel<CollabReportResponse>> AccountReportList(PagingRequest paging, int accountId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new Exceptions.ErrorResponse(401, (int)AccountReportErrorEnum.UNAUTHORIZED, AccountReportErrorEnum.UNAUTHORIZED.GetDisplayName());
                }

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
