using SFML.Window;
using SFML.Graphics;
using static sfml.net_3d.Constants;

namespace sfml.net_3d
{
    public static class Data
    {
        public static RenderWindow window = new RenderWindow
        (
            new VideoMode
            (
                winSizeX,
                winSizeY
            ),

            winTitle,
            winStyle
        );
    }
}