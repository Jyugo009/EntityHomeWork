using EntityHomeWork.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EntityHomeWork.Services
{
    internal class HashService : IHashService
    {
        public (byte[] hash, byte[] salt) GetHash(string value, byte[]? salt = null)
        {
            var hmac = salt != null ? new HMACSHA512(salt) : new HMACSHA512();
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));

            return (hash, hmac.Key);
        }

        public bool VerifyPassword(string enteredPassword, byte[] storedSalt, byte[] storedHash)
        {
            var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i]) return false;
            }

            return true;
        }
    }
}
