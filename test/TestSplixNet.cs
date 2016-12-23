using System;

namespace Splix.Net.Test {
	public class TestSplixNet {

        public static String nickName = "";
        public static String serverName = "";
        public static int serverPort = 25575;

        public static bool isDead = false;
        public static int myId = -1;

        public static bool doYouWantToShowMap = false;

        public static void testMain(){
			SplixNet net = new SplixNet(new TestListener());

            Console.Clear();
            Console.Write("请输入服务器地址:");
            serverName = Console.ReadLine(); 
            
            Console.Write("请输入服务器端口({0}):", serverPort);
            var tmp = Console.ReadLine();
            if(tmp.Trim().Length > 0)
                serverPort = Convert.ToInt32(tmp);

            Console.Write("请输入你的昵称:");
            nickName = Console.ReadLine();

            myId = net.Login(nickName, serverName, serverPort);
            Console.WriteLine("你的游戏ID是: {0}", myId);
            Console.Write("按下任意键开始显示游戏画面及操控(使用WASD控制, Q退出游戏) > ");
            
			doYouWantToShowMap = true;

            Console.ReadKey();

            while (true) {
                var key = Console.ReadKey().KeyChar;
				switch(key){
                    case 'w': net.Move(0);break;
                    case 's': net.Move(1);break;
                    case 'a': net.Move(2);break;
                    case 'd': net.Move(3);break;
					case 'q': net.Logout();return;
				}
            }
        }
	}
    public class TestListener : SplixNetListener {
        public override void LostConnection() {
            console.log("遇到网络问题, 正在退出中...");
            Environment.Exit(1);
        }

        public override void RefreshMap(int mapX, int mapY, int[,] map) {
			if(TestSplixNet.isDead)return ;//玩家死了, 不刷新界面了
			if(!TestSplixNet.doYouWantToShowMap)return ;//玩家还没有准备好显示地图
            int width = 20,//仅显示部分区域
                CENTER = 40;
            string str = "";
            Console.Clear();
            Console.WriteLine("目前地图中心点:({0}, {1})", mapX, mapY);
            for(int y = CENTER - width ; y <= CENTER + width ; y ++, str = "") {
                for(int x = CENTER -width ; x <= CENTER + width  ; x ++) {
                    str += (map[y, x] + "").PadLeft(4);
                }
                Console.WriteLine(str);
            }
        }

        public override void UserDie(int id, int type) {
            TestSplixNet.isDead = TestSplixNet.myId == id;;
            Console.WriteLine("--------\n用户死亡消息:\n  死亡者ID: {0}\n  死亡原因: {1}\n--------", 
                id, type);
        }

        public override void UserIn(string username, int id) {
           Console.WriteLine("--------\n新的用户加入信息:\n  加入者ID: {0}\n  用户名: {1}\n--------", 
                id, username);
        }

        public override void UserLogin(string username, int id){
            // throw new NotImplementedException();
        }
    }
}