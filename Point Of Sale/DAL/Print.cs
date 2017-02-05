using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text.pdf;
using System.IO;

namespace Point_Of_Sale.DAL
{
    class Print
    {
        public static void previewPdfFile(string filename)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = filename;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                throw new Exception("Pdf reader error.\n\nDetailed Error:" + ex.Message);
            }
        }
    }
}
//myProcess.StartInfo.FileName = "acroRd32.exe"; //not the full application path
//myProcess.StartInfo.Arguments = "file.pdf";