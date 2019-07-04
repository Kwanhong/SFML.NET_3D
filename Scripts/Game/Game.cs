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
        Box box;

        public Game()
        {
            Initialize();
            Run();
        }

        private void Initialize()
        {
            window.SetFramerateLimit(30);
            window.Closed += OnClosed;
            window.KeyPressed += OnKeyPressed;

            box = new Box
            (
                new Vector3f(200, 200, 200),
                new Vector3f(winSizeX / 2, winSizeY / 2, winSizeX / 2),
                new Vector3f(ToRadian(-45), -MathF.Atan(1/MathF.Sqrt(2)), 0)
            );
        }

        private void Update()
        {
            box.Rotate(new Vector3f(0.05f, -0.05f, 0.05f));
        }

        private void Display()
        {
            box.Display();

            window.Display();
            window.Clear(new Color(25, 25, 25));
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
        }
        private void OnClosed(object sender, EventArgs e)
        {
            window.Close();
        }
        #endregion
    }
}