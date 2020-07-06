using EmployeeManagementCommon.Models;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Search;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementCommon
{
    public class EmailHelper : IEmailHelper
    {
        private readonly EmailConfiguration _configuration;

        public EmailHelper(EmailConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResultOrHttpError<EmailModel, string>> SendEmailAsync(EmailModel emailModel)
        {
            try
            {
                //using (SmtpClient client = new SmtpClient(_host, 587))
                //{                    
                //    MailMessage mailMessage = new MailMessage();
                //    mailMessage.From = new MailAddress(_from);
                //    mailMessage.BodyEncoding = Encoding.UTF8;
                //    mailMessage.To.Add(emailModel.To);
                //    mailMessage.Body = emailModel.Message;
                //    mailMessage.Subject = emailModel.Subject;
                //    mailMessage.IsBodyHtml = emailModel.IsBodyHtml;
                //    client.Credentials = new NetworkCredential(_userId, _password);
                //    client.EnableSsl = true;
                //    await client.SendMailAsync(mailMessage);
                //    return new ResultOrHttpError<EmailModel, string>(emailModel);
                //}

                var message = new MimeMessage();
                message.To.Add(new MailboxAddress(emailModel.To, emailModel.To));
                message.From.Add(new MailboxAddress(_configuration.From, _configuration.From));

                message.Subject = emailModel.Subject;
                //We will say we are sending HTML. But there are options for plaintext etc. 
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = emailModel.Message
                };

                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using (var emailClient = new SmtpClient())
                {
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect(_configuration.SmtpServer, _configuration.SmtpPort, false);

                    //Remove any OAuth functionality as we won't be using it. 
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                    emailClient.Authenticate(_configuration.SmtpUsername, _configuration.SmtpPassword);

                    await emailClient.SendAsync(message);
                    emailClient.Disconnect(true);
                }
                return new ResultOrHttpError<EmailModel, string>(emailModel);
            }
            catch (Exception ex)
            {
                return new ResultOrHttpError<EmailModel, string>(ex.Message);
            }
        }

        public async Task<ResultOrHttpError<List<EmailMessage>, string>> GetAllLeaveApprovalRejectionEmails()
        {
            try
            {
                List<EmailMessage> emails = new List<EmailMessage>();
                using (var emailClient = new ImapClient())
                {
                    emailClient.Connect(_configuration.ImapServer, _configuration.ImapPort, true);

                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    emailClient.Authenticate(_configuration.ImapUsername, _configuration.ImapPassword);
                    await emailClient.Inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

                    var ids = emailClient.Inbox.Search(SearchQuery.SubjectContains("-request-REQ"));

                    foreach (var id in ids)
                    {
                        
                        var message = await emailClient.Inbox.GetMessageAsync(id);
                        var body = message.TextBody.Replace("\r", "");
                        body = body.Replace("\n", "");
                        var emailMessage = new EmailMessage
                        {
                            Content = body,
                            Subject = message.Subject,
                            EmailId = id
                        };
                        emailMessage.ToAddresses.AddRange(message.To.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                        emailMessage.FromAddresses.AddRange(message.From.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                        emails.Add(emailMessage);
                    }

                    await emailClient.Inbox.CloseAsync();
                }
                return new ResultOrHttpError<List<EmailMessage>, string>(emails);
            }
            catch (Exception ex)
            {
                return new ResultOrHttpError<List<EmailMessage>, string>(ex.Message);
            }
        }


        public async Task DeleteAllApprovalRejectionEmailsAsync(List<EmailMessage> emails)
        {
            using (var emailClient = new ImapClient())
            {
                emailClient.Connect(_configuration.ImapServer, _configuration.ImapPort, true);
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_configuration.ImapUsername, _configuration.ImapPassword);

                await emailClient.Inbox.OpenAsync(MailKit.FolderAccess.ReadWrite);
                await (await emailClient.GetFolderAsync("UpdatedLeaveRelatedEmails")).OpenAsync(MailKit.FolderAccess.ReadWrite);

                foreach (var email in emails)
                {
                    
                    await emailClient.Inbox.MoveToAsync(email.EmailId, await emailClient.GetFolderAsync("UpdatedLeaveRelatedEmails"));
                }
                await (await emailClient.GetFolderAsync("UpdatedLeaveRelatedEmails")).CloseAsync();
                await emailClient.Inbox.CloseAsync();
            }
        }
    }

}
