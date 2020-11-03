using Dokk.Shared;
using System;
using System.Windows.Forms;

namespace Dokk.Host
{
    public partial class Host : Form
    {
        private readonly DBConnection connection = new DBConnection();
        private readonly Timer loop = new Timer();

        public Host()
        {
            InitializeComponent();
            loop.Tick += new EventHandler(onTimerLoop);
            loop.Interval = 3000;
        }

        private void onTimerLoop(object sender, EventArgs e)
        {
            switch (connection.checkStatus())
            {
                case 0:
                    label1.Text = "Status: Idle";
                    break;
                case 1:
                    label1.Text = "Status: Ringing";
                    button1.Enabled = true;
                    break;
                default:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "New Code")
            {
                if (connection.createHostSession())
                    textBox1.Text = connection.sessionCode;

                if (connection.isConnected)
                    loop.Start();

                button1.Enabled = false;
                button1.Text = "Stop Ringing";
            }
            else if (button1.Text == "Stop Ringing")
            {
                if (connection.statusCode != 1)
                    return;

                button1.Enabled = false;

                connection.setRingingStatus(0);
            }
        }

        private void Host_FormClosing(object sender, FormClosingEventArgs e) => connection.endHostSession();
    }
}
