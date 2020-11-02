using Dokk.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dokkaebi_Test_App
{
    public partial class Client : Form
    {
        private DBConnection connection = new DBConnection();
        private Timer loop = new Timer();

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
