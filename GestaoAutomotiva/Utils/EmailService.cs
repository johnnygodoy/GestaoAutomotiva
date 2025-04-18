using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    public void EnviarEmailComAnexo(byte[] arquivoPdf, string destinatario, string assunto) {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse("seuemail@dominio.com"));
        message.To.Add(MailboxAddress.Parse(destinatario));
        message.Subject = assunto;

        var builder = new BodyBuilder
        {
            TextBody = "Segue o gráfico em anexo."
        };

        builder.Attachments.Add("planejamento.pdf", arquivoPdf, ContentType.Parse("application/pdf"));
        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        smtp.Connect("smtp.dominio.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        smtp.Authenticate("seuemail@dominio.com", "sua_senha");
        smtp.Send(message);
        smtp.Disconnect(true);
    }
}
