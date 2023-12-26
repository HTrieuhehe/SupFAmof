using AutoMapper;
using System.Net;
using ServiceStack;
using OfficeOpenXml;
using Service.Commons;
using OfficeOpenXml.Style;
using SupFAmof.Data.Entity;
using System.Globalization;
using OfficeOpenXml.Drawing;
using Path = System.IO.Path;
using LAK.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Utilities;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using static SupFAmof.Service.Helpers.Enum;
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

            }
            catch (Exception ex)
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
                byte[] array = memoryStream.ToArray();
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
                                            Size = new SixLabors.ImageSharp.Size(frontImage.Width, frontImage.Height),
                                            Mode = ResizeMode.Max
                                        }));

                                        // Save the resized front image to a temporary file with a specific extension
                                        string tempFrontFileName = Path.ChangeExtension(Path.GetTempFileName(), "jpeg");
                                        frontImage.Save(tempFrontFileName);

                                        // Add the front image to the worksheet using the temporary file
                                        FileInfo frontImageFileInfo = new FileInfo(tempFrontFileName);
                                        ExcelPicture frontPicture = worksheet.Drawings.AddPicture($"FrontImg_{row}", frontImageFileInfo);
                                        frontPicture.From.Column = 7;
                                        frontPicture.From.Row = row - 1;
                                        frontPicture.SetSize(60, 80);
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
                                            Size = new SixLabors.ImageSharp.Size(backImage.Width, backImage.Height),
                                            Mode = ResizeMode.Max
                                        }));

                                        // Save the resized back image to a temporary file with a specific extension
                                        string tempBackFileName = Path.ChangeExtension(Path.GetTempFileName(), "jpeg");
                                        backImage.Save(tempBackFileName);

                                        // Add the back image to the worksheet using the temporary file
                                        FileInfo backImageFileInfo = new FileInfo(tempBackFileName);
                                        ExcelPicture backPicture = worksheet.Drawings.AddPicture($"BackImg_{row}", backImageFileInfo);
                                        backPicture.From.Column = 8;
                                        backPicture.From.Row = row - 1;
                                        backPicture.SetSize(50, 80);
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


        public async Task<byte[]> GenerateOpendayReportMonthly(int accountId, FinancialReportRequest request)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new Exceptions.ErrorResponse(401, (int)AccountReportErrorEnum.UNAUTHORIZED, AccountReportErrorEnum.UNAUTHORIZED.GetDisplayName());
                }


                var data = await OpenDayMonthlyReportGenerator(request);
                return data;

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        private async Task<byte[]> OpenDayMonthlyReportGenerator(FinancialReportRequest request)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var openDayId = await _unitOfWork.Repository<PostCategory>().FindAsync(x => x.PostCategoryType == PostCategoryStatusEnum.OPD.GetDisplayName().ToUpper().Trim());
            var accounts = _unitOfWork.Repository<Account>().GetAll().Where(x => x.Email.EndsWith("@fpt.edu.vn"));
            var posts = _unitOfWork.Repository<Post>().GetAll().Where(x => x.DateFrom.Year == request.Year && x.DateFrom.Month == request.Month
                                                                    && x.PostCategoryId == openDayId.Id);
            if (posts.Count() <= 0)
            {
                throw new Exceptions.ErrorResponse(400, 400, "Does not have post or account to generate in that month of the year");
            }
            HashSet<DateTime> uniqueDates = new HashSet<DateTime>();
            Dictionary<ExcelRange, Tuple<string, string>> collumJob = new Dictionary<ExcelRange, Tuple<string, string>>();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ExcelPackage xlPackage = new ExcelPackage(memoryStream))
                {
                    int nameRow = 2;
                    int mergeRow = 3;
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add($"OD Tháng {request.Month}");
                    Dictionary<int, string> keyValuePairs = new Dictionary<int, string>
                    {
                        { 1,"STT"},{ 2,"Họ tên"},{ 3,"MSSV"},{ 4,"CMND"},{ 5,"MST"}
                    };
                    foreach (var index in keyValuePairs)
                    {
                        var cellAddress = worksheet.Cells[nameRow,index.Key];
                        var  cellmerge = worksheet.Cells[mergeRow, index.Key];
                        worksheet.Cells[$"{cellAddress}:{cellmerge}"].Merge = true;
                        worksheet.Cells[$"{cellAddress}"].Value = "STT";
                        worksheet.Cells[$"{cellAddress}"].Value = index.Value;
                        worksheet.Cells[$"{cellAddress}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[$"{cellAddress}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[$"{cellAddress}"].Style.Font.Bold = true;

                    }
                    foreach (var post in posts)
                    {
                        uniqueDates.UnionWith(Enumerable.Range(0, ((DateTime)post.DateTo - post.DateFrom).Days + 1)
                                                   .Select(offset => post.DateFrom.AddDays(offset)));

                    }
                    int postCollumn = 6;
                    foreach (var date in uniqueDates)
                    { 
                        var cellAddress = worksheet.Cells[mergeRow,postCollumn];
                        worksheet.Cells[$"{cellAddress}"].Value =  $"{date.ToString("dd/MM")}";
                        worksheet.Cells[$"{cellAddress}"].Style.Font.Bold = true;
                        collumJob.Add(cellAddress, new Tuple<string, string>("OPEN DAY", $"{date:dd/MM}"));
                        postCollumn++;
                    }
                    int postCollumn1 = 6;
                    int count = uniqueDates.Count();
                    var cellAddress1 = worksheet.Cells[nameRow, postCollumn1];
                    var cellAddress2 = worksheet.Cells[nameRow,postCollumn1+ count-1];
                    worksheet.Cells[$"{cellAddress1}:{cellAddress2}"].Merge = true;
                    worksheet.Cells[$"{cellAddress1}"].Value = "Nội dung công việc";
                    worksheet.Cells[$"{cellAddress1}"].Style.Font.Bold = true;
                    worksheet.Cells[$"{cellAddress1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Thành Tiền
                    var columnLetter3 =  count + 5;
                    var  cellAddress3 = worksheet.Cells[2,columnLetter3+1];
                    var cellmerge3 = worksheet.Cells[ 3, columnLetter3+1];
                    worksheet.Cells[$"{cellAddress3}:{cellmerge3}"].Merge = true;
                    worksheet.Cells[$"{cellAddress3}"].Value = "Thành Tiền";
                    worksheet.Cells[$"{cellAddress3}"].Style.Font.Bold = true;
                    worksheet.Cells[$"{cellAddress3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"{cellAddress3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    int valueRow = 4;
                    foreach (var account in accounts)
                    {
                        double? combinedSalary = 0;

                        worksheet.Cells[valueRow, 1].Value = account.Id;
                        worksheet.Cells[valueRow, 2].Value = account.Name;
                        worksheet.Cells[valueRow, 3].Value = account.AccountInformation?.IdStudent;
                        worksheet.Cells[valueRow, 4].Value = account.AccountInformation?.IdentityNumber;
                        worksheet.Cells[valueRow, 5].Value = account.AccountInformation?.TaxNumber;

                        if (account.AccountReports.Any())
                        {
                            Dictionary<DateTime, double?> dateTotalSalary = new Dictionary<DateTime, double?>();
                            foreach (var report in account.AccountReports.Where(x => x.Position.Post.PostCategoryId == openDayId.Id))
                            {
                                combinedSalary += report.Salary;
                                DateTime dateToWork = report.Position.Date.Date;

                                if (dateTotalSalary.TryGetValue(dateToWork, out var totalSalary))
                                {
                                    dateTotalSalary[dateToWork] += report.Salary;
                                }
                                else
                                {
                                    dateTotalSalary.Add(dateToWork, report.Salary);
                                }
                            }

                            foreach (var kvp in dateTotalSalary)
                            {
                                DateTime dateToWork = kvp.Key;
                                double? totalSalary = kvp.Value;

                                foreach (var KVP in collumJob)
                                {
                                    string columnLetter = KVP.Key.ToString().Substring(0, 1);
                                    string matchedCellAddress = $"{columnLetter}{valueRow}";
                                    Tuple<string, string> cellValues = KVP.Value;

                                    string openDayText = cellValues.Item1;
                                    string dateText = cellValues.Item2;

                                    // Use Date property to consider only the date part
                                    if (DateTime.TryParseExact(dateText.Trim(), "dd/MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                                    {
                                        Console.WriteLine($"Checking cell: {matchedCellAddress}, Date: {parsedDate}, Expected Date: {dateToWork}");

                                        if (parsedDate == dateToWork)
                                        {
                                            Console.WriteLine($"Match found for cell: {matchedCellAddress}");
                                            worksheet.Cells[$"{matchedCellAddress}"].Value = totalSalary;
                                            worksheet.Cells[$"{matchedCellAddress}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        worksheet.Cells[valueRow, collumJob.Count + 6].Value = combinedSalary;
                        valueRow++;
                    }
                    worksheet.Cells[$"{worksheet.Cells[1,1]}:{worksheet.Cells[1, columnLetter3 + 1]}"].Merge = true;
                    worksheet.Cells[1,1].Value = $"DANH SÁCH CỘNG TÁC VIÊN HỖ TRỢ TUYỂN SINH THÁNG {request.Month}/{request.Year}";
                    worksheet.Cells[$"{worksheet.Cells[1, 1]}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"{worksheet.Cells[1, 1]}"].Style.Font.Bold = true;
                    await xlPackage.SaveAsync();
                }
                byte[] array = memoryStream.ToArray();
                return array;
            }
        }

        public async Task<byte[]> GenerateTuyenSinhReportMonthly(int accountId, FinancialReportRequest request)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new Exceptions.ErrorResponse(401, (int)AccountReportErrorEnum.UNAUTHORIZED, AccountReportErrorEnum.UNAUTHORIZED.GetDisplayName());
                }


                var data = await TuyenSinhMonthlyReportGenerator(request);
                return data;

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        private async Task<byte[]> TuyenSinhMonthlyReportGenerator(FinancialReportRequest request)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ExcelPackage xlPackage = new ExcelPackage(memoryStream))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add($"OD Tháng {request.Month}");

                    SetTuyenSinhHeader(worksheet, 2, 3, request);
                    SetTuyenSinhData(worksheet, 4, request);

                    await xlPackage.SaveAsync();

                }
                return memoryStream.ToArray();
            }
        }
        private void SetTuyenSinhHeader(ExcelWorksheet worksheet, int nameRow, int mergeRow, FinancialReportRequest request)
        {
            worksheet.Cells[$"A1:J1"].Merge = true;
            worksheet.Cells["A1"].Value = $"DANH SÁCH CỘNG TÁC VIÊN HỖ TRỢ TUYỂN SINH THÁNG {request.Month}/{request.Year}";
            worksheet.Cells["A1"].Style.Font.Bold = true;

            var keyValuePairs = new Dictionary<int, string>
    {
        { 1, "STT" },{ 2, "Họ tên" },{ 3, "MSSV" },{ 4, "CMND" },{ 5, "MST" },{ 6,"Tư vấn lớp 100.000đ /Buổi"},{ 7,"Tổng số buổi "},{ 8,"Thành Tiền"},{9,"Cam Kết"},{10,"Ghi Chú" }
    };

            foreach (var index in keyValuePairs)
            {
                if (index.Key == 6 || index.Key == 7)
                {
                    char columnLetter2 = (char)('A' + index.Key - 1);
                    string cellDown = $"{columnLetter2}{mergeRow}";
                    worksheet.Cells[$"{cellDown}"].Value = index.Value;
                    worksheet.Cells[cellDown].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[cellDown].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[cellDown].Style.Font.Bold = true;

                    continue;
                }
                char columnLetter = (char)('A' + index.Key - 1);
                string cellAddress = $"{columnLetter}{nameRow}";
                string cellmerge = $"{columnLetter}{mergeRow}";

                worksheet.Cells[$"{cellAddress}:{cellmerge}"].Merge = true;
                worksheet.Cells[$"{cellAddress}"].Value = index.Value;
                worksheet.Cells[cellAddress].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[cellAddress].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[cellAddress].Style.Font.Bold = true;

            }
            //Merge Noi Dung cong viec
            char columnMerge1 = (char)('A' + 6 - 1);
            char columnMerge2 = (char)('A' + 7 - 1);
            string cellMerge1 = $"{columnMerge1}{nameRow}";
            string cellMerge2 = $"{columnMerge2}{nameRow}";
            worksheet.Cells[$"{cellMerge1}:{cellMerge2}"].Merge = true;
            worksheet.Cells[$"{cellMerge1}"].Value = "Nội dung công việc";
            worksheet.Cells[cellMerge1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[cellMerge1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[cellMerge1].Style.Font.Bold = true;
        }

        private void SetTuyenSinhData(ExcelWorksheet worksheet, int nameRow, FinancialReportRequest request)
        {
            var tuyenSinhId = _unitOfWork.Repository<PostCategory>().Find(x => x.PostCategoryType == PostCategoryStatusEnum.TVL.GetDisplayName().ToUpper().Trim());
            var accountsWithReports = _unitOfWork.Repository<Account>()
                                                            .GetAll()
                                                            .Where(x => x.Email.EndsWith("@fpt.edu.vn") &&
                                                                   x.AccountReports.Any(report => report.Position.Date.Month == request.Month
                                                                   && report.Position.Post.PostCategoryId == tuyenSinhId.Id
                                                                   && report.Position.Date.Year == request.Year));

            foreach (var account in accountsWithReports)
            {
                worksheet.Cells[nameRow, 1].Value = account.Id;
                worksheet.Cells[nameRow, 2].Value = account.Name;
                worksheet.Cells[nameRow, 3].Value = account.AccountInformation?.IdStudent;
                worksheet.Cells[nameRow, 4].Value = account.AccountInformation?.IdentityNumber;
                worksheet.Cells[nameRow, 5].Value = account.AccountInformation?.TaxNumber;
                Dictionary<DateTime, int> dateCountDictionary = new Dictionary<DateTime, int>();
                double? sum = 0;
                foreach (var report in account.AccountReports.Where(x => x.Position.Post.PostCategoryId == tuyenSinhId.Id))
                {
                    sum += report.Salary;
                    if (dateCountDictionary.TryGetValue(report.Position.Date, out var count))
                    {
                        // Date already exists in the dictionary, increment the count
                        dateCountDictionary[report.Position.Date] = count + 1;
                    }
                    else
                    {
                        // Date doesn't exist in the dictionary, add with count 1
                        dateCountDictionary.Add(report.Position.Date, 1);
                    }
                }
                var cellF3 = worksheet.Cells[nameRow, 6];
                var cellG3 = worksheet.Cells[nameRow, 7];

                foreach (var kvp in dateCountDictionary)
                {
                    cellF3.Style.WrapText = true;
                    cellF3.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    var r1 = cellF3.RichText.Add($"{kvp.Key.ToString("dd/MM")}" + "\r\n");
                    cellF3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cellF3.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //G3
                    cellG3.Style.WrapText = true;
                    cellG3.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    var r2 = cellG3.RichText.Add($"{kvp.Value}" + "\r\n");
                    cellG3.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cellG3.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                worksheet.Cells[nameRow, 8].Value = sum;
                worksheet.Cells[nameRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[nameRow, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //H3
                worksheet.Cells[nameRow, 9].Value = "X";
                worksheet.Cells[nameRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[nameRow, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                nameRow++;
            }
        }



        public async Task<BaseResponseViewModel<dynamic>> GetMoneyFrom12Months(int accountId,int year)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new Exceptions.ErrorResponse(401, (int)AccountReportErrorEnum.UNAUTHORIZED, AccountReportErrorEnum.UNAUTHORIZED.GetDisplayName());
                }
                var report = await _unitOfWork.Repository<AccountReport>().GetWhere(x => x.Position.Date.Year == year);

                if (report == null || !report.Any())
                {
                    return new BaseResponseViewModel<dynamic>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Generate yearly report success",
                            Success = true,
                            ErrorCode = 0
                        }
               ,
                        Data = Enumerable.Repeat(0.0, 12).ToList()
                    };
                }

                var monthlySums = new List<double>();

                for (int i = 1; i <= 12; i++)
                {
                    double sumForMonth = (double)report
                        .Where(x => x.Position.Date.Month == i)
                        .Sum(x => x.Salary);

                    monthlySums.Add(sumForMonth);
                }

                return new BaseResponseViewModel<dynamic>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Generate yearly report success",
                        Success = true,
                        ErrorCode = 0
                    }
                    ,Data = monthlySums
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
