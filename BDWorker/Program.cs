using System;

namespace BDWorker
{
    internal class Program
    {
        private static PDFReader _reader;

        static void Main(string[] args)
        {
            Console.WriteLine("Press ENTER");
            Console.ReadLine();
            _reader = new PDFReader(@"C:\Users\Сенин\Documents\DTP123.pdf");
            _reader.Converter();
            _reader.Parser();
        }
    }
}
