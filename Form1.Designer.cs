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
   matchesList = new ListBox();
   filesProcessed = new Label();
   curEntry = new Label();
   button1 = new Button();
   scriptFilename = new Label();
   fileView = new RichTextBox();
   armaPath = new TextBox();
   numFilesRead = new Label();
   SuspendLayout();
   // 
   // matchesList
   // 
   matchesList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
   matchesList.FormattingEnabled = true;
   matchesList.ItemHeight = 15;
   matchesList.Location = new Point(12, 77);
   matchesList.MultiColumn = true;
   matchesList.Name = "matchesList";
   matchesList.Size = new Size(222, 364);
   matchesList.TabIndex = 1;
   matchesList.SelectedIndexChanged += matchesList_SelectedIndexChanged;
   // 
   // filesProcessed
   // 
   filesProcessed.AutoSize = true;
   filesProcessed.Location = new Point(46, 43);
   filesProcessed.Name = "filesProcessed";
   filesProcessed.Size = new Size(84, 15);
   filesProcessed.TabIndex = 2;
   filesProcessed.Text = "files processed";
   // 
   // curEntry
   // 
   curEntry.AutoSize = true;
   curEntry.Location = new Point(681, 53);
   curEntry.Name = "curEntry";
   curEntry.Size = new Size(38, 15);
   curEntry.TabIndex = 3;
   curEntry.Text = "label1";
   // 
   // button1
   // 
   button1.Location = new Point(516, 53);
   button1.Name = "button1";
   button1.Size = new Size(120, 23);
   button1.TabIndex = 5;
   button1.Text = "button1";
   button1.UseVisualStyleBackColor = true;
   button1.Click += button1_Click;
   // 
   // scriptFilename
   // 
   scriptFilename.AutoSize = true;
   scriptFilename.Location = new Point(318, 61);
   scriptFilename.Name = "scriptFilename";
   scriptFilename.Size = new Size(38, 15);
   scriptFilename.TabIndex = 6;
   scriptFilename.Text = "label1";
   // 
   // fileView
   // 
   fileView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
   fileView.Location = new Point(275, 96);
   fileView.Name = "fileView";
   fileView.Size = new Size(476, 350);
   fileView.TabIndex = 7;
   fileView.Text = "";
   // 
   // armaPath
   // 
   armaPath.Location = new Point(299, 11);
   armaPath.Name = "armaPath";
   armaPath.Size = new Size(434, 23);
   armaPath.TabIndex = 8;
   // 
   // numFilesRead
   // 
   numFilesRead.AutoSize = true;
   numFilesRead.Location = new Point(46, 14);
   numFilesRead.Name = "numFilesRead";
   numFilesRead.Size = new Size(56, 15);
   numFilesRead.TabIndex = 9;
   numFilesRead.Text = "num files";
   // 
   // Form1
   // 
   AutoScaleDimensions = new SizeF(7F, 15F);
   AutoScaleMode = AutoScaleMode.Font;
   ClientSize = new Size(763, 458);
   Controls.Add(numFilesRead);
   Controls.Add(armaPath);
   Controls.Add(fileView);
   Controls.Add(scriptFilename);
   Controls.Add(button1);
   Controls.Add(curEntry);
   Controls.Add(filesProcessed);
   Controls.Add(matchesList);
   Name = "Form1";
   Text = "Form1";
   ResumeLayout(false);
   PerformLayout();
  }

  #endregion
  private ListBox matchesList;
  private Label filesProcessed;
  private Label curEntry;
  private Button button1;
  private Label scriptFilename;
  private RichTextBox fileView;
  private TextBox armaPath;
  private Label numFilesRead;
 }
}
