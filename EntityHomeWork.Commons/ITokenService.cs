using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityHomeWork.Commons
{
    public interface ITokenService
    {
        string GetToken(object user);
    }
}
