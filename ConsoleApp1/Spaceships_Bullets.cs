using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
    //Пуля
    class Bullet : BaseObject, ICollision, IDisposable
    {
        Image laser = Image.FromFile(@"PNG space\Laser.png");

        public Bullet(Point pos, Point dir) : base(pos, dir, new Size(25, 6))
        {
        }
        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(laser, Pos.X, Pos.Y, Size.Width, Size.Height);
        }
        public override void Update()
        {
            Pos = new Point(Pos.X + (Dir.X * 15), Pos.Y);
        }

        public bool Collision(ICollision o) => o.Rect.IntersectsWith(this.Rect);

		public void Dispose()
		{
			laser.Dispose();
			laser = null;
			GC.SuppressFinalize(this);
		}

		public Rectangle Rect => new Rectangle(Pos, Size);
    }

    //Класс для работы со всеми пулями 
    static class Bullets
    {
        //Хранит все пули
        static List<Bullet> list = new List<Bullet>();

        static public void Add(Bullet bullet)
        {
            list.Add(bullet);
        }

		static public void Remove(Bullet bullet)
		{
			list.Remove(bullet);
			bullet.Dispose();
		}

        //Отрисовка
        static public void Draw()
        {
            foreach (Bullet bullet in list)
            {
                bullet.Draw();
            }
        }

        static public void Update()
        {
			for (int i = 0; i < list.Count; i++)
			{
                list[i].Update();
				if (list[i].Pos.X + list[i].Size.Width < 0 || list[i].Pos.X > Game.Width)
					Remove(list[i]);
            }
        }

        //Проверка на столкновение пуль с кораблем
        static public bool Interacts(Spaceship spaceship)
        {
			for (int i = 0; i < list.Count; i++)
			{ 
                if (spaceship.Rect.IntersectsWith(list[i].Rect))
                {
					Remove(list[i]);
					spaceship.Get_Damage(15);
                    return true;
                }
            }
            return false;
        }
    }

    //Базовый класс корабля
    abstract class Spaceship : BaseObject, ICollision
    {
        public bool Alive { get; protected set; }

        public int HP { get; protected set; }
        protected int Speed;

        Image Img;

        protected Spaceship(int hp, Point pos, int speed, Size size, string path) : base(pos, new Point(0, 0), size)
        {
            this.Alive = true;
            this.HP = hp;
            this.Speed = speed;
            this.Img = Image.FromFile(@path);
        }

        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(Img, Pos.X, Pos.Y, Size.Width, Size.Height);
            Bullets.Draw();
        }

        public void Get_Damage(int damage)
        {
            HP -= damage;
            if (HP <= 0)
            {
                Alive = false;
            }
        }

        public bool Collision(ICollision o)
        {
            return o.Rect.IntersectsWith(this.Rect);
        }
        public Rectangle Rect => new Rectangle(Pos, Size);
    }

    //Класс противников
    class Enemy : Spaceship
    {
        public Enemy(Point pos) : base(200, pos, 10, new Size(70, 50), System.IO.Path.GetFullPath(@"PNG space\UFO.png"))
        {
        }

        public void Shoot()
        {
            Bullets.Add(new Bullet(new Point(Pos.X - 25, Pos.Y + Size.Height / 2), new Point(-1, 0)));
        }

        public void Respawn()
        {
            HP = 200;
            Alive = true;
        }

		public override void Update()
		{
		}

		public void Update(int y)
        {
            Pos = new Point(Pos.X, Pos.Y + y);
        }
    }

    //Класс персонажа
    class Player : Spaceship
    {
		public new Point Dir;

		public Player(Point pos) : base(400, pos, 5, new Size(50, 50), System.IO.Path.GetFullPath(@"PNG space\Spaceship.png"))
        {
        }

        //Управление персонажем
        public override void Update()
        {

			Pos = new Point(Pos.X + (Dir.X * this.Speed), Pos.Y + (Dir.Y * this.Speed));


            if (Pos.X < 0) Pos = new Point(0, Pos.Y);
            if (Pos.X > Game.Width - this.Size.Width) Pos = new Point(Game.Width - this.Size.Width, Pos.Y);
            if (Pos.Y < 0) Pos = new Point(Pos.X, 0);
            if (Pos.Y > Game.Height - this.Size.Height) Pos = new Point(Pos.X, Game.Height - this.Size.Height);
        }

        public void Shoot()
        {
            Bullets.Add(new Bullet(new Point(Pos.X + Size.Width, Pos.Y + Size.Height / 2), new Point(1, 0)));
        }
    }

	class HealPack : BaseObject, ICollision
	{
		public bool Exist { get; private set; }
		Image Img = Image.FromFile(@"PNG space\ToolCase.png");
		int Heal;

		public HealPack(Point pos) : base(pos, new Point(-5, 0), new Size(30, 30))
		{
			Exist = true;
			Heal = 100;
		}


		public override void Draw()
		{
			Game.Buffer.Graphics.DrawImage(Img, Pos.X, Pos.Y, Size.Width, Size.Height);
		}

		public override void Update()
		{
			Pos = new Point(Pos.X + Dir.X, Pos.Y);
			if (Pos.X + Size.Width < 0)
				Death();
		}

		public void Respawn() { Exist = true;  Pos = new Point(Game.Width, Pos.Y); }
		private void Death() { Exist = false; }

		public Rectangle Rect => new Rectangle(Pos, Size);

		public bool Collision(ICollision o)
		{
			bool b = o.Rect.IntersectsWith(this.Rect);
			if (b)
				Death();
			return b;
		}
	}
}
