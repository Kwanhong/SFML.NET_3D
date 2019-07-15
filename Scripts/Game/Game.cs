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
    class Game
    {
        NoiseFactors noiseFactors;
        float[] noise;
        Box[,,] boxes;
        List<Box> boxList;
        Vector3f boxCount = new Vector3f(21, 1, 21);
        Vector3f boxSize = new Vector3f(15, 15, 15);

        bool mousePressed = false;
        Vector2i mousePrePos = new Vector2i(0, 0);

        public Game()
        {
            Awake();
            Run();
        }

        private void Awake()
        {
            window.SetFramerateLimit(60);
            window.Closed += OnClosed;
            window.KeyPressed += OnKeyPressed;
            window.MouseButtonPressed += OnMouseButtonPressed;
            window.MouseButtonReleased += OnMouseButtonReleased;
            window.MouseMoved += OnMousePointerMoved;

            Initialize();
        }

        private void Initialize()
        {
            noiseFactors = new NoiseFactors
            (
                size: 800,
                octave: 10,
                interval: 1,
                randomSeed: new Random().Next(255),
                softness: 2.35f
            );
            noise = Noise(noiseFactors);
            Renderer.Clear();

            boxes = new Box[(int)boxCount.X, (int)boxCount.Y, (int)boxCount.Z];
            for (int x = 0; x < (int)boxCount.X; x++)
                for (int y = 0; y < (int)boxCount.Y; y++)
                    for (int z = 0; z < (int)boxCount.Z; z++)
                        SetBoxState(x, y, z);

            boxList = new List<Box>();

            foreach (var box in boxes)
                boxList.Add(box);
        }

        private void SetBoxState(int x, int y, int z)
        {
            boxes[x, y, z] = new Box
            (
                size: boxSize,
                position: new Vector3f
                (
                    x * boxSize.X - ((boxCount.X - 1) * boxSize.X) / 2,
                    y * boxSize.Y - ((boxCount.Y - 1) * boxSize.Y) / 2,
                    z * boxSize.Z - ((boxCount.Z - 1) * boxSize.Z) / 2 + 0.0001f // Preventing Zero Exeption
                ),
                rotation: new Vector3f
                (
                    MathF.PI / 4f,
                    -MathF.Atan(1 / MathF.Sqrt(2)),
                    0
                ),
                fillColor: Color.Black,
                type: PrimitiveType.Quads
            );
        }

        float offset1 = 0;
        private void Update()
        {
            for (int x = 0; x < boxCount.X; x++)
            {
                for (int y = 0; y < boxCount.Y; y++)
                {
                    for (int z = 0; z < boxCount.Z; z++)
                    {
                        float dist = Distnace(new Vector2f(x + 0.5f, z + 0.5f), new Vector2f(boxCount.X / 2, boxCount.Z / 2));
                        float offset2 = Map(dist, 0, Distnace(new Vector2f(0, 0), new Vector2f(boxCount.X / 2, boxCount.Z / 2)), 0, 1000);
                        float height = 180 + 90 * Map(noise[(int)(offset1 + offset2) % noiseFactors.Size], GetMin(noise), GetMax(noise), -1, 1);

                        boxes[x, y, z].SetSize(new Vector3f(boxSize.X, height, boxSize.Y));
                        //boxes[x, y, z].SetSize(boxes[x,y,z].Size);

                        byte r = (byte)(Map(x, 0, boxCount.X, 0, 255));
                        byte g = (byte)(Map(z, 0, boxCount.Z, 0, 255));
                        byte b = (byte)(Map(x + z, 0, boxCount.X + boxCount.Z, 0, 255));

                        Color fillColor = new Color
                        (
                            (byte)(Map(noise[(int)(offset1 + offset2) % noiseFactors.Size], GetMin(noise), GetMax(noise), 0, r)),
                            (byte)(Map(noise[(int)(offset1 + offset2) % noiseFactors.Size], GetMin(noise), GetMax(noise), 0, g)),
                            (byte)(Map(noise[(int)(offset1 + offset2) % noiseFactors.Size], GetMin(noise), GetMax(noise), 0, b)),
                            255
                        );
                        boxes[x, y, z].FillColor = fillColor;
                        //boxes[x, y, z].FillColor = new Color((byte)Map(boxes[x,y,z].Position.Z,-winDepth,winDepth,0,255),(byte)Map(boxes[x,y,z].Position.Z,-winDepth,winDepth,0,255),(byte)Map(boxes[x,y,z].Position.Z,-winDepth,winDepth,0,255));
                    }
                }
            }

            foreach (var box in boxList)
                box.Update();

            if (offset1 <= noiseFactors.Size - noiseFactors.Interval * 1.01f)
                offset1 += noiseFactors.Interval;
            else
                offset1 = 0;
        }

        private void Display()
        {
            Renderer.Render();
            DisplayNoiseWave(new Color(255, 255, 255, 100), 20);

            window.Display();
            window.Clear(new Color(25, 25, 25));
        }

        private void DisplayNoiseWave(Color color, float weight)
        {
            int offset = (int)offset1;

            VertexArray line = new VertexArray(PrimitiveType.LineStrip);

            Vector2f prevPos = new Vector2f
            (
                Map(0, 0, noiseFactors.Size, 0, winSizeX),
                Map(noise[(offset) % noiseFactors.Size], GetMin(noise), GetMax(noise),
                    -winSizeY * 0.25f, winSizeY * 0.25f) + winSizeY * 0.5f
            );
            Vector2f currPos = new Vector2f
            (
                Map((int)weight, 0, noiseFactors.Size, 0, winSizeX),
                Map(noise[((int)weight + offset) % noiseFactors.Size], GetMin(noise), GetMax(noise),
                    -winSizeY * 0.25f, winSizeY * 0.25f) + winSizeY * 0.5f
            );

            Vector2f vector = currPos - prevPos;
            vector = RotateVector(vector, MathF.PI/2);
            vector = SetMagnitude(vector, weight);

            Vertex v1 = new Vertex(prevPos, color);
            Vertex v2 = new Vertex(v1.Position + vector, color);

            for (int i = (int)weight; i < noiseFactors.Size; i += (int)weight)
            {
                currPos = new Vector2f
                (
                    Map(i, 0, noiseFactors.Size, 0, winSizeX),
                    Map(noise[(i + offset) % noiseFactors.Size], GetMin(noise), GetMax(noise),
                        -winSizeY * 0.25f, winSizeY * 0.25f) + winSizeY * 0.5f
                );

                line.Append(v1);
                line.Append(v2);

                vector = currPos - prevPos;
                vector = RotateVector(vector, ToRadian(90));
                vector = SetMagnitude(vector, weight);

                v1 = new Vertex(prevPos, color);
                v2 = new Vertex(v1.Position + vector, color);
                line.Append(v2);
                line.Append(v1);

                prevPos = currPos;
            }
            window.Draw(line);
        }

        private void LateUpdate()
        {
            foreach (var box in boxList)
            {
                box.LateUpdate();
            }
        }

        private void Run()
        {
            while (window.IsOpen)
            {
                HandleEvent();
                Update();
                Display();
                LateUpdate();
            }
        }

        private void HandleEvent()
        {
            window.DispatchEvents();
            DispatchEventImmediately();
        }

        private void DispatchEventImmediately()
        {
            OnKeyPressed();
        }

        #region EVENTS 
        private void OnKeyPressed()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                foreach (var box in boxList)
                    box.Rotation += new Vector3f(0, -0.05f, 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                foreach (var box in boxList)
                    box.Rotation += new Vector3f(-0.05f, 0, 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                foreach (var box in boxList)
                    box.Rotation += new Vector3f(0, 0.05f, 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                foreach (var box in boxList)
                    box.Rotation += new Vector3f(0.05f, 0, 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            {
                foreach (var box in boxList)
                    box.Rotation += new Vector3f(0, 0, 0.05f);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.E))
            {
                foreach (var box in boxList)
                    box.Rotation += new Vector3f(0, 0, -0.05f);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
            {
                foreach (var box in boxList)
                    box.Position += new Vector3f(0, 0, 10);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            {
                foreach (var box in boxList)
                    box.Position += new Vector3f(-10, 0, 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
            {
                foreach (var box in boxList)
                    box.Position += new Vector3f(0, 0, -10);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
            {
                foreach (var box in boxList)
                    box.Position += new Vector3f(10, 0, 0);
            }
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape || e.Code == Keyboard.Key.F4)
            {
                window.Close();
            }
            if (e.Code == Keyboard.Key.F5)
            {
                Initialize();
            }
            if (e.Code == Keyboard.Key.F6)
            {
                if (winViewMode == ViewMode.Perspective)
                {
                    winViewMode = ViewMode.Orthographic;
                    winTitle = "SFML.NET 3D - View Mode : Orthographic";
                    window.SetTitle(winTitle);
                }
                else
                {
                    winViewMode = ViewMode.Perspective;
                    winTitle = "SFML.NET 3D - View Mode : Perspective";
                    window.SetTitle(winTitle);
                }
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            window.Close();
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Right && !mousePressed)
            {
                mousePressed = true;
            }
        }

        private void OnMousePointerMoved(object sender, MouseMoveEventArgs e)
        {
            if (mousePressed)
            {
                foreach (var box in boxList)
                    box.Rotation = new Vector3f
                    (
                       Map(Mouse.GetPosition(window).X, 0, winSizeX, MathF.PI, -MathF.PI),
                       Map(Mouse.GetPosition(window).Y, 0, winSizeY, MathF.PI, -MathF.PI),
                       box.Rotation.Z
                    );
            }
        }

        private void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Right && mousePressed)
            {
                mousePressed = false;
            }
        }
        #endregion
    }
}