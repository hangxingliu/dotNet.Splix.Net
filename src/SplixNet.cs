using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Splix.Net.Decoder;
using Splix.Net.Structure;
using Splix.Net.Utils;

namespace Splix.Net {
	public class SplixNet: NettyNetworkEvent{

        SplixNetListener listener;
        NettyNetwork network;
        Exception networkException;

        IdContainer idc;
        string playerUserName;
        string[] playersUserNameCache = new string[1<<8];
		int playerLocalId;

        public SplixNet(SplixNetListener listener) {
            this.listener = listener;
            //初始化用户名缓存
            for (int i = 0; i < playersUserNameCache.Length; i ++ )
				playersUserNameCache[i] = "";
			network = new NettyNetwork(this);
		}

		public int Login(string username, string serverip, int port) {
			username = fixStringLength(username, 19);
			playerUserName = username;
            
            playerLocalId = -1;
            networkException = null;

            idc = new IdContainer(1<<6);
            idc.pushNetworkId(-88);//占用localId = 0,以防止localId=0被使用
            MapDecoder.setGlobalIdContainer(idc);

            network.connectSync(serverip, port);
            //网络错误
            if(networkException != null)
                return -1;
            
            //等待得到玩家ID
            _waitAndGetPlayerId();
            
            //网络错误
            if(networkException != null)
                return -1;
                
            //获取玩家ID出错
            if(playerLocalId < 0)
                return -1;
                
            return playerLocalId;
        }

		public void Logout () => network.disconnectSync();

		public void Move (int direction) =>
            new NettyWriter(network.networkChannel).add(102).add(direction).finishSync();
        
        int _waitAndGetPlayerId() {
            while(networkException == null) {
                if(playerLocalId > 0)
                    return playerLocalId;
            }
            return -1;
        }
        Task _getGameStartSignAsync(IByteBuffer msg) {
            //获取玩家ID
            playerLocalId = idc.pushNetworkId(msg.ReadInt());
            listener.UserLogin(playerUserName, playerLocalId);
            return Task.Run(() => {
                //读取玩家列表
                int playersLength = msg.ReadInt();
                while (playersLength-- > 0) {
                    int id = idc.getLocalIdByNetworkId(msg.ReadInt());
                    String nickName = _readStrFromBytesBuffer(msg);
					playersUserNameCache[id] = nickName;
					//玩家本人
					if (id == playerLocalId)
                        continue;
                    listener.UserIn(nickName, id);
                }
                //读取地图
                Map map = MapDecoder.decodeMap81(msg);
                //回调刷新地图
                listener.RefreshMap(map.centerX, map.centerY, map.mapData);
            });
        }
        String _readStrFromBytesBuffer(IByteBuffer msg){
            int strLen = msg.ReadInt();
            byte[] bs = new byte[strLen];
            msg.ReadBytes(bs);
            return System.Text.Encoding.UTF8.GetString(bs);
        }
        //-----------------------------------------------------------


        public override void onNetworkError(Exception e) {
            Console.WriteLine("SplixNet Exception Report: {0}", this.networkException = e);
            listener.LostConnection();
        }

        public override void onNetworkInit(IChannelHandlerContext context) =>
            new NettyWriter(context.Channel).add(101).add(playerUserName).finishSync();
        public override void onNetworkReceive(IChannelHandlerContext context, IByteBuffer msg) {
            int msgType = msg.ReadInt();
            Console.WriteLine("SplixNet get a package, id: {0}", msgType);
            switch(msgType) {
                case 1://开局
                    _getGameStartSignAsync(msg);
                    break;
                case 2://每秒4次的地图刷新包
                    Map map = MapDecoder.decodeMap81(msg);
                    listener.RefreshMap(map.centerX, map.centerY, map.mapData);
                    return;
                case 3://高分榜
					int size = msg.ReadInt();
					int rank = 1;
					string text = "", textPart = "";
					while(size-- > 0) {
						textPart = (rank < 10 ? "0" : "") + rank + "  " + fixStringLength(
							playersUserNameCache[idc.getLocalIdByNetworkId(msg.ReadInt())], 15) + "  " + 
                            (""+msg.ReadInt()).PadLeft(5) + "\n";
                        if(rank <= 10)
							text += textPart;
						rank++;
					}
                    listener.Scoreboard(text);
					break;
				case 7://任意一个玩家死亡
                    int deadPlayerLocalId = idc.getLocalIdByNetworkId(msg.ReadInt());
                    int deadBecause = msg.ReadInt();

                    int killFromNetworkId = msg.ReadInt(),
                        killFromLocalId = killFromNetworkId < 0 ? -1 : idc.getLocalIdByNetworkId(killFromNetworkId);

                    listener.UserDie(deadPlayerLocalId, deadBecause);
                    break;
                case 6://玩家出生
                    int id = idc.getLocalIdByNetworkId(msg.ReadInt());
                    String nickName = _readStrFromBytesBuffer(msg);
                    playersUserNameCache[id] = nickName;
                    //玩家本人
                    if(id == playerLocalId)
                        return;
                    listener.UserIn(nickName, id);
                    break;
                default:
                    Console.WriteLine("Splix Warning Report: {0}", msgType);
                    break;
            }
        }

        string fixStringLength(string str, int len) {
			int realLen = str.Length;
            if(realLen > len) return str.Substring(0, len - 3) + "...";
			return str.PadRight(len, ' ');
		}
    }
}