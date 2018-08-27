using System;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace chatBoxHome
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Socket SckSs;   //定義socket
        string LocalIP; //定義本地IP
        int SPort = 20282;  //定義本地Port
        String HostName = Dns.GetHostName();    //取得本地主機名稱

        private void Form1_Load(object sender, EventArgs e)
        {
            IPHostEntry iphostentry = Dns.GetHostEntry(HostName);   //取得本地IP
            foreach (IPAddress ipaddress in iphostentry.AddressList)    //過濾非本地IP位址
            {
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ipaddress.ToString().StartsWith("192"))
                {
                    LocalIP = ipaddress.ToString();
                }
            }

            textBox1.Text += "Server_IP : " + LocalIP + "\r\n";
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Listen();
        }

        private void Listen()   //開始監聽
        {
            SckSs = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//設定socket
            SckSs.Bind(new IPEndPoint(IPAddress.Parse(LocalIP), SPort));//綁定IP,Porrt
            SckSs.Listen(1);
            SckSWaitAccept(true);
        }

        private void SckSWaitAccept(bool s)   //等待連線
        {
            Thread SckSAcceptTd = new Thread(SckSAcceptProc);   //開始新執行續 避免程式當住
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

        private void SckSAcceptProc()
        {
            try
            {
                SckSs = SckSs.Accept(); //成功連接
                textBox1.AppendText("Connect!!\r\n");

                button1.Enabled = true;
                button2.Enabled = false;
                textBox2.Enabled = true;
                send("first");//喚醒

                while (true)  

                {
                    byte[] clientData = new byte[3000];//設定緩衝區大小
                    SckSs.Receive(clientData);  //接收資料放在clientData
                    textBox1.AppendText(Encoding.UTF8.GetString(clientData));//轉換資料
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

        private void send(string s)
        {

            switch (s)
            {
                case "first":
                    SckSs.Send(Encoding.UTF8.GetBytes("//Connect successful//\r\n"));
                    return;
                case "disconnect":
                    try
                    {
                        SckSs.Send(Encoding.UTF8.GetBytes("//Server disconnect//\r\n"));
                    }
                    catch { }
                    break;
                default:
                    if (SckSs.Connected == true)
                    {
                        if (textBox2.Text != "")
                        {
                            try
                            {
                                string SendS = "Server : " + textBox2.Text + "\r\n";
                                SckSs.Send(Encoding.UTF8.GetBytes(SendS));
                                textBox1.AppendText(textBox2.Text + "\r\n");
                                textBox2.Text = "";
                            }
                            catch
                            {
                            }
                        }
                    }
                    break;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.EndsWith("//Client Disconnect//\r\n"))    //用戶端斷線
            {
                MessageBox.Show("Client Disconnect\r\nPlease restart the application","Connect Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            send("disconnect"); //伺服端斷線
        }

        private void button2_Click(object sender, EventArgs e)//切換ip
        {
            IPHostEntry iphostentry = Dns.GetHostEntry(HostName);
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ipaddress.ToString().StartsWith("192") && button2.Text == "Private")
                {
                    LocalIP = ipaddress.ToString();
                    SckSs.Close();
                    SckSWaitAccept(false);
                    Listen();
                    textBox1.Text += "Server_IP : " + LocalIP + "\r\n";
                    button2.Text = "Public";
                    return;
                }
                else if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !ipaddress.ToString().StartsWith("192") && button2.Text != "Private")
                {
                    LocalIP = ipaddress.ToString();
                    SckSs.Close();
                    SckSWaitAccept(false);
                    Listen();
                    textBox1.Text += "Server_IP : " + LocalIP + "\r\n";
                    button2.Text = "Private";
                    return;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)//偵錯
        {
            textBox1.Text += SckSs.LocalEndPoint + "\r\n";
        }
    }
}