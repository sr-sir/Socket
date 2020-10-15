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

namespace Socket消息传输
{
    public partial class Server : Form
    {
        //记录通信用的Socket
        Dictionary<string, Socket> dic = new Dictionary<string, Socket>();

        public Server()
        {
            InitializeComponent();
        }

        private void Server_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ip 地址
            IPAddress ip = IPAddress.Parse(textBox1.Text);

            //端口号
            IPEndPoint point = new IPEndPoint(ip, int.Parse(textBox2.Text));

            //创建监听的Socket
            /*
              AddressFamily.InterNetWork：使用 IP4地址。
              SocketType.Stream：支持可靠、双向、基于连接的字节流，而不重复数据。此类型的 Socket 与单个对方主机进行通信，并且在通信开始之前需要远程主机连接。Stream 使用传输控制协议 (Tcp) ProtocolType 和 InterNetworkAddressFamily。
              ProtocolType.Tcp：使用传输控制协议。
            */
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //创建好socket后，必须告诉socket绑定的IP地址和端口号
            //让socket监听point
            try
            {
                //socket 监听哪个端口
                socket.Bind(point);
                //同一个时间点过来的10个客户端，排队
                socket.Listen(10);
                ShowMsg("服务器开始监听");
                Thread thread = new Thread(AcceptInfo);
                thread.IsBackground = true;
                thread.Start(socket);
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

        private void AcceptInfo(object o)
        {
            Socket socket = o as Socket;
            while (true)
            {
                //通信用socket
                try
                {
                    //创建通信用的Socket
                    Socket tSocket = socket.Accept();
                    string point = tSocket.RemoteEndPoint.ToString();
                    ShowMsg(point + "连接成功！");
                    comboBox1.Items.Add(point);
                    dic.Add(point, tSocket);
                    //接收消息
                    Thread th = new Thread(ReceiveMsg);
                    th.IsBackground = true;
                    th.Start(tSocket);

                }
                catch (Exception ex)
                {

                }
            }
        }

        //接收消息
        private void ReceiveMsg(object o)
        {
            Socket client = o as Socket;
            while (true)
            {
                //接收客户端发送过来的数据
                try
                {
                    //定义byte数组存放从客户端接受过来的数据
                    byte[] buffer = new byte[1024 * 1024];
                    //将接收过来的数据放到buffer中，并返回实际接受数据的长度
                    int n = client.Receive(buffer);
                    //将字节转换成字符串
                    string words = Encoding.UTF8.GetString(buffer, 0, n);
                    ShowMsg(client.RemoteEndPoint.ToString() + ":" + words);
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message);
                    break;
                }
            }
        }

        //给客户端发送消息
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ShowMsg(richTextBox2.Text);
                string ip = comboBox1.Text;
                byte[] buffer = Encoding.UTF8.GetBytes(richTextBox2.Text);
                dic[ip].Send(buffer);
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message);
            }
        }
    }
}
