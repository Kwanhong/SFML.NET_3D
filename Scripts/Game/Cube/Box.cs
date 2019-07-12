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
        public BoxVertexArray BoxVertexArray { get; set; }
        public PrimitiveType Type
        {
            get => typ; set
            {
                typ = value;
                if (BoxVertexArray != null)
                    BoxVertexArray.Type = typ;
            }
        }
        public Color FillColor
        {
            get => col; set
            {
                col = value;
                if (BoxVertexArray != null)
                    BoxVertexArray.FillColor = col;
            }
        }
        public Vector3f Position
        {
            get => pos; set
            {
                Vector3f prePos = pos;
                pos = value;
                DoMovement(pos - prePos);
            }
        }
        public Vector3f Rotation
        {
            get => rot; set
            {
                Vector3f preRot = rot;
                rot = value;
                DoRotation(rot - preRot);
            }
        }
        public Vector3f Size { get; set; }

        private Color col;
        private Vector3f pos;
        private Vector3f rot;
        private PrimitiveType typ;
        private List<float> offsetMags;
        private List<float> posMags;

        public Box(Vector3f size, Vector3f position, Vector3f rotation, Color fillColor, PrimitiveType type = PrimitiveType.LineStrip)
        {
            offsetMags = new List<float>();
            posMags = new List<float>();
            this.FillColor = new Color
            (
                (byte)Limit((float)fillColor.R, 155),
                (byte)Limit((float)fillColor.G, 155),
                (byte)Limit((float)fillColor.B, 155),
                fillColor.A
            );
            this.Type = type;
            this.Size = size;
            SetVertexState();
            this.Position = position;
            DoMovement(Position);
            this.Rotation = rotation;
            DoRotation(Rotation);
        }

        private void Initialize(Vector3f size, Vector3f position, Vector3f rotation, Color fillColor, PrimitiveType type = PrimitiveType.LineStrip)
        {
            offsetMags = new List<float>();
            posMags = new List<float>();
            this.FillColor = new Color
            (
                (byte)Limit((float)fillColor.R, 155),
                (byte)Limit((float)fillColor.G, 155),
                (byte)Limit((float)fillColor.B, 155),
                fillColor.A
            );
            this.Type = type;
            this.Size = size;
            SetVertexState();
            this.Position = position;
            this.Rotation = rotation;
            DoRotation(Rotation);
        }

        private void SetVertexState()
        {
            BoxVertexArray = new BoxVertexArray(this.FillColor, this.Type);
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
                BoxVertexArray.Append(new BoxVertex
                (
                    pos: Position,
                    offset: Multiply(Size / 2, signs[i]),
                    side: sides[i]
                ));

            BoxVertexArray.ApplyVertexToRenderer();
        }

        public void Update()
        {
            posMags.Clear();
            offsetMags.Clear();
            foreach (var boxVertex in BoxVertexArray.ToList)
            {
                posMags.Add(GetMagnitude(boxVertex.Position));
                offsetMags.Add(GetMagnitude(boxVertex.Offset));
            }

            AddPerspectivePos();
        }

        public void LateUpdate()
        {
            SubtractPerspectivePos();
        }

        private void AddPerspectivePos()
        {
            foreach (var boxVertex in BoxVertexArray.ToList)
            {
                float scaleFactor = Map(boxVertex.Position.Z - boxVertex.Offset.Z, -winDepth, winDepth, 2f, 0.2f);
                if (winViewMode == ViewMode.Orthographic) scaleFactor = 1f;
                boxVertex.Offset = SetMagnitude(boxVertex.Offset, GetMagnitude(boxVertex.Offset) * scaleFactor);
                boxVertex.Position = SetMagnitude(boxVertex.Position, GetMagnitude(boxVertex.Position) * scaleFactor);
            }
        }

        private void SubtractPerspectivePos()
        {
            foreach (var boxVertex in BoxVertexArray.ToList)
            {
                float scaleFactor = Map(boxVertex.Position.Z - boxVertex.Offset.Z, -winDepth, winDepth, 2f, 0.2f);
                if (winViewMode == ViewMode.Orthographic) scaleFactor = 1f;
                boxVertex.Offset = SetMagnitude(boxVertex.Offset, offsetMags[BoxVertexArray.ToList.IndexOf(boxVertex)]);
                boxVertex.Position = SetMagnitude(boxVertex.Position, posMags[BoxVertexArray.ToList.IndexOf(boxVertex)]);
            }
        }

        public void Rotate(Vector3f rotation)
        {
            Rotation += rotation;
        }

        public void Move(Vector3f movement)
        {
            Position += movement;
        }

        private void DoRotation(Vector3f rotation)
        {
            foreach (var boxVertex in BoxVertexArray.ToList)
            {
                boxVertex.Position = RotateVector(boxVertex.Position, -rotation);
                boxVertex.Offset = RotateVector(boxVertex.Offset, new Vector3f(rotation.X, rotation.Y, -rotation.Z));
            }
        }

        private void DoMovement(Vector3f movement)
        {
            foreach (var boxVertex in BoxVertexArray.ToList)
            {
                boxVertex.Position += movement;
            }
        }

        public void Scale(Vector3f scaleFactor)
        {
            Retire();
            Initialize(Multiply(Size, scaleFactor), Position, Rotation, FillColor, Type);
        }

        public void SetSize(Vector3f size)
        {
            Retire();
            Initialize(size, Position, Rotation, FillColor, Type);
        }

        private void Retire()
        {
            BoxVertexArray.RemoveVertexFromRenderer();
        }
    }
}
