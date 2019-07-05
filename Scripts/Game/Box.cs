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
        public List<BoxVertex> ToList { get => list; }

        List<BoxVertex> list;
        PrimitiveType type;
        Color fillColor;

        public BoxVertexArray(Color _fillColor, PrimitiveType _type = PrimitiveType.LineStrip)
        {
            list = new List<BoxVertex>();
            type = _type;
            fillColor = _fillColor;
        }

        public void Append(BoxVertex vertex)
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
                new Color(30,30,30) + fillColor,
                new Color(40,40,40) + fillColor,
                new Color(70,70,70) + fillColor,
                new Color(100,100,100) + fillColor,
                new Color(130,130,130) + fillColor,
                new Color(160,160,160) + fillColor
            };
            VertexArray plane = new VertexArray(type, 4);
            Vertex[] v = new Vertex[4];

            List<float> depthCount = new List<float>();
            for (int i = 0; i < 6; i++)
            {
                depthCount.Add(0f);
                for (int j = 0; j < 4; j++)
                {
                    BoxVertex v3D = GetVertexFromSide(sides[i, j]);
                    depthCount[depthCount.Count - 1] += v3D.Offset.Z;
                }
            }

            float average = GetAverageOf(depthCount);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    BoxVertex v3D = GetVertexFromSide(sides[i, j]);
                    v[j] = v3D.Point;
                    v[j].Color = colors[i];
                    plane[(uint)j] = v[j];
                }
                if (depthCount[i] > average || type == PrimitiveType.LineStrip)
                    window.Draw(plane);
            }
        }

        private BoxVertex GetVertexFromSide(Side side)
        {
            foreach (var vertex in list)
                if (vertex.Side == side) return vertex;

            return list[0];
        }
    }

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
        public PrimitiveType Type { get; set; }
        public Color FillColor { get; set; }
        public Vector3f Position { get => pos; set
        {
            Vector3f prePos = pos;
            pos = value;
        } }
        public Vector3f Rotation { get => rot; set
        {
            Vector3f preRot = rot;
            rot = value;
            Rotate(rot - preRot);
        } }
        private Vector3f pos;
        private Vector3f rot;

        public Box(Vector3f size, Vector3f position, Vector3f rotation, Color fillColor, PrimitiveType type = PrimitiveType.LineStrip)
        {
            this.FillColor = new Color
            (
                (byte)Limit((float)fillColor.R, 95),
                (byte)Limit((float)fillColor.G, 95),
                (byte)Limit((float)fillColor.B, 95)
            );
            this.Type = type;
            this.Size = size;
            this.Position = position;
            SetVertexPositions();
            this.Rotation = rotation;
            Rotate(Rotation);
        }

        private void SetVertexPositions()
        {
            boxVertexArray = new BoxVertexArray(this.FillColor, this.Type);
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
                boxVertexArray.Append(new BoxVertex
                (
                    pos: Position,
                    offset: Multiply(Size / 2, signs[i]),
                    side: sides[i]
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
