using Dokk.Shared;
using System;
using System.Windows.Forms;

namespace Dokk.Host
{
    public partial class Host : Form
    {
        public Host()
        {
            InitializeComponent();
        }

        private DBConnection connection = new DBConnection();

        private void button1_Click(object sender, EventArgs e)
        {
            if (connection.createHostSession())
                textBox1.Text = connection.sessionCode;
        }

        private void Host_FormClosing(object sender, FormClosingEventArgs e)
        {
            connection.endHostSession();
        }
    }
}
