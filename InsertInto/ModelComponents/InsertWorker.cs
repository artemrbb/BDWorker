using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
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

        public string PathFile { get; private set; }
        public string FileName { get; private set; }


        #endregion

        #region Methods

        public Result<List<DTP>> Parser(string pathFile) 
        {
            return new Result<List<DTP>>(() =>
            {
                PathFile = pathFile;

                int pageCount = 0;
                PdfReader pdfReader = new PdfReader(PathFile);
                pageCount = pdfReader.NumberOfPages;

                var sb = new StringBuilder();
                List<string> parseString = new List<string>();
                for (int page = 1; page <= pageCount; page++)
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    string text = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    text = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(text)));
                    var res = sb.Append(text);
                    parseString.Add(text);
                }

                pdfReader.Close();

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
                        for (var c = 0; c < arrayChar.Length - 1; c++) // цикл проверить
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
                    string tableName = null;

                    var resSplit = line.Split(' ');
                    var typeOff = false;
                    for (int i = 0; i < resSplit.Length - 1; i++)
                    {
                        if (int.TryParse(resSplit[1], out int resIntParse))
                        {
                            id = resIntParse;
                        }

                        if (DateTime.TryParse(resSplit[i], out DateTime resParseDate)) // парсит время, а не дату
                        {
                            switch (resParseDate.Month)
                            {
                                case 01: tableName = "januaryTable"; break;
                                case 02: tableName = "februaryTable"; break;
                                case 03: tableName = "marchTable"; break;
                                case 04: tableName = "aprilTable"; break;
                                case 05: tableName = "mayTable"; break;
                                case 06: tableName = "juneTable"; break;
                                case 07: tableName = "julyTable"; break;
                                case 08: tableName = "augustTable"; break;
                                case 09: tableName = "septemberTable"; break;
                                case 10: tableName = "octoberTable"; break;
                                case 11: tableName = "novemberTable"; break;
                                case 12: tableName = "decemberTable"; break;
                            }
                        }

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
                    _dtpList.Add(new DTP(tableName, id, type, longitude, latitude, line));
                }

                var actual = _dtpList.Where(p => p.Latitude != "0" || p.Longitude != "0").ToList();
                //foreach (var actDtp in actual) 
                //{
                //    actDtp.Dowload();
                //}
                _eventAggregator.Push(actual);

                return _dtpList.Where(p => p.Latitude == "0" || p.Longitude == "0").ToList();
            });

        }
        private void DateParse(string date) 
        {
        }

        #endregion
    }
}
