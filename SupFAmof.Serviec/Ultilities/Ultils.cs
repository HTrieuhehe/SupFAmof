using System.Text;
using ServiceStack;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Service.Commons;
using System.Collections;
using System.Linq;
using System.Linq.Dynamic.Core;
using SupFAmof.Data.Entity;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Utilities
{
    public static class Ultils
    {
        private const decimal R = 6371.0m;
        public static string GenerateRandomCode()
        {
            var randomCode = new Random();

            string chars = "0123456789";
            int length = 10;
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[randomCode.Next(s.Length)]).ToArray());
        }

        public static int GenerateRandom6DigitNumber()
        {
            var random = new Random();
            int min = 100000; // Giá trị nhỏ nhất có thể tạo (100000)
            int max = 999999; // Giá trị lớn nhất có thể tạo (999999)

            return random.Next(min, max + 1);
        }

        public static byte[] GetHash(string password, string supFAmOf)
        {
            //convert conbine password and config of appsetting
            byte[] byteCode = Encoding.Unicode.GetBytes(string.Concat(password, supFAmOf));

            SHA256Managed hashCode = new SHA256Managed();
            byte[] pass = hashCode.ComputeHash(byteCode);

            return pass;
        }

        public static bool CompareHash(string attemptedPassword, byte[] hash, string salt)
        {
            string base64Hash = Convert.ToBase64String(hash);
            string base64AttemptedHash = Convert.ToBase64String(GetHash(attemptedPassword, salt));

            var result = base64Hash == base64AttemptedHash;
            return result;
        }

        public static string RandomPassword()
        {
            Random pass = new Random();
            var chars = "1234567890qwertyuiopasdfghjklzxcvbnm";
            var length = 10;
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[pass.Next(s.Length)]).ToArray());
        }

        public static string ToSnakeCase(this string o) => Regex.Replace(o, @"(\w)([A-Z])", "$1-$2").ToLower();

        public static bool CheckVNPhone(string phoneNumber)
        {
            string strRegex = @"(^(0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[0-6|8|9]|9[0-4|6-9])[0-9]{7}$)";
            Regex re = new Regex(strRegex);
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (re.IsMatch(phoneNumber))
            {
                return true;
            }
            else
                return false;
        }

        public static (DateTime, DateTime) GetLastAndFirstDateInCurrentMonth()
        {
            var now = DateTime.Now;
            var first = new DateTime(now.Year, now.Month, 1);
            var last = first.AddMonths(1).AddDays(-1);
            return (first, last);
        }

        public static DateTime GetStartOfDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
        }

        public static DateTime GetEndOfDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
        }

        public static bool CheckStudentId(string studentId)
        {
            string strRegex = @"^[A-Za-z]{2}\d{6}$";
            return Regex.IsMatch(studentId, strRegex);
        }

        public static bool CheckPersonalId(string personId)
        {
            string strRegex = @"^\d{12}$";
            return Regex.IsMatch(personId, strRegex);
        }
        public static bool CompareDateTime(DateTime? dateTime1, DateTime? dateTime2, TimeSpan? maxTimeDifference)
        {
            TimeSpan? timeDifference = dateTime2 - dateTime1;
            return timeDifference <= maxTimeDifference;
        }
        public static bool CheckOneDayDifference(DateTime dateFrom, DateTime createAt, int day)
        {
            DateTime createAtDateOnly = createAt.Date;
            DateTime dateFromDateOnly = dateFrom.Date;
            TimeSpan difference = dateFromDateOnly - createAtDateOnly;
            if (difference == TimeSpan.FromDays(day))
            {
                return false;
            }
            return true;
        }

        public static DateTime GetCurrentDatetime()
        {
            TimeZoneInfo hoChiMinhTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hoChiMinhTimeZone);
        }

        public static bool CheckIsOnlyNumber(string number)
        {
            return Regex.IsMatch(number, @"^[0-9]+$");
        }

        public static string RemoveDiacritics(string input)
        {
            // Chuyển đổi chuỗi thành chuỗi không dấu bằng cách sử dụng NormalizationForm.FormD
            string normalizedString = input.Normalize(NormalizationForm.FormD);

            // Tạo bộ ký tự Regex để tìm và thay thế các dấu thanh âm
            Regex regex = new Regex(@"[\p{Mn}]");

            // Thay thế các dấu thanh âm bằng chuỗi trống
            string result = regex.Replace(normalizedString, "");

            return result;
        }
        public static decimal CalculateDistance(decimal? lat1, decimal? lng1, decimal? lat2, decimal? lng2)
        {
            decimal? dLat = ToRadians(lat2 - lat1);
            decimal? dLon = ToRadians(lng2 - lng1);

            // convert to radians
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            decimal a = (decimal)Math.Pow((double)Math.Sin((double)(dLat / 2)), 2) +
                       (decimal)Math.Pow((double)Math.Sin((double)(dLon / 2)), 2) *
                       (decimal)Math.Cos((double)lat1) *
                       (decimal)Math.Cos((double)lat2);
            decimal c = 2 * (decimal)Math.Asin((double)Math.Sqrt((double)a));
            return R * c;
        }

        private static decimal ToRadians(decimal? degrees)
        {
            return (decimal)(degrees * (decimal)Math.PI / 180.0m);
        }

        public static List<string> TurnToExpoPushToken(List<string> tokens)
        {
            List<string> result = new List<string>();
            foreach (var token in tokens)
            {
                result.Add($"ExponentPushToken[{token}]");
            }
            return result;
        }

        public static string NumberToText(double inputNumber)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return result;
        }

        public static bool CheckAccountBanned(ICollection<AccountBanned> accountBanneds)
        {
            if (accountBanneds.Any(x => x.IsActive == true) && accountBanneds.Max(x => x.DayEnd >= Ultils.GetCurrentDatetime()))
            {
                //account in banned process
                return true;
            }
            return false;
        }

    }
}
