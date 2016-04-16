
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PDFIndex
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            System.IO.FileInfo fi = null;
            try
            {
                fi = new System.IO.FileInfo(args[1]);
            }
            catch (ArgumentException)
            {
            }
            catch (System.IO.PathTooLongException)
            {
            }
            catch (NotSupportedException)
            {
            }
            if (ReferenceEquals(fi, null))
            {
                Console.WriteLine("Please provide a valid file path.");
                return;
            }

            Dictionary<string, List<int>> wordIndex = BuildIndexFromPdf(args[1]);


        }

        private static Dictionary<string, List<int>> BuildIndexFromPdf(string path)
        {
            var wordIndex = new Dictionary<string, List<int>>();

            using (PdfReader reader = new PdfReader(path))
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    string pageText = PdfTextExtractor.GetTextFromPage(reader, page);

                    string[] words = pageText.Split(' ');

                    foreach (string word in words.Where(w => w.Length > 2))
                    {
                        string w = word.ToLower();
                        if (!wordIndex.ContainsKey(w))
                        {
                            wordIndex[w] = new List<int>();
                        }

                        if (wordIndex[w].Contains(page))
                        {
                            wordIndex[w].Add(page);
                        }
                    }
                }
            }

            return wordIndex;
        }
    }
}
