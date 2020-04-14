using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tasavalta
{
    public partial class OpenGL : Form
    {
        public OpenGL()
        {
            InitializeComponent();

            //OpenGL ikkunaa luotaessa osa komponenteista pitää luoda dynaamisesti.
            //CheckBoxComboBox:
            CheckBoxComboBox siirto = new CheckBoxComboBox();
            siirto.Dock = DockStyle.Top;
            flowLayoutPanel1.Controls.Add(siirto);

            //Toinen ToolStrip CheckBoxComboBoxin oikealle puolelle
            ToolStrip ts = new ToolStrip();
            siirto.Dock = DockStyle.Top;
            flowLayoutPanel1.Controls.Add(ts);

            //Play nappi:
            ToolStripButton button = new ToolStripButton();
            button.Name = "kaynnista";
            button.Text = "Play";
            button.Dock = DockStyle.Top;
            ts.Items.Add(button);

            //Stop/Rewind nappi:
            button = new ToolStripButton();
            button.Name = "pysayta";
            button.Text = "Stop/Rewind";
            button.Dock = DockStyle.Top;
            ts.Items.Add(button);
        }
    }
}
