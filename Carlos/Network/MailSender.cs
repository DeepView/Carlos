using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Mime;
using System.Net.Mail;
using Carlos.Exceptions;
using System.Threading.Tasks;
namespace Carlos.Network
{
    /// <summary>
    /// 用于发送电子邮件的类。
    /// </summary>
    /// <remarks>值得注意的是，某些电子邮件服务提供商可能并不会开放POP/IMAP/SMTP等服务器接口，而且也不能排除某些电子邮件服提供商要求使用安全密码在其他支持POP/IMAP/SMTP协议的客户端进行登录。</remarks>
    class MailSender
    {
        private SmtpClient mSmtpClient;//主要处理用smtp方式发送此邮件的配置信息（如：邮件服务器、发送端口号、验证方式等等）
        /// <summary>
        /// 构造函数，通过MailMessage实例初始化当前的电子邮件发送实例。
        /// </summary>
        /// <param name="messageInstance">一个MailMessage实例，用于存储收件人，邮件主题，邮件主题等属性的参数。</param>
        /// <param name="server">发件箱的邮件服务器地址。</param>
        /// <param name="userName">发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）。</param>
        /// <param name="password">发件人邮箱密码。</param>
        /// <param name="port">发送邮件所用的端口号（stmp协议默认为25）。</param>
        /// <param name="enableSsl">true表示对邮件内容进行socket层加密传输，false表示不加密。</param>
        /// <param name="isCheckedPassword">true表示对发件人邮箱进行密码验证，false表示不对发件人邮箱进行密码验证。</param>
        public MailSender(MailMessage messageInstance, string server, string userName, string password, string port, bool enableSsl, bool isCheckedPassword)
        {
            MailMessage = messageInstance;
            Server = server;
            SenderUserName = userName;
            SenderPassword = password;
            Port = Convert.ToInt32(port);
            EnableSSL = enableSsl;
            Authentication = isCheckedPassword;
        }
        /// <summary>
        /// 构造函数，初始化当前的电子邮件发送实例，默认启用SSL加密和密码检查。
        /// </summary>
        /// <param name="server">发件箱的邮件服务器地址。</param>
        /// <param name="to">收件人地址（可以是多个收件人，程序中是以“;"进行区分的）。</param>
        /// <param name="from">发件人地址。</param>
        /// <param name="subject">邮件标题。</param>
        /// <param name="body">邮件内容（可以以html格式进行设计）。</param>
        /// <param name="userName">发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）。</param>
        /// <param name="password">发件人邮箱密码。</param>
        /// <param name="port">发送邮件所用的端口号（stmp协议默认为25）。</param>
        public MailSender(string server, string to, string from, string subject, string body, string userName, string password, int port)
        {
            MailMessage = new MailMessage();
            MailMessage.To.Add(to);
            MailMessage.From = new MailAddress(from);
            MailMessage.Subject = subject;
            MailMessage.Body = body;
            MailMessage.IsBodyHtml = true;
            MailMessage.BodyEncoding = Encoding.UTF8;
            MailMessage.Priority = MailPriority.Normal;
            Server = server;
            SenderUserName = userName;
            SenderPassword = password;
            Port = port;
            EnableSSL = true;
            Authentication = true;
        }
        /// <summary>
        /// 构造函数，初始化当前的电子邮件发送实例。
        /// </summary>
        /// <param name="server">发件箱的邮件服务器地址。</param>
        /// <param name="to">收件人地址（可以是多个收件人，程序中是以“;"进行区分的）。</param>
        /// <param name="from">发件人地址。</param>
        /// <param name="subject">邮件标题。</param>
        /// <param name="body">邮件内容（可以以html格式进行设计）。</param>
        /// <param name="userName">发件箱的用户名（即@符号前面的字符串，例如：hello@163.com，用户名为：hello）。</param>
        /// <param name="password">发件人邮箱密码。</param>
        /// <param name="port">发送邮件所用的端口号（stmp协议默认为25）。</param>
        /// <param name="enableSsl">true表示对邮件内容进行socket层加密传输，false表示不加密。</param>
        /// <param name="isCheckedPassword">true表示对发件人邮箱进行密码验证，false表示不对发件人邮箱进行密码验证。</param>
        public MailSender(string server, string to, string from, string subject, string body, string userName, string password, int port, bool enableSsl, bool isCheckedPassword)
        {
            MailMessage = new MailMessage();
            MailMessage.To.Add(to);
            MailMessage.From = new MailAddress(from);
            MailMessage.Subject = subject;
            MailMessage.Body = body;
            MailMessage.IsBodyHtml = true;
            MailMessage.BodyEncoding = Encoding.UTF8;
            MailMessage.Priority = MailPriority.Normal;
            Server = server;
            SenderUserName = userName;
            SenderPassword = password;
            Port = port;
            EnableSSL = enableSsl;
            Authentication = isCheckedPassword;
        }
        /// <summary>
        /// 获取或者设置当前实例的邮件附件。
        /// </summary>
        /// <exception cref="NullReferenceException">当赋予附件集合属性的值为null或者Count=0时，则会抛出这个异常。</exception>
        public AttachmentCollection Attachments
        {
            get => MailMessage.Attachments;
            set
            {
                if (value == null) throw new NullReferenceException("The attachment collection cannot be empty.");
                else Parallel.For(0, value.Count, (index) => MailMessage.Attachments.Add(value[index]));
            }
        }
        /// <summary>
        /// 获取或设置当前实例的SMTP客户端，邮件主体内容可以在这个实例中进行设置。
        /// </summary>
        public MailMessage MailMessage { get; set; }
        /// <summary>
        /// 获取或设置当前实例的SMPT协议。
        /// </summary>
        public SmtpClient SmtpClient { get; set; }
        /// <summary>
        /// 获取或设置当前实例用于发送邮件的端口号。
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 获取或设置当前实例发件箱的邮件服务器地址，这个地址的格式允许为IP形式或字符串形式。
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 获取或设置当前实例的发件人的邮箱帐号密码。
        /// </summary>
        public string SenderPassword { get; set; }
        /// <summary>
        /// 获取或设置当前实例的发件人的用户名称。
        /// </summary>
        public string SenderUserName { get; set; }
        /// <summary>
        /// 获取或设置当前实例的SSL启用标识。
        /// </summary>
        public bool EnableSSL { get; set; }
        /// <summary>
        /// 获取或设置当前实例发件者邮箱密码验证标识。
        /// </summary>
        public bool Authentication { get; set; }
        /// <summary>
        /// 向电子邮件中添加附件。
        /// </summary>
        /// <param name="attachments">附件的路径集合，以分号分隔。</param>
        public void AddAttachments(string attachments)
        {
            string[] path = attachments.Split(';');
            Attachment data;
            ContentDisposition disposition;
            for (int i = 0; i < path.Length; i++)
            {
                data = new Attachment(path[i], MediaTypeNames.Application.Octet);
                disposition = data.ContentDisposition;
                disposition.CreationDate = File.GetCreationTime(path[i]);
                disposition.ModificationDate = File.GetLastWriteTime(path[i]);
                disposition.ReadDate = File.GetLastAccessTime(path[i]);
                MailMessage.Attachments.Add(data);
            }
        }
        /// <summary>
        /// 从电子邮件中移除所有的附件。
        /// </summary>
        public void ClearAttachments() => MailMessage.Attachments.Clear();
        /// <summary>
        /// 发送一封电子邮件。
        /// </summary>
        /// <exception cref="CannotSendMailException">当出现其他异常导致邮件发送失败时，则会统一抛出的异常。</exception>
        public void Send()
        {
            try
            {
                if (MailMessage != null)
                {
                    mSmtpClient = new SmtpClient()
                    {
                        Host = Server,
                        Port = Port,
                        UseDefaultCredentials = false,
                        EnableSsl = EnableSSL
                    };
                    if (Authentication)
                    {
                        NetworkCredential credential = new NetworkCredential(SenderUserName, SenderPassword);
                        mSmtpClient.Credentials = credential.GetCredential(mSmtpClient.Host, mSmtpClient.Port, "NTLM");
                    }
                    else
                    {
                        mSmtpClient.Credentials = new NetworkCredential(SenderUserName, SenderPassword);
                    }
                    mSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mSmtpClient.Send(MailMessage);
                }
            }
            catch { throw new CannotSendMailException(); }
        }
    }
}
