using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BDWorker
{
    internal class PDFReader
    {
        #region Constructor

        public PDFReader(string path)
        {
            _pathPDF = path;
        }

        #endregion

        #region Fields

        private string _pathPDF;
        private string _folderName = @"C:\Users\Сенин\Documents\PDFConvert";
        private List<string> _parseString;
        private Regex _regex;
        private List<DTP> _dtpList;

        #endregion

        #region Methods

        public void Converter()
        {
            int pageCount = 0;
            PdfReader pdfReader = new PdfReader(_pathPDF);
            pageCount = pdfReader.NumberOfPages;

            var sb = new StringBuilder();
            _parseString = new List<string>();
            for (int page = 1; page <= pageCount; page++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                string text = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                text = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(text)));
                var res = sb.Append(text);
                _parseString.Add(text);
            }

            pdfReader.Close();
        }


        public void Parser()
        {
            List<string> resLines = new List<string>();

            bool isFirstLine = false;
            int j = 0;
            foreach (string page in _parseString)
            {
                var splitLines = page.Split('\n');
                var lines = new List<string>(splitLines);
                for (int i = 0; i <= lines.Count - 1; i++)                 
                {

                    if (isFirstLine == false)
                    {
                        isFirstLine = true;
                        continue;
                    }


                    if (lines[i].StartsWith("36")) // Надо дороботать и избавиться от этого условия
                    {
                        resLines.Add($"{++j} " + lines[i]);
                        continue;
                    }

                    resLines.Insert(resLines.Count - 1, resLines[resLines.Count - 1] + " " + lines[i]);
                    resLines.RemoveAt(resLines.Count - 1);

                }
            }

            // Логика на сбор данных
            Match matchLine;
            Match matchCoordinates;
            _dtpList = new List<DTP>();
            foreach (string line in resLines)
            {
                int id = 0;
                double longitude = 0;
                double latitude = 0;
                string type = null;

                //matchLine = Regex.Match(line, @"(-?\d+(?:\,\d+))");
                //if (matchLine.Success == false)
                //{
                //    Console.WriteLine("!WARNING!\nВ строчке нет координат, нужно дописать:\n" + line);
                //    // тут надо придумать логику где клиенту выходит сообщение о том что в строчке нет координатов, и необходимо их внести
                //}

                var resSplit = line.Split(' ');
                var typeOff = false;
                for (int i = 0; i < resSplit.Length - 1; i++)
                {
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
                            if (resParse > 52)
                            {
                                latitude = resParse;
                            }
                            else if (resParse < 52 && resParse != 0)
                            {
                                longitude = resParse;
                            }
                        }
                    }
                }
                _dtpList.Add(new DTP("ran", id, type, longitude, latitude));
                foreach (DTP dtp in _dtpList) 
                {
                    Console.WriteLine(dtp.InsertInto());
                }
            }
            Console.ReadLine();
        }

        #endregion
    }
}
