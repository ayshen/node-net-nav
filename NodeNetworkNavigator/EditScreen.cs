using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NodeNetworkNavigator
{
    public partial class EditWindow : Form
    {
        String title { get; set; }
        Node node { get; set; }
        public EditWindow(string name, Node node)
        {
            InitializeComponent();
            this.title = name;
            this.node = node;
            this.Location = new Point(1500, 400);
        }

        public EditWindow()
        {
            InitializeComponent();
            title = "Title from Edit Screen";
            node = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.title = ((TextBox)sender).Text;
            node.changeContent(this.title);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
