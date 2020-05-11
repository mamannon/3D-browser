namespace Tasavalta
{
    partial class Teksti
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

        private Paneeli paneeli;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.taakse = new System.Windows.Forms.ToolStripButton();
            this.eteen = new System.Windows.Forms.ToolStripButton();
            this.tulosta = new System.Windows.Forms.ToolStripButton();
            this.otsikkoLista = new System.Windows.Forms.ToolStripComboBox();
            this.fokusaattori = new System.Windows.Forms.ToolStripTextBox();
            this.paneeli = new Tasavalta.Paneeli();
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
            this.otsikkoLista,
            this.fokusaattori});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 28);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // taakse
            // 
            this.taakse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.taakse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.taakse.Name = "taakse";
            this.taakse.Size = new System.Drawing.Size(44, 25);
            this.taakse.Text = "Back";
            this.taakse.Click += new System.EventHandler(this.TaakseClick);
            // 
            // eteen
            // 
            this.eteen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.eteen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.eteen.Name = "eteen";
            this.eteen.Size = new System.Drawing.Size(67, 25);
            this.eteen.Text = "Forward";
            this.eteen.Click += new System.EventHandler(this.EteenClick);
            // 
            // tulosta
            // 
            this.tulosta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tulosta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tulosta.Name = "tulosta";
            this.tulosta.Size = new System.Drawing.Size(43, 25);
            this.tulosta.Text = "Print";
            this.tulosta.Click += new System.EventHandler(this.TulostaClick);
            // 
            // otsikkoLista
            // 
            this.otsikkoLista.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.otsikkoLista.Name = "otsikkoLista";
            this.otsikkoLista.Size = new System.Drawing.Size(400, 28);
            this.otsikkoLista.Click += new System.EventHandler(this.ValittuOtsikko);
            this.otsikkoLista.MouseEnter += new System.EventHandler(this.PaivitaOtsikot);
            // 
            // fokusaattori
            // 
            this.fokusaattori.Name = "fokusaattori";
            this.fokusaattori.Size = new System.Drawing.Size(1, 28);
            // 
            // paneeli
            // 
            this.paneeli.AutoScroll = true;
            this.paneeli.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paneeli.Location = new System.Drawing.Point(0, 28);
            this.paneeli.Name = "paneeli";
            this.paneeli.Size = new System.Drawing.Size(800, 422);
            this.paneeli.TabIndex = 1;
            this.paneeli.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PaneeliClick);
            this.paneeli.MouseLeave += new System.EventHandler(this.HiiriPoistuu);
            this.paneeli.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HiiriLiikkuu);
            // 
            // Teksti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.paneeli);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Teksti";
            this.Text = "Teksti";
            this.Resize += new System.EventHandler(this.MuutaKokoa);
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
        private System.Windows.Forms.ToolStripComboBox otsikkoLista;
 //       private System.Windows.Forms.Panel paneeli;
        private System.Windows.Forms.ToolStripTextBox fokusaattori;
    }
}