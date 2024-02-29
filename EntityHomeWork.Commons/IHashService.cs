using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityHomeWork.Commons
{
    public interface IHashService
    {
        (byte[] hash, byte[] salt) GetHash(string value, byte[]? key = null);

        public bool VerifyPassword(string enteredPassword, byte[] storedSalt, byte[] storedHash);

    }
}
