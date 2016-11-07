using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        String filename;
        Boolean halt = false;
        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        delegate void Ts();
        public Form2()
        {
            InitializeComponent();
            ValueChanged += value_Changed;
        }
        public void value_Changed(Object sender, ValueChangedEventArgs args)
        {
             circularGauge_71.CurrentValue = args.Value;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            filename = openFileDialog1.FileName;
            openFileDialog1.Reset();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            Action act = new Action(GenerateValues);
            Task.Factory.StartNew(act);
            button2.Enabled = true;

        }
        void GenerateValues()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            FileStream stream = new FileStream(filename, FileMode.Open);
            /*
            WebRequest req = WebRequest.Create(@"https://onedrive.live.com/download?cid=AAA36CBF75CC15B8&resid=AAA36CBF75CC15B8%21161&authkey=AC10x1BgbWUm5dw");
            WebResponse response = req.GetResponse() ;
            Stream content = response.GetResponseStream() ;*/
            StreamReader reader = new StreamReader(stream);
            //var reader = new System.IO.StreamReader(File.OpenRead(@"d:\data\sine.csv"));
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                listA.Add(values[0]);
                listB.Add(values[1]);
            }
            foreach (String element in listB)
            {

                
                double result;
                double.TryParse(element, out result);
                if (halt)
                {
                    halt = false;
                    stream.Close();
                    return;
                }  
                ValueChangedEventArgs args = new ValueChangedEventArgs(result);
                Control targetForm = ValueChanged.Target as System.Windows.Forms.Control;
                targetForm.Invoke(ValueChanged, new object[] { this,args });
                System.Threading.Thread.Sleep(7);
            }
            stream.Close();

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            circularGauge_71.Width = 70 * trackBar1.Value;
            circularGauge_71.Height = 70 * trackBar1.Value;
            circularGauge_71.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            halt = true;
        }
    }
}
