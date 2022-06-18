using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UltimateCore.AppManagement;
using UltimateCore.EventManagement;
using UltimateCore.LRI;

namespace InsertInto.ModelComponents
{
    internal sealed class InsertWorker : Singleton<InsertWorker>
    {
        #region Constructor

        private InsertWorker()
        {
            _dtpList = new List<DTP>();

            _eventAggregator = EventAggregator.GetInstance();
        }

        #endregion

        #region Fields

        private readonly List<DTP> _dtpList;
        private readonly EventAggregator _eventAggregator;
        #endregion

        #region Properties


        #endregion

        #region Methods

        public Result<List<DTP>> Parser(string pathFile) 
        {
            return new Result<List<DTP>>(() =>
            {

                int pageCount = 0;
                List<string> parseString = new List<string>();

                using (PdfReader pdfReader = new PdfReader(pathFile))
                {
                    pageCount = pdfReader.NumberOfPages;

                    for (int page = 0; page < pageCount; page++)
                    {
                        var strategy = new SimpleTextExtractionStrategy();
                        string text = PdfTextExtractor.GetTextFromPage(pdfReader, page + 1, strategy);
                        text = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(text)));
                        parseString.Add(text);
                    }
                }

                List<string> resLines = new List<string>();

                bool isFirstLine = false;
                char[] arrayChar;

                int j = 0;
                foreach (string page in parseString)
                {
                    var splitLines = page.Split('\n');
                    var lines = new List<string>(splitLines);
                    string idParse;
                    for (int i = 0; i <= lines.Count - 1; i++)
                    {
                        idParse = string.Empty;
                        if (isFirstLine == false)
                        {
                            isFirstLine = true;
                            continue;
                        }

                        arrayChar = lines[i].ToCharArray();
                        for (var c = 0; c < arrayChar.Length - 1; c++) // цикл проверить, присваивает ненужные чары вместо айдишников "Назначительная"
                        {
                            if (arrayChar[c] == ' ')
                                break;
                            idParse += arrayChar[c].ToString();
                        }

                        if (lines[i].StartsWith("36") && idParse.Length == 9)
                        {
                            resLines.Add($"{++j} " + lines[i]);
                            continue;
                        }

                        if (lines[i].StartsWith("Список ДТП"))
                            break;


                        resLines.Insert(resLines.Count - 1, resLines[resLines.Count - 1] + " " + lines[i]);
                        resLines.RemoveAt(resLines.Count - 1);

                    }
                }

                // Логика на сбор данных
                Match matchCoordinates;
                _dtpList.Clear();
                foreach (string line in resLines)
                {
                    int id = 0;
                    double longitude = 0;
                    double latitude = 0;
                    string type = null;
                    MonthEnum tableName = default(MonthEnum);
                    DateTime dateTime = default(DateTime);

                    var resSplit = line.Split(' ');
                    var typeOff = false;
                    for (int i = 0; i < resSplit.Length - 1; i++)
                    {
                        if (int.TryParse(resSplit[i], out int resIntParse) && resSplit[i].Length == 9)
                        {
                            id = resIntParse;
                        }

                        var resMonthParse = MonthEnumParse(resSplit[i]);
                        if (resMonthParse.IsOk && resMonthParse.ResultObject != MonthEnum.None)
                        {
                            tableName = resMonthParse.ResultObject;
                            dateTime = DateTime.Parse(resSplit[i]);
                        }
                        ;
                        if (i > 3 && typeOff == false)
                        {
                            if (resSplit[i].StartsWith("Самарская"))
                            {
                                typeOff = true;
                                continue;
                            }
                            type += resSplit[i] + " ";
                        }

                        matchCoordinates = Regex.Match(resSplit[i], @"(-?\d+(?:\,\d+))");
                        if (matchCoordinates.Success == true)
                        {
                            var rez = matchCoordinates.Groups[1].Value;
                            if (double.TryParse(rez, out double resParse))
                            {
                                if (longitude != resParse && longitude != 0)
                                {
                                    latitude = resParse;
                                    continue;
                                }
                                longitude = resParse;
                            }
                        }
                    }
                    ;
                    _dtpList.Add(new DTP(tableName, id, type, longitude, latitude, line, dateTime));
                }
                var actual = _dtpList.Where(p => p.Latitude != "0" || p.Longitude != "0").ToList();

                _eventAggregator.Push(actual);
                return _dtpList.Where(p => p.Latitude == "0" || p.Longitude == "0").ToList();
            });
        }
        private Result<MonthEnum> MonthEnumParse(string tableName) 
        {
            return new Result<MonthEnum>(() =>
            {
                if (DateTime.TryParseExact(tableName, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resParseEnum))
                {
                    return (MonthEnum)resParseEnum.Month;
                }

                return MonthEnum.None;

            });
        }

        #endregion
    }
}
