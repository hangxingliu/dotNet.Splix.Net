namespace Splix.Net.Structure {
	class Map {
		public int[,] mapData;
        public int leftTopX = 0, leftTopY = 0;
        public int mapWidth;

		public Map(int width) {
            mapWidth = width;
            mapData = new int[width, width];
        }
    }
}