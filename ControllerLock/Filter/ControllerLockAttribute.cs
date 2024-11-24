using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ControllerLock.Exceptions;
using ControllerLock.Abstractions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Reflection;

namespace ControllerLock.Filter
{
    /// <summary>
    /// 控制器锁特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ControllerLockAttribute : Attribute, IActionFilter, IDisposable
    {
        /// <summary>
        /// 缓存参数
        /// </summary>
        private readonly string _lockArguments;

        /// <summary>
        /// 超时时间（秒）
        /// </summary>
        private readonly int _timeout;

        /// <summary>
        /// 缓存key是否md5
        /// </summary>
        private readonly bool _isMd5;

        /// <summary>
        /// 是否等待锁
        /// </summary>
        private readonly bool _isDelay;

        private string _key = string.Empty;

        private string _value = string.Empty;

        private ILocker? _locker;

        /// <summary>
        /// 控制器锁
        /// </summary>
        /// <param name="lockArguments">缓存参数</param>
        /// <param name="timeout">超时时间（秒）</param>
        /// <param name="isDelay">是否等待锁</param>
        /// <param name="isMd5">缓存key是否md5</param>
        public ControllerLockAttribute(string lockArguments = "", int timeout = 10, bool isDelay = false, bool isMd5 = false)
        {
            _lockArguments = lockArguments;
            _timeout = timeout;
            _isMd5 = isMd5;
            _isDelay = isDelay;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var sp = context.HttpContext.RequestServices;

            _key = GetLockKey(context);

            _locker = sp.GetRequiredService<ILocker>();
            if (_locker.Lock(_key, _value, TimeSpan.FromSeconds(_timeout), _isDelay))
            {
                return;
            }

            context.Result = sp.GetRequiredService<ILockerResult>().GetActionResult();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            UnLock();
        }

        public void Dispose()
        {
            UnLock();
        }

        /// <summary>
        /// 获取锁key
        /// </summary>
        /// <param name="lockArguments"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetLockKey(ActionExecutingContext context)
        {
            var sb = new StringBuilder();

            var lockArguments = _lockArguments.Split(',');
            if (string.IsNullOrEmpty(_lockArguments) || lockArguments.Length == 0)
            {
                foreach (var item in context.ActionArguments.Select(item => item.Value))
                {
                    if (item == null)
                    {
                        continue;
                    }

                    var properties = item.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        sb.Append(property.GetValue(item)?.ToString());
                        sb.Append('_');
                    }
                }
            }
            else
            {
                foreach (var key in lockArguments)
                {
                    if (context.HttpContext.Items.TryGetValue(key, out var value))
                    {
                        sb.Append(value?.ToString());
                        sb.Append('_');
                    }
                    else if (context.ActionArguments.Select(item => item.Value).Any(a => GetPropertyInfoIgnoreCase(a, key) != null))
                    {
                        foreach (var argument in context.ActionArguments.Select(item => item.Value).Where(item => item != null))
                        {
                            var propertyValue = GetPropertyInfoValueIgnoreCase(argument!, key);
                            if (propertyValue != null)
                            {
                                sb.Append(propertyValue);
                                sb.Append('_');
                            }
                        }
                    }
                    else
                    {
                        throw new ControllerLockException($"lockArguments {key} does not exist");
                    }
                }
            }

            var lockKey = sb.ToString().TrimEnd('_');

            _value = lockKey;

            return _isMd5 ? Md5(lockKey) : lockKey;
        }


        /// <summary>
        /// 解锁
        /// </summary>
        private void UnLock()
        {
            _locker!.UnLock(_key, _value, _isDelay);
        }

        /// <summary>
        /// 获取属性信息数据(忽略大小写)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string? GetPropertyInfoValueIgnoreCase(object obj, string key)
        {
            if (obj == null)
            {
                throw new ControllerLockException($"GetPropertyInfoValueIgnoreCase obj is null");
            }

            var property = GetPropertyInfoIgnoreCase(obj, key);
            return property?.GetValue(obj)?.ToString();
        }

        /// <summary>
        /// 获取属性信息(忽略大小写)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private PropertyInfo? GetPropertyInfoIgnoreCase(object obj, string key)
        {
            if (obj == null)
            {
                throw new ControllerLockException($"GetPropertyInfoIgnoreCase obj is null");
            }

            return obj.GetType().GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string Md5(string str)
        {
            using var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var strResult = BitConverter.ToString(result);
            return strResult.Replace("-", "");
        }
    }
}
