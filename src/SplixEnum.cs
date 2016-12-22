namespace Splix.Code {
	/* 分别为 {成功,重名,连接失败},有新代码再追加 */
	public enum LoginCode {
		SUCCEED,
		WRONG_USER,
		WRONG_SERVER
	}

	/* 分别为 头,身体(走过痕迹),大本营(圈起来的地) */
	public enum BlockAttribute {
		HEAD,
		BODY,
		HOME
	}

	/* 移动方向 */
	public enum MoveDirection {
		UP,
		DOWN,
		LEFT,
		RIGHT
	}

	/* 死亡类型 */
	public enum DeathReason {
		KILL_BY_PLAYER,
		KILL_BY_SELF,
		KILL_BY_BORDER,
		KILL_BY_DISCONNECT
	}
}