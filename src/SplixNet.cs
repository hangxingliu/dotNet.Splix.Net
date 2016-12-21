
namespace Splix.Net {
	public class SplixNet {

        SplixNetListener listener;

        public SplixNet(SplixNetListener listener) {
            this.listener = listener;
        }

		public int Login (string username, string serverip, int port){
            return 1;
        }

		public void Logout (){
			
		}

		public void Move (int direction){

		}
		
	}
}