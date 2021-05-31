using System;
using System.Collections.Generic;
using System.Drawing;

namespace MyGame
{
    //Исключения объектов
    class GameObjectException : Exception
    {
        public GameObjectException()
        {
            Console.WriteLine(base.Message);
        }
    }
    //Интерфейс пересечения
    interface ICollision
    {
        bool Collision(ICollision obj);
        Rectangle Rect { get; }
    }

    //Базовый объект
    abstract class BaseObject
    {
		public Point Pos { get; protected set; }
        protected Point Dir;
        public Size Size { get; protected set; }
		protected BaseObject(Point pos, Point dir, Size size)
        {
            Pos = pos;
            Dir = dir;
            if (size.Height <= 0 || size.Width <= 0)
                throw new GameObjectException();
            Size = size;
        }
        public abstract void Draw();

        public abstract void Update();

    }

    //Круглые астероды
    class Asteroid : BaseObject
    {
        public int Power { get; set; }
        public Asteroid(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            Power = 1;
        }
        public override void Draw()
        {
            Game.Buffer.Graphics.FillEllipse(Brushes.White, Pos.X, Pos.Y, Size.Width, Size.Height);
        }

        public override void Update()
        {
			Pos = new Point(Pos.X + Dir.X, Pos.Y + Dir.Y);
            if (Pos.X < 0) Dir.X = -Dir.X;
            if (Pos.X > Game.Width) Dir.X = -Dir.X;
            if (Pos.Y < 0) Dir.Y = -Dir.Y;
            if (Pos.Y > Game.Height) Dir.Y = -Dir.Y;
        }
    }

    //Звёзды
    class Star : BaseObject
    {
        public Star(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
        }

        public override void Draw()
        {
            Game.Buffer.Graphics.DrawLine(Pens.White, Pos.X, Pos.Y, Pos.X + Size.Width, Pos.Y + Size.Height);
            Game.Buffer.Graphics.DrawLine(Pens.White, Pos.X + Size.Width, Pos.Y, Pos.X, Pos.Y + Size.Height);
        }

        public override void Update()
        {
			Pos = new Point(Pos.X + Dir.X, Pos.Y);
            if (Pos.X < 0) Pos = new Point(Game.Width + Size.Width, Pos.Y);
        }
    }


}