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
   numEntries = new Label();
   curEntry = new Label();
   button1 = new Button();
   scriptFilename = new Label();
   fileView = new RichTextBox();
   SuspendLayout();
   // 
   // matchesList
   // 
   matchesList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
   matchesList.FormattingEnabled = true;
   matchesList.ItemHeight = 15;
   matchesList.Location = new Point(12, 47);
   matchesList.MultiColumn = true;
   matchesList.Name = "matchesList";
   matchesList.Size = new Size(222, 394);
   matchesList.TabIndex = 1;
   matchesList.SelectedIndexChanged += matchesList_SelectedIndexChanged;
   // 
   // numEntries
   // 
   numEntries.AutoSize = true;
   numEntries.Location = new Point(93, 19);
   numEntries.Name = "numEntries";
   numEntries.Size = new Size(38, 15);
   numEntries.TabIndex = 2;
   numEntries.Text = "label1";
   // 
   // curEntry
   // 
   curEntry.AutoSize = true;
   curEntry.Location = new Point(535, 19);
   curEntry.Name = "curEntry";
   curEntry.Size = new Size(38, 15);
   curEntry.TabIndex = 3;
   curEntry.Text = "label1";
   // 
   // button1
   // 
   button1.Location = new Point(338, 27);
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
   // Form1
   // 
   AutoScaleDimensions = new SizeF(7F, 15F);
   AutoScaleMode = AutoScaleMode.Font;
   ClientSize = new Size(763, 458);
   Controls.Add(fileView);
   Controls.Add(scriptFilename);
   Controls.Add(button1);
   Controls.Add(curEntry);
   Controls.Add(numEntries);
   Controls.Add(matchesList);
   Name = "Form1";
   Text = "Form1";
   ResumeLayout(false);
   PerformLayout();
  }

  #endregion
  private ListBox matchesList;
  private Label numEntries;
  private Label curEntry;
  private Button button1;
  private Label scriptFilename;
  private RichTextBox fileView;
 }
}
