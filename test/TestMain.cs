namespace Splix.Net.Test {
    public class TestMain {
        public static void Main(string[] args) {
            console.log("\nSplix.Net包测试程序已启动!");

            // console.log("正在准备测试IDContainer包...");
            // TestIdContainer.testMain();

            // console.log("Testing DotNetty ...");
            // TestDotNetty.testMainSync().Wait();

            // console.log("Testing Utils NettyNetwork ...");
            // TestNettyNetwork.testMain();

            console.log("正在准备测试控制台Splix程序...");
            TestSplixNet.testMain();
        }
    }
}
