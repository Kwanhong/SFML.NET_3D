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
        Box[,,] boxes;
        List<Box> boxList;
        Vector3f boxCount = new Vector3f(10, 1, 10);
        Vector3f boxSize = new Vector3f(30, 30, 30);

        bool mousePressed = false;
        Vector2i mousePrePos = new Vector2i(0, 0);

        public Game()
        {
            Awake();
            Run();
        }

        private void Awake()
        {
            window.SetFramerateLimit(30);
            window.Closed += OnClosed;
            window.KeyPressed += OnKeyPressed;
            window.MouseButtonPressed += OnMouseButtonPressed;
            window.MouseButtonReleased += OnMouseButtonReleased;
            window.MouseMoved += OnMousePointerMoved;

            Initialize();
        }

        private void Initialize()
        {
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
                    x * boxSize.X * 1.2f - ((boxCount.X - 1) * boxSize.X * 1.2f) / 2,
                    y * boxSize.Y * 1.2f - ((boxCount.Y - 1) * boxSize.Y * 1.2f) / 2,
                    z * boxSize.Z * 1.2f - ((boxCount.Z - 1) * boxSize.Z * 1.2f) / 2
                ),
                rotation: new Vector3f(MathF.PI / 4f,-MathF.Atan(1 / MathF.Sqrt(2)),0),
                fillColor: new Color((byte)Map(x, 0, boxCount.X, 1, 200),(byte)Map(z, 0, boxCount.Z, 1, 200) , (byte)Map(z, 0, boxCount.Z, 1, 200)),
                type: PrimitiveType.Quads
            );
        }

        private void Update()
        {
            for (int i = 0; i < boxList.Count; i++)
            {
                boxList[i].SetSize(new Vector3f(30,50,30));
                boxList[i].Update();
            }
        }

        private void Display()
        {
            Renderer.Render();
            window.Display();
            window.Clear(new Color(25, 25, 25));
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

        #region EVENTS 
        private void HandleEvent()
        {
            window.DispatchEvents();
            DispatchEventImmediately();
        }

        private void DispatchEventImmediately()
        {
            OnKeyPressed();
        }

        private void OnKeyPressed()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                foreach (var box in boxList)
                    box.Rotation += new Vector3f(0, 0.05f, 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                foreach (var box in boxList)
                    box.Rotation += new Vector3f(0.05f, 0, 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                foreach (var box in boxList)
                    box.Rotation += new Vector3f(0, -0.05f, 0);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                foreach (var box in boxList)
                    box.Rotation += new Vector3f(-0.05f, 0, 0);
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
                    winViewMode = ViewMode.Orthographic;
                else
                    winViewMode = ViewMode.Perspective;
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