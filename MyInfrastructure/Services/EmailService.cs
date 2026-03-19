using MailKit.Net.Smtp;
using MailKit.Security;
using Marqelle.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Marqelle.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string otpCode)
        {
            await SendEmailAsync(toEmail,
                subject: "Verify your Marqelle account",
                body: $@"
                    <div style='font-family: serif; max-width: 480px; margin: 0 auto; padding: 40px 20px;'>
                        <h2 style='font-size: 28px; font-weight: 400; color: #111; margin-bottom: 8px;'>Marqelle.</h2>
                        <hr style='border: none; border-top: 1px solid #111; margin-bottom: 32px;' />
                        <p style='color: #444; font-size: 14px; margin-bottom: 24px;'>
                            Welcome! Use the code below to verify your email address and complete your registration.
                            This code expires in <strong>10 minutes</strong>.
                        </p>
                        <div style='background: #f5f5f5; border-radius: 12px; padding: 24px; text-align: center; margin-bottom: 24px;'>
                            <span style='font-size: 36px; font-weight: 600; letter-spacing: 12px; color: #111;'>{otpCode}</span>
                        </div>
                        <p style='color: #999; font-size: 12px;'>
                            If you did not create an account, please ignore this email.
                        </p>
                    </div>");
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            await SendEmailAsync(toEmail,
                subject: "Reset your Marqelle password",
                body: $@"
                    <div style='font-family: serif; max-width: 480px; margin: 0 auto; padding: 40px 20px;'>
                        <h2 style='font-size: 28px; font-weight: 400; color: #111; margin-bottom: 8px;'>Marqelle.</h2>
                        <hr style='border: none; border-top: 1px solid #111; margin-bottom: 32px;' />
                        <p style='color: #444; font-size: 14px; margin-bottom: 24px;'>
                            You requested a password reset. Use the code below to set a new password.
                            This code expires in <strong>5 minutes</strong>.
                        </p>
                        <div style='background: #f5f5f5; border-radius: 12px; padding: 24px; text-align: center; margin-bottom: 24px;'>
                            <span style='font-size: 36px; font-weight: 600; letter-spacing: 12px; color: #111;'>{otpCode}</span>
                        </div>
                        <p style='color: #999; font-size: 12px;'>
                            If you did not request a password reset, please ignore this email.
                        </p>
                    </div>");
        }

        public async Task SendLoginNotificationAsync(string toEmail, string firstName)
        {
            await SendEmailAsync(toEmail,
                subject: "New sign-in to your Marqelle account",
                body: $@"
                    <div style='font-family: serif; max-width: 480px; margin: 0 auto; padding: 40px 20px;'>
                        <h2 style='font-size: 28px; font-weight: 400; color: #111; margin-bottom: 8px;'>Marqelle.</h2>
                        <hr style='border: none; border-top: 1px solid #111; margin-bottom: 32px;' />
                        <p style='color: #444; font-size: 14px; margin-bottom: 24px;'>
                            Hi <strong>{firstName}</strong>, you have successfully signed in to your Marqelle account.
                        </p>
                        <p style='color: #444; font-size: 14px; margin-bottom: 24px;'>
                            If this was not you, please reset your password immediately.
                        </p>
                        <p style='color: #999; font-size: 12px;'>Happy shopping at Marqelle!</p>
                    </div>");
        }

        public async Task SendLogoutNotificationAsync(string toEmail, string firstName)
        {
            await SendEmailAsync(toEmail,
                subject: "You have been signed out of Marqelle",
                body: $@"
                    <div style='font-family: serif; max-width: 480px; margin: 0 auto; padding: 40px 20px;'>
                        <h2 style='font-size: 28px; font-weight: 400; color: #111; margin-bottom: 8px;'>Marqelle.</h2>
                        <hr style='border: none; border-top: 1px solid #111; margin-bottom: 32px;' />
                        <p style='color: #444; font-size: 14px; margin-bottom: 24px;'>
                            Hi <strong>{firstName}</strong>, you have been signed out of your Marqelle account.
                        </p>
                        <p style='color: #444; font-size: 14px; margin-bottom: 24px;'>
                            Please login and continue shopping with us.
                        </p>
                        <p style='color: #999; font-size: 12px;'>
                            If you did not sign out, please reset your password immediately.
                        </p>
                    </div>");
        }

        public async Task SendPasswordResetNotificationAsync(string toEmail, string firstName)
        {
            await SendEmailAsync(toEmail,
                subject: "Your Marqelle password has been reset",
                body: $@"
                    <div style='font-family: serif; max-width: 480px; margin: 0 auto; padding: 40px 20px;'>
                        <h2 style='font-size: 28px; font-weight: 400; color: #111; margin-bottom: 8px;'>Marqelle.</h2>
                        <hr style='border: none; border-top: 1px solid #111; margin-bottom: 32px;' />
                        <p style='color: #444; font-size: 14px; margin-bottom: 24px;'>
                            Hi <strong>{firstName}</strong>, your Marqelle account password has been successfully reset.
                        </p>
                        <p style='color: #444; font-size: 14px; margin-bottom: 24px;'>
                            If you did not make this change, please contact us immediately.
                        </p>
                        <p style='color: #999; font-size: 12px;'>Happy shopping at Marqelle!</p>
                    </div>");
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = _config["Email:SmtpHost"];
            var smtpPort = int.Parse(_config["Email:SmtpPort"]);
            var smtpUser = _config["Email:SmtpUser"];
            var smtpPass = _config["Email:SmtpPass"];
            var fromName = _config["Email:FromName"] ?? "Marqelle";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, smtpUser));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUser, smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}