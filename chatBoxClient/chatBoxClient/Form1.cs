using System;
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

        Socket SckSPortLocal;    //定義socket
        string RmIp;    //定義目標IP
        int SPort = 20282;  //定義port

        private void Form1_Load(object sender, EventArgs e)
        {
            SckSPortLocal = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (Properties.Settings.Default.IP == "")//定義目標IP
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
                while (true)
                {
                    byte[] clientData = new byte[3000];
                    SckSPortLocal.Receive(clientData);
                    textBox1.AppendText(Encoding.UTF8.GetString(clientData));
                    button1.Enabled = true;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    textBox2.Enabled = true;
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
            if (textBox2.Text != "")
            {
                try
                {
                    string SendS = "Guest : " + textBox2.Text + "\r\n";
                    SckSPortLocal.Send(Encoding.UTF8.GetBytes(SendS));
                    textBox1.AppendText(textBox2.Text + "\r\n");
                    textBox2.Text = "";
                }
                catch
                {
                    MessageBox.Show("server disconnect!", "Connect Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (s == "disconnect")
            {
                try
                {
                    SckSPortLocal.Send(Encoding.UTF8.GetBytes("//Client Disconnect//"));
                    return;
                }
                catch { }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SckSPortLocal = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                SckSPortLocal.Connect(new IPEndPoint(IPAddress.Parse(RmIp), SPort));
                SckSWaitAccept(true);
            }
            catch
            {
                MessageBox.Show("connect failure", "Connect Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SckSWaitAccept(bool s)   //等待連線
        {
            Thread SckSAcceptTd = new Thread(SckSReceiveProc);   //開始新執行續 避免程式當住
            if (s)
            {
                SckSAcceptTd.Start();
                SckSAcceptTd.IsBackground = true;
            }
            else
            {
                SckSAcceptTd.Abort();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textBox1.Text.Contains("//Server disconnect//")) { return; }
            send("disconnect");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 IP = new Form2();
            IP.ShowDialog();
            DialogResult dr = IP.DialogResult;
            if (!IP.getRem())
            {
                RmIp = IP.getIP();
                SckSPortLocal = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                SckSWaitAccept(false);
                textBox1.Text += "Server_IP : " + RmIp + "\r\n";
            }
            else if(IP.getRem())
            {
                RmIp = IP.getIP();
                SckSPortLocal = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                SckSWaitAccept(false);
                textBox1.Text += "Server_IP : " + RmIp + "\r\n";
                Properties.Settings.Default.IP = IP.getIP();
                Properties.Settings.Default.Save();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.EndsWith("//Server disconnect//\r\n"))
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!SckSPortLocal.Connected)
            {
                button1.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = true;
                textBox2.Enabled = false;
            }
        }
    }
}
