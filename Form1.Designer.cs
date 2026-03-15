namespace ArmaSQFBrowser
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
  ///  Required method for Designer support - do not modify
  ///  the contents of this method with the code editor.
  /// </summary>
  private void InitializeComponent()
  {
   components = new System.ComponentModel.Container();
   matchesList = new ListBox();
   filesProcessed = new Label();
   startSearch = new Button();
   scriptFilename = new Label();
   fileView = new RichTextBox();
   armaPath = new TextBox();
   label1 = new Label();
   searchFor = new TextBox();
   formTooltips = new ToolTip(components);
   SuspendLayout();
   // 
   // matchesList
   // 
   matchesList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
   matchesList.FormattingEnabled = true;
   matchesList.ItemHeight = 15;
   matchesList.Location = new Point(12, 77);
   matchesList.Name = "matchesList";
   matchesList.Size = new Size(322, 469);
   matchesList.TabIndex = 1;
   formTooltips.SetToolTip(matchesList, "Found matches in SQF files");
   matchesList.SelectedIndexChanged += matchesList_SelectedIndexChanged;
   // 
   // filesProcessed
   // 
   filesProcessed.AutoSize = true;
   filesProcessed.Location = new Point(12, 46);
   filesProcessed.Name = "filesProcessed";
   filesProcessed.Size = new Size(84, 15);
   filesProcessed.TabIndex = 2;
   filesProcessed.Text = "files processed";
   // 
   // startSearch
   // 
   startSearch.Location = new Point(895, 11);
   startSearch.Name = "startSearch";
   startSearch.Size = new Size(120, 23);
   startSearch.TabIndex = 5;
   startSearch.Text = "Search";
   formTooltips.SetToolTip(startSearch, "Start search");
   startSearch.UseVisualStyleBackColor = true;
   startSearch.Click += startSearch_Click;
   // 
   // scriptFilename
   // 
   scriptFilename.AutoSize = true;
   scriptFilename.Location = new Point(340, 77);
   scriptFilename.Name = "scriptFilename";
   scriptFilename.Size = new Size(88, 15);
   scriptFilename.TabIndex = 6;
   scriptFilename.Text = "No file selected";
   // 
   // fileView
   // 
   fileView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
   fileView.Location = new Point(340, 96);
   fileView.Name = "fileView";
   fileView.Size = new Size(675, 450);
   fileView.TabIndex = 7;
   fileView.Text = "";
   formTooltips.SetToolTip(fileView, "File content");
   // 
   // armaPath
   // 
   armaPath.Location = new Point(171, 11);
   armaPath.Name = "armaPath";
   armaPath.Size = new Size(434, 23);
   armaPath.TabIndex = 8;
   formTooltips.SetToolTip(armaPath, "Arma 3 installation path");
   // 
   // label1
   // 
   label1.AutoSize = true;
   label1.Location = new Point(12, 11);
   label1.Name = "label1";
   label1.Size = new Size(39, 15);
   label1.TabIndex = 9;
   label1.Text = "Status";
   // 
   // searchFor
   // 
   searchFor.Location = new Point(611, 11);
   searchFor.Name = "searchFor";
   searchFor.Size = new Size(278, 23);
   searchFor.TabIndex = 10;
   formTooltips.SetToolTip(searchFor, "String to search for");
   // 
   // Form1
   // 
   AutoScaleDimensions = new SizeF(7F, 15F);
   AutoScaleMode = AutoScaleMode.Font;
   ClientSize = new Size(1027, 563);
   Controls.Add(searchFor);
   Controls.Add(label1);
   Controls.Add(armaPath);
   Controls.Add(fileView);
   Controls.Add(scriptFilename);
   Controls.Add(startSearch);
   Controls.Add(filesProcessed);
   Controls.Add(matchesList);
   Name = "Form1";
   Text = "SQF Browser";
   FormClosing += Form1_FormClosing;
   ResumeLayout(false);
   PerformLayout();
  }

  #endregion
  private ListBox matchesList;
  private Label filesProcessed;
  private Button startSearch;
  private Label scriptFilename;
  private RichTextBox fileView;
  private TextBox armaPath;
  private Label label1;
  private TextBox searchFor;
  private ToolTip formTooltips;
 }
}
