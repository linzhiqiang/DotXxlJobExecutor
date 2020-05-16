﻿using DotXxlJobExecutor.DTO;
using DotXxlJobExecutor.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotXxlJobExecutor
{
    /// <summary>
    /// xxljob执行器中间件，处理token验证
    /// </summary>
    public class XxlJobMiddleware : IMiddleware
    {
        ILogger<XxlJobMiddleware> _logger;
        XxlJobOption _xxlJobOption;
        public XxlJobMiddleware(ILogger<XxlJobMiddleware> logger, XxlJobOption xxlJobOption)
        {
            _logger = logger;
            _xxlJobOption = xxlJobOption;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value.StartsWith("/api/xxljob/", StringComparison.OrdinalIgnoreCase))
            {
                //进行token验证
                context.Request.Headers.TryGetValue(XxlJobConstant.Token, out StringValues token);
                if (token != _xxlJobOption.Token)
                {
                    await context.Response.WriteAsync(JsonUtils.ToJson(ReturnT.Failed("token验证失败")));
                    _logger.LogError($"xxljob token验证失败:{context.Request.Path.Value}");
                    return;
                }
            }

            await next(context);
        }
    }
}
