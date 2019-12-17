using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple2D.Core
{
    public class Tileset : IDisposable
    {
        public string ImageFile { get; private set; }
        public int ColumnsCount { get; private set; }
        public int RowsCount { get; private set; }
        /// <summary>
        /// Tile size in pixels
        /// </summary>
        public Size TileSize { get; private set; }

        private Point curretTile;
        public Point CurretTile
        {
            get { return curretTile; }
            set
            {
                if (curretTile.X >= ColumnsCount) throw new ArgumentException("X must be 0 based and smaller than ColumnsCount");
                if (curretTile.Y >= RowsCount) throw new ArgumentException("Y must be 0 based and smaller than RowsCount");
                curretTile = value;
            }
        }

        private Bitmap tileImage;

        public Tileset(string FilePath, int TilesX, int TilesY)
        {
            ImageFile = FilePath;
            var img = (Bitmap)Bitmap.FromFile(FilePath);

            int ts = img.Width / TilesX;

            baseCtor(img, ts, TilesX, TilesY);
        } 

        public Tileset(string FilePath, int TileSize, int TilesX, int TilesY)
            : this((Bitmap)Bitmap.FromFile(FilePath), TileSize, TilesX, TilesY)
        {
            ImageFile = FilePath;
        }
        public Tileset(Bitmap Image, int TileSize, int TilesX, int TilesY)
        {
            baseCtor( Image,  TileSize,  TilesX,  TilesY);
        }
        private void baseCtor(Bitmap Image, int TileSize, int TilesX, int TilesY)
        {
            if (Image.Width != TileSize * TilesX) throw new ArgumentException("Image Width must be TilseSize * TileX");
            if (Image.Height != TileSize * TilesY) throw new ArgumentException("Image Height must be TilseSize * TilesY");

            this.ColumnsCount = TilesX;
            this.RowsCount = TilesY;
            this.TileSize = new Size(TileSize, TileSize);

            tileImage = Image;
        }

        private Rectangle getTileRect(Point Coordinate)
        {
            return new Rectangle(Mult(Coordinate, TileSize), TileSize);
        }
        public static Point Mult(Point P, Size S)
        {
            return new Point(P.X * S.Width,
                             P.Y * S.Height);
        }
        
        /// <summary>
        /// MUST dispose bitmap after use
        /// </summary>
        public Bitmap GetTileCopy(Point Coordinate)
        {
            Bitmap bmp = new Bitmap(TileSize.Width, TileSize.Height);
            using (Graphics g = Graphics.FromImage(bmp)) DrawTile(g, new Point() , Coordinate);
            return bmp;
        }

        public void DrawCurrentTile(Graphics g, PointF ScreenCoordinates)
        {
            DrawCurrentTile(g, new Point((int)ScreenCoordinates.X, (int)ScreenCoordinates.Y));
        }
        public void DrawCurrentTile(Graphics g, Point ScreenCoordinates)
        {
            DrawTile(g, ScreenCoordinates, curretTile);
        }
        public void DrawTile(Graphics g, Point ScreenCoordinates, Point TileCoordinate)
        {
            g.DrawImage(tileImage, new Rectangle(ScreenCoordinates, TileSize), getTileRect(TileCoordinate), GraphicsUnit.Pixel);
        }

        public void SetCurrTileRow(int Row)
        {
            curretTile.Y = Row;
        }
        public void SetCurrTileColumn(int Column)
        {
            curretTile.X = Column;
        }
        public void StepCurrTileX()
        {
            curretTile.X = (curretTile.X + 1) % ColumnsCount;
        }
        public void StepCurrTileY()
        {
            curretTile.Y = (curretTile.Y + 1) % RowsCount;
        }

        public Point TileLinearToCoordinate(int TileNumber)
        {
            return new Point(TileNumber % ColumnsCount,
                             TileNumber / ColumnsCount);
        }
        public int TileCoordinateToLinear(Point Coordinate)
        {
            return Coordinate.Y * ColumnsCount + Coordinate.X;
        }

        public void Dispose()
        {
            if (tileImage != null)
            {
                tileImage.Dispose();
                tileImage = null;
            }
        }
    }
}
