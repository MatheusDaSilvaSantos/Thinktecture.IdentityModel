﻿/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Owin
{
    public class ClaimsTransformationMiddleware
    {
        readonly ClaimsTransformationOptions _options;
        readonly Func<IDictionary<string, object>, Task> _next;

        public ClaimsTransformationMiddleware(Func<IDictionary<string, object>, Task> next, ClaimsTransformationOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);
            
            if (context.Authentication != null && 
                context.Authentication.User != null &&
                context.Authentication.User.Identity != null)
            {
                var transformedPrincipal = await _options.ClaimsTransformation(context.Authentication.User);
                context.Authentication.User = new ClaimsPrincipal(transformedPrincipal);
            }

            await _next(env);
        }
    }
}