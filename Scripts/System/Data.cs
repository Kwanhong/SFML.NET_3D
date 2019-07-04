using SFML.Window;
using SFML.Graphics;
using static SFML_NET_3D.Constants;

namespace SFML_NET_3D
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