using ControllerLock.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ControllerLock.Memory
{
    public class MemoryResult : ILockerResult
    {
        public IActionResult GetActionResult()
        {
            return new JsonResult(new { code = 500, msg = "请求过于频繁，请稍后再试(MemoryResult)" });
        }
    }
}
