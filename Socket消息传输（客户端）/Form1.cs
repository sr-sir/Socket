using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Socket消息传输_客户端_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private void button1_Click(object sender, EventArgs e)
        {
            //连接到的目标IP
            IPAddress ip = IPAddress.Parse(textBox1.Text);
            //连接到目标IP的那个应用（端口号！）
            IPEndPoint point = new IPEndPoint(ip, int.Parse(textBox2.Text));
            try
            {
                //连接到服务器
                client.Connect(point);
                ShowMsg("连接成功");
                ShowMsg("服务器：" + client.RemoteEndPoint.ToString());
                ShowMsg("客户端：" + client.LocalEndPoint.ToString());
                //连接成功后，就可以接收服务器发送的信息了
                Thread th = new Thread(ReceiveMsg);
                th.IsBackground = true;
                th.Start();

            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message);
            }
        }

        private void ShowMsg(string msg)
        {
            richTextBox1.AppendText(msg + "\r\n");
        }

        //接收服务器的消息
        private void ReceiveMsg()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024];
                    int n = client.Receive(buffer);
                    string s = Encoding.UTF8.GetString(buffer, 0, n);
                    ShowMsg(client.RemoteEndPoint.ToString() + ":" + s);
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message);
                    break;
                }
            }

        }

        //客户端给服务器发消息
        private void button2_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                try
                {
                    ShowMsg(richTextBox2.Text);
                    byte[] buffer = Encoding.UTF8.GetBytes(richTextBox2.Text);
                    client.Send(buffer);
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message);
                }
            }

        }
    }
}
