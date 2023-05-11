using SelectPdf;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using PdfDocument = SelectPdf.PdfDocument;

namespace Template.Models
{
    public class HTMLToPDF
    {
        public string converthtmlTOPDF(string html, string filepath, string filename, string pageOrientation, string templatePwd = "")
        {
            string finalPWD = string.Empty;

            if (!string.IsNullOrEmpty(templatePwd))
            {
                string[] pwdValidation = templatePwd.Split('_');
                foreach (var item in pwdValidation)
                {
                    string firstElement = item;
                    string cleanedFirstElement = new string(firstElement.Where(char.IsLetterOrDigit).ToArray());

                    if (cleanedFirstElement.Length >= 4 && char.IsLetter(cleanedFirstElement[0]) && char.IsLetter(cleanedFirstElement[1])
                        && char.IsDigit(cleanedFirstElement[2]) && char.IsDigit(cleanedFirstElement[3]))
                    {
                        // Looks like a PAN number
                        string panNumber = cleanedFirstElement.Substring(0, 4) + cleanedFirstElement.Substring(5);
                        bool isPanValid = ValidatePANNumber(panNumber);
                        if (!isPanValid)
                        {
                            // handle invalid PAN number
                        }
                    }
                    finalPWD += cleanedFirstElement + "_";
                }
                finalPWD = finalPWD.ToUpper().TrimEnd('_');
            }
            //SendPasswordtoMail(finalPWD, "ravitejan011@gmail.com", "raviteja7071@gmail.com");

            string htmlString = html;

            PdfDocument document = new PdfDocument();

            HtmlToPdf converter = new HtmlToPdf();
            if (!string.IsNullOrEmpty(finalPWD))
            {
                converter.Options.SecurityOptions.UserPassword = finalPWD;
            }

            converter.Options.PdfPageSize = PdfPageSize.A4;
            if (pageOrientation == "landscape")
            {
                converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;

            }
            else
            {
                converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            }
            PdfDocument doc = converter.ConvertHtmlString(htmlString);

            foreach (PdfPage page in doc.Pages)
            {
                document.AddPage(page);
            }

            // Set options after calling ConvertHtmlString
            converter.Options.MarginLeft = 30;
            converter.Options.MarginRight = 30;

            byte[] pdf = doc.Save();
            doc.Close();

            string filePath = Path.Combine(filepath, filename + ".pdf");

            // Check if the file already exists
            if (File.Exists(filePath))
            {
                int counter = 1;
                string newFilename = filename + " (" + counter.ToString() + ")";
                filePath = Path.Combine(filepath, newFilename + ".pdf");

                // Keep incrementing the counter until a unique filename is found
                while (File.Exists(filePath))
                {
                    counter++;
                    newFilename = filename + " (" + counter.ToString() + ")";
                    filePath = Path.Combine(filepath, newFilename + ".pdf");
                }
            }

            // Save the PDF with the unique filename
            File.WriteAllBytes(filePath, pdf);


            return "success";
        }

        private bool ValidatePANNumber(string panNumber)
        {
            // Step 1: Check if PAN number is of correct length
            if (panNumber.Length != 10)
            {
                return false;
            }

            // Step 2: Check if first five characters are letters
            for (int i = 0; i < 5; i++)
            {
                if (!char.IsLetter(panNumber[i]))
                {
                    return false;
                }
            }

            // Step 3: Check if next four characters are digits
            for (int i = 5; i < 9; i++)
            {
                if (!char.IsDigit(panNumber[i]))
                {
                    return false;
                }
            }

            // Step 4: Check if last character is a letter
            if (!char.IsLetter(panNumber[9]))
            {
                return false;
            }

            // All checks passed, PAN number is valid
            return true;
        }

        private string SendPasswordtoMail(string password, string fromEmail, string toEmail)
        {
            string subject = "Password for your account";
            string body = "Your password is: " + password;

            MailMessage message = new MailMessage(fromEmail, toEmail, subject, body);
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("ravitejan011@gmail.com", "Raviteja1234");

            client.Send(message);
            return "";
        }

    }
}
