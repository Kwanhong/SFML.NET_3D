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
    public enum Side { LTF, RTF, LBF, RBF, LTB, RTB, LBB, RBB };
    public class BoxVertexArray
    {
        public List<Vertex3D> ToList { get => list; }

        List<Vertex3D> list;
        PrimitiveType type;
        Side side;

        public BoxVertexArray(PrimitiveType _type = PrimitiveType.LineStrip)
        {
            list = new List<Vertex3D>();
            type = _type;
        }

        public void Append(Vertex3D vertex)
        {
            list.Add(vertex);
        }
        public void Display()
        {
            Side[,] sides = new Side[6, 4]
            {
                {Side.LTF,Side.RTF,Side.RBF,Side.LBF},
                {Side.LTB,Side.RTB,Side.RBB,Side.LBB},
                {Side.LTB,Side.RTB,Side.RTF,Side.LTF},
                {Side.RTB,Side.RBB,Side.RBF,Side.RTF},
                {Side.RBB,Side.LBB,Side.LBF,Side.RBF},
                {Side.LBB,Side.LTB,Side.LTF,Side.LBF}
            };
            Color[] colors = new Color[6] 
            {
                new Color(255,0,0),
                new Color(255,255,0),
                new Color(0,255,0),
                new Color(0,255,255),
                new Color(0,0,255),
                new Color(255,0,255)
            };
            VertexArray plane = new VertexArray(PrimitiveType.Quads, 4);
            Vertex[] v = new Vertex[4];

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    v[j] = GetVertexFromSide(sides[i, j]).Point;
                    v[j].Color = colors[i];
                    plane[(uint)j] = v[j];
                }
                window.Draw(plane);
            }
        }

        private Vertex3D GetVertexFromSide(Side side)
        {
            foreach (var vertex in list)
                if (vertex.Side == side) return vertex;

            return list[0];
        }
    }

    public class Vertex3D
    {
        public Vertex3D(Vector3f pos, Vector3f offset, Side side)
        {
            this.Side = side;
            this.Position = pos;
            this.Offset = offset;
            this.Point = new Vertex(new Vector2f(Position.X + Offset.X, Position.Y + Offset.Y));
        }

        public Vertex Point;
        public Side Side { get; private set; }
        public Vector3f Position { get; set; }
        public Vector3f Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
                Point.Position = new Vector2f(Position.X + Offset.X, Position.Y + Offset.Y);
            }
        }
        public float X { get => Position.X; }
        public float Y { get => Position.Y; }
        public float Z { get => Position.Z; }

        private Vector3f offset;
    }

    public class Box
    {
        BoxVertexArray boxVertexArray;
        public Vector3f Size { get; set; }
        public Vector3f Position { get; set; }
        public Vector3f Rotation { get; set; }

        public Box(Vector3f size, Vector3f position, Vector3f rotation)
        {
            this.Size = size;
            this.Position = position;
            SetVertexPositions();
            this.Rotation = rotation;
            Rotate(Rotation);
        }

        private void SetVertexPositions(){
            boxVertexArray = new BoxVertexArray();
            Side[] sides = new Side[8]
            {
                Side.LTF,Side.RTF,Side.LBF,Side.RBF,
                Side.LTB,Side.RTB,Side.LBB,Side.RBB
            };
            Vector3f[] signs = new Vector3f[8]
            {
                new Vector3f(-1, -1, -1), new Vector3f(+1, -1, -1),
                new Vector3f(-1, +1, -1), new Vector3f(+1, +1, -1),
                new Vector3f(-1, -1, +1), new Vector3f(+1, -1, +1),
                new Vector3f(-1, +1, +1), new Vector3f(+1, +1, +1)
            };
            for (int i = 0; i < 8; i++)
            boxVertexArray.Append(new Vertex3D
            (
                Position,
                Multiply(Size / 2, signs[i]),
                sides[i]
            ));
        }

        public void Update()
        {

        }

        public void Rotate(Vector3f rotation)
        {
            foreach (var boxVertex in boxVertexArray.ToList)
                boxVertex.Offset = RotateVector(boxVertex.Offset, rotation);
        }

        public void Display()
        {
            boxVertexArray.Display();
        }
    }
}
