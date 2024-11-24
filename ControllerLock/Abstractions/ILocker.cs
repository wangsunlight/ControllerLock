using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerLock.Abstractions
{
    public interface ILocker
    {
        /// <summary>
        /// 锁定
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout"></param>
        /// <param name="isdelay"></param>
        /// <returns></returns>
        public bool Lock(string key, string value, TimeSpan timeout, bool isdelay);

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="isdelay"></param>
        /// <returns></returns>
        public void UnLock(string key, string value, bool isdelay);
    }
}
