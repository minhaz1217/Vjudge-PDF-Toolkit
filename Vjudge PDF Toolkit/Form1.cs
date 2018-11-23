using HtmlAgilityPack;
using PdfSharp;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace Vjudge_PDF_Toolkit
{
    public partial class Form1 : Form
    {
        public string ProgPdfName = "Problem Set To Pdf";
        public class Problem {
            public string title, description, timeLimit, memoryLimit;
            public Problem(string title, string desc, string tLimit, string mLimit) {
                this.title = title;
                this.description = desc;
                this.timeLimit = tLimit;
                this.memoryLimit = mLimit;
            }
        }
        List<Problem> problemList = new List<Problem>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(textBox1.Text);
        }


        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);

            }
        }



        public Problem getVjudge(string url)
        {
            var doc = new HtmlWeb();
            var html = doc.Load(url);
            string problemTitle = html.GetElementbyId("prob-title").SelectSingleNode("//h2").InnerHtml;
            string problemLink = html.GetElementbyId("frame-description").GetAttributeValue("src", "ERROR");
            string problemTimeLimit = html.DocumentNode.SelectNodes("//dd")[0].InnerHtml;
            string problemMemoryLimit = html.DocumentNode.SelectNodes("//dd")[1].InnerHtml;
            if (problemLink != "ERROR")
            {
                problemLink = "http://vjudge.net" + problemLink;
            }
            Regex reg = new Regex(@"[0-9] B|kB|mB|MB|KB|b");
            if (!reg.Match(problemMemoryLimit).Success)
            {
                problemMemoryLimit = "ERROR";
            }
            reg = new Regex(@"[0-9] ms");
            if (!reg.Match(problemTimeLimit).Success)
            {
                problemTimeLimit = "ERROR";
            }
            Console.WriteLine(problemMemoryLimit);

            html = doc.Load(problemLink);
            string problemDiscription = html.Text;

            Problem ob1 = new Problem(problemTitle, problemDiscription, problemTimeLimit, problemMemoryLimit);
            return ob1;
        }
        public void generatePDF(string pdfString)
        {


            PdfDocument pdf = new PdfDocument();
            pdf.AddPage();
            pdf = PdfGenerator.GeneratePdf(pdfString, PageSize.A4);
            //PdfDocument pdf = PdfGenerator.GeneratePdf("<p><h1>Hello World</h1>This is html rendered text</p>", PageSize.A4);
            //pdf.Save("documen" + DateTime.Now.ToString("hh_mm_ss") + ".pdf");
            pdf.Save(ProgPdfName + ".pdf");

        }

        private void button2_Click(object sender, EventArgs e)
        {


            for (int i = 0; i < listBox1.Items.Count; i++) {
                listBox1.SelectedIndex = i;
                problemList.Add(getVjudge(listBox1.Items[i].ToString()));
                //Thread.Sleep(1000);
            }
            string mainString = "<html><body><style type='text/css'> td, h1, h2, h3, p, div,body { page-break-inside: avoid; } </style>";
            foreach(var i in problemList){
                mainString += String.Format("<div ><center><h1>{0}</h1><h3>Time Limit: {1}  & Memory Limit: {2}</h3></center><br>{3}</div><br>", i.title, i.timeLimit == "ERROR" ? "" : i.timeLimit, i.memoryLimit == "ERROR" ? "" : i.memoryLimit, i.description);
            }
            mainString += "</body></html>";
            generatePDF(mainString);

            MessageBox.Show("GENERATION DONE");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
            listBox1.Items.Add("https://vjudge.net/problem/CodeForces-432B");
            listBox1.Items.Add("https://vjudge.net/problem/CodeForces-850B");

            listBox1.Items.Add("https://vjudge.net/problem/CodeChef-L56GAME");
            listBox1.Items.Add("https://vjudge.net/problem/CodeChef-TAPAIR");
            listBox1.Items.Add("https://vjudge.net/problem/LightOJ-1389");
            */
             
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                listBox1.Items.Add(textBox1.Text);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string extension = Path.GetExtension(openFileDialog1.FileName);
                if (extension == ".txt")
                {
                                    
                    System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName);
                    string str = sr.ReadLine();
                    while (str != null)
                    {
                        if (str != "")
                        {
                            listBox1.Items.Add(str);
                        }
                        str = sr.ReadLine();
                    }
                    sr.Close();
                    MessageBox.Show("Successfully laoded: " + openFileDialog1.FileName);
                }
                else
                {
                    MessageBox.Show("Please load from a text file.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("No items to save", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Text File|*.txt";
                saveFileDialog1.Title = "Save the urls";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName != "")
                {
                    string str = "";
                    foreach (var i in listBox1.Items)
                    {
                        str += i + "\n";
                    }
                    File.WriteAllText(saveFileDialog1.FileName, str);
                    MessageBox.Show("Successfully Saved: " + saveFileDialog1.FileName);
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This was created by Minhazul Hayat Khan.\nFor any suggestion or help contact me: minhaz1217@gmail.com\nAll the problem sets belong to the respective owners\n", "About");
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1. Place any vjudge problem link in the text field, get them from here https://vjudge.net/problem/ \n2. Click 'Add' or 'Press Enter' to add them in the queue.\n3. Double click on any added link to remove it.\n4. Press 'Make PDF' to generate a pdf.\n5. Pdf will be generated with the name '" + ProgPdfName + "'", "HOW TO USE - Read carefully", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
    }
}
