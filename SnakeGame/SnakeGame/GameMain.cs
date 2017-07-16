using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    using Augite;

    class GameMain : Game
    {
        private FontManager _fontManager;
        private List<IWork> _works;

        public GameMain()
            :base()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.gameWindow.title = string.Format("貪食蛇{0}.{1} by ahui", version.Major, version.Minor);
            this.gameWindow.width = 960;
            this.gameWindow.height = 540;
            this.gameWindow.frameRate = 24;

            _works = new List<IWork>();
            _fontManager = new FontManager();

            this.gameUpdateEvent += GameMain_gameUpdateEvent;
        }

        private void GameMain_gameUpdateEvent(GameTime gameTime)
        {
            if(_works.Count > 0)
            {
                lock (_syncWorks)
                {
                    TimeSpan totalDelta = TimeSpan.Zero;
                    int execWorkCount = 0;
                    int totalWorks = _works.Count;

                    for (int i = 0; i < _works.Count;)
                    {
                        var t1 = DateTime.Now;

                        IWork work = _works[i];
                        _works.RemoveAt(i);

                        work.run(gameTime);

                        var delta = DateTime.Now - t1;
                        totalDelta += delta;
                        if (totalDelta.TotalSeconds > 0.01f)
                        {
                            break;
                        }
                        execWorkCount++;
                    }

                    if (execWorkCount > 0)
                    {
                       // Console.WriteLine("execWork={0}/{1} totalDelta={2}", execWorkCount, totalWorks, totalDelta);
                    }
                }
            }
           
        }

        public FontManager fontManager
        {
            get
            {
                return _fontManager;
            }
        }

        public void playNewGame()
        {
            this.stage.children.Clear();

            this.stage.addChild(this.newSprite<GamePlay>());
        }


        public override void init()
        {
            fontManager.load();
            TextureManager.sharedInst.load(this);


            this.stage.addChild(this.newSprite<StartMenuSprite>());
        }


        private object _syncWorks = new object();

        public void putWork(IWork work)
        {
            lock(_syncWorks)
            {
                _works.Add(work);
            }
        }

    }

    interface IWork
    {
        void run(GameTime gameTime);
    }

}
