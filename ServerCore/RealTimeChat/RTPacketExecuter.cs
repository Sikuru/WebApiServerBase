using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.RealTimeChat
{
	public class RTPacketExecuter
	{
		public void Execute(string connection_id, Type packet_type, byte[] packet_bytes)
		{
			var method_info = this.GetType().GetMethod(packet_type.Name);
			if (method_info == null)
			{
				Trace.WriteLine($"RTPacketExecuter.Execute ; {packet_type.Name} METHOD NOT FOUND");
				return;
			}

			try
			{
				var packet = BinConverter.ClassMaker(packet_type, packet_bytes);
				Task.WhenAll(new[] { (Task)method_info.Invoke(this, new[] { connection_id, packet }) });
			}
			catch (Exception e)
			{
				Trace.WriteLine(e.ToString());
			}
		}

		public async Task RTP_RequestChatChannelJoin(string connection_id, RTP_RequestChatChannelJoin packet)
		{
			Trace.WriteLine(packet.ChannelID.ToString());

			int joined_channel_id = 0;
			if (packet.ChannelID == 0)
			{
				// 자동 진입
				for (int i = 1; i <= 999; ++i)
				{
					if (RTChatCore.ChatChannelManager.JoinChannel(i * -1, connection_id, 30) == true)
					{
						joined_channel_id = i;
						break;
					}
				}
			}
			else
			{
				if (RTChatCore.ChatChannelManager.JoinChannel(packet.ChannelID * -1, connection_id) == true)
				{
					joined_channel_id = packet.ChannelID;
				}
			}

			if (joined_channel_id == 0)
			{
				// 진입 실패 (채널 없음)
				await RTChatCore.Context.Send(connection_id, new RTP_ResponseChatChannelJoin()
				{
					ResultCode = ResultCode.OK_Full,
					ChannelID = 0,
				});
			}
			else
			{
				var last_messages = RTChatCore.ChatChannelManager.GetLastMessages(joined_channel_id * -1, packet.LastMessageID);

				// 진입 성공
				await RTChatCore.Context.Send(connection_id, new RTP_ResponseChatChannelJoin()
				{
					ResultCode = ResultCode.OK,
					ChannelID = joined_channel_id,
					LastMessages = last_messages
				});
			}
		}

		public async Task RTP_RequestChatMessage(string connection_id, RTP_RequestChatMessage packet)
		{
			var nickname = RTChatCore.ChatChannelManager.GetNickname(packet.AccountUID);
			if (string.IsNullOrEmpty(nickname) == true)
			{
				return;
			}

			List<string> channel_members;
			SimpleChatMessageInfo message_info;
			if (RTChatCore.ChatChannelManager.AddMessage(connection_id, packet.AccountUID, packet.Message, false, out channel_members, out message_info))
			{
				await RTChatCore.Context.Send(channel_members, new RTP_ResponseChatMessage()
				{
					Message = message_info
				});
			}
		}
	}
}
