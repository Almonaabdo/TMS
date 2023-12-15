using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMS_Project.Model;

namespace TMS_Project.Model
{
    public class PasswordHasher
    {
        // default constructor
        public PasswordHasher() 
        {
            
        }

        //initialized variables
        private const int KeySize = 256 / 8;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        private const char Delimiter = ';';
        private static readonly byte[] FixedSalt = Encoding.UTF8.GetBytes("MoreSaltForBetterSafety");


        /*
        * METHOD NAME: Hash
        * DESCRIPTION: Converts passwords to random chars 
        * 
        * RETURN: password string
        */
        public string Hash(string password)
        {
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, FixedSalt, Iterations, _hashAlgorithmName, KeySize);

            return string.Join(Delimiter, Convert.ToBase64String(FixedSalt), Convert.ToBase64String(hash));
        }


        /*
        * METHOD NAME: verify
        * DESCRIPTION: verifies the user password
        * 
        * RETURN: bool - true if correct, false otherwise
        */
        public bool verify(string passwordHash, string inputPassword)
        {
            Hash(inputPassword);
            if (passwordHash == inputPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}