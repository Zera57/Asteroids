using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
    //Базовый класс планет
    abstract class Planet : BaseObject
    {
        Image img;

        protected Planet(Point pos, Size size, string path) : base(pos, new Point(3, 0), size)
        {
            img = Image.FromFile(path);
        }

        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(img, Pos.X, Pos.Y, Size.Width, Size.Height);
        }

        public override void Update()
        {
			Pos = new Point(Pos.X - Dir.X, Pos.Y);
			if (Pos.X + Size.Width < 0) Pos = new Point(7200, Pos.Y);
		}
    }

    //Планеты
    class AlienPlanet : Planet
    {
        public AlienPlanet(Point pos) : base(pos, new Size(130, 130), System.IO.Path.GetFullPath(@"PNG space\AlienPlanet.png"))
        {
        }
    }
    class Earth : Planet
    {
        public Earth(Point pos) : base(pos, new Size(150, 150), System.IO.Path.GetFullPath(@"PNG space\Earth.png"))
        {
        }
    }
    class Jupiter : Planet
    {
        public Jupiter(Point pos) : base(pos, new Size(140, 120), System.IO.Path.GetFullPath(@"PNG space\Jupiter.png"))
        {
        }
    }
    class MagmaPlanet : Planet
    {
        public MagmaPlanet(Point pos) : base(pos, new Size(100, 100), System.IO.Path.GetFullPath(@"PNG space\MagmaPlanet.png"))
        {
        }
    }
    class PinkPlanet : Planet
    {
        public PinkPlanet(Point pos) : base(pos, new Size(175, 175), System.IO.Path.GetFullPath(@"PNG space\PinkPlanet.png"))
        {
        }
    }
    class Sun : Planet
    {
        public Sun(Point pos) : base(pos, new Size(380, 380), System.IO.Path.GetFullPath(@"PNG space\Sun.png"))
        {
        }
    }
}
