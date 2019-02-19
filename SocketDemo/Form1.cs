using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;

namespace SocketDemo
{
    public partial class Form1 : Form
    {
        public Action<string> Msg;
        public Action<string> MsgList;
        public Action<string> DeteLists;
        public SocketManager m_socket;
        public Form1()
        {
            InitializeComponent();
        }
        private int receive;
        private int conncount;
        private void Form1_Load(object sender, EventArgs e)
        {
            ReadConfig();
            this.Msg = new Action<string>(ShowMsg);
            this.MsgList = new Action<string>(ShowList);
            this.DeteLists = new Action<string>(DeteList);

            m_socket = new SocketManager(conncount, receive);
            m_socket.Msg = this.Msg;
            m_socket.MsgList = this.MsgList;
            m_socket.DeteList = this.DeteLists;
            m_socket.Init();
            Msg.Invoke("启动Socket服务，端口号13909");
            Msg.Invoke("当前最大连接数：200，缓冲区大小：1024字节" + "\r\n");
            m_socket.Start(new IPEndPoint(IPAddress.Any, 13909));
        }

        private void ReadConfig()
        {
            conncount = int.Parse(textBoxConncount.Text);
            receive = int.Parse(textBoxReceive.Text);
        }

        public void ShowMsg(string value)
        {
            this.Invoke(new Action(() =>
            {
                textBox1.AppendText(value+ "\r\n");
            }));
        }
        public void ShowList(string value)
        {
            this.Invoke(new Action(() =>
            {
                listBox1.Items.Add(value);
            }));
        }
        public void DeteList(string value)
        {
            this.Invoke(new Action(() =>
            {
                listBox1.Items.Remove(value);
            }));
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //发送给全部
            if (comboBox1.SelectedIndex==0)
            {
                Msg.Invoke(DateTime.Now + "：主机发送给全部客户端消息\r\n" + textBox2.Text+ "\r\n");
                for (int i = 0; i < m_socket.m_clients.Count; i++)
                {
                    m_socket.SendMessage(m_socket.m_clients[i], System.Text.Encoding.Default.GetBytes(textBox2.Text));
                }
            }
            else
            {
                if (listBox1.SelectedItem!=null)
                {
                    var result = m_socket.m_clients.Find(c => c.Remote.ToString() == listBox1.SelectedItem.ToString());
                    if (result != null)
                    {
                        m_socket.SendMessage(result, System.Text.Encoding.Default.GetBytes(textBox2.Text));
                        Msg.Invoke(DateTime.Now + "：主机发送给客户端："+result.Remote+"消息\r\n"+textBox2.Text+ "\r\n");
                        return;
                    }
                    MessageBox.Show("未搜索到选择客户端信息");
                }
                else
                {
                    MessageBox.Show("请在左侧选择需要发送的客户端");
                }
            }
        }
    }
}
