using System;
using DotNetty.Buffers;
using Splix.Net.Structure;
using Splix.Net.Utils;

namespace Splix.Net.Decoder {
	class MapDecoder {

        static IdContainer idc;
        public static void setGlobalIdContainer(IdContainer idc) { MapDecoder.idc = idc; }
        public static Map decodeMap81(IByteBuffer buffer) => decodeMapWithWidth(buffer, 81);
        public static Map decodeMap600(IByteBuffer buffer) {
            Console.WriteLine("  error: decodeMap600 is deprecated!");
            return null;
        }
        public static Map decodeMapWithWidth(IByteBuffer buffer, int width) {
            Map map = new Map(width);
            map.centerX = buffer.ReadInt();
            map.centerY = buffer.ReadInt();
            // Console.WriteLine("Analyze Map cX = {0} cY = {1}", map.centerX, map.centerY);
            int blockType = 0, v = 0;
            int x = 0, y = 0;
            for(int i = 0 , end = width * width ; i < end ; i ++ ){
                //因为服务起发过来的内容是X/Y反转的
                if (y >= width) { y=0; x ++; }
                blockType = buffer.ReadInt();
				switch(blockType) {
                //0: 空气
					case 0: v = 0; break;
                //1: 边界
					case 1: v = -1; break;
                //2: 坚强部分 id
					case 2: v = _combine(buffer.ReadInt(), 2, 0);break;
                //3: 脆弱部分 id
					case 3: v = _combine(buffer.ReadInt(), 1, 0);break;
                //4: 头 id direction
					case 4: v = _combine(buffer.ReadInt(), 0, buffer.ReadInt());break;
                //7: 虚空
                    case 7: v = -1; break;
                    default: 
                        Console.WriteLine("  error: read wrong data from server blockType = {3}, i = {0}, x = {1}, y = {2}", i,x,y, blockType);
                        return null;
                }
                map.mapData[y++, x] = v;
            }
            return map;
        }

		static int _combine(int networkId, int snakeType, int headDirection) =>
            idc.getLocalIdByNetworkId(networkId) * 100 + snakeType * 10 + headDirection;
	}
}