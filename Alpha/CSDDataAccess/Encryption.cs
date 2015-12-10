using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

namespace CSDeveloper
{
    public static class Encryption
    {
        const  String SOURCE_STRING= "aBcDeFgHiJkLmNoPqRsTuVwWyZ_#@$+-/*=!1234567890AbCdEfGhIjKlMnOpQrStUvWxYz";

        public static string CreateToken(int size = 64)
        {
            return CreateToken(DateTime.Now, size); 
        }

        public static string CreateToken(DateTime stamp, int size = 64)
        {
            size = size < 10 ? 16 : size;
            size = size > SOURCE_STRING.Length ? SOURCE_STRING.Length : size;
            string ret = CreateRandomPassword(size);
            int mid =  (int) Math.Floor(Convert.ToDecimal((size - 9)/2));

            ret = ret.Substring(0, mid) + CreateDateTimeHash(stamp) + ret.Substring(mid);
            return ret.Substring(0, size);
        }

        private static DateTime GetTokenTime(string token)
        {
            DateTime ret = DateTime.MinValue;
            int size = token.Length;
            int mid = (int)Math.Floor(Convert.ToDecimal((size - 9) / 2));

            ret = CreateTokenTime(token.Substring(mid, 9)); 
            return ret;
        }

        public static bool Validated(string token, int duration = 5)
        {
            bool ret = false;
            DateTime val = GetTokenTime(token);
            TimeSpan ts = DateTime.Now - val;

            ret = (ts.Minutes < duration) || (ts.Minutes == duration && ts.Seconds == 0); 

            return ret;
        }

        private static DateTime CreateTokenTime(string token)
        {
            DateTime ret = DateTime.MinValue;
            try
            {
                int sec = SOURCE_STRING.IndexOf(token.Substring(0, 1));
                int min = SOURCE_STRING.IndexOf(token.Substring(1, 1));
                int hour = SOURCE_STRING.IndexOf(token.Substring(2, 1));
                int day = SOURCE_STRING.IndexOf(token.Substring(3, 1));
                int month = SOURCE_STRING.IndexOf(token.Substring(4, 1));
                string year = SOURCE_STRING.IndexOf(token.Substring(5, 1)).ToString() +
                                SOURCE_STRING.IndexOf(token.Substring(6, 1)).ToString() +
                                SOURCE_STRING.IndexOf(token.Substring(7, 1)).ToString() +
                                SOURCE_STRING.IndexOf(token.Substring(8, 1)).ToString();
                ret = new DateTime(Convert.ToInt32(year), month, day, hour, min, sec);
            }
            catch
            {
                ret = DateTime.MinValue;
            }
            return ret;
        }

        private static string CreateDateTimeHash(DateTime now)
        {
            char[] year = now.Year.ToString().ToCharArray();  
            string ret = SOURCE_STRING.Substring(now.Second,1) +
                   SOURCE_STRING.Substring(now.Minute,1) +
                   SOURCE_STRING.Substring(now.Hour,1) +
                   SOURCE_STRING.Substring(now.Day, 1) +
                   SOURCE_STRING.Substring(now.Month, 1) +
                   SOURCE_STRING.Substring(Convert.ToInt16(year[0].ToString()), 1) +
                   SOURCE_STRING.Substring(Convert.ToInt16(year[1].ToString()), 1) +
                   SOURCE_STRING.Substring(Convert.ToInt16(year[2].ToString()), 1) +
                   SOURCE_STRING.Substring(Convert.ToInt16(year[3].ToString()), 1);
            return ret;
        }

        public static string CreateRandomPassword(int PasswordLength = 10)
        {
            String _allowedChars = SOURCE_STRING;
            Byte[] randomBytes = new Byte[PasswordLength];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)randomBytes[i] % allowedCharCount];
            }
            return new string(chars);
        }

        public static int CreateRandomSalt()
        {
            Byte[] _saltBytes = new Byte[6];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(_saltBytes);

            return (
                (((int)_saltBytes[4]) << 40) +
                (((int)_saltBytes[0]) << 32) +
                (((int)_saltBytes[3]) << 24) +
                (((int)_saltBytes[5]) << 16) +
                (((int)_saltBytes[1]) << 8) +
                ((int)_saltBytes[2]));
        }

        public static string HashPassword(string password, int salt)
        {
            // Create Byte array of password string
            ASCIIEncoding encoder = new ASCIIEncoding();
            Byte[] _secretBytes = encoder.GetBytes(password);

            // Create a new salt
            Byte[] _saltBytes = new Byte[6];
            _saltBytes[4] = (byte)(salt >> 40);
            _saltBytes[0] = (byte)(salt >> 32);
            _saltBytes[3] = (byte)(salt >> 24);
            _saltBytes[5] = (byte)(salt >> 16);
            _saltBytes[1] = (byte)(salt >> 8);
            _saltBytes[2] = (byte)(salt);

            // append the two arrays
            Byte[] toHash = new Byte[_secretBytes.Length + _saltBytes.Length];
            Array.Copy(_secretBytes, 0, toHash, 0, _secretBytes.Length);
            Array.Copy(_saltBytes, 0, toHash, _secretBytes.Length, _saltBytes.Length);

            SHA1 sha1 = SHA1.Create();
            Byte[] computedHash = sha1.ComputeHash(toHash);

            return encoder.GetString(computedHash);
        }

        public static void EncryptFile(string file, string password, string encriptedFile = "")
        {
            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
            encriptedFile = encriptedFile == string.Empty ? file + ".enc" : encriptedFile;

            File.WriteAllBytes(encriptedFile, bytesEncrypted);
        }

        public static void DecryptFile(string file, string password, string decriptedFile = "")
        {
            byte[] bytesToBeDecrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);
            decriptedFile = decriptedFile == string.Empty ? file + ".dec" : decriptedFile;
            File.WriteAllBytes(decriptedFile, bytesDecrypted);
        }

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 9, 7, 0, 0, 2, 2, 4 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 9, 7, 0, 0, 2, 2, 4 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }
            return decryptedBytes;
        }

    }
}
