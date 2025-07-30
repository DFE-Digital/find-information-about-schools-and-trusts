using ClosedXML.Excel;
using DfE.FindInformationAcademiesTrusts.Data.Enums;
using DfE.FindInformationAcademiesTrusts.Pages;
using DfE.FindInformationAcademiesTrusts.Services.School;
using DfE.FindInformationAcademiesTrusts.Services.Trust;

namespace DfE.FindInformationAcademiesTrusts.Services.Export
{
    public abstract class ExportBuilder
    {
        private readonly XLWorkbook workbook = new();

        public IXLWorksheet Worksheet { get; set; }

        public readonly int TrustRows = 3;
        public readonly int SchoolRows = 3;
        public readonly int HeaderRows = 1;

        public int CurrentRow { get; set; }

        protected ExportBuilder(string sheetName)
        {
            Worksheet = workbook.Worksheets.Add(sheetName);
        }

        public byte[] Build()
        {
            Worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public ExportBuilder WriteTrustInformation(TrustSummaryServiceModel trustSummary)
        {
            Worksheet.Cell(1, 1).Value = trustSummary.Name;
            Worksheet.Row(1).Style.Font.Bold = true;
            Worksheet.Cell(2, 1).Value = trustSummary.Type;

            CurrentRow += TrustRows;

            return this;
        }

        public ExportBuilder WriteSchoolInformation(SchoolOverviewServiceModel school, SchoolCategory category)
        {
            Worksheet.Cell(1, 1).Value = school.Name;
            Worksheet.Row(1).Style.Font.Bold = true;

            Worksheet.Cell(2, 1).Value = category switch
            {
                SchoolCategory.LaMaintainedSchool => "Not part of a trust",
                SchoolCategory.Academy => $"Joined trust on {school.DateJoinedTrust:dd/MM/yyyy}",
                _ => throw new ArgumentOutOfRangeException(nameof(category))
            };

            CurrentRow += SchoolRows;

            return this;
        }

        public ExportBuilder WriteHeaders(List<string> headers)
        {
            for (var i = 0; i < headers.Count; i++)
            {
                Worksheet.Cell(CurrentRow, i + 1).Value = headers[i];
            }

            Worksheet.Row(CurrentRow).Style.Font.Bold = true;

            CurrentRow += HeaderRows;

            return this;
        }

        internal void SetTextCell(Enum column, string value)
        {
            Worksheet.Cell(CurrentRow, (int)(object)column).SetValue(value);
        }

        public void SetDateCell(Enum column, DateTime? dateValue)
        {
            var cell = Worksheet.Cell(CurrentRow, (int)(object)column);
            if (dateValue.HasValue)
            {
                cell.Value = dateValue.Value;
                cell.Style.NumberFormat.SetFormat(StringFormatConstants.DisplayDateFormat);
            }
            else
            {
                cell.Value = string.Empty;
            }
        }

        public void SetBoolCell(Enum column, bool? boolValue)
        {
            var cell = Worksheet.Cell(CurrentRow, (int)(object)column);
            if (boolValue.HasValue)
            {
                cell.Value = boolValue.Value ? "Yes" : "No";
            }
            else
            {
                cell.Value = string.Empty;
            }
        }

        internal ExportBuilder WriteRows(Action action)
        {
            action.Invoke();

            return this;
        }
    }
}
