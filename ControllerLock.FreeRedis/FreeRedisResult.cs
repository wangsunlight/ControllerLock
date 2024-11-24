using ControllerLock.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerLock.FreeRedis
{
    public class FreeRedisResult : ILockerResult
    {
        public IActionResult GetActionResult()
        {
            return new JsonResult(new { code = 500, msg = "请求过于频繁，请稍后再试(FreeRedisResult)" });
        }
    }
}
