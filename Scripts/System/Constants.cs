using System;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;

namespace SFML_NET_3D
{
    public static class Constants
    {
        public enum ViewMode { Perspective, Orthographic };

        public const uint winSizeX = 800;
        public const uint winSizeY = 600;
        public const uint winDepth = 1200;

        public const string winTitle = "SFML.NET 3D";
        public static Styles winStyle = Styles.Titlebar;
        public static ContextSettings winSetting = new ContextSettings(1,1,8);
        public static ViewMode winViewMode = ViewMode.Orthographic;
    }
}