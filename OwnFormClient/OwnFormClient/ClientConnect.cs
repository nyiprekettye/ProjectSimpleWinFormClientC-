using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OwnFormClient
{
    public partial class ClientConnect : Form
    {
        public Connect con = new Connect();
        //public Player player;
        public ClientConnect()
        {
            InitializeComponent();        }

        private void button1_Click(object sender, EventArgs e)
        {
            String ip = textBox1.Text;
            String port = textBox2.Text;
            String name = textBox3.Text;
            string password = textBox4.Text;
            con.Init(ip, port);
            Console.WriteLine("inicializáltam a klienst");
            con.Start();

            con.StartSend("<NAME>"+textBox3.Text+"<EOF>");
            System.Threading.Thread.Sleep(5000);
            con.StartSend("<PASSWORD>"+textBox4.Text+"<EOF>");
            
            //String str = con.StarReceive();
            //Console.WriteLine(str);
            // backgroundWorker1.RunWorkerAsync();          
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
            
           
            //con.StartSend( "üzenet");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String sr = textBox5.Text;
            //con.StartSend("Ez egy tesztüzenet vég néklül!");
           // System.Threading.Thread.Sleep(2000);
            con.StartSend(sr);
           // con.StarReceive();
           // textBox5.Text = "<NAME>1234<EOF>";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            con.StartSend("<DISCONNECT><EOF>");
            con.Disconnect();
            //backgroundWorker1.Dispose();
        }
    }
}
