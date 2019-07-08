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
        
        public class PlaneVertexArray
        {
            public PrimitiveType Type { get; set; }
            public List<BoxVertex> Plane { get; set; }

            private Color fillColor;

            public PlaneVertexArray(PrimitiveType type, Color color)
            {
                this.Type = type;
                fillColor = color;
                Plane = new List<BoxVertex>();
            }
            public void Append(BoxVertex vertex)
            {
                Plane.Add(vertex);
            }
            public void Display()
            {
                VertexArray array = new VertexArray(this.Type, 4);
                for (int i = 0; i < Plane.Count; i++)
                {
                    Vertex ver = Plane[i].Point;
                    ver.Color = fillColor;
                    array[(uint)i] = ver;
                }
                window.Draw(array);
            }
            public float GetDepth()
            {
                float sum = 0f;
                foreach (var vertex in Plane)
                    sum += vertex.Offset.Z;
                return sum;
            }
        }

        public List<BoxVertex> ToList { get => list; }
        public PrimitiveType Type { get; set; }
        public Color FillColor
        {
            get => fillCol; set
            {
                fillCol = value;
                SetPlaneFillColor();
            }
        }

        List<BoxVertex> list;
        Side[,] vertexSides;
        Color[] planeColors;
        Color fillCol;

        public BoxVertexArray(Color fillColor, PrimitiveType type = PrimitiveType.LineStrip)
        {
            list = new List<BoxVertex>();
            this.Type = type;
            this.FillColor = fillColor;
            vertexSides = new Side[6, 4]
            {
                {Side.LTF,Side.RTF,Side.RBF,Side.LBF},
                {Side.LTB,Side.RTB,Side.RBB,Side.LBB},
                {Side.LTB,Side.RTB,Side.RTF,Side.LTF},
                {Side.RTB,Side.RBB,Side.RBF,Side.RTF},
                {Side.RBB,Side.LBB,Side.LBF,Side.RBF},
                {Side.LBB,Side.LTB,Side.LTF,Side.LBF}
            };
            SetPlaneFillColor();
        }

        private void SetPlaneFillColor()
        {
            planeColors = new Color[6]
            {
                new Color(0,0,0) + FillColor,
                new Color(20,20,20) + FillColor,
                new Color(40,40,40) + FillColor,
                new Color(60,60,60) + FillColor,
                new Color(80,80,80) + FillColor,
                new Color(100,100,100) + FillColor
            };
        }

        public void Append(BoxVertex vertex)
        {
            list.Add(vertex);
        }
        public void Display()
        {
            List<PlaneVertexArray> planes = new List<PlaneVertexArray>();

            for (int i = 0; i < 6; i++)
            {
                PlaneVertexArray plane;
                SetPlaneData(out plane, i);
                planes.Add(plane);
            }

            planes = SortByZOrder(planes, 0, planes.Count - 1);

            foreach (var plane in planes)
                plane.Display();
            // for (int i = 0; i < 6; i++)
            //     planes[i].Display();

            planes.Clear();
        }

        private void SetPlaneData(out PlaneVertexArray plane, int index)
        {
            plane = new PlaneVertexArray(Type, planeColors[index]);
            for (int j = 0; j < 4; j++)
            {
                BoxVertex v3D = GetVertexFromSide(vertexSides[index, j]);
                plane.Append(v3D);
            }
        }

        private List<PlaneVertexArray> SortByZOrder(List<PlaneVertexArray> list, int start, int end)
        {
            int i;
            if (start < end)
            {
                i = Partition(list, start, end);

                SortByZOrder(list, start, i - 1);
                SortByZOrder(list, i + 1, end);
            }

            return list;
        }

        private int Partition(List<PlaneVertexArray> list, int start, int end)
        {
            PlaneVertexArray temp;
            PlaneVertexArray p = list[end];
            int i = start - 1;

            for (int j = start; j <= end - 1; j++)
            {
                if (list[j].GetDepth() <= p.GetDepth())
                {
                    i++;
                    temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }

            temp = list[i + 1];
            list[i + 1] = list[end];
            list[end] = temp;
            return i + 1;
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
        public enum ViewMode { Perspective, Orthographic };
        public ViewMode View { get; set; } = ViewMode.Perspective;
        public Vector3f Size { get; set; }
        public PrimitiveType Type
        {
            get => typ; set
            {
                typ = value;
                if (boxVertexArray != null)
                    boxVertexArray.Type = typ;
            }
        }
        public Color FillColor
        {
            get => col; set
            {
                col = value;
                if (boxVertexArray != null)
                    boxVertexArray.FillColor = col;
            }
        }
        public Vector3f Position
        {
            get => pos; set
            {
                Vector3f prePos = pos;
                pos = value;
            }
        }
        public Vector3f Rotation
        {
            get => rot; set
            {
                Vector3f preRot = rot;
                rot = value;
                Rotate(rot - preRot);
            }
        }
        private Vector3f pos;
        private Vector3f rot;
        private PrimitiveType typ;
        private Color col;
        private List<float> offsetMags = new List<float>();

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
            SetVertexState();
            this.Rotation = rotation;
            Rotate(Rotation);
        }

        private void SetVertexState()
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
            AddPerspectivePos();
        }

        public void LateUpdate()
        {
            SubtractPerspectivePos();
        }

        private void AddPerspectivePos()
        {
            if (View == ViewMode.Orthographic)
                return;

            offsetMags.Clear();
            foreach (var boxVertex in boxVertexArray.ToList)
            {
                float scaleFactor = Map(boxVertex.Position.Z - boxVertex.Offset.Z, -winDepth, winDepth, 2f, 0);
                offsetMags.Add(GetMagnitude(boxVertex.Offset));
                boxVertex.Offset = SetMagnitude(boxVertex.Offset, GetMagnitude(boxVertex.Offset) * scaleFactor);
            }
        }

        private void SubtractPerspectivePos()
        {
            if (View == ViewMode.Orthographic)
                return;

            foreach (var boxVertex in boxVertexArray.ToList)
            {
                float scaleFactor = Map(boxVertex.Position.Z - boxVertex.Offset.Z, -winDepth, winDepth, 2f, 0);
                boxVertex.Offset = SetMagnitude(boxVertex.Offset, offsetMags[boxVertexArray.ToList.IndexOf(boxVertex)]);
            }
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
