using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public static class Core
    {
        private static int _is_initialized = 0;
        public static bool IsInitialized { get { return _is_initialized == 1; } }
        internal static EncryptManager EncryptManager { get; private set; }

        public static void Initialize(string physical_root_path, string server_encryption_private_key)
        {
            if (Interlocked.Exchange(ref _is_initialized, 1) != 0)
            {
                return;
            }

            EncryptManager = new EncryptManager(server_encryption_private_key);
        }
    }
}
