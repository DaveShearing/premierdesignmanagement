using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PremierDesignManagement
{
    public class Security
    {

        private static SHA256CryptoServiceProvider sha256SP = new SHA256CryptoServiceProvider();
        private static RNGCryptoServiceProvider rngSP = new RNGCryptoServiceProvider();

        //Hashes password with salt using SHA256
        public string HashPassword(string password, string salt)
        {

            string hashedPassword;
            byte[] hashByte;
            byte[] passwordByte = Encoding.Default.GetBytes(password);
            byte[] saltByte = Encoding.Default.GetBytes(salt);
            byte[] saltedPassword = new byte[passwordByte.Length + saltByte.Length];

            System.Buffer.BlockCopy(passwordByte, 0, saltedPassword, 0, passwordByte.Length);
            System.Buffer.BlockCopy(saltByte, 0, saltedPassword, passwordByte.Length, salt.Length);

            

            hashByte = sha256SP.ComputeHash(saltedPassword);
            //hashedPassword = System.Text.Encoding.Default.GetString(hashByte);
            hashedPassword = Convert.ToBase64String(hashByte);

            return hashedPassword;

        }

        //Generates salt for password hashing
        public string GenerateSalt()
        {
            string salt;
            byte[] saltByte = new byte[24];

            rngSP.GetBytes(saltByte);
            //salt = System.Text.Encoding.Default.GetString(saltByte);
            salt = Convert.ToBase64String(saltByte);
            
            return salt;
        }

        //Verifies entered password against database hash
        private bool VerifyPassword(string username, string password)
        {
            bool verified = false;




            return verified;
        }



    }
}
