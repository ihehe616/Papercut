﻿// Papercut
//
// Copyright © 2008 - 2012 Ken Robertson
// Copyright © 2013 - 2017 Jaben Cargman
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Papercut.Service.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class StaticContentController: ControllerBase
    {

        [HttpGet("{*anything}", Order = short.MaxValue)]
        [ResponseCache(
        #if DEBUG
                Duration = 30
#else
                Duration = 600
#endif
            )]
        public IActionResult Get()
        {
            var resourceName = GetRequetedResourceName(Request.Path);
            var resourceContent = GetResourceStream(resourceName);
            if (resourceContent == null)
            {
                return NotFound();
            }

            return new FileStreamResult(resourceContent, GetMimeType(resourceName));
        }

        static string GetRequetedResourceName(string requestUri)
        {
            var filename = requestUri
                        .TrimStart('/')
                        .TrimStart('.')
                        .Replace("%", "")
                        .Replace("$", "")
                        .Replace('/', Path.DirectorySeparatorChar)
                        .Replace(Path.DirectorySeparatorChar, '.');

            if (string.IsNullOrEmpty(filename))
            {
                filename = "index.html";
            }

            return filename;
        }

        static Stream GetResourceStream(string relativePath)
        {
            var currentAssembly = typeof(StaticContentController).GetTypeInfo().Assembly;
            var resource = string.Format(ResourcePath, currentAssembly.GetName().Name, relativePath);

            return currentAssembly.GetManifestResourceStream(resource);
        }

        static string GetMimeType(string filename)
        {
            var extension = Path.GetExtension(filename)?.TrimStart('.');
            string mimeType;
            if (extension == null || !MimeMapping.TryGetValue(extension, out mimeType))
            {
                mimeType = "application/octet-stream";
            }
            return mimeType;
        }

        const string ResourcePath = "{0}.Web.Assets.{1}";
        static readonly Dictionary<string, string> MimeMapping = new Dictionary<string, string>()
        {
            { "htm", "text/html" },
            { "html", "text/html" },
            { "txt", "text/plain" },
            { "js", "text/javascript" },
            { "css", "text/css" },
            { "ico", "image/x-icon" },
            { "png", "image/png" },
            { "jpeg", "image/jpeg" },
            { "jpg", "image/jpeg" },
            { "gif", "image/gif" },
            { "svg", "image/svg+xml" },
            { "ttf", "application/x-font-ttf" },
            { "woff", "application/font-woff" },
            { "woff2", "application/font-woff2" },
        };
        

    }
}