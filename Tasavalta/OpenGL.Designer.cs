namespace Tasavalta
{
    partial class OpenGL
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenGL));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.taakse = new System.Windows.Forms.ToolStripButton();
            this.eteen = new System.Windows.Forms.ToolStripButton();
            this.tulosta = new System.Windows.Forms.ToolStripButton();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.kaynnista = new System.Windows.Forms.ToolStripButton();
            this.pysayta = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.taakse,
            this.eteen,
            this.tulosta,
            this.toolStripComboBox1,
            this.kaynnista,
            this.pysayta});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 28);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // taakse
            // 
            this.taakse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.taakse.Image = ((System.Drawing.Image)(resources.GetObject("taakse.Image")));
            this.taakse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.taakse.Name = "taakse";
            this.taakse.Size = new System.Drawing.Size(44, 25);
            this.taakse.Text = "Back";
            // 
            // eteen
            // 
            this.eteen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.eteen.Image = ((System.Drawing.Image)(resources.GetObject("eteen.Image")));
            this.eteen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.eteen.Name = "eteen";
            this.eteen.Size = new System.Drawing.Size(67, 25);
            this.eteen.Text = "Forward";
            // 
            // tulosta
            // 
            this.tulosta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tulosta.Image = ((System.Drawing.Image)(resources.GetObject("tulosta.Image")));
            this.tulosta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tulosta.Name = "tulosta";
            this.tulosta.Size = new System.Drawing.Size(43, 25);
            this.tulosta.Text = "Print";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 28);
            // 
            // kaynnista
            // 
            this.kaynnista.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.kaynnista.ForeColor = System.Drawing.SystemColors.ControlText;
            this.kaynnista.Image = ((System.Drawing.Image)(resources.GetObject("kaynnista.Image")));
            this.kaynnista.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.kaynnista.Name = "kaynnista";
            this.kaynnista.Size = new System.Drawing.Size(40, 25);
            this.kaynnista.Text = "Play";
            // 
            // pysayta
            // 
            this.pysayta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.pysayta.Image = ((System.Drawing.Image)(resources.GetObject("pysayta.Image")));
            this.pysayta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pysayta.Name = "pysayta";
            this.pysayta.Size = new System.Drawing.Size(99, 25);
            this.pysayta.Text = "Stop/Rewind";
            // 
            // OpenGL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.toolStrip1);
            this.Name = "OpenGL";
            this.Text = "OpenGL";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton taakse;
        private System.Windows.Forms.ToolStripButton eteen;
        private System.Windows.Forms.ToolStripButton tulosta;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripButton kaynnista;
        private System.Windows.Forms.ToolStripButton pysayta;
    }
}