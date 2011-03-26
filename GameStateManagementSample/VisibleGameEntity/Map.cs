using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GameStateManagement
{
    public class Map : VisibleGameEntity
    {
        #region Fields

        public List<List<int>> MapMatrix { get; private set; }

        #endregion

        #region Properties

        public Point MatrixSize { set; get; }
        

        #endregion

        #region Init

        public Map() {
        }

        public Map(string file) : this()
        {
            var path = @"Map\" + file;
            if (!File.Exists(path)) return;
            StreamReader stream = new StreamReader(path);
            {
                var x = Convert.ToInt32(stream.ReadLine());
                var y = Convert.ToInt32(stream.ReadLine());
                MatrixSize = new Point(x,y);
                MapMatrix = new List<List<int>>();

                for (int i = 0; i < y; i++) {
                    var list = stream.ReadLine().Split(' ').Select(s => Convert.ToInt32(s)).ToList();
                    MapMatrix.Add(list);
                }
            }
        }

        #endregion

        #region Public Methods

        public Point GetMatrixPosition(Point mousePosition,int square) {
            var x = mousePosition.X /square;
            var y = mousePosition.Y/square;
            if (x >= 0 && x < MatrixSize.X && y >= 0 && y < MatrixSize.Y) 
                return new Point(x,y);
            return new Point(-1,-1);
        }

        #endregion
    }
}
