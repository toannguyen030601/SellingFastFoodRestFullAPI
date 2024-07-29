using System.Net.Mail;
using System.Net;
using System.Text;

namespace ASM_Client.Models
{
    public class SendMail
    {
        public void Send(string email, string matkhau, bool isMatkhau = false)
        {
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            NetworkCredential cred = new NetworkCredential("toannguyen0251@gmail.com", "olik tgqv sjiv yzcr");
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("toannguyen0251@gmail.com");
            mailMessage.To.Add(email);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div style='border: 1px solid #ccc; padding: 20px; border-radius: 10px;'>");
            sb.AppendLine("<p>Xin chào,</p>");

            if (isMatkhau)
            {
                mailMessage.Subject = "Quên mật khẩu";
                sb.AppendLine("<p>Bạn đã sử dụng tính năng quên mật khẩu.</p>");
                sb.AppendLine("<p>Mật khẩu mới của bạn là: <strong>" + matkhau + "</strong></p>");
                sb.AppendLine("<p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>");
            }
            else
            {
                mailMessage.Subject = "Thông tin đăng nhập!";
                sb.AppendLine("<p>Dưới đây là thông tin đăng nhập của bạn:</p>");
                sb.AppendLine("<p>Mật khẩu của bạn là: <strong>" + matkhau + "</strong></p>");
            }

            sb.AppendLine("<p>Trân trọng,</p>");
            sb.AppendLine("</div>");

            mailMessage.Body = sb.ToString();
            mailMessage.IsBodyHtml = true; // Thiết lập thuộc tính này để cho phép HTML trong email

            smtp.Credentials = cred;
            smtp.EnableSsl = true;

            smtp.Send(mailMessage);
        }

        // Tạo mật khẩu ngẫu nhiên
        public string getPassword()
        {
            Random r = new Random();
            StringBuilder builder = new StringBuilder();
            builder.Append(randomString(4, true));
            builder.Append(r.Next(1000, 9999));
            builder.Append(randomString(2, false));
            return builder.ToString();
        }

        private string randomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random r = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * r.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
            {
                return builder.ToString().ToUpper();
            }
            else
            {
                return builder.ToString().ToLower();
            }
        }
    }
}
