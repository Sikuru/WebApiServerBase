using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace WebApiServerBase
{
    public static class WebCore
    {
        public static bool IsInitialized
        {
            get { return ServerCore.Core.IsInitialized; }
        }

        public static void Startup(string physical_root_path)
        {
            ServerCore.Core.Initialize(physical_root_path, WebConfigurationManager.AppSettings.Get("ServerEncryptionPrivateKey"));
        }
    }
}