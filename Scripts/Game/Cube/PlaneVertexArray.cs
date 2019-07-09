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
            foreach (var plane in Plane)
                plane.Position += new Vector3f(winSizeX, winSizeY, winDepth) / 2;

            VertexArray array = new VertexArray(this.Type, 4);
            for (int i = 0; i < Plane.Count; i++)
            {
                Vertex ver = Plane[i].Point;
                ver.Color = fillColor;
                array[(uint)i] = ver;
            }
            window.Draw(array);
            
            foreach (var plane in Plane)
                plane.Position -= new Vector3f(winSizeX, winSizeY, winDepth) / 2;
        }

        public float GetDepth()
        {
            float sum = 0f;
            for (int i = 0; i < Plane.Count; i++)
                sum += (Plane[i].Offset.Z - Plane[i].Position.Z);
            return (sum);
        }
    }
}