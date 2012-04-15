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
            //t.Start();
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
                lock (msgs)
                {
                    for (int i = msgs.Count - 1; i > 0; i--)
                    {
                        SetText(msgs[i]);
                        msgs.RemoveAt(i);
                    }
                }

            }
            Thread.Sleep(1);
        }
        }

        delegate void SetTextCallback(string text);
        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            text = this.textBox1.Text + "\\n" + text;
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.Text = text;
            }
        }

    }
}
