
using System;
using System.Collections.Generic;
using System.Globalization;
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
                fi = new System.IO.FileInfo(args[0]);
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

            string filename = args[0];
            Console.WriteLine("Building word index for \n{0} ...", filename);

            SortedDictionary<string, List<int>> wordIndex = BuildIndexFromPdf(filename);

            var file = new System.IO.StreamWriter("word_index.txt");
            foreach (var word in wordIndex)
            {
                string values = null;
                word.Value.ForEach(v => values += v + ", ");
                file.WriteLine("{0}\t\t{1}", word.Key, values);
            }

            Console.WriteLine("Completed! nb of words: {0}", wordIndex.Count);
        }

        private static SortedDictionary<string, List<int>> BuildIndexFromPdf(string path)
        {
            var wordIndex = new SortedDictionary<string, List<int>>();

            using (PdfReader reader = new PdfReader(path))
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    string pageText = null;
                    try
                    {
                        pageText = PdfTextExtractor.GetTextFromPage(reader, page);
                    }
                    catch (InlineImageUtils.InlineImageParseException e)
                    {
                        Console.WriteLine("Exception! {0}. ", e.Message);
                        Console.WriteLine("Skipping page {0} ...", page);
                        continue;
                    }

                    var sb = new StringBuilder();
                    char[] arr = pageText.ToCharArray();
                    foreach (char c in arr)
                    {
                        if ((char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-'))
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            sb.Append(' ');
                        }
                    }

                    pageText = sb.ToString();
                    string[] words = pageText.Split(new []{' ', '\n'}, StringSplitOptions.RemoveEmptyEntries);

                    int n;
                    foreach (string word in words.Where(w => w.Length > 2 && !int.TryParse(w, out n)))
                    {
                        string w = word.ToLower();
                        if (!wordIndex.ContainsKey(w))
                        {
                            wordIndex[w] = new List<int>();
                        }

                        if (!wordIndex[w].Contains(page))
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
