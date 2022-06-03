using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;
using System.Text;

namespace InsertInto.MVVM
{
    internal class InsertIntoModel
    {
        #region Constructor

        public InsertIntoModel()
        {

        }
        #endregion

        #region Fields

        private string _pathPDF;
        private List<string> _parseString;


        #endregion


        #region Methods

        //Сделать рефакторинг конверта. Разобраться с полями
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
        #endregion
    }
}