using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace chatBoxClient
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        private string IP;
        private bool rem = false;
        private void button1_Click(object sender, EventArgs e)
        {
            setIP();
        }

        public void setIP()
        {
            if (textBox1.Text == "")
            {
                String HostName = Dns.GetHostName();
                IPHostEntry iphostentry = Dns.GetHostEntry(HostName);
                foreach (IPAddress ipaddress in iphostentry.AddressList)
                {
                    if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ipaddress.ToString().StartsWith("192"))
                    {
                        IP = ipaddress.ToString();
                        return;
                    }
                }
            }
            IP = textBox1.Text;
        }

        public string getIP()
        {
            return IP;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            rem = checkBox1.Checked;
        }
        public bool getRem()
        {
            return rem;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            textBox1.Text = Properties.Settings.Default.IP;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if ((e.KeyChar <= 57 && e.KeyChar >= 48 || e.KeyChar <= 69 && e.KeyChar >= 60) || (e.KeyChar == 8 || e.KeyChar == '.'))
            {
                e.Handled = false;
            }
            else { e.Handled = true; }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            setIP();
        }
    }
}
