using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace MyGame
{
    static class Game
    {
        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;
        // Свойства
        // Ширина и высота игрового поля
        public static int Width { get; set; }
        public static int Height { get; set; }
		public static int Score { get; set; }
        static Timer timerFPS = new Timer { Interval = 80 };
		static Timer timerEnemy = new Timer { Interval = 30000 };
		static Timer timerHealPack = new Timer { Interval = 30000 };
		static Game()
        {
        }
        public static void Init(Form form)
        {
            //Проверка размера формы
            if(form.Height > 1000 || form.Width > 1000 || form.Width < 0 || form.Height < 0)
            {
                throw new ArgumentOutOfRangeException("Границы не правильных размеров.");
            }

            // Графическое устройство для вывода графики            
            Graphics g;
            // Предоставляет доступ к главному буферу графического контекста для текущего приложения
            _context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics();
            // Создаем объект (поверхность рисования) и связываем его с формой
            // Запоминаем размеры формы
            Width = form.ClientSize.Width;
            Height = form.ClientSize.Height;
			Score = 0;
            // Связываем буфер в памяти с графическим объектом, чтобы рисовать в буфере
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));

            Load();

            //Таймер обновления экрана
            timerFPS.Start();
            timerFPS.Tick += Timer_Tick;

            //Таймер обновления врага
            timerEnemy.Tick += new System.EventHandler(Timer_SpawnEnemy);

			//Таймер аптечек
			timerHealPack.Start();
			timerHealPack.Tick += Timer_SpawnHealPack;

			form.KeyDown += Form_KeyDown;
		}


		private static void Form_Space(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 32)
				player.Shoot();
		}

		private static void Form_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Up:
					player.Dir = new Point(0, -1);
					break;
				case Keys.Down:
					player.Dir = new Point(0, 1);
					break;
				case Keys.Left:
					player.Dir = new Point(-1, 0);
					break;
				case Keys.Right:
					player.Dir = new Point(1, 0);
					break;
				case Keys.Space:
					player.Dir = new Point(0, 0);
					player.Shoot();
					break;
				case Keys.Escape:
					System.Environment.Exit(0);
					break;
				default:
					//Console.WriteLine("Default case");
					break;
			}
			player.Update();

		}


		//Метод отрисовки всех объектов
		public static void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            
            foreach (BaseObject obj in _objs)
                obj.Draw();

            foreach (Planet planet in Planets)
                planet.Draw();

            //Отрисовка врага
            if (enemy.Alive) enemy.Draw();

            //Отрисовка игрока
            if (player.Alive) player.Draw();

            //Отрисовка всех лазеров
            Bullets.Draw();

			if(healPack.Exist) healPack.Draw();

			Game.Buffer.Graphics.DrawString(Score.ToString(), new Font("Arial", 24, FontStyle.Bold), Brushes.Yellow, new RectangleF(new Point(1, 1), new Size(75, 50)));
			Game.Buffer.Graphics.DrawString($"HP:{player.HP.ToString()}", new Font("Arial", 24, FontStyle.Bold), Brushes.Yellow, new RectangleF(new Point(Game.Width -130, 1), new Size(130, 50)));

			Buffer.Render();
        }


		public static void Update()
		{
			foreach (Planet planet in Planets)
				planet.Update();

			foreach (BaseObject obj in _objs)
				obj.Update();

			if (player.Alive)
			{
				//Если пули пресекаются, в самом методе прописано наносение урона
				if (Bullets.Interacts(player))
				{
					using (System.Media.SoundPlayer simpleSound = new System.Media.SoundPlayer(@"PNG Space\PlayerDeathSound.wav"))
					{
						simpleSound.Play();
						simpleSound.Dispose();
					}
				}
			}

			var random = new Random();

			if (enemy.Alive && player.Alive)
			{
				//Обновление положения игрока
				if (enemy.Pos.Y != player.Pos.Y)
				{
					if (random.Next(1, 100) <= 20)
					{
						enemy.Update(player.Pos.Y > enemy.Pos.Y ? 10 : -10);
					}
				}
				//Стрельба
				if (random.Next(1, 100) <= 20)
				{
					enemy.Shoot();
				}
				//Попадание по врагу
				if (Bullets.Interacts(enemy))
				{
					using (System.Media.SoundPlayer simpleSound = new System.Media.SoundPlayer(@"PNG Space\PlayerDeathSound.wav"))
					{
						simpleSound.Play();
						simpleSound.Dispose();
					}
				}
			}

			//Обновление пуль
			Bullets.Update();

			if (healPack.Exist)
			{
				healPack.Update();

				if (healPack.Collision(player))
					player.Get_Damage(-healPack.Heal);
			}
			if (!timerEnemy.Enabled && !enemy.Alive)
			{
				timerEnemy.Start();
				Score++;
			}
		}

        public static BaseObject[] _objs;
        public static List<Planet> Planets;
        public static Player player;
        public static Enemy enemy;
		static HealPack healPack;


		public static void Load()
        {
            Planets = new List<Planet>();

            //Добавляем планеты
            Planets.Add(new AlienPlanet(new Point(Planets.Count * 1200 + 100, 100)));

            Planets.Add(new Earth(new Point(Planets.Count * 1200 + 100, 350)));

            Planets.Add(new Jupiter(new Point(Planets.Count * 1200 + 100, 250)));

            Planets.Add(new MagmaPlanet(new Point(Planets.Count * 1200 + 100, 160)));

            Planets.Add(new PinkPlanet(new Point(Planets.Count * 1200 + 100, 420)));

            Planets.Add(new Sun(new Point(Planets.Count * 1200 + 100, -170)));

            //Создаем игрока
            player = new Player(new Point(50, Game.Height/2));

            //Создаем врага
            enemy = new Enemy(new Point(Game.Width - 100, Game.Height / 2));

            var rand = new Random();

			healPack = new HealPack(new Point(Game.Width, rand.Next(50, Game.Width - 50)));

			_objs = new BaseObject[20];
            for (int i = 0; i < _objs.Length / 2; i++)
                _objs[i] = new Asteroid(new Point(rand.Next(1, Game.Width), rand.Next(1, Game.Height)), new Point(-i-1, -i), new Size(10, 10));


            for (int i = 0; i < _objs.Length / 2; i++)
                _objs[i + _objs.Length / 2] = new Star(new Point(600, i * 60), new Point(-rand.Next(5, 20), 0), new Size(5, 5));
        }


        //Обновление кадра
        private static void Timer_Tick(object sender, EventArgs e)
        {
            Draw();
            Update();
        }

        //Таймер спавна врага
        private static void Timer_SpawnEnemy(object sender, EventArgs e)
        {
			if (!enemy.Alive)
			{
				enemy.Draw();
				enemy.Respawn();
			}
			timerEnemy.Stop();
		}

		private static void Timer_SpawnHealPack(object sender, EventArgs e)
		{
			healPack.Respawn();
		}
    }
}