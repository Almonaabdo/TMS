using System;
using System.Security.Cryptography;
using System.Text;


namespace TMS_Project.Model
{
    public class PasswordHasher
    {

        //initialized variables
        private const int KeySize = 256 / 8;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName HashAlgorithmName = HashAlgorithmName.SHA256;
        private const char Delimiter = ';';
        private static readonly byte[] FixedSalt = Encoding.UTF8.GetBytes("MoreSaltForBetterSafety");


        /*
        * METHOD NAME: Hash
        * DESCRIPTION: Converts passwords to random chars 
        * 
        * RETURN: password string
        */
        public string? Hash(string? password)
        {
            if (password != null)
            {
                var hash = Rfc2898DeriveBytes.Pbkdf2(password, FixedSalt, Iterations, HashAlgorithmName, KeySize);

                return string.Join(Delimiter, Convert.ToBase64String(FixedSalt), Convert.ToBase64String(hash));
            }

            return null;
        }


        /*
        * METHOD NAME: verify
        * DESCRIPTION: verifies the user password
        * 
        * RETURN: bool - true if correct, false otherwise
        */
        public bool Verify(string? passwordHash, string? inputPassword)
        {
            Hash(inputPassword);
            return passwordHash == inputPassword;
        }
    }
}