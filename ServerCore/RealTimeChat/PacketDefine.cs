using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.RealTimeChat
{
    public enum ResultCode
    {
        Error = 0,
        OK = 1,
        OK_Full,
    }

    public class RTPData
    {
        public string PN { get; set; }
        public string PV { get; set; }
    }

    public class SimpleChatMessageInfo : BinConvertable
    {
        public int ID { get; set; }
        public long AccountUID { get; set; }
        public string Nickname { get; set; }
        public string Message { get; set; }
    }

    public abstract class RTPRequestBase : BinConvertable
    {
    }

    public abstract class RTPResponseBase : BinConvertable
    {
    }

    /// <summary>
	/// 채팅 채널 진입 요청
	/// </summary>
	public class RTP_RequestChatChannelJoin : RTPRequestBase
    {
        /// <summary>
        /// 채널 아이디
        /// </summary>
        public int ChannelID { get; set; }
        /// <summary>
        /// 마지막 메시지 아이디. 0 인 경우 최근 100개. 아이디가 있는 경우 그 아이디 이후
        /// </summary>
        public int LastMessageID { get; set; }
    }

    public class RTP_ResponseChatChannelJoin : RTPResponseBase
    {
        public ResultCode ResultCode { get; set; }
        /// <summary>
        /// 진입한 채널 아이디 (실패인 경우 0)
        /// </summary>
        public int ChannelID { get; set; }
        /// <summary>
        /// 마지막 메시지
        /// </summary>
        public List<SimpleChatMessageInfo> LastMessages { get; set; }
    }

    /// <summary>
    /// 채팅 채널 메시지
    /// </summary>
    public class RTP_RequestChatMessage : RTPRequestBase
    {
        public long AccountUID { get; set; }
        public string Message { get; set; }
    }

    public class RTP_ResponseChatMessage : RTPResponseBase
    {
        public SimpleChatMessageInfo Message { get; set; }
    }
}
