using System;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Splix.Net.Utils {
    public class NettyWriter{

        IChannel channels;
        IByteBuffer buffer;
        public NettyWriter(IChannel channels) {
            this.channels = channels;
            this.buffer = channels.Allocator.Buffer();
        }

        public NettyWriter add(int v) { buffer.WriteInt(v); return this; }
        public NettyWriter add(String str) {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            buffer.WriteInt(bytes.Length);
            buffer.WriteBytes(bytes);
            return this;
        }

		public void finishSync() {
			channels.WriteAndFlushAsync(buffer).Wait();
            // buffer.Release();
        }

    }

}