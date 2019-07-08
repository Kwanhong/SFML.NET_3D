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
        Box[,] boxes;
        List<Box> list;
        Vector2i boxCount = new Vector2i(5, 5);
        Vector3f boxSize = new Vector3f(50, 50, 50);

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

            Initialize();
        }

        private void Initialize()
        {
            boxes = new Box[boxCount.X, boxCount.Y];
            for (int x = 0; x < boxCount.X; x++)
                for (int y = 0; y < boxCount.Y; y++)
                    SetBoxState(x, y);

            list = new List<Box>();
            foreach (var box in boxes)
                list.Add(box);
        }

        private void SetBoxState(int x, int y)
        {
            byte color = (byte)new Random().Next(255);

            boxes[x, y] = new Box
            (
                boxSize,

                new Vector3f
                (
                    Map(x, 0, boxCount.X, winSizeX / (boxCount.X + 1), winSizeX),
                    Map(y, 0, boxCount.Y, winSizeY / (boxCount.Y + 1), winSizeY),
                    0
                ),

                new Vector3f(ToRadian(-45), -MathF.Atan(1 / MathF.Sqrt(2)), 0),

                new Color(color, (byte)((color * 5) % 255), (byte)((color * 5) % 255)),

                PrimitiveType.Quads
            );
        }

        private void Update()
        {
            foreach (var box in boxes)
            {
                box.Update();
                box.Rotation = new Vector3f
                (
                    Map(Mouse.GetPosition(window).X, 0, winSizeX, 0, -MathF.PI * 2),
                    Map(Mouse.GetPosition(window).Y, 0, winSizeY, 0, -MathF.PI * 2),
                    box.Rotation.Z
                );
                box.Rotate(new Vector3f(0, 0, 0.01f));
            }
            SortByZOrder(list, 0, list.Count - 1);
        }

        private void Display()
        {
            List<Color> cols = new List<Color>();
            foreach (var box in list)
                cols.Add(box.FillColor);

            foreach (var box in list)
            {
                box.Type = PrimitiveType.LineStrip;
                box.FillColor = Color.White;

                box.Display();

                box.Type = PrimitiveType.Quads;
                box.FillColor = cols[list.IndexOf(box)];
                box.Display();

            }

            window.Display();
            window.Clear(new Color(35, 35, 35));
        }

        private void LateUpdate()
        {
            foreach (var box in boxes)
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

        private void SortByZOrder(List<Box> list, int start, int end)
        {
            int i;
            if (start < end)
            {
                i = Partition(list, start, end);

                SortByZOrder(list, start, i - 1);
                SortByZOrder(list, i + 1, end);
            }
        }

        private int Partition(List<Box> list, int start, int end)
        {
            Box temp;
            Box p = list[end];
            int i = start - 1;

            for (int j = start; j <= end - 1; j++)
            {
                if (list[j].Position.Z >= p.Position.Z)
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

        #region EVENTS 
        private void HandleEvent()
        {
            window.DispatchEvents();
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
        #endregion
    }
}