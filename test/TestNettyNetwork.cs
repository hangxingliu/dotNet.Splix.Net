using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Splix.Net.Decoder;
using Splix.Net.Structure;
using Splix.Net.Utils;

namespace Splix.Net.Test {
	public class TestNettyNetwork {
		public static void testMain()  {
            TestEventListener e = new TestEventListener();
            NettyNetwork network = new NettyNetwork(e);
            e.setNetwork(network);
            network.connectSync("192.168.199.233", 25575);
            Console.ReadLine();
        }
	}

	public class TestEventListener: NettyNetworkEvent{

        NettyNetwork network;
        public void setNetwork(NettyNetwork network) {
            this.network = network;
        }

        public override void onNetworkError(Exception e){
            console.log(e);
        }
		public override void onNetworkInit(IChannelHandlerContext context){
            console.log("OnInit");
        }
		public override void onNetworkReceive(IChannelHandlerContext context, IByteBuffer msg){
            console.log("Receive Data:");
            msg.ReadInt();
            IdContainer idc = new IdContainer(100);
            MapDecoder.setGlobalIdContainer(idc);
            Map map = MapDecoder.decodeMap600(msg);
			for(int y = 0;y < 2; y++) {
				for(int x = 0 ; x < 600; x ++ ) {
                    Console.Write(map.mapData[y, x]);
                }
                Console.WriteLine("");
            }
            network.disconnectSync();
        }
	}
}