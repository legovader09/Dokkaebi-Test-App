using Dokk.Shared;
using System;
using System.Windows.Forms;

namespace Dokkaebi_Test_App
{
    public partial class Client : Form
    {
        private readonly DBConnection connection = new DBConnection();
        private readonly Timer loop = new Timer();

        public Client()
        {
            InitializeComponent();
            loop.Tick += new EventHandler(onTimerLoop);
            loop.Interval = 3000;
        }

        private void onTimerLoop(object sender, EventArgs e)
        {
            if (connection.checkStatus() == 0)
            {
                label1.Text = "Status: Not Ringing";
                button1.Enabled = true;
            }
            else
            {
                label1.Text = "Status: Ringing";
                button1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 6)
            {
                connection.sessionCode = textBox1.Text;
                connection.setRingingStatus(1);
                loop.Start();
            }
        }
    }
}
