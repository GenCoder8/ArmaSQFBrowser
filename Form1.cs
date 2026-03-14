using BisUtils.PBO;
using System;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ArmaSQFBrowser
{
 public partial class Form1 : Form
 {
  XmlReader xmlReader;
  static string xmlFilePath = "settings.xml";

  public Form1()
  {
   InitializeComponent();

   readSettings();

  List<Match> foundMatches = new List<Match>();

   //searchString();
   filesProcessed.Text = "";

   numFiles++;

   showProcessText();

   Thread worker = new Thread(new ThreadStart(() => searchString("functions_f.pbo", foundMatches)) );

   worker.Start();

   worker.Join();

   filesProcessed.Text = "Done";

   matchesList.Items.Clear();

   foreach (Match match in foundMatches)
   {
    matchesList.Items.Add(match.fileName);
    allMatches.Add(match);
   }

  }

  class Match
  {
   public string fileContents;
   public string fileName;
   public int matchIndex;

   public Match(string fileName, string fileContents,int matchIndex)
   {
    this.fileName = fileName;
    this.fileContents = fileContents;
    this.matchIndex = matchIndex;
   }
  }

  List<Match> allMatches = new List<Match>();

  public string matchString;

  int numFiles = 0;
  int numFilesDone = 0;

  private void searchString(string pboName, List<Match> foundMatches)
  {

   try
   {
    PboFile pbo = new BisUtils.PBO.PboFile("functions_f.pbo");

    var entries = pbo.GetDataEntries();

    filesProcessed.Text = entries.Count().ToString();

    int num = 50;

    int index = 0;
    foreach (var entry in entries)
    {

     if (Path.GetExtension(entry.EntryName) != ".sqf") continue;

     //var t = Encoding.UTF8.GetString(entry.EntryData);
     //MessageBox.Show(t);

     curEntry.Text = index.ToString();
     index++;

     string read = "";
     matchString = "createunit";

     var data = entry.EntryData;

     int textLength = 0;
     for (int i = 0; i < data.Length; i++)
     {
      byte b = data[i];
      //MessageBox.Show(Convert.ToChar(b).ToString());

      char c = Convert.ToChar(b);

      read += c;

      if (c != '\n')
       textLength++;

      if (read.Length > matchString.Length)
       read = read.Remove(0, 1);

      if (matchString.Equals(read, StringComparison.OrdinalIgnoreCase))
      {
       //MessageBox.Show("match!");

       foundMatches.Add(new Match(entry.EntryName,Encoding.UTF8.GetString(data), textLength - matchString.Length));

       break;
      }
     }
     //Console.WriteLine(t);

     num--;
     if (num < 0)
      break;

     //Thread.Sleep(10);
    }
   }
   catch (Exception e)
   {
    MessageBox.Show(e.ToString());
   }

   numFilesDone++;

   showProcessText();

  }

  private void showProcessText()
  {

   filesProcessed.Text = "Processing files... " + numFilesDone.ToString() + " / " + numFiles.ToString();
  }

  private void matchesList_SelectedIndexChanged(object sender, EventArgs e)
  {
   int index = matchesList.SelectedIndex;

   if (index < 0) return;

   Match match = allMatches[index];

   fileView.Text = match.fileContents;

   scriptFilename.Text = match.fileName;

   fileView.SelectionStart = match.matchIndex;
   fileView.SelectionLength = matchString.Length;
   fileView.SelectionColor = Color.Red;


   fileView.SelectionStart = (match.matchIndex - 200);
   fileView.SelectionLength = 0;

   fileView.ScrollToCaret();

  }

  private void button1_Click(object sender, EventArgs e)
  {

  }

  void readSettings()
  {
   xmlReader = XmlReader.Create(xmlFilePath, new XmlReaderSettings());


   List<string> nopFiles = new List<string>();
   List<string> noObFiles = new List<string>();

   while (xmlReader.Read())
   {
    if (xmlReader.IsStartElement())
    {

     readSetting("armaPath", armaPath);

    }
   }

   xmlReader.Close();
  }

  private bool xmlRead(string name)
  {
   if (xmlReader.Name.ToLower() == name.ToLower())
   {
    return xmlReader.Read();
   }
   return false;
  }

  private void readSetting(string name, TextBox ctrl)
  {
   if (xmlRead(name))
   {

    ctrl.Text = xmlReader.Value;

   }
  }
 }
}
