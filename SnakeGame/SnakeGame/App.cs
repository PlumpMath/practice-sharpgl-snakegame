using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    class App
    {

        private static object _syncObj = new object();
        private static App _inst = null;

        public static App sharedInst
        {
            get
            {
                if (_inst == null)
                {
                    lock (_syncObj)
                    {
                        if (_inst == null)
                        {
                            _inst = new App();
                        }
                    }
                }

                return _inst;
            }
          
        }

      

        public App()
        {
            
        }      
       
          
        public void run()
        {
           

            using (var game = new GameMain())
            {

                //
                game.run();
            }
        }


    }
}
