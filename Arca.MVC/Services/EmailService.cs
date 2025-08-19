using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.IO;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void EnviarCorreo(string destinatarios, string asunto, string cuerpo, byte[] archivo = null, string nombreArchivo = null)
    {
        var emailConfig = _configuration.GetSection("EmailSettings");
        var fromAddress = new MailAddress(emailConfig["SenderEmail"], emailConfig["SenderName"]);
        var smtp = new SmtpClient(emailConfig["SmtpServer"], int.Parse(emailConfig["SmtpPort"]))
        {
            Credentials = new NetworkCredential(emailConfig["SenderEmail"], emailConfig["Password"]),
            EnableSsl = true
        };

        var message = new MailMessage()
        {
            From = fromAddress,
            Subject = asunto,
            Body = cuerpo,
            IsBodyHtml = true
        };

        // Agregar destinatarios separados por coma
        foreach (var email in destinatarios.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            message.To.Add(email.Trim());
        }

        // Adjuntar archivo si lo hay
        if (archivo != null && nombreArchivo != null)
        {
            message.Attachments.Add(new Attachment(new MemoryStream(archivo), nombreArchivo));
        }

        smtp.Send(message);
    }
}