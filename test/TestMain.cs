namespace Splix.Net.Test {
    public class TestMain {
        public static void Main(string[] args) {
            console.log("\nStart Splix.Net package test program");
            
            console.log("  Testing Utils Id Container ...");
            TestIdContainer.testMain();

            // console.log("  Testing DotNetty ...");
            // TestDotNetty.testMainSync().Wait();

            console.log("  Testing Utils NettyNetwork ...");
            TestNettyNetwork.testMain();
        }
    }
}
