//#define RUN_AT_START

using BisUtils.PBO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace ArmaSQFBrowser
{
 public partial class MainForm : Form
 {
  XmlReader xmlReader;
  XmlTextWriter xmlWriter;

  static string xmlSettingsFilePath = "settings.xml";
  static string xmlListsFilePath = "lists.xml";

  StringBuilder log = new StringBuilder();

  public List<string> blacklist = new List<string>();

  public string matchString = "createunit";

  public List<SqfSearch.Match> allMatches;

  public static MainForm form;

  private SqfSearch search;

  public MainForm()
  {
   form = this;

   InitializeComponent();

   System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
   FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
   Text += " Version: " + fvi.FileVersion;

   readSettings();
   readLists();

   search = new SqfSearch();

#if RUN_AT_START
   matchString = searchFor.Text;
   var mainThread = new Thread(new ThreadStart(() => search.searchFiles()));
   mainThread.Start();
#endif
  }



  public void addToMatches(string fileName)
  {
   if (matchesList.InvokeRequired)
    matchesList.Invoke(() => addToMatches(fileName));
   else
    matchesList.Items.Add(fileName);
  }

  public void clearMatches()
  {
   if (matchesList.InvokeRequired)
    matchesList.Invoke(() => clearMatches());
   else
    matchesList.Items.Clear();
  }

  public void setSearchButton(bool enable)
  {
   if (matchesList.InvokeRequired)
    matchesList.Invoke(() => setSearchButton(enable));
   else
    MainForm.form.startSearch.Enabled = enable;
  }




  public void showProcessText(string msg)
  {
   if (filesProcessed.InvokeRequired)
    filesProcessed.Invoke(() => showProcessText(msg));
   else
    filesProcessed.Text = msg;
  }

  private void matchesList_SelectedIndexChanged(object sender, EventArgs e)
  {
   int index = matchesList.SelectedIndex;

   if (index < 0) return;

   SqfSearch.Match match = allMatches[index];

   fileView.Text = match.fileContents;

   scriptFilename.Text = match.fileName + "  -  " + match.pboName;

   fileView.SelectionStart = match.matchIndex;
   fileView.SelectionLength = matchString.Length;
   fileView.SelectionColor = Color.Red;

   int sp = (match.matchIndex - 200);
   if (sp < 0) sp = 0;

   fileView.SelectionStart = sp;
   fileView.SelectionLength = 0;

   fileView.ScrollToCaret();

  }

  bool stringInList(List<string> list, string str)
  {
   return list.Any(toCheck => toCheck.Equals(str, StringComparison.OrdinalIgnoreCase));
  }
  bool subStringInList(List<string> list, string str)
  {
   return list.Any(tocheck => tocheck.ToLower().Contains(str.ToLower()));
  }

  public bool isBlacklistedItemInList(List<string> list, string str)
  {
   return list.Any(toCheck => str.Contains(toCheck));
  }

  private void saveSettings()
  {
   xmlWriter = new XmlTextWriter(xmlSettingsFilePath, System.Text.Encoding.UTF8);
   xmlWriter.WriteStartDocument(true);
   xmlWriter.Formatting = Formatting.Indented;
   xmlWriter.Indentation = 2;

   xmlWriter.WriteStartElement("Table");

   writeSetting("armaPath",armaPath.Text);

   writeSetting("searchFor", searchFor.Text);


   xmlWriter.WriteEndElement();
   xmlWriter.WriteEndDocument();
   xmlWriter.Close();
  }

  private void writeSetting(string name, string value)
  {
   xmlWriter.WriteStartElement(name);
   xmlWriter.WriteString(value);
   xmlWriter.WriteEndElement();
  }

  void readSettings()
  {
   try
   {
    xmlReader = XmlReader.Create(xmlSettingsFilePath, new XmlReaderSettings());

    while (xmlReader.Read())
    {
     if (xmlReader.IsStartElement())
     {

      readSetting("armaPath", armaPath);

      readSetting("searchFor", searchFor);

     }
    }

    xmlReader.Close();


    if (!Directory.Exists(armaPath.Text))
     throw new Exception("Invalid arma path: '" + armaPath.Text + "'");

   }
   catch (Exception e)
   {
    MessageBox.Show("Setting error " + e.ToString());
   }
  }

  void readLists()
  {
   try
   {
    xmlReader = XmlReader.Create(xmlListsFilePath, new XmlReaderSettings());

    while (xmlReader.Read())
    {
     if (xmlReader.IsStartElement())
     {

      if (xmlRead("blacklist"))
      {
       string[] blisted = xmlReader.Value.Split(new[] { '\r', '\n', ' ' });

       foreach (string str in blisted)
       {
        if (str.Length > 0)
         blacklist.Add(str);
       }

       writeLog("Blacklisted " + blacklist.Count.ToString() + " files");
      }

     }
    }

    xmlReader.Close();

   }
   catch (Exception e)
   {
    MessageBox.Show("Lists error " + e.ToString());
   }
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
    ctrl.Text = xmlReader.Value.Trim('\r', '\n');
   }
  }

  private void readList(string prefix, List<string> li, int maxEntries)
  {
   for (int index = li.Count; index < maxEntries; index++)
   {
    if (xmlRead(prefix + (index + 1).ToString()))
    {
     //log("Reading: " + xmlReader.Value);
     li.Add(xmlReader.Value);
     //log("Size: " + li.Count);
    }
   }
  }

  private void Form1_FormClosing(object sender, FormClosingEventArgs e)
  {
   saveSettings();

   File.WriteAllText("log.txt", log.ToString());
  }

  public void writeLog(string msg)
  {
   log.Append(msg + "\n");
  }

  private void startSearch_Click(object sender, EventArgs e)
  {
   matchString = searchFor.Text;

   if (matchString.Length == 0)
   {
    MessageBox.Show("Must have search string");
    return;
   }

    var mainThread = new Thread(new ThreadStart(() => search.searchFiles()));
   mainThread.Start();
  }
 }
}
