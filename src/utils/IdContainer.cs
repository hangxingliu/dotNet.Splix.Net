using System;

namespace Splix.Net.Utils {
	class IdContainer {
        int[] ids;//int[localId] = networkId
        int[] freeLocalId;
        int idsLength, freeLocalIdsLength;
        public IdContainer(int size) {
            ids = new int[size];
            freeLocalId = new int[size];
            idsLength = 0;
            freeLocalIdsLength = 0;
        }
        public int getLocalIdByNetworkId(int networkId) {
            int localId = Array.FindIndex(ids, id => id == networkId);
			return localId == -1 ? pushNetworkId(networkId) : localId;
        }
		public int pushNetworkId(int networkId) {
			if(idsLength < ids.Length) {
                ids[idsLength] = networkId;
                return idsLength++;
            }
            //没有顺序空位了
            int newLocalId = freeLocalIdsLength > 0  ? freeLocalId[freeLocalIdsLength--] : 0;
            ids[newLocalId] = networkId;
            return newLocalId;
        }

		public void popNetworkId(int networkId) {
            int localId = getLocalIdByNetworkId(networkId);
			if(localId != -1)
				popLocalId(localId);
        }
		public void popLocalId(int localId) {
			if(localId < 0 || localId >= idsLength)
                return;
            freeLocalId[freeLocalIdsLength++] = localId;
        }

		public int getNetworkIdByLocalId(int localId) {
			if(localId < 0 || localId >= idsLength)
                return -1;
			return ids[localId];
        }

        public int _getFreeLocalIdsLength() => freeLocalIdsLength;
        public int _length() => idsLength;
    }
}