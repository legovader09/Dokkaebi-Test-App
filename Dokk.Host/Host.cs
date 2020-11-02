﻿using Dokk.Shared;
using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace Dokk.Host
{
    public partial class Host : Form
    {
        private DBConnection connection = new DBConnection();
        private Timer loop = new Timer();

        public Host()
        {
            InitializeComponent();
            loop.Tick += new EventHandler(onTimerLoop);
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
                    loop.Stop();
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
                {
                    loop.Interval = 5000;
                    loop.Start();
                }
                button1.Text = "Stop Ringing";
            }
            else if (button1.Text == "Stop Ringing")
            {
                if (connection.statusCode != 1)
                    return;

                connection.setRingingStatus(0);
            }
        }

        private void Host_FormClosing(object sender, FormClosingEventArgs e)
        {
            connection.endHostSession();
        }
    }
}
