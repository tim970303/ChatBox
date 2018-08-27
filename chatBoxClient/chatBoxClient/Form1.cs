﻿using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace chatBoxClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Socket SckSPort;

        string RmIp;

        int SPort = 20282;

        int RDataLen;
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.rem)
            {
                String HostName = Dns.GetHostName();
                IPHostEntry iphostentry = Dns.GetHostEntry(HostName);
                foreach (IPAddress ipaddress in iphostentry.AddressList)
                {
                    if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ipaddress.ToString().StartsWith("192"))
                    {
                        RmIp = ipaddress.ToString();
                    }
                }
            }
            else
            {
                RmIp = Properties.Settings.Default.IP;
            }
            textBox1.Text += "Server_IP : " + RmIp + "\r\n";
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void SckSReceiveProc()
        {

            try
            {

                long IntAcceptData;
                while (true)
                {
                    byte[] clientData = new byte[3000];

                    IntAcceptData = SckSPort.Receive(clientData);

                    string S = Encoding.UTF8.GetString(clientData);

                    button1.Enabled = true;
                    button2.Enabled = false;
                    textBox2.Enabled = true;
                    textBox1.AppendText(S);
                }

            }

            catch
            {
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            send("");
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void send(string s)
        {

            RDataLen = Encoding.UTF8.GetBytes(textBox2.Text + "\r\n").Length;

            if (textBox2.Text != "")
            {
                try
                {
                    string SendS = "Guest : " + textBox2.Text + "\r\n";
                    SckSPort.Send(Encoding.UTF8.GetBytes(SendS));
                    textBox1.AppendText(textBox2.Text + "\r\n");
                    textBox2.Text = "";
                }
                catch
                {
                    MessageBox.Show("server disconnect!", "Connect Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Text = "";
                    button1.Enabled = false;
                    button2.Enabled = true;
                    textBox2.Enabled = false;
                    textBox2.Text = "";
                }
            }
            else if (s == "disconnect")
            {
                try
                {
                    SckSPort.Send(Encoding.UTF8.GetBytes("//Client Disconnect//"));
                    return;
                }
                catch { }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try

            {

                SckSPort = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                SckSPort.Connect(new IPEndPoint(IPAddress.Parse(RmIp), SPort));

                Thread SckSReceiveTd = new Thread(SckSReceiveProc);
                SckSReceiveTd.IsBackground = true;
                send("");
                SckSReceiveTd.Start();


            }
            catch { MessageBox.Show("connect failure", "Connect Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            send("disconnect");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 IP = new Form2();
            IP.ShowDialog();
            DialogResult dr = IP.DialogResult;
            if (dr == DialogResult.OK && !IP.getRem())
            {
                RmIp = IP.getIP();
                textBox1.Text += "Server_IP : " + RmIp + "\r\n";
                Properties.Settings.Default.IP = IP.getIP();
                Properties.Settings.Default.rem = IP.getRem();
                Properties.Settings.Default.Save();
            }
            else if(dr == DialogResult.OK && IP.getRem())
            {
                RmIp = IP.getIP();
                textBox1.Text += "Server_IP : " + RmIp + "\r\n";
                Properties.Settings.Default.IP = IP.getIP();
                Properties.Settings.Default.rem = IP.getRem();
                Properties.Settings.Default.Save();
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.EndsWith("//Server disconnect//\r\n"))
            {
                button1.Enabled = false;
                button2.Enabled = true;
                textBox2.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text += RmIp +"\r\n";
        }
    }
}