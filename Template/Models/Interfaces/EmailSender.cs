using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Template.Models.Interfaces;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient("smtp.office365.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("ravitejan011@gmail.com", "Raviteja1234")
        };

        return client.SendMailAsync(
            new MailMessage(from: "ravitejan011@gmail.com",
                            to: email,
                            subject,
                            message
                            ));
    }
}