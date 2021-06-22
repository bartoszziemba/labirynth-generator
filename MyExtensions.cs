using SFML.Graphics;

namespace labirynth
{
    static class MyExtensions
    {
        public static void DrawList(this RenderWindow window, System.Collections.Generic.List<Drawable> list)
        {
            foreach (var el in list)
            {
                window.Draw(el);
            }
        }
    }
}