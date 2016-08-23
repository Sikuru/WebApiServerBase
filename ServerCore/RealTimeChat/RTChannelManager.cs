using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore.RealTimeChat
{
    public class ChatChannelInfo
    {
        public int ChannelID { get; set; }
        public HashSet<string> ChannelConnections { get; set; }
    }

    public class PlayerChannelInfo
    {
        public string ConnectionID { get; set; }
        public int ChannelID { get; set; }
    }

    public class RTChatChannelManager
    {
        private ConcurrentDictionary<string, PlayerChannelInfo> _connection_channel_dic;
        private ConcurrentDictionary<int, ChatChannelInfo> _channel_dic;
        private ConcurrentDictionary<long, string> _account_nickname_dic;
        private ConcurrentDictionary<long, ConcurrentQueue<SimpleChatMessageInfo>> _channel_log;
        private int _message_id = 0;

        public RTChatChannelManager()
        {
            _account_nickname_dic = new ConcurrentDictionary<long, string>();
            _connection_channel_dic = new ConcurrentDictionary<string, PlayerChannelInfo>();
            _channel_dic = new ConcurrentDictionary<int, ChatChannelInfo>();
            _channel_log = new ConcurrentDictionary<long, ConcurrentQueue<SimpleChatMessageInfo>>();

            // TODO: 임시로 1~999 채널 생성
            for (int i = 1; i <= 999; ++i)
            {
                _channel_dic.TryAdd(i, new ChatChannelInfo()
                {
                    ChannelConnections = new HashSet<string>(),
                    ChannelID = i
                });
            }
        }

        public void AddConnection(string connection_id)
        {
            _connection_channel_dic.TryAdd(connection_id, new PlayerChannelInfo()
            {
                ConnectionID = connection_id
            });

            Trace.WriteLine($"CONCURRENT CONNECTION ; CONCURRENT={_connection_channel_dic.Count}, CID={connection_id}");
        }

        public void RemoveConnection(string connection_id)
        {
            PlayerChannelInfo info;
            if (_connection_channel_dic.TryRemove(connection_id, out info))
            {
                ChatChannelInfo normal_chat_info;
                if (_channel_dic.TryGetValue(info.ChannelID, out normal_chat_info))
                {
                    lock (normal_chat_info)
                    {
                        normal_chat_info.ChannelConnections.Remove(connection_id);
                    }
                }
            }

            Trace.WriteLine($"CONCURRENT CONNECTION ; CONCURRENT={_connection_channel_dic.Count}, CID={connection_id}");
        }

        public bool JoinChannel(int channel_id, string connection_id, int channel_count_max = 30) // TODO: 임시로 채널 인원 30명
        {
            if (channel_id < 0)
            {
                Trace.WriteLine($"RTChannelManager.JoinChannel ; INVALID CHANNEL ID={channel_id}, CID={connection_id}");
                return false;
            }

            PlayerChannelInfo old_channel_info;
            int leave_channel_id = 0;
            if (_connection_channel_dic.TryGetValue(connection_id, out old_channel_info))
            {
                lock (old_channel_info)
                {
                    if (old_channel_info.ChannelID == channel_id)
                    {
                        Trace.WriteLine($"RTChannelManager.JoinChannel ; SAME CHANNEL ID={channel_id}, CID={connection_id}");
                        return true;
                    }

                    leave_channel_id = old_channel_info.ChannelID;
                }
            }

            // 일단 채널에서 이탈
            if (leave_channel_id != 0)
            {
                LeaveChannel(connection_id, leave_channel_id);
            }

            ChatChannelInfo ci;
            if (!_channel_dic.TryGetValue(channel_id, out ci))
            {
                if (channel_id < 0)
                {
                    Trace.WriteLine($"RTChannelManager.JoinChannel ; CHANNEL ID NOT FOUND={channel_id}, CID={connection_id}");
                    return false;
                }
                else
                {
                    ci = new ChatChannelInfo() { ChannelConnections = new HashSet<string>(), ChannelID = channel_id };
                    if (!_channel_dic.TryAdd(channel_id, ci))
                    {
                        Trace.WriteLine($"RTChannelManager.JoinChannel ; CHANNEL CREATE FAILED={channel_id}, CID={connection_id}");
                        return false;
                    }
                }
            }

            // 이미 있는 채널이면...
            bool is_successful = false;
            lock (ci)
            {
                if (ci.ChannelConnections.Count >= channel_count_max)
                {
                    Trace.WriteLine($"RTChannelManager.JoinChannel ; USER OVER ; CHANNEL={channel_id}, CID={connection_id}");
                    return false;
                }

                is_successful = ci.ChannelConnections.Add(connection_id);
            }

            if (is_successful)
            {
                PlayerChannelInfo info;
                if (_connection_channel_dic.TryGetValue(connection_id, out info))
                {
                    lock (info)
                    {
                        info.ChannelID = channel_id;
                        Trace.WriteLine($"RTChannelManager.JoinChannel ; Normal CHANNEL=0->{info.ChannelID}, CID={connection_id}");
                    }
                }

                return true;
            }

            return false;
        }

        public bool LeaveChannel(string connection_id, int channel_id)
        {
            PlayerChannelInfo info;
            if (_connection_channel_dic.TryGetValue(connection_id, out info))
            {
                lock (info)
                {
                    if (info.ChannelID == channel_id)
                    {
                        Trace.WriteLine($"RTChannelManager.LeaveChannel ; CHANNEL={info.ChannelID}->0, CID={connection_id}");
                        info.ChannelID = 0;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if (channel_id != 0)
            {
                ChatChannelInfo ci;
                if (_channel_dic.TryGetValue(channel_id, out ci))
                {
                    lock (ci)
                    {
                        return ci.ChannelConnections.Remove(connection_id);
                    }
                }
            }

            return false;
        }

        public List<string> GetChannel(int channel_id)
        {
            ChatChannelInfo ci;
            if (_channel_dic.TryGetValue(channel_id, out ci))
            {
                lock (ci)
                {
                    return ci.ChannelConnections.ToList();
                }
            }
            else
            {
                return null;
            }
        }

        public List<string> GetChannel(string connection_id)
        {
            PlayerChannelInfo info;
            if (_connection_channel_dic.TryGetValue(connection_id, out info))
            {
                return GetChannel(info.ChannelID);
            }
            else
            {
                return null;
            }
        }

        public void RegisterNickname(long account_uid, string nickname)
        {
            _account_nickname_dic.AddOrUpdate(account_uid, nickname, (key, old_value) => nickname);
        }

        public string GetNickname(long account_uid)
        {
            string nickname;
            if (_account_nickname_dic.TryGetValue(account_uid, out nickname))
            {
                return nickname;
            }
            else
            {
                return "GUEST";
            }
        }

        public bool AddMessage(string connection_id, long account_uid, string message, bool is_guild_message, out List<string> channel_members, out SimpleChatMessageInfo message_info)
        {
            int message_id = Interlocked.Increment(ref _message_id);

            PlayerChannelInfo info;
            if (_connection_channel_dic.TryGetValue(connection_id, out info))
            {
                int channel_id = info.ChannelID;
                channel_members = GetChannel(channel_id);
                if (channel_members != null)
                {
                    string nickname = GetNickname(account_uid);
                    if (string.IsNullOrEmpty(nickname) == false)
                    {
                        message_info = new SimpleChatMessageInfo()
                        {
                            AccountUID = account_uid,
                            ID = message_id,
                            Message = message,
                            Nickname = nickname
                        };

                        ConcurrentQueue<SimpleChatMessageInfo> queue = null;
                        if (!_channel_log.TryGetValue(channel_id, out queue))
                        {
                            queue = new ConcurrentQueue<SimpleChatMessageInfo>();
                            _channel_log.TryAdd(channel_id, queue);
                        }

                        queue.Enqueue(message_info);
                        while (queue.Count > 100)
                        {
                            SimpleChatMessageInfo dummy;
                            queue.TryDequeue(out dummy);
                        }
                        return true;
                    }
                }
            }

            channel_members = null;
            message_info = null;
            return false;
        }

        public List<SimpleChatMessageInfo> GetLastMessages(long channel_id, int message_id = 0)
        {
            ConcurrentQueue<SimpleChatMessageInfo> queue;
            if (_channel_log.TryGetValue(channel_id, out queue))
            {
                if (message_id > 0)
                {
                    return queue.ToList().Where(w => w.ID > message_id).ToList();
                }
                else
                {
                    return queue.ToList();
                }
            }

            return null;
        }
    }
}
