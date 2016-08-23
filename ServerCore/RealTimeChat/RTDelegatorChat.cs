using System;

namespace ServerCore.RealTimeChat
{
    public static class RTDelegatorChat
	{
		public static void OnConnected(string connection_id)
		{
			RTChatCore.ChatChannelManager.AddConnection(connection_id);
		}

		public static void OnDisconnected(string connection_id)
		{
			RTChatCore.ChatChannelManager.RemoveConnection(connection_id);
		}

		public static void OnReconnected(string connection_id)
		{
			RTChatCore.ChatChannelManager.RemoveConnection(connection_id);
			RTChatCore.ChatChannelManager.AddConnection(connection_id);
		}

		public static void OnReceived(string connection_id, Type packet_type, byte[] packet_bytes)
		{
			RTChatCore.PacketExecuter.Execute(connection_id, packet_type, packet_bytes);
		}
	}
}
