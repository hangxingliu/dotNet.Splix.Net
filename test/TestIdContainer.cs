using Splix.Net.Utils;

namespace Splix.Net.Test {
	public class TestIdContainer {
		public static void testMain()  {
            IdContainer idc = new IdContainer(3);

			Assert.equal(idc.getLocalIdByNetworkId(1024), 0, "0");
			Assert.equal(idc.getLocalIdByNetworkId(1025), 1, "1");
			Assert.equal(idc.getLocalIdByNetworkId(1024), 0, "2");
            Assert.equal(idc.getNetworkIdByLocalId(1), 1025, "3");
            Assert.equal(idc.getNetworkIdByLocalId(3), -1, "4");

            idc.popNetworkId(1025);
            Assert.equal(idc.getNetworkIdByLocalId(1025), -1, "5");
            idc.pushNetworkId(1025);
            Assert.equal(idc.getNetworkIdByLocalId(2), 1025, "6");
            idc.pushNetworkId(1026);
            idc.popLocalId(1);
			Assert.equal(idc.getNetworkIdByLocalId(1), 1025, "7");

            console.log("Test IdContainer Success!");
        }
	}
}