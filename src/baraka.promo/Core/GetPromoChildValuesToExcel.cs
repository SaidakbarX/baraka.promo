using baraka.promo.Models;
using MediatR;
using baraka.promo.Data;
using Microsoft.Extensions.Caching.Memory;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace baraka.promo.Core
{
    public class GetPromoChildValuesToExcel
    {
        public class Command : IRequest<ApiBaseResultModel<string>>
        {
            public Command(long promo_id)
            {
                PromoId = promo_id;
            }
            public long PromoId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<string>>
        {
            readonly ApplicationDbContext _db;
            readonly ILogger<GetPromoChildValuesToExcel> _logger;
            IMemoryCache _memory_cache;

            public Handler(ApplicationDbContext db, ILogger<GetPromoChildValuesToExcel> logger, IMemoryCache memoryCache)
            {
                _db = db;
                _logger = logger;
                _memory_cache = memoryCache;
            }
            public async Task<ApiBaseResultModel<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var cache_key = "PromoChildValuesInfo_CACHE_KEY=" + request.PromoId;

                    if (!_memory_cache.TryGetValue(cache_key, out List<string> result))
                    {
                        result = _db.PromoChildValues.Where(x => x.PromoId == request.PromoId).Select(z=>z.Name).ToList();
                        _memory_cache.Set(cache_key, result, DateTimeOffset.Now.AddSeconds(5));
                    }

                    //var report = result;
                    //IXLWorkbook workbook = new XLWorkbook();
                    //IXLWorksheet worksheet = workbook.Worksheets.Add("Promos");

                    //worksheet.Cell(1, 1).Value = "Название";
                    ////worksheet.Cell(1, 2).Value = "";
                    ////worksheet.Cell(1, 3).Value = "";

                    //for (int index = 1; index <= report.Count; index++)
                    //{
                    //    worksheet.Cell(index + 1, 1).Value = report[index - 1];
                    //}
                    //using (var stream = new MemoryStream())
                    //{
                    //    workbook.SaveAs(stream);
                    //    var content = Convert.ToBase64String(stream.ToArray());
                    //    return new ApiBaseResultModel<string>(content);
                    //}

                    using (var stream = new MemoryStream())
                    {
                        using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                        {
                            WorkbookPart workbookPart = document.AddWorkbookPart();
                            workbookPart.Workbook = new Workbook();
                            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                            worksheetPart.Worksheet = new Worksheet(new SheetData());

                            Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                            Sheet sheet = new Sheet()
                            {
                                Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                                SheetId = 1,
                                Name = "Promos"
                            };
                            sheets.Append(sheet);

                            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                            //Row headerRow = new Row();
                            //headerRow.Append(new Cell
                            //{
                            //    DataType = CellValues.String,
                            //    CellValue = new CellValue("Название")
                            //});
                            //sheetData.Append(headerRow);

                            for (int index = 0; index < result.Count; index++)
                            {
                                Row row = new Row();
                                row.Append(new Cell
                                {
                                    DataType = CellValues.String,
                                    CellValue = new CellValue(result[index])
                                });
                                sheetData.Append(row);
                            }

                            workbookPart.Workbook.Save();
                        }

                        return new ApiBaseResultModel<string>(Convert.ToBase64String(stream.ToArray()));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"GetPromoChildValues error -> {ex.Message}");
                    return new ApiBaseResultModel<string>();
                }
            }
        }
    }
}
