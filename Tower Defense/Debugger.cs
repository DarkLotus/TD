using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tower_Defense
{
    public partial class Debugger : Form
    {
        public Debugger()
        {
            InitializeComponent();
            Thread t = new Thread(new ThreadStart(PumpLog));
            t.IsBackground = true;
            t.Start();
        }
        private List<string> msgs = new List<string>();

        public void Debug(string s)
        {
            //msgs.Add(s);
        }
        private void PumpLog()
        {
        while(true)
        {
            if(msgs.Count > 0)
            {
                foreach (var s in msgs)
                    this.textBox1.Text += s;
                lock (msgs)
                    msgs.Clear();
            }
            Thread.Sleep(5);
        }
        }
    }
}
