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

    public class Box
    {
        BoxVertexArray boxVertexArray;
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
                Move(pos - prePos);
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

        private Color col;
        private Vector3f pos;
        private Vector3f rot;
        private PrimitiveType typ;
        private List<float> offsetMags;

        public Box(Vector3f size, Vector3f position, Vector3f rotation, Color fillColor, PrimitiveType type = PrimitiveType.LineStrip)
        {
            offsetMags = new List<float>();
            this.FillColor = new Color
            (
                (byte)Map((float)fillColor.R, 0, 255, 0, 155),
                (byte)Map((float)fillColor.G, 0, 255, 0, 155),
                (byte)Map((float)fillColor.B, 0, 255, 0, 155)
            );
            this.Type = type;
            this.Size = size;
            SetVertexState();
            this.Position = position;
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
            if (winViewMode == ViewMode.Orthographic)
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
            if (winViewMode == ViewMode.Orthographic)
                return;

            foreach (var boxVertex in boxVertexArray.ToList)
            {
                float scaleFactor = Map(boxVertex.Position.Z - boxVertex.Offset.Z, -winDepth, winDepth, 2f, 0);
                boxVertex.Offset = SetMagnitude(boxVertex.Offset, offsetMags[boxVertexArray.ToList.IndexOf(boxVertex)]);
            }
        }

        public void Rotate(Vector3f rotation)
        {
            Vector3f prePosition = this.Position;
            //this.Position -= new Vector3f(winSizeX / 2, winSizeY / 2, winSizeY / 2);

            this.Position = RotateVector(this.Position, -rotation);
            foreach (var boxVertex in boxVertexArray.ToList)
            {
                boxVertex.Offset = RotateVector(boxVertex.Offset, rotation);
            }
            //Position += new Vector3f(winSizeX / 2, winSizeY / 2, winSizeY / 2);
        }

        public void Move(Vector3f movement)
        {
            foreach (var boxVertex in boxVertexArray.ToList)
            {
                boxVertex.Position += movement;
            }
        }

        public void Display()
        {
            boxVertexArray.Display();
        }
    }
}
