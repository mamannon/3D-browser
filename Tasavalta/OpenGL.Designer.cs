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

        private CheckBoxComboBox layeriLista;
        private System.Windows.Forms.ToolStripButton kaynnista;
        private System.Windows.Forms.ToolStripButton pysayta;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenGL));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.taakse = new System.Windows.Forms.ToolStripButton();
            this.eteen = new System.Windows.Forms.ToolStripButton();
            this.tulosta = new System.Windows.Forms.ToolStripButton();
            this.flowLayoutPanel1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AllowDrop = true;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.toolStrip2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(800, 27);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.taakse,
            this.eteen,
            this.tulosta});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(167, 27);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // taakse
            // 
            this.taakse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.taakse.Image = ((System.Drawing.Image)(resources.GetObject("taakse.Image")));
            this.taakse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.taakse.Name = "taakse";
            this.taakse.Size = new System.Drawing.Size(44, 24);
            this.taakse.Text = "Back";
            // 
            // eteen
            // 
            this.eteen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.eteen.Image = ((System.Drawing.Image)(resources.GetObject("eteen.Image")));
            this.eteen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.eteen.Name = "eteen";
            this.eteen.Size = new System.Drawing.Size(67, 24);
            this.eteen.Text = "Forward";
            // 
            // tulosta
            // 
            this.tulosta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tulosta.Image = ((System.Drawing.Image)(resources.GetObject("tulosta.Image")));
            this.tulosta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tulosta.Name = "tulosta";
            this.tulosta.Size = new System.Drawing.Size(43, 24);
            this.tulosta.Text = "Print";
            // 
            // OpenGL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "OpenGL";
            this.Text = "OpenGL";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NappaimistoClick);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton taakse;
        private System.Windows.Forms.ToolStripButton eteen;
        private System.Windows.Forms.ToolStripButton tulosta;
    }
}