using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Splix.Net.Utils {
	public class NettyNetwork: ChannelHandlerAdapter{

        string serverHostName;
        int serverPort;
        public IChannel networkChannel = null;
        MultithreadEventLoopGroup networkGroup = null;
		NettyNetworkEvent eventListener;

        public NettyNetwork(NettyNetworkEvent e) {
            this.eventListener = e;
        }

        public void connectSync(string serverHostName, int serverPort) {
            this.serverHostName = serverHostName;
            this.serverPort = serverPort;
            //先关闭现有连接
            _disconnectAsync().Wait();
            _connectAsync().Wait();
        }

		public void disconnectSync() {
            _disconnectAsync().Wait();
        }

        async Task _disconnectAsync() {
			try {
				if (networkChannel != null) {
					await networkChannel.CloseAsync();
					networkChannel = null;
				}
				if (networkGroup != null) {
					await networkGroup.ShutdownGracefullyAsync();
					networkGroup = null;
				}
			}catch(Exception e) {
                eventListener.onNetworkError(e);
            }
        }

        async Task _connectAsync() {
            try {
                networkGroup = new MultithreadEventLoopGroup();
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(networkGroup)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel => {
                        IChannelPipeline pipeline = channel.Pipeline;
						//OUT
						pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
						//IN
						pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
						pipeline.AddLast(this);
                }));
                networkChannel = await bootstrap.ConnectAsync(IPAddress.Parse(serverHostName), serverPort);
            }catch(Exception e) {
                eventListener.onNetworkError(e);
            }
        }

        public override void ChannelActive(IChannelHandlerContext context) => eventListener.onNetworkInit(context);

        public override void ChannelRead(IChannelHandlerContext context, object message) => 
			eventListener.onNetworkReceive(context, message as IByteBuffer);

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
            //关闭现有连接
            Task.Run(()=>{ _disconnectAsync().Wait(); });
            eventListener.onNetworkError(exception);
        }
    }



	public abstract class NettyNetworkEvent {
        public abstract void onNetworkError(Exception e);
        public abstract void onNetworkInit(IChannelHandlerContext context);
		public abstract void onNetworkReceive(IChannelHandlerContext context, IByteBuffer msg);
    }
}