using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore.RealTimeChat
{
	public static class RTChatCore
	{
		private static int _is_initialized = 0;
		public static bool IsInitialized { get { return _is_initialized == 1; } }
		public static RTPacketExecuter PacketExecuter { get; private set; }
		public static RTChatChannelManager ChatChannelManager { get; private set; }
		public static IRTChatContext Context { get; private set; }

		public static void Initialize(IRTChatContext context)
		{
			if (Interlocked.Exchange(ref _is_initialized, 1) != 0)
			{
				return;
			}

			Context = context;
			PacketExecuter = new RTPacketExecuter();
			ChatChannelManager = new RTChatChannelManager();
		}
	}
}
