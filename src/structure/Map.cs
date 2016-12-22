namespace Splix.Net.Structure {
	class Map {
		public int[,] mapData;
        public int centerX = 0, centerY = 0;
        public int mapWidth;

		public Map(int width) {
            mapWidth = width;
            mapData = new int[width, width];
        }
    }
}