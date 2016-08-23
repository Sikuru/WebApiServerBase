using Microsoft.AspNet.SignalR;
using ServerCore;
using ServerCore.RealTimeChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiServerBase.PConnection
{
    public class RTChatContext : IRTChatContext
    {
        public async Task Send<T>(string connection_id, T packet) where T : RTPResponseBase
        {
            var context = GlobalHost.ConnectionManager.GetConnectionContext<ChatConnection>();
            if (context == null)
            {
                System.Diagnostics.Trace.WriteLine("ChatConnection CONTEXT NOT FOUND");
                return;
            }

            string pn = typeof(T).FullName;
            string pv = Convert.ToBase64String(BinConverter.BinMaker<T>(packet));
            var sb = new StringBuilder();
            sb.Append("{ PN:'").Append(pn).Append("', PV:'").Append(pv).Append("' }");
            await context.Connection.Send(connection_id, sb.ToString());
        }

        public async Task Send<T>(List<string> connection_id_list, T packet) where T : RTPResponseBase
        {
            var context = GlobalHost.ConnectionManager.GetConnectionContext<ChatConnection>();
            if (context == null)
            {
                System.Diagnostics.Trace.WriteLine("ChatConnection CONTEXT NOT FOUND");
                return;
            }

            string pn = typeof(T).FullName;
            string pv = Convert.ToBase64String(BinConverter.BinMaker<T>(packet));
            var sb = new StringBuilder();
            sb.Append("{ PN:'").Append(pn).Append("', PV:'").Append(pv).Append("' }");
            await context.Connection.Send(connection_id_list, sb.ToString());
        }
    }
}
