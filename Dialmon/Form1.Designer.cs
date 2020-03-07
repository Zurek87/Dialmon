namespace Dialmon
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.adaptersList = new System.Windows.Forms.ListView();
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.connectionList = new System.Windows.Forms.ListView();
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // adaptersList
            // 
            this.adaptersList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader15,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader9,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader10});
            this.adaptersList.Dock = System.Windows.Forms.DockStyle.Top;
            this.adaptersList.FullRowSelect = true;
            this.adaptersList.HideSelection = false;
            this.adaptersList.Location = new System.Drawing.Point(0, 0);
            this.adaptersList.Margin = new System.Windows.Forms.Padding(6);
            this.adaptersList.Name = "adaptersList";
            this.adaptersList.Size = new System.Drawing.Size(1156, 136);
            this.adaptersList.TabIndex = 1;
            this.adaptersList.UseCompatibleStateImageBehavior = false;
            this.adaptersList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Name";
            this.columnHeader15.Width = 170;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Interface";
            this.columnHeader1.Width = 226;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "IP";
            this.columnHeader2.Width = 99;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "MAC";
            this.columnHeader3.Width = 130;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Type";
            this.columnHeader4.Width = 82;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Speed";
            this.columnHeader5.Width = 79;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Status";
            this.columnHeader9.Width = 90;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "In";
            this.columnHeader6.Width = 78;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Out";
            this.columnHeader7.Width = 75;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Errors (In/Out)";
            this.columnHeader10.Width = 96;
            // 
            // connectionList
            // 
            this.connectionList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader16,
            this.columnHeader17,
            this.columnHeader8,
            this.columnHeader11,
            this.columnHeader13,
            this.columnHeader12,
            this.columnHeader14,
            this.columnHeader18});
            this.connectionList.FullRowSelect = true;
            this.connectionList.Location = new System.Drawing.Point(0, 172);
            this.connectionList.MultiSelect = false;
            this.connectionList.Name = "connectionList";
            this.connectionList.Size = new System.Drawing.Size(1156, 242);
            this.connectionList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.connectionList.TabIndex = 2;
            this.connectionList.TileSize = new System.Drawing.Size(100, 100);
            this.connectionList.UseCompatibleStateImageBehavior = false;
            this.connectionList.View = System.Windows.Forms.View.Details;
            this.connectionList.SelectedIndexChanged += new System.EventHandler(this.connectionList_SelectedIndexChanged);
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "";
            this.columnHeader16.Width = 24;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "Name";
            this.columnHeader17.Width = 169;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "PID";
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "State";
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Local IP";
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Local Port";
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Remote IP";
            this.columnHeader14.Width = 103;
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Remote Port";
            this.columnHeader18.Width = 85;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(12, 429);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1132, 163);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(100, 145);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1156, 604);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.connectionList);
            this.Controls.Add(this.adaptersList);
            this.Name = "Form1";
            this.Text = "Dialmon";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView adaptersList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ListView connectionList;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}

