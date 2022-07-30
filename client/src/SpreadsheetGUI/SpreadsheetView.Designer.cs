namespace SpreadsheetGUI
{
    partial class SpreadsheetForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpreadsheetForm));
            this.cellNameTextBox = new System.Windows.Forms.TextBox();
            this.cellValueTextBox = new System.Windows.Forms.TextBox();
            this.cellContentsTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSpreadsheet = new System.Windows.Forms.ToolStripMenuItem();
            this.undoSpreadsheet = new System.Windows.Forms.ToolStripMenuItem();
            this.revertCellSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.closeSpreadsheet = new System.Windows.Forms.ToolStripMenuItem();
            this.darkModeButton = new System.Windows.Forms.RadioButton();
            this.lightModeButton = new System.Windows.Forms.RadioButton();
            this.connectButton = new System.Windows.Forms.Button();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.spreadsheetPanel = new SS.SpreadsheetPanel();
            this.serverNameTextBox = new System.Windows.Forms.TextBox();
            this.serverNameLabel = new System.Windows.Forms.Label();
            this.spreadsheetComboBox = new System.Windows.Forms.ComboBox();
            this.cellNameLabel = new System.Windows.Forms.Label();
            this.cellValueLabel = new System.Windows.Forms.Label();
            this.cellContentsLabel = new System.Windows.Forms.Label();
            this.new_spreadsheet_box = new System.Windows.Forms.TextBox();
            this.spreadsheetButton = new System.Windows.Forms.Button();
            this.newSpreadsheetLabel = new System.Windows.Forms.Label();
            this.existingSpreadsheetLabel = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // cellNameTextBox
            // 
            this.cellNameTextBox.Enabled = false;
            this.cellNameTextBox.Location = new System.Drawing.Point(115, 35);
            this.cellNameTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cellNameTextBox.Name = "cellNameTextBox";
            this.cellNameTextBox.ReadOnly = true;
            this.cellNameTextBox.Size = new System.Drawing.Size(337, 22);
            this.cellNameTextBox.TabIndex = 1;
            // 
            // cellValueTextBox
            // 
            this.cellValueTextBox.Enabled = false;
            this.cellValueTextBox.Location = new System.Drawing.Point(115, 63);
            this.cellValueTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cellValueTextBox.Name = "cellValueTextBox";
            this.cellValueTextBox.ReadOnly = true;
            this.cellValueTextBox.Size = new System.Drawing.Size(337, 22);
            this.cellValueTextBox.TabIndex = 2;
            // 
            // cellContentsTextBox
            // 
            this.cellContentsTextBox.AcceptsReturn = true;
            this.cellContentsTextBox.Location = new System.Drawing.Point(115, 94);
            this.cellContentsTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cellContentsTextBox.Name = "cellContentsTextBox";
            this.cellContentsTextBox.Size = new System.Drawing.Size(337, 22);
            this.cellContentsTextBox.TabIndex = 3;
            this.cellContentsTextBox.TextChanged += new System.EventHandler(this.contents_textbox_changed);
            // 
            // menuStrip
            // 
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 2);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(53, 28);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSpreadsheet,
            this.undoSpreadsheet,
            this.revertCellSelected,
            this.closeSpreadsheet});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newSpreadsheet
            // 
            this.newSpreadsheet.Name = "newSpreadsheet";
            this.newSpreadsheet.Size = new System.Drawing.Size(197, 26);
            this.newSpreadsheet.Text = "New (Ctrl + N)";
            this.newSpreadsheet.Click += new System.EventHandler(this.newSpreadsheet_Click);
            // 
            // undoSpreadsheet
            // 
            this.undoSpreadsheet.Name = "undoSpreadsheet";
            this.undoSpreadsheet.Size = new System.Drawing.Size(197, 26);
            this.undoSpreadsheet.Text = "Undo(Ctrl + Z)";
            this.undoSpreadsheet.Click += new System.EventHandler(this.undoSpreadsheet_Click);
            // 
            // revertCellSelected
            // 
            this.revertCellSelected.Name = "revertCellSelected";
            this.revertCellSelected.Size = new System.Drawing.Size(197, 26);
            this.revertCellSelected.Text = "Revert (Ctrl + Y)";
            this.revertCellSelected.Click += new System.EventHandler(this.revertCellSelected_Click);
            // 
            // closeSpreadsheet
            // 
            this.closeSpreadsheet.Name = "closeSpreadsheet";
            this.closeSpreadsheet.Size = new System.Drawing.Size(197, 26);
            this.closeSpreadsheet.Text = "Close (Alt + F4)";
            this.closeSpreadsheet.Click += new System.EventHandler(this.closeSpreadsheet_Click);
            // 
            // darkModeButton
            // 
            this.darkModeButton.AutoSize = true;
            this.darkModeButton.ForeColor = System.Drawing.Color.Black;
            this.darkModeButton.Location = new System.Drawing.Point(808, 64);
            this.darkModeButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.darkModeButton.Name = "darkModeButton";
            this.darkModeButton.Size = new System.Drawing.Size(98, 21);
            this.darkModeButton.TabIndex = 6;
            this.darkModeButton.TabStop = true;
            this.darkModeButton.Text = "Dark Mode";
            this.darkModeButton.UseVisualStyleBackColor = true;
            this.darkModeButton.CheckedChanged += new System.EventHandler(this.darkModeButton_CheckedChanged);
            // 
            // lightModeButton
            // 
            this.lightModeButton.AutoSize = true;
            this.lightModeButton.Location = new System.Drawing.Point(808, 35);
            this.lightModeButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lightModeButton.Name = "lightModeButton";
            this.lightModeButton.Size = new System.Drawing.Size(99, 21);
            this.lightModeButton.TabIndex = 7;
            this.lightModeButton.TabStop = true;
            this.lightModeButton.Text = "Light Mode";
            this.lightModeButton.UseVisualStyleBackColor = true;
            this.lightModeButton.CheckedChanged += new System.EventHandler(this.lightModeButton_CheckedChanged);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(510, 98);
            this.connectButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(259, 26);
            this.connectButton.TabIndex = 8;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Location = new System.Drawing.Point(589, 69);
            this.userNameTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(179, 22);
            this.userNameTextBox.TabIndex = 9;
            // 
            // userNameLabel
            // 
            this.userNameLabel.AutoSize = true;
            this.userNameLabel.Location = new System.Drawing.Point(506, 73);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(77, 17);
            this.userNameLabel.TabIndex = 10;
            this.userNameLabel.Text = "Username:";
            // 
            // spreadsheetPanel
            // 
            this.spreadsheetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadsheetPanel.Location = new System.Drawing.Point(0, 128);
            this.spreadsheetPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spreadsheetPanel.Name = "spreadsheetPanel";
            this.spreadsheetPanel.Size = new System.Drawing.Size(1351, 903);
            this.spreadsheetPanel.TabIndex = 5;
            this.spreadsheetPanel.Load += new System.EventHandler(this.spreadsheetPanel_Load);
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.Location = new System.Drawing.Point(589, 32);
            this.serverNameTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Size = new System.Drawing.Size(179, 22);
            this.serverNameTextBox.TabIndex = 11;
            // 
            // serverNameLabel
            // 
            this.serverNameLabel.AutoSize = true;
            this.serverNameLabel.Location = new System.Drawing.Point(507, 33);
            this.serverNameLabel.Name = "serverNameLabel";
            this.serverNameLabel.Size = new System.Drawing.Size(54, 17);
            this.serverNameLabel.TabIndex = 12;
            this.serverNameLabel.Text = "Server:";
            // 
            // spreadsheetComboBox
            // 
            this.spreadsheetComboBox.FormattingEnabled = true;
            this.spreadsheetComboBox.Location = new System.Drawing.Point(1045, 69);
            this.spreadsheetComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spreadsheetComboBox.Name = "spreadsheetComboBox";
            this.spreadsheetComboBox.Size = new System.Drawing.Size(305, 24);
            this.spreadsheetComboBox.TabIndex = 13;
            this.spreadsheetComboBox.Text = "Select Spreadsheet";
            this.spreadsheetComboBox.SelectedIndexChanged += new System.EventHandler(this.spreadsheetComboBox_SelectedIndexChanged);
            // 
            // cellNameLabel
            // 
            this.cellNameLabel.AutoSize = true;
            this.cellNameLabel.Location = new System.Drawing.Point(13, 38);
            this.cellNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cellNameLabel.Name = "cellNameLabel";
            this.cellNameLabel.Size = new System.Drawing.Size(72, 17);
            this.cellNameLabel.TabIndex = 14;
            this.cellNameLabel.Text = "CellName:";
            // 
            // cellValueLabel
            // 
            this.cellValueLabel.AutoSize = true;
            this.cellValueLabel.Location = new System.Drawing.Point(13, 69);
            this.cellValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cellValueLabel.Name = "cellValueLabel";
            this.cellValueLabel.Size = new System.Drawing.Size(75, 17);
            this.cellValueLabel.TabIndex = 15;
            this.cellValueLabel.Text = "Cell Value:";
            // 
            // cellContentsLabel
            // 
            this.cellContentsLabel.AutoSize = true;
            this.cellContentsLabel.Location = new System.Drawing.Point(13, 97);
            this.cellContentsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cellContentsLabel.Name = "cellContentsLabel";
            this.cellContentsLabel.Size = new System.Drawing.Size(95, 17);
            this.cellContentsLabel.TabIndex = 16;
            this.cellContentsLabel.Text = "Cell Contents:";
            // 
            // new_spreadsheet_box
            // 
            this.new_spreadsheet_box.Location = new System.Drawing.Point(1019, 35);
            this.new_spreadsheet_box.Margin = new System.Windows.Forms.Padding(4);
            this.new_spreadsheet_box.Name = "new_spreadsheet_box";
            this.new_spreadsheet_box.Size = new System.Drawing.Size(234, 22);
            this.new_spreadsheet_box.TabIndex = 17;
            // 
            // spreadsheetButton
            // 
            this.spreadsheetButton.Location = new System.Drawing.Point(1262, 29);
            this.spreadsheetButton.Margin = new System.Windows.Forms.Padding(4);
            this.spreadsheetButton.Name = "spreadsheetButton";
            this.spreadsheetButton.Size = new System.Drawing.Size(88, 28);
            this.spreadsheetButton.TabIndex = 18;
            this.spreadsheetButton.Text = "New";
            this.spreadsheetButton.UseVisualStyleBackColor = true;
            this.spreadsheetButton.Click += new System.EventHandler(this.spreadsheetbotton_clicked);
            // 
            // newSpreadsheetLabel
            // 
            this.newSpreadsheetLabel.AutoSize = true;
            this.newSpreadsheetLabel.Location = new System.Drawing.Point(927, 37);
            this.newSpreadsheetLabel.Name = "newSpreadsheetLabel";
            this.newSpreadsheetLabel.Size = new System.Drawing.Size(85, 17);
            this.newSpreadsheetLabel.TabIndex = 19;
            this.newSpreadsheetLabel.Text = "Create New:";
            // 
            // existingSpreadsheetLabel
            // 
            this.existingSpreadsheetLabel.AutoSize = true;
            this.existingSpreadsheetLabel.Location = new System.Drawing.Point(927, 72);
            this.existingSpreadsheetLabel.Name = "existingSpreadsheetLabel";
            this.existingSpreadsheetLabel.Size = new System.Drawing.Size(112, 17);
            this.existingSpreadsheetLabel.TabIndex = 20;
            this.existingSpreadsheetLabel.Text = "Choose Existing:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Location = new System.Drawing.Point(0, 1031);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1363, 24);
            this.statusStrip1.TabIndex = 21;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1363, 1055);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.spreadsheetPanel);
            this.Controls.Add(this.existingSpreadsheetLabel);
            this.Controls.Add(this.newSpreadsheetLabel);
            this.Controls.Add(this.spreadsheetButton);
            this.Controls.Add(this.new_spreadsheet_box);
            this.Controls.Add(this.cellContentsLabel);
            this.Controls.Add(this.cellValueLabel);
            this.Controls.Add(this.cellNameLabel);
            this.Controls.Add(this.spreadsheetComboBox);
            this.Controls.Add(this.serverNameLabel);
            this.Controls.Add(this.serverNameTextBox);
            this.Controls.Add(this.userNameLabel);
            this.Controls.Add(this.userNameTextBox);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.lightModeButton);
            this.Controls.Add(this.darkModeButton);
            this.Controls.Add(this.cellContentsTextBox);
            this.Controls.Add(this.cellValueTextBox);
            this.Controls.Add(this.cellNameTextBox);
            this.Controls.Add(this.menuStrip);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SpreadsheetForm";
            this.Text = "Spreadsheet";
            this.SizeChanged += new System.EventHandler(this.SpreadsheetForm_SizeChanged);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox cellNameTextBox;
        private System.Windows.Forms.TextBox cellValueTextBox;
        private System.Windows.Forms.TextBox cellContentsTextBox;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newSpreadsheet;
        private System.Windows.Forms.ToolStripMenuItem closeSpreadsheet;
        private SS.SpreadsheetPanel spreadsheetPanel;
        private System.Windows.Forms.RadioButton darkModeButton;
        private System.Windows.Forms.RadioButton lightModeButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.TextBox serverNameTextBox;
        private System.Windows.Forms.Label serverNameLabel;
        private System.Windows.Forms.ToolStripMenuItem undoSpreadsheet;
        private System.Windows.Forms.ToolStripMenuItem revertCellSelected;
        private System.Windows.Forms.ComboBox spreadsheetComboBox;
        private System.Windows.Forms.Label cellNameLabel;
        private System.Windows.Forms.Label cellValueLabel;
        private System.Windows.Forms.Label cellContentsLabel;
        private System.Windows.Forms.TextBox new_spreadsheet_box;
        private System.Windows.Forms.Button spreadsheetButton;
        private System.Windows.Forms.Label newSpreadsheetLabel;
        private System.Windows.Forms.Label existingSpreadsheetLabel;
        private System.Windows.Forms.StatusStrip statusStrip1;
    }
}