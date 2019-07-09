using SFML.Window;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using static SFML_NET_3D.Data;
using static SFML_NET_3D.Constants;
using static SFML_NET_3D.Utility;

namespace SFML_NET_3D
{
    public class BoxVertex
    {
        public BoxVertex(Vector3f pos, Vector3f offset, Side side)
        {
            this.Side = side;
            this.Position = pos;
            this.Offset = offset;
            this.Point = new Vertex(new Vector2f(Position.X + Offset.X, Position.Y + Offset.Y));
        }

        public Vertex Point;
        public Side Side { get; private set; }
        public Vector3f Position
        {
            get => position; set
            {
                position = value;
                Point.Position = new Vector2f(Position.X + Offset.X, Position.Y + Offset.Y);
            }
        }
        public Vector3f Offset
        {
            get => offset; set
            {
                offset = value;
                Point.Position = new Vector2f(Position.X + Offset.X, Position.Y + Offset.Y);
            }
        }

        private Vector3f offset;
        private Vector3f position;
    }
}