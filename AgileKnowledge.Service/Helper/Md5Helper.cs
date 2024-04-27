using System.Security.Cryptography;
using System.Text;

namespace AgileKnowledge.Service.Helper
{
	public static class Md5Helper
	{
		public static string HashPassword(string password, string salt)
		{
			var saltBytes = Encoding.UTF8.GetBytes(salt);
			var passwordBytes = Encoding.UTF8.GetBytes(password);

			var saltedPasswordBytes = new byte[saltBytes.Length + passwordBytes.Length];
			Array.Copy(saltBytes, 0, saltedPasswordBytes, 0, saltBytes.Length);
			Array.Copy(passwordBytes, 0, saltedPasswordBytes, saltBytes.Length, passwordBytes.Length);

			using MD5 md5 = MD5.Create();
			var hashedBytes = md5.ComputeHash(saltedPasswordBytes);
			var sb = new StringBuilder();
			foreach (var t in hashedBytes)
			{
				sb.Append(t.ToString("x2"));
			}
			return sb.ToString();
		}
	}
}
