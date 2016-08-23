using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerCore.RealTimeChat
{
	public interface IRTChatContext
	{
		Task Send<T>(string connection_id, T packet) where T : RTPResponseBase;
		Task Send<T>(List<string> connection_id_list, T packet) where T : RTPResponseBase;
	}
}
