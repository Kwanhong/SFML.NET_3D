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
            VertexArray plane = new VertexArray(PrimitiveType.Quads, 4);
            Vertex[] v = new Vertex[4];

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    v[j] = GetVertexFromSide(sides[i, j]).Point;
                    v[j].Color = new Color(255, 255, 255, 10);
                    plane.Append(v[j]);
                }
                window.Draw(plane);
            }
        }

        private Vertex3D GetVertexFromSide(Side side)
        {
            foreach (var vertex in list)
            {
                if (vertex.Side == side) return vertex;
            }
            return list[0];
        }
        public void Display(Vector3f size)
        {
            float[] sizeArray = new float[] { size.X, size.Y, size.Z };
            float maxSize = GetMax(sizeArray);

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 1; j < list.Count; j++)
                {
                    if (i == j) continue;

                    Vertex vertex1 = list[i].Point;
                    Vertex vertex2 = list[j].Point;

                    if (Distnace(vertex1.Position, vertex2.Position) >= maxSize * 1.001f)
                        continue;

                    vertex1.Color = new Color(255, 255, 255, 100);
                    vertex2.Color = new Color(255, 255, 255, 100);

                    VertexArray line = new VertexArray(PrimitiveType.Lines, 2);
                    line[0] = vertex1;
                    line[1] = vertex2;

                    window.Draw(line);
                }
            }
        }
    }
    public class Vertex3D
    {
        public Vertex3D(Vector3f pos, Side side)
        {
            this.Side = side;
            this.Position = pos;
            this.Point = new Vertex(new Vector2f(Position.X, Position.Y));
        }

        public Side Side { get; private set; }
        public Vertex Point { get; set; }
        public Vector3f Position { get; set; }
        public float X { get => Position.X; }
        public float Y { get => Position.Y; }
        public float Z { get => Position.Z; }
    }

    public class Box
    {
        public Vector3f Size { get; set; }
        public Vector3f Position { get; set; }

        public Box(Vector3f size, Vector3f position)
        {
            this.Size = size;
            this.Position = position;
        }
        public void Update()
        {

        }
        public void Display()
        {
            BoxVertexArray box = new BoxVertexArray();
            box.Append(new Vertex3D(Position + new Vector3f(-Size.X / 2, -Size.X / 2, -Size.X / 2), Side.LTF));
            box.Append(new Vertex3D(Position + new Vector3f(+Size.X / 2, -Size.X / 2, -Size.X / 2), Side.RTF));
            box.Append(new Vertex3D(Position + new Vector3f(-Size.X / 2, +Size.X / 2, -Size.X / 2), Side.LBF));
            box.Append(new Vertex3D(Position + new Vector3f(+Size.X / 2, +Size.X / 2, -Size.X / 2), Side.RBF));

            box.Append(new Vertex3D(Position + new Vector3f(-Size.X / 2 + 30, -Size.X / 2 - 30, +Size.X / 2), Side.LTB));
            box.Append(new Vertex3D(Position + new Vector3f(+Size.X / 2 + 30, -Size.X / 2 - 30, +Size.X / 2), Side.RTB));
            box.Append(new Vertex3D(Position + new Vector3f(-Size.X / 2 + 30, +Size.X / 2 - 30, +Size.X / 2), Side.LBB));
            box.Append(new Vertex3D(Position + new Vector3f(+Size.X / 2 + 30, +Size.X / 2 - 30, +Size.X / 2), Side.RBB));
            box.Display();
        }
    }
}
