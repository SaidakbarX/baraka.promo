using System.Security.Cryptography;
using System.Text;

namespace baraka.promo.Extensions
{
    public static class CodeGenerator
    {
        private static readonly char[] Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        static string NextCode()
        {
            var sb = new StringBuilder(6);
            for (int i = 0; i < 6; i++)
            {
                int idx = RandomNumberGenerator.GetInt32(Chars.Length);
                sb.Append(Chars[idx]);
            }
            return sb.ToString();
        }

        public static List<string> GenerateUniqueCodes(long n)
        {
            if (n < 0) throw new ArgumentException("n must be >= 0");
            if (n > (long)Math.Pow(36, 6)) throw new ArgumentException("n too large");
            var set = new HashSet<string>();
            while (set.Count < n)
            {
                set.Add(NextCode());
            }
            return new List<string>(set);
        }
    }
}
