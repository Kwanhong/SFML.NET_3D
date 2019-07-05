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
                        new Vector3f(Map(x, 0, boxCount.X, winSizeX / (boxCount.X + 1), winSizeX), Map(y, 0, boxCount.Y, winSizeY / (boxCount.Y + 1), winSizeY),Map((x + y), -12, 12, -winDepth, winDepth * 0.5f)),
                        //new Vector3f((x + y) * 50 + winSizeX * 0.25f, winSizeY / 2,),
                        new Vector3f(ToRadian(-45), -MathF.Atan(1 / MathF.Sqrt(2)), 0),
                        //new Vector3f(0, 0, ToRadian(265.9f)),
                        new Color(color, (byte)((color * 5) % 255), (byte)((color * 5) % 255)),
                        PrimitiveType.Quads//(x + y == 0) ? PrimitiveType.Quads : PrimitiveType.LineStrip
                    );
                }
            }

            list = new List<Box>();
            foreach (var box in boxes)
                list.Add(box);
        }

        private void Update()
        {
            foreach (var box in boxes)
            {
                box.Update();
                box.Rotate(new Vector3f(0, 0, 0.025f));
            }

        }

        private void Display()
        {
            SortByZOrder(list, 0, list.Count - 1);
            foreach (var box in list)
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