namespace Splix.Net {
	public abstract class SplixNetListener {
		
		/* 连接丢失时调用 */
		abstract public void LostConnection ();

		/* 收到新地图信息时调用,传入map[x,y]为地图对应块的信息
		 * 为四位整数 [ID(2b),属性{头身地}(1b),头方向(1b)], 0: 空气, -1: 无效地方(边界)
		 * 值参照BlockAttribute , MoveDirection */
		abstract public void RefreshMap (int mapX, int mapY, int[,] map);

		/* 有新用户加入时调用,传入用户名和用户ID */
		abstract public void UserIn (string username, int id);

		/* 有谁死了死了死了死了调用,传入用户ID和死亡类型id,参考DeathReason */
		abstract public void UserDie (int id, int type);
	}
}