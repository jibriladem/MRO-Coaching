using MROCoatching.Abstractions.SkyLight;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Diagnostics;
using System.IO;

namespace MROCoatching.DataObjects.Utilities
{
    public class HtmlToPdfConverter : IHtmlToPdfConverter
    {
        public byte[] Convert(string htmlCode, string pdfName, IHostingEnvironment _environment)
        {
            try
            {
                var inputFileName = $"input_{Guid.NewGuid()}.html";

                string baseDir = _environment.WebRootPath;

                FileIsLocked(Path.Combine(baseDir, "VoucherPDFFiles", "rasterize.js"), FileAccess.ReadWrite, _environment);

                if (!File.Exists(Path.Combine(baseDir, "VoucherPDFFiles", pdfName)))
                {
                    if (File.Exists(Path.Combine(baseDir, "VoucherPDFFiles", inputFileName)))
                        File.Delete(Path.Combine(baseDir, "VoucherPDFFiles", inputFileName));

                    File.WriteAllText(Path.Combine(baseDir, "VoucherPDFFiles", inputFileName), htmlCode);

                    var startInfo = new ProcessStartInfo(Path.Combine(_environment.WebRootPath, "VoucherPDFFiles", "phantomjs.exe"))
                    {
                        WorkingDirectory = Path.Combine(baseDir, "VoucherPDFFiles"),
                        Arguments = $"{"rasterize.js"} \"{inputFileName}\" \"{pdfName}\" \"A4\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                    };
                     
                    using (var process = new Process())
                    {

                        process.StartInfo = startInfo;

                        try
                        {
                            process.Start();

                            // read the output here...
                            var output = process.StandardOutput.ReadToEnd();
                            var errorOutput = process.StandardError.ReadToEnd();
                                 
                            process.WaitForExit(60000);

                            // read the exit code, close process
                            int returnCode = process.ExitCode;

                            process.Close();

                            // if 0 or 2, it worked so return path of pdf
                            if (!((returnCode == 0) || (returnCode == 2)))                                 
                                throw new Exception(errorOutput);
                        }
                        catch (Exception e)
                        {
                            WriteToFile(e.Message, _environment.WebRootPath);
                            return null;
                        }
                    }

                    if (File.Exists(Path.Combine(baseDir, "VoucherPDFFiles", inputFileName)))
                        File.Delete(Path.Combine(baseDir, "VoucherPDFFiles", inputFileName));
                }

                var bytes = File.ReadAllBytes(Path.Combine(baseDir, "VoucherPDFFiles", pdfName));

                return bytes;
            }
            catch (Exception e)
            {
                WriteToFile(e.Message, _environment.WebRootPath);
                return null;
            }
        }

        // Return true if the file is locked for the indicated access.
        private bool FileIsLocked(string filename, FileAccess file_access, IHostingEnvironment _environment)
        {
            // Try to open the file with the indicated access.
            try
            {
                FileStream fs =
                    new FileStream(filename, FileMode.Open, file_access);
                fs.Close();
                return false;
            }
            catch (IOException ex)
            {
                WriteToFile("File is locked by another process", _environment.WebRootPath);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public byte[] Convert(string htmlCode, string pdfName, IHostingEnvironment _environment)
        //{
        //    try
        //    {
        //        var inputFileName = $"input_{Guid.NewGuid()}.html";

        //        string baseDir = _environment.ContentRootPath;
        //        //Path.Combine(baseDir, "urlrewrite.xml")
        //        if (!File.Exists(pdfName))
        //        {
        //            if (File.Exists(inputFileName))
        //                File.Delete(inputFileName);

        //            File.WriteAllText(inputFileName, htmlCode);
        //            var startInfo = new ProcessStartInfo("phantomjs.exe")
        //            {
        //                WorkingDirectory = Environment.CurrentDirectory /*+ "\\ETHolidaysPackage.Utilities\\PDF\\"*/,
        //                Arguments = $"rasterize.js \"{inputFileName}\" \"{pdfName}\" \"A4\"",
        //                UseShellExecute = false,
        //            };

        //            using (var process = new Process())
        //            {
        //                process.StartInfo = startInfo;
        //                process.Start();
        //                process.WaitForExit();
        //            }

        //            if (File.Exists(inputFileName))
        //                File.Delete(inputFileName);
        //        }

        //        var bytes = File.ReadAllBytes(pdfName);

        //        return bytes;
        //    }
        //    catch (Exception e)
        //    {
        //        WriteToFile(e.Message, _environment.WebRootPath);
        //        return null;
        //    }
        //}
        public void WriteToFile(string text, string rootPath)
        {
            try
            {
                string path = Path.Combine(rootPath, $"Logging\\");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = DateTime.Now.ToString("dd-MM-yyyy tt") + " SystemLog.txt";
                string filePath = path + $"{fileName}";
                StreamWriter erroLogWriter = new StreamWriter(filePath, true);
                erroLogWriter.WriteLine(text);
                erroLogWriter.Close();
            }
            catch (Exception e)
            {

            }
        }
    }
}
