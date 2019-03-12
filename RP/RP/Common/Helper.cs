using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Winnovative;
//using Winnovative.WnvHtmlConvert;

namespace RP.Common
{
    public class Helper
    {

        public void GeneratePDF(object obj)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {


                //string htmlContent = "<div> PDF Code </div>"; // you html code (for example table from your page)
                //Document document = new Document();
                //string FileName = Guid.NewGuid().ToString();
                //PdfWriter.GetInstance(document, new FileStream("C:\\...\\...\\PDF\\" + FileName + ".pdf", FileMode.Create));
                //document.Open();

                //HTMLWorker worker = new HTMLWorker(document);
                //worker.Parse(new StringReader(htmlContent));

                //document.Close();

                //***********
                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 10, 10, 10, 10);

                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                Chunk chunk = new Chunk("This is from chunk. ");
                document.Add(chunk);

                Phrase phrase = new Phrase("This is from Phrase.");
                document.Add(phrase);

                Paragraph para = new Paragraph("This is from paragraph.");
                document.Add(para);

                string text = @"you are successfully created PDF file.";
                Paragraph paragraph = new Paragraph();
                paragraph.SpacingBefore = 10;
                paragraph.SpacingAfter = 10;
                paragraph.Alignment = Element.ALIGN_LEFT;
                paragraph.Font = FontFactory.GetFont(FontFactory.HELVETICA, 12f, BaseColor.GREEN);
                paragraph.Add(text);
                document.Add(paragraph);

                document.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                //Response.Clear();
                //Response.ContentType = "application/pdf";

                //string pdfName = "User";
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + pdfName + ".pdf");
                //Response.ContentType = "application/pdf";
                //Response.Buffer = true;
                //Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                //Response.BinaryWrite(bytes);
                //Response.End();
                //Response.Close();
            }
        }


        public class RazorService
        {
            public static string GetRazorTemplate(string template, string key, object item)
            {
                var result = Engine.Razor.RunCompile(template, key, null, item);
                return result;
            }
            public static string GetMd5Hash(string input)
            {
                var md5 = MD5.Create();
                var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                var hash = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                foreach (byte t in hash)
                {
                    sb.Append(t.ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public PdfConverter GetPdfConverter()
        {

            var pdfConverter = new PdfConverter(1524);

            //if (!string.Empty.Equals(ConfigurationManager.AppSettings["LicenseKey"]))
            //    pdfConverter.LicenseKey = "dbM3czdzM3L3crTzd3OzNPMz9PExMTE";
             pdfConverter.LicenseKey ="jqW/rr+uv762ua62oL6uvb+gv7ygt7e3tw==";
            ////pdfConverter.PageWidth = 1524;
            ////pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;
            pdfConverter.PdfDocumentOptions.SinglePage = true;

            //set the PDF page size 
            pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4; //(PdfPageSize)Enum.Parse(typeof(PdfPageSize), "A4");
            // set the PDF compression level
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = (PdfCompressionLevel)Enum.Parse(typeof(PdfCompressionLevel), "Normal");
            // set the PDF page orientation (portrait or landscape)
            ////pdfConverter.PdfDocumentOptions.PdfPageOrientation = (PDFPageOrientation)Enum.Parse(typeof(PDFPageOrientation), "Portrait");
            //set the PDF standard used to generate the PDF document
            ////pdfConverter.PdfStandardSubset = PdfStandardSubset.Full;
            // show or hide header and footer
            pdfConverter.PdfDocumentOptions.ShowHeader = true;
            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            //set the PDF document margins
            pdfConverter.PdfDocumentOptions.LeftMargin = 5;
            pdfConverter.PdfDocumentOptions.RightMargin = 5;
            pdfConverter.PdfDocumentOptions.TopMargin = 2;
            pdfConverter.PdfDocumentOptions.BottomMargin = 2;

            // set if the HTTP links are enabled in the generated PDF
            pdfConverter.PdfDocumentOptions.LiveUrlsEnabled = true;
            ////pdfConverter.AvoidTextBreak = true;
            ////pdfConverter.AvoidImageBreak = true;
            // set if the HTML content is resized if necessary to fit the PDF page width - default is true
            pdfConverter.PdfDocumentOptions.FitWidth = true;
            // set if the PDF page should be automatically resized to the size of the HTML content when FitWidth is false
            pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;
            // embed the true type fonts in the generated PDF document
            pdfConverter.PdfDocumentOptions.EmbedFonts = true;
            // compress the images in PDF with JPEG to reduce the PDF document size - default is true
            pdfConverter.PdfDocumentOptions.JpegCompressionEnabled = true;
            // set if the JavaScript is enabled during conversion 
            ////pdfConverter.ScriptsEnabled = pdfConverter.ScriptsEnabledInImage = true;
            ////pdfConverter.AvoidImageBreak = true;
            pdfConverter.PdfHeaderOptions.HeaderHeight = Convert.ToInt16(70);
            ////pdfConverter.PdfHeaderOptions.DrawHeaderLine = false;
            ////pdfConverter.PdfFooterOptions.ShowPageNumber = false;
            ////pdfConverter.PdfFooterOptions.PageNumberYLocation = 10;
            ////pdfConverter.PdfBookmarkOptions.TagNames = null;
            return pdfConverter;
        }


        static string base64String = null;
        public string ImageToBase64(string filepath)
        {
            //string path = "D:\\sampleImg.jpg";
            var base64String = "";
            string sPath = "~" + filepath;
            sPath = System.Web.Hosting.HostingEnvironment.MapPath(sPath);
            if (File.Exists(sPath))
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(sPath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        base64String = Convert.ToBase64String(imageBytes);
                        //return base64String;
                    }
                }
            }
            return base64String;

        }

   
    }


    public static class CommonMethod
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
      (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}