using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Website.Siegwart.BLL.Services.Interfaces;
using Website.Siegwart.DAL.Data.Configurations;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.BLL.Services.Classes
{
    /// <summary>
    /// Robust SMTP sender using MailKit.
    /// - Resolves host A/AAAA records and tries each IP.
    /// - If no addresses found, it attempts to lookup MX for the domain (via nslookup) and tries that.
    /// - Sanitizes hostnames and IPv6 zone ids to avoid UriFormatException.
    /// </summary>
    public class SmtpEmailSender : IAppEmailSender
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<SmtpSettings> options, ILogger<SmtpEmailSender> logger)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public async Task SendEmailAsync(Email message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(message.To)) throw new ArgumentException("Recipient (To) is required.", nameof(message));

            var mime = BuildMimeMessage(message);

            var hostToTry = _settings.Host?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(hostToTry))
                throw new InvalidOperationException("SMTP host is not configured.");

            IPAddress[] addrs = Array.Empty<IPAddress>();

            try
            {
                addrs = await ResolveHostAddressesAsync(hostToTry).ConfigureAwait(false);

                if (addrs.Length == 0)
                {
                    _logger.LogWarning("No A/AAAA records found for {Host}. Trying MX lookup...", hostToTry);
                    var domain = ExtractDomain(hostToTry);
                    if (!string.IsNullOrEmpty(domain))
                    {
                        var mxHost = await LookupFirstMxAsync(domain).ConfigureAwait(false);
                        if (!string.IsNullOrEmpty(mxHost))
                        {
                            mxHost = SanitizeCandidateHost(mxHost);
                            _logger.LogInformation("MX record found: {MxHost}. Will try resolving it.", mxHost);
                            hostToTry = mxHost;
                            addrs = await ResolveHostAddressesAsync(hostToTry).ConfigureAwait(false);
                        }
                        else
                        {
                            _logger.LogWarning("No MX record found for domain {Domain}", domain);
                        }
                    }
                }

                if (addrs.Length == 0)
                {
                    _logger.LogWarning("No IP addresses resolved for SMTP host or MX. Will attempt connect using hostname directly: {Host}", hostToTry);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while resolving host addresses for {Host}", _settings.Host);
            }

            // Choose socket options
            var socketOptions = _settings.EnableSsl
                ? (_settings.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls)
                : SecureSocketOptions.Auto;

            Exception? lastEx = null;

            // Try using resolved IPs first (sanitize IPv6)
            if (addrs.Length > 0)
            {
                foreach (var ip in addrs)
                {
                    try
                    {
                        using var client = new SmtpClient();
                        client.Timeout = 10000;

                        var endpointHost = FormatIpForConnect(ip);
                        _logger.LogInformation("Trying SMTP connect to {HostIp}:{Port} (original host: {OriginalHost})", endpointHost, _settings.Port, hostToTry);

                        await client.ConnectAsync(endpointHost, _settings.Port, socketOptions).ConfigureAwait(false);
                        if (!string.IsNullOrWhiteSpace(_settings.UserName))
                            await client.AuthenticateAsync(_settings.UserName, _settings.Password).ConfigureAwait(false);

                        await client.SendAsync(mime).ConfigureAwait(false);
                        await client.DisconnectAsync(true).ConfigureAwait(false);

                        _logger.LogInformation("Email sent to {To} via {Ip}", message.To, endpointHost);
                        return;
                    }
                    catch (Exception ex)
                    {
                        lastEx = ex;
                        _logger.LogWarning(ex, "Attempt to send via IP {Ip} failed", ip);
                    }
                }
            }

            // Try sanitized host candidates
            var candidates = new List<string>();
            candidates.Add(SanitizeCandidateHost(hostToTry));
            if (!string.Equals(hostToTry, _settings.Host, StringComparison.OrdinalIgnoreCase) && _settings.Host != null)
                candidates.Add(SanitizeCandidateHost(_settings.Host));

            candidates = candidates.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var candidate in candidates)
            {
                try
                {
                    var sanitized = candidate;
                    // Final check: if IPv6 literal without brackets, wrap in brackets for Uri checks
                    var candidateForCheck = sanitized;
                    if (IPAddress.TryParse(sanitized, out var parsedIp) && parsedIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        candidateForCheck = $"[{sanitized}]";

                    var hostNameType = Uri.CheckHostName(candidateForCheck);
                    if (hostNameType == UriHostNameType.Unknown)
                    {
                        _logger.LogWarning("Sanitized host '{Candidate}' is not a recognized host name. Skipping.", candidate);
                        continue;
                    }

                    using var client = new SmtpClient();
                    client.Timeout = 10000;

                    _logger.LogInformation("Attempting SMTP connect to hostname '{HostCandidate}' (port {Port})", sanitized, _settings.Port);
                    await client.ConnectAsync(sanitized, _settings.Port, socketOptions).ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(_settings.UserName))
                        await client.AuthenticateAsync(_settings.UserName, _settings.Password).ConfigureAwait(false);

                    await client.SendAsync(mime).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);

                    _logger.LogInformation("Email sent to {To} via hostname {Host}", message.To, sanitized);
                    return;
                }
                catch (Exception ex) when (ex is UriFormatException || ex is ArgumentException)
                {
                    lastEx = ex;
                    _logger.LogWarning(ex, "Host candidate '{Candidate}' caused Uri/Argument exception.", candidate);
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    _logger.LogWarning(ex, "Attempt to send via hostname '{Candidate}' failed", candidate);
                }
            }

            throw new InvalidOperationException($"Failed to send email to {message.To}. See inner exception for details.", lastEx);
        }

        // Build MimeMessage with proper From/To/Subject/Body
        private MimeMessage BuildMimeMessage(Email message)
        {
            var mime = new MimeMessage();

            // Determine sender (From) — prefer FromEmail, fallback to UserName
            var fromEmail = !string.IsNullOrWhiteSpace(_settings.FromEmail) ? _settings.FromEmail.Trim() : _settings.UserName?.Trim();
            var fromName = !string.IsNullOrWhiteSpace(_settings.FromName) ? _settings.FromName.Trim() : string.Empty;

            if (string.IsNullOrWhiteSpace(fromEmail))
                throw new InvalidOperationException("SMTP From address is not configured. Set Smtp:FromEmail or Smtp:UserName in configuration.");

            mime.From.Add(new MailboxAddress(fromName, fromEmail));

            // Recipient(s) — support comma-separated
            var recipients = message.To.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(r => r.Trim())
                                       .Where(r => !string.IsNullOrWhiteSpace(r));
            foreach (var r in recipients)
            {
                mime.To.Add(MailboxAddress.Parse(r));
            }

            mime.Subject = message.Subject ?? string.Empty;

            var bodyBuilder = new BodyBuilder();
            if (message.IsHtml) bodyBuilder.HtmlBody = message.Body ?? string.Empty;
            else bodyBuilder.TextBody = message.Body ?? string.Empty;
            mime.Body = bodyBuilder.ToMessageBody();

            return mime;
        }

        private static string ExtractDomain(string host)
        {
            if (string.IsNullOrWhiteSpace(host)) return string.Empty;
            if (IPAddress.TryParse(host, out _)) return string.Empty;
            var parts = host.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return string.Join('.', parts.Skip(parts.Length - 2));
            return host;
        }

        private static async Task<IPAddress[]> ResolveHostAddressesAsync(string host)
        {
            try
            {
                var addrs = await Dns.GetHostAddressesAsync(host).ConfigureAwait(false);
                return addrs ?? Array.Empty<IPAddress>();
            }
            catch
            {
                return Array.Empty<IPAddress>();
            }
        }

        private static string SanitizeCandidateHost(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            var host = raw.Trim();

            // Remove common prefixes that appear in nslookup output
            host = Regex.Replace(host, @"mail exchanger\s*=\s*", "", RegexOptions.IgnoreCase);

            // keep only last token that looks like hostname or ip
            var tokens = host.Split(new[] { ' ', ',', ';', '=' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = tokens.Length - 1; i >= 0; i--)
            {
                var t = tokens[i].Trim().TrimEnd('.');
                if (string.IsNullOrEmpty(t)) continue;
                // If looks like hostname or IP, return it
                if (Regex.IsMatch(t, @"^([A-Za-z0-9\-]+\.)+[A-Za-z]{2,}$") || IPAddress.TryParse(t, out _))
                    return t;
            }

            // fallback: remove whitespace and trailing punctuation
            host = host.Trim().TrimEnd('.', ',', ';');
            return host;
        }

        private static string FormatIpForConnect(IPAddress ip)
        {
            var ipStr = ip.ToString();
            // remove scope id if exists (fe80::...%12)
            var percentIndex = ipStr.IndexOf('%');
            if (percentIndex >= 0)
                ipStr = ipStr.Substring(0, percentIndex);

            // For IPv6 wrap in brackets when passing to ConnectAsync as host string if needed
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                return $"[{ipStr}]";

            return ipStr;
        }

        /// <summary>
        /// Run nslookup -type=mx domain and return first MX host token (best-effort).
        /// </summary>
        private static async Task<string?> LookupFirstMxAsync(string domain)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "nslookup",
                    Arguments = $"-type=mx {domain}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc == null) return null;

                var output = await proc.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
                await proc.WaitForExitAsync().ConfigureAwait(false);

                // find tokens that look like hostnames
                var hostMatches = Regex.Matches(output, @"([A-Za-z0-9\-_]+\.)+[A-Za-z]{2,}", RegexOptions.IgnoreCase);
                foreach (Match m in hostMatches)
                {
                    var candidate = m.Value.Trim().TrimEnd('.');
                    if (!string.IsNullOrEmpty(candidate))
                        return candidate;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}