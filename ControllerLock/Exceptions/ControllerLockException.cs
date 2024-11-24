using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerLock.Exceptions
{
    public class ControllerLockException : Exception
    {
        public ControllerLockException()
        {
        }

        public ControllerLockException(string message)
            : base(message)
        {
        }

    }
}
