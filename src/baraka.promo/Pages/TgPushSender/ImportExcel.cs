using OfficeOpenXml;
using System.Globalization;

namespace baraka.promo.Pages.TgPushSender
{
    public class ImportExcel
    {
        public async Task<List<string>> GetPhoneNumbersFromExcelAsync(Stream fileStream)
        {
            List<string> phoneNumbers = new List<string>();

            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                using (var excelPackage = new ExcelPackage(memoryStream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet != null)
                    {
                        int rowCount = worksheet.Dimension.Rows;
                        for (int row = 1; row <= rowCount; row++)
                        {
                            var cellValue = worksheet.Cells[row, 1].GetValue<string>();
                            // Assuming phone numbers are in the first column
                            if (!string.IsNullOrEmpty(cellValue) && IsPhoneNumber(cellValue))
                            {
                                phoneNumbers.Add(cellValue);
                            }
                        }
                    }
                }
            }
            
            return phoneNumbers;
        }

        public async Task<List<string>> GetPhoneNumbersFromCsvAsync(Stream fileStream)
        {
            List<string> phoneNumbers = new List<string>();

            using (var reader = new StreamReader(fileStream))
            {
                using (var csvReader = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    while (await csvReader.ReadAsync())
                    {
                        var phoneNumber = csvReader.GetField<string>(0); // Assuming phone numbers are in the first column
                        if (!string.IsNullOrEmpty(phoneNumber) && IsPhoneNumber(phoneNumber))
                        {
                            phoneNumbers.Add(phoneNumber);
                        }
                    }
                }
            }

            return phoneNumbers;
        }

        private bool IsPhoneNumber(string value)
        {
            // Implement your phone number validation logic here
            // For simplicity, let's assume all non-empty strings are phone numbers
            return !string.IsNullOrEmpty(value);
        }
    }
}
