using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tower_Defense
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Game game = new Game();
            GameForm gf;
            gf = new GameForm(game);
            Thread t = new Thread(new ParameterizedThreadStart(game.Start));
            t.IsBackground = true;
            t.Start(gf);
                   
            gf.Show2();
            if (t.ThreadState == ThreadState.Running)
                t.Abort();

            //Application.Run(new GameForm());
        }
    }
}
