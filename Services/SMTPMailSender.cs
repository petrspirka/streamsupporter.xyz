using MailKit.Net.Smtp;
using MimeKit;
using NewStreamSupporter.Contracts;
using System.Net;
using System.Threading;

namespace NewStreamSupporter.Services
{
    /// <summary>
    /// Implementace třídy <see cref="IImplementedEmailSender"/> pro posílání emailů uživatelům (reset hesla, potvrzení registrace...)
    /// </summary>
    public class SMTPMailSender : IImplementedEmailSender
    {
        private readonly NetworkCredential _mailCredential;

        //Z které adresy jdou maily
        private MailboxAddress? _fromAddress;
        private readonly int _port;
        private readonly string _host;

        /// <summary>
        /// Vytvoří novou instanci třídy SMTPMailSender
        /// </summary>
        /// <param name="email">Email, pod kterým se přihlašujeme do SMTP</param>
        /// <param name="password">Heslo, pod kterým se přihlašujeme do SMTP</param>
        /// <param name="host">SMTP host</param>
        /// <param name="port">Port na kterém běží SMTP hosta</param>
        public SMTPMailSender(string email, string password, string host, int port)
        {
            _port = port;
            _host = host;
            _fromAddress = new MailboxAddress(String.Empty, email);
            _mailCredential = new NetworkCredential(email, password);
        }

        /// <inheritdoc/>
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailboxAddress toAddress = new(String.Empty, email);
            MimeMessage message = new()
            {
                From = { _fromAddress },
                To = { toAddress },
                Body = new Multipart
                {
                    {
                        new TextPart("html"){
                            Text = htmlMessage
                        }
                    }
                },
                Subject = subject
            };

            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_host, _port, _port == 465);
            await smtpClient.AuthenticateAsync(_mailCredential);
            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
