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
    public class BoxVertexArray
    {
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

        PlaneVertexArray[] planes;
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
            planes = new PlaneVertexArray[6];

            SetPlaneFillColor();
        }

        private void SetPlaneFillColor()
        {
            planeColors = new Color[6]
            {
                new Color(0,0,0,0)        + FillColor,
                new Color(20,20,20,0)     + FillColor,
                new Color(40,40,40,0)     + FillColor,
                new Color(60,60,60,0)     + FillColor,
                new Color(80,80,80,0)     + FillColor,
                new Color(100,100,100,0)  + FillColor
            };
        }

        public void Display()
        {
            List<PlaneVertexArray> list = new List<PlaneVertexArray>();
            for (int i = 0; i < 6; i++)
            {
                SetPlaneData(out planes[i], i);
                list.Add(planes[i]);
            }

            list = Renderer.SortByZOrder(list, 0, list.Count - 1);

            for (int i = 0; i < 6; i++)
                list[i].Display();
        }

        public void Append(BoxVertex vertex)
        {
            list.Add(vertex);
        }

        public void ApplyVertexToRenderer()
        {
            for (int i = 0; i < 6; i++)
            {
                SetPlaneData(out planes[i], i);
                Renderer.Add(planes[i]);
            }
        }

        public void RemoveVertexFromRenderer()
        {
            for (int i = 0; i < 6; i++)
            {
                if (planes[i] == null) return;
                Renderer.Remove(planes[i]);
            }
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

        private BoxVertex GetVertexFromSide(Side side)
        {
            foreach (var vertex in list)
                if (vertex.Side == side) return vertex;

            return list[0];
        }
    }
}