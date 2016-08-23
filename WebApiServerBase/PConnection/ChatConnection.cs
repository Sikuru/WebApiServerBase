using Microsoft.AspNet.SignalR;
using ServerCore;
using ServerCore.RealTimeChat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiServerBase.PConnection
{
	public class ChatConnection : PersistentConnection
	{
		protected override Task OnReceived(IRequest request, string connectionId, string data)
		{
			System.Diagnostics.Trace.WriteLine($"ONRECEIVED ; {connectionId} {data}");

			var data_obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RTPData>(data);

			var packet_type = BinConverter.TypeIdentity(data_obj.PN);
			if (packet_type == null)
			{
                System.Diagnostics.Trace.WriteLine($"PacketTypeNotFound ; {data_obj.PN}");
			}
			else
			{
				byte[] packet_bytes = Convert.FromBase64String(data_obj.PV);
				RTDelegatorChat.OnReceived(connectionId, packet_type, packet_bytes);
			}

			return base.OnReceived(request, connectionId, data);
		}

		protected override Task OnConnected(IRequest request, string connectionId)
		{
			RTDelegatorChat.OnConnected(connectionId);

            System.Diagnostics.Trace.WriteLine($"ONCONNECTED ; {connectionId}");
			return base.OnConnected(request, connectionId);
		}

		protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
		{
			RTDelegatorChat.OnDisconnected(connectionId);

            System.Diagnostics.Trace.WriteLine($"ONDISCONNECTED ; {connectionId}, {stopCalled}");
			return base.OnDisconnected(request, connectionId, stopCalled);
		}

		protected override Task OnReconnected(IRequest request, string connectionId)
		{
			RTDelegatorChat.OnReconnected(connectionId);

            System.Diagnostics.Trace.WriteLine($"ONRECONNECTED ; {connectionId}");
			return base.OnReconnected(request, connectionId);
		}
	}
}
