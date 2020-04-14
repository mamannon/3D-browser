namespace Tasavalta
{
    partial class Form1
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

        private System.Windows.Forms.ToolStripMenuItem N3D1;
        private System.Windows.Forms.ToolStripMenuItem N3D2;
        private System.Windows.Forms.ToolStripMenuItem N3D3;
        private System.Windows.Forms.ToolStripMenuItem N3D4;
        private System.Windows.Forms.ToolStripMenuItem N3D5;
        private System.Windows.Forms.ToolStripMenuItem Stories1;
        private System.Windows.Forms.ToolStripMenuItem Stories2;
        private System.Windows.Forms.ToolStripMenuItem Stories3;
        private System.Windows.Forms.ToolStripMenuItem Stories4;
        private System.Windows.Forms.ToolStripMenuItem Stories5;
        private System.Windows.Forms.ToolStripMenuItem Apua;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.bookmarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageEnglishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useElaborateGraphicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteBookmarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createBookmarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bookmarksToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // bookmarksToolStripMenuItem
            // 
            this.bookmarksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languageEnglishToolStripMenuItem,
            this.useElaborateGraphicsToolStripMenuItem,
            this.deleteBookmarksToolStripMenuItem,
            this.createBookmarkToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem9,
            this.exitToolStripMenuItem});
            this.bookmarksToolStripMenuItem.Name = "bookmarksToolStripMenuItem";
            this.bookmarksToolStripMenuItem.Size = new System.Drawing.Size(96, 24);
            this.bookmarksToolStripMenuItem.Text = "Bookmarks";
            // 
            // languageEnglishToolStripMenuItem
            // 
            this.languageEnglishToolStripMenuItem.CheckOnClick = true;
            this.languageEnglishToolStripMenuItem.Name = "languageEnglishToolStripMenuItem";
            this.languageEnglishToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.languageEnglishToolStripMenuItem.Text = "Language english";
            this.languageEnglishToolStripMenuItem.Click += new System.EventHandler(this.ChangeLanguageClick);
            // 
            // useElaborateGraphicsToolStripMenuItem
            // 
            this.useElaborateGraphicsToolStripMenuItem.CheckOnClick = true;
            this.useElaborateGraphicsToolStripMenuItem.Name = "useElaborateGraphicsToolStripMenuItem";
            this.useElaborateGraphicsToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.useElaborateGraphicsToolStripMenuItem.Text = "Use elaborate graphics";
            this.useElaborateGraphicsToolStripMenuItem.Click += new System.EventHandler(this.UseSimpleGraphicsClick);
            // 
            // deleteBookmarksToolStripMenuItem
            // 
            this.deleteBookmarksToolStripMenuItem.Name = "deleteBookmarksToolStripMenuItem";
            this.deleteBookmarksToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.deleteBookmarksToolStripMenuItem.Text = "Delete bookmarks";
            this.deleteBookmarksToolStripMenuItem.Click += new System.EventHandler(this.DeleteBookmarksClick);
            // 
            // createBookmarkToolStripMenuItem
            // 
            this.createBookmarkToolStripMenuItem.Name = "createBookmarkToolStripMenuItem";
            this.createBookmarkToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.createBookmarkToolStripMenuItem.Text = "Create bookmark";
            this.createBookmarkToolStripMenuItem.Click += new System.EventHandler(this.CreateBookmarkClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(244, 26);
            this.toolStripMenuItem1.Text = "Bookmark 1";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.BM1Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(244, 26);
            this.toolStripMenuItem2.Text = "Bookmark 2";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.BM2Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(244, 26);
            this.toolStripMenuItem3.Text = "Bookmark 3";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.BM3Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(244, 26);
            this.toolStripMenuItem4.Text = "Bookmark 4";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.BM4Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(244, 26);
            this.toolStripMenuItem5.Text = "Bookmark 5";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.BM5Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(244, 26);
            this.toolStripMenuItem6.Text = "Bookmark 6";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.BM6Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(244, 26);
            this.toolStripMenuItem7.Text = "Bookmark 7";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.BM7Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(244, 26);
            this.toolStripMenuItem8.Text = "Bookmark 8";
            this.toolStripMenuItem8.Click += new System.EventHandler(this.BM8Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(244, 26);
            this.toolStripMenuItem9.Text = "Bookmark 9";
            this.toolStripMenuItem9.Click += new System.EventHandler(this.BM9Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.lopetus);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem bookmarksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageEnglishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useElaborateGraphicsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteBookmarksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createBookmarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
    }
}

