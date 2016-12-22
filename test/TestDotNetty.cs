using System;
using System.Net;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Splix.Net.Test {
	public class TestDotNetty {

        public static IChannel clientChannel = null;
        public static IEventLoopGroup group = new MultithreadEventLoopGroup();

        const string SERVER_HOSTNAME = "192.168.199.233";
		const int SERVER_PORT = 2016;

        public static async System.Threading.Tasks.Task testMainSync()
        {
            
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                // .Option(ChannelOption.TcpNodelay, true)
                // .Option(ChannelOption.SoKeepalive, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    //OUT
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                    // pipeline.AddLast(new StringEncoder());
                    //IN
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    // pipeline.AddLast(new StringDecoder());
                    pipeline.AddLast("echo", new MyHandler());
                }));
            clientChannel = await bootstrap.ConnectAsync(IPAddress.Parse(SERVER_HOSTNAME), SERVER_PORT);
            // IByteBuffer buffer = clientChannel.Allocator.Buffer();
            // buffer.WriteByte(88);
            // await clientChannel.WriteAndFlushAsync(buffer);
            Console.ReadLine();
            await clientChannel.CloseAsync();
            group.ShutdownGracefullyAsync().Wait();
        }
    }
	public class MyHandler: ChannelHandlerAdapter {

		public MyHandler() {
        }

        public override void ChannelRead(IChannelHandlerContext context, object message) {
            var msg = ((IByteBuffer)message);
            console.log(msg.ReadInt());
            console.log(msg.ReadLong());
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();


        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
	}
}