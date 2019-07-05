using SFML.Window;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;
using System;
using static SFML_NET_3D.Data;
using static SFML_NET_3D.Constants;
using static SFML_NET_3D.Utility;

namespace SFML_NET_3D
{
    class Game
    {
        Box[,] boxes;
        Vector2i boxCount = new Vector2i(8, 6);

        public Game()
        {
            Initialize();
            Run();
        }

        private void Initialize()
        {
            window.SetFramerateLimit(60);
            window.Closed += OnClosed;
            window.KeyPressed += OnKeyPressed;

            boxes = new Box[boxCount.X, boxCount.Y];
            for (int x = 0; x < boxCount.X; x++)
            {
                for (int y = 0; y < boxCount.Y; y++)
                {
                    byte color = (byte)new Random().Next(255);
                    boxes[x, y] = new Box
                    (
                        new Vector3f(50, 50, 50),
                        new Vector3f(Map(x, 0, boxCount.X, winSizeX / (boxCount.X + 1), winSizeX), Map(y, 0, boxCount.Y, winSizeY / (boxCount.Y + 1), winSizeY), winSizeX / 2 + (x + y) * 10),
                        new Vector3f(ToRadian(-45), -MathF.Atan(1 / MathF.Sqrt(2)), 0),
                        new Color(color, (byte)((color * 5) % 255), (byte)((color * 5) % 255)),
                        PrimitiveType.Quads//(x + y == 0) ? PrimitiveType.Quads : PrimitiveType.LineStrip
                    );
                }
            }
        }

        private void Update()
        {
            foreach (var box in boxes) {
                box.Rotation = new Vector3f
                (  
                    Map(Mouse.GetPosition(window).X, 0, winSizeX, 0, -MathF.PI * 2),
                    Map(Mouse.GetPosition(window).Y, 0, winSizeY, 0, -MathF.PI * 2),
                    box.Rotation.Z
                );
                box.Rotate(new Vector3f(0, 0, 0.025f));
            }

        }

        private void Display()
        {
            foreach (var box in boxes)
                box.Display();

            window.Display();
            window.Clear(new Color(35, 35, 35));
        }

        private void Run()
        {
            while (window.IsOpen)
            {
                HandleEvent();
                Update();
                Display();
            }
        }

        private void HandleEvent()
        {
            window.DispatchEvents();
        }

        #region EVENTS 
        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                window.Close();
            }
            if (e.Code == Keyboard.Key.F5)
            {
                Initialize();
            }
        }
        private void OnClosed(object sender, EventArgs e)
        {
            window.Close();
        }
        #endregion
    }
}