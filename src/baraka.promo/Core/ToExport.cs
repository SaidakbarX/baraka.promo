using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using OfficeOpenXml;
using System.Globalization;
using baraka.promo;
using System.Linq.Dynamic.Core;
using baraka.promo.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace baraka.promo.Core
{
    //[ApiController]
    //[Route("export/[action]")]
    ////[ApiExplorerSettings(IgnoreApi = true)]
    public class ToExport
    {
        //readonly ApplicationDbContext _db;
        //public ToExport(ApplicationDbContext db)
        //{
        //    _db = db;
        //}

        #region excel
        public FileStreamResult ToExcel(IQueryable query)
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                var columns = GetProperties(query.ElementType);

                // Filter out columns with no values in the data
                var nonEmptyColumns = columns
                    .Where(column => query.Cast<object>().Any(item => GetValue(item, column.Key) != null))
                    .ToDictionary(column => column.Key, column => column.Value);

                if (nonEmptyColumns.Count == 0)
                {
                    // No non-empty columns, return an empty Excel file
                    package.Save();
                    stream.Seek(0, SeekOrigin.Begin);
                    return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"{query.ElementType}.xls"
                    };
                }

                var columnTypes = nonEmptyColumns.Values.ToArray();
                var rowIndex = 1;

                // Add headers to the worksheet
                for (var i = 1; i <= nonEmptyColumns.Count; i++)
                {
                    worksheet.Cells[rowIndex, i].Value = nonEmptyColumns.Keys.ElementAt(i - 1);
                }

                rowIndex++;

                // Add data to the worksheet
                foreach (var item in query)
                {
                    var propertyValues = nonEmptyColumns
                        .Select(col => GetValue(item, col.Key))
                        .ToArray();

                    for (var i = 1; i <= nonEmptyColumns.Count; i++)
                    {
                        var columnType = columnTypes[i - 1];
                        var value = propertyValues[i - 1];

                        if (columnType != typeof(DateTime) && columnType != typeof(DateTime?))
                        {
                            worksheet.Cells[rowIndex, i].Value = value;
                        }
                        else if (value is DateTime date)
                        {
                            worksheet.Cells[rowIndex, i].Value = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            worksheet.Cells[rowIndex, i].Value = value != null ? value.ToString() : "";
                        }
                    }
                    rowIndex++;
                }
                package.Save();
            }

            if (stream?.Length > 0)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var result = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"{query.ElementType}.xls"
            };
            return result;
        }



        //private string FileName(int id)
        //{
        //    var segment = _db.Segments.FirstOrDefault(a => a.Id == id);
        //    return segment.Name;
        //}
        private Dictionary<string, Type> GetProperties(Type type)
        {
            return type.GetProperties()
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, p => p.PropertyType);
        }

        private object GetValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property?.GetValue(obj);
        }
        #endregion
    }
}
