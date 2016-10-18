using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        String filename;
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            filename = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileStream stream = new FileStream(filename,FileMode.Open);

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
                circularGauge_71.CurrentValue = result;
                System.Threading.Thread.Sleep(70);
            }

        }

    
}
}
