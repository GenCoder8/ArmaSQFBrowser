#define QUICK_TEST

using BisUtils.PBO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace ArmaSQFBrowser
{
 public partial class Form1 : Form
 {
  XmlReader xmlReader;
  XmlTextWriter xmlWriter;

  static string xmlSettingsFilePath = "settings.xml";
  static string xmlListsFilePath = "lists.xml";

  const int maxThreads = 5;

  StringBuilder log = new StringBuilder();

  List<string> blacklist = new List<string>();

  public Form1()
  {
   InitializeComponent();

   readSettings();
   readLists();

   var mainThread = new Thread(new ThreadStart(() => searchFiles()));
   mainThread.Start();
  }

  class Match
  {
   public string fileContents;
   public string fileName;
   public int matchIndex;
   public string pboName;

   public Match(string fileName, string fileContents, int matchIndex, string pboName)
   {
    this.fileName = fileName;
    this.fileContents = fileContents;
    this.matchIndex = matchIndex;
    this.pboName = pboName;
   }
  }

  List<Match> allMatches;

  public string matchString;

  int numFiles = 0;
  int numFilesDone = 0;

  List<string> noSqfFiles;

  private void searchFiles()
  {
   try
   {
    var watch = System.Diagnostics.Stopwatch.StartNew();

   noSqfFiles = new List<string>();

   numFiles = 0;
   numFilesDone = 0;

   allMatches = new List<Match>();


    showProcessText("Starting");

    clearMatches();

    string[] allFiles = Directory.GetFiles(armaPath.Text, "*.pbo", SearchOption.AllDirectories);

    List<string> files = new List<string>();

    foreach (string file in allFiles)
    {
     var info = new System.IO.FileInfo(file);

     if (!(file.ToLower().Contains("!Workshop".ToLower())))
      if (!isBlacklistedItemInList(blacklist, file))
       if (info.Length < (200 * 1000000))    // Some size limit in BisUtils, will get stuck if too large
        files.Add(file);
    }

    // writeLog($"Skipping too large file '{pboName}'");

    numFiles = files.Count;


    for (int fi = 0; fi < numFiles; fi += maxThreads)
    {

     List<Thread> threads = new List<Thread>();

     List<List<Match>> foundMatchesList = new List<List<Match>>();

     for (int fb = 0; fb < maxThreads; fb++)
     {

      int nextIndex = fi + fb;

      if (nextIndex >= files.Count)
       break;

      string filename = files[nextIndex];


      foundMatchesList.Add(new List<Match>());

      var list = foundMatchesList.Last();

      Thread worker = new Thread(new ThreadStart(() => searchString(filename, list)));
      threads.Add(worker);

      worker.Start();
     }

     int workerI = 0;
     foreach (Thread worker in threads)
     {
      worker.Join();

      numFilesDone++;

      showProcessText("Processing files... " + numFilesDone.ToString() + " / " + numFiles.ToString());

      var matches = foundMatchesList[workerI];

      foreach (Match match in matches)
      {
       addToMatches(match.fileName);

       allMatches.Add(match);
      }

      workerI++;
     }

     // Thread.Sleep(1);
    }


    watch.Stop();

    // Thread.Sleep(2000);
    showProcessText("Done. Time taken: " + (watch.ElapsedMilliseconds / 1000.0).ToString() + " seconds");

    if (noSqfFiles.Count > 0)
    {
     writeLog("List of no SQF files:");

     foreach (string file in noSqfFiles)
     {
      writeLog(file);
     }
    }
   }
   catch (Exception e)
   {
    MessageBox.Show("Reading fail: " + e.ToString());
   }

  }

  private void addToMatches(string fileName)
  {
   if (matchesList.InvokeRequired)
    matchesList.Invoke(() => addToMatches(fileName));
   else
    matchesList.Items.Add(fileName);
  }

  private void clearMatches()
  {
   if (matchesList.InvokeRequired)
    matchesList.Invoke(() => clearMatches());
   else
    matchesList.Items.Clear();
  }



  private void searchString(string pboFile, List<Match> foundMatches)
  {
   if (true)
   {
    try
    {

     PboFile pbo = new BisUtils.PBO.PboFile(pboFile, PboFileOption.Read);


     var entries = pbo.GetDataEntries();

     string pboName = Path.GetFileName(pboFile);

     int numLoopsLeft = 2000;

#if QUICK_TEST
     numLoopsLeft = 10;
#endif

     int numSqf = 0;

     foreach (var entry in entries)
     {

      if (Path.GetExtension(entry.EntryName) != ".sqf") continue;

      numSqf++;

      //var t = Encoding.UTF8.GetString(entry.EntryData);
      //MessageBox.Show(t);


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

        foundMatches.Add(new Match(entry.EntryName, Encoding.UTF8.GetString(data), textLength - matchString.Length, pboName));

        break;
       }
      }

      numLoopsLeft--;
      if (numLoopsLeft < 0)
       break;

     }

     pbo.Dispose();

     if (numLoopsLeft < 0)
      writeLog("Max iters reached on '" + pboName + "'");

     if (numSqf == 0)
     {
      writeLog("PBO " + pboName + " does not have any SQF");
      noSqfFiles.Add(Path.GetFileNameWithoutExtension(pboName));
     }
    }
    catch (Exception e)
    {
     writeLog("Failed to process '" + pboFile + "'");
     MessageBox.Show(e.ToString());
    }

   }


  }

  private void showProcessText(string msg)
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

   Match match = allMatches[index];

   fileView.Text = match.fileContents;

   scriptFilename.Text = match.fileName + " - " + match.pboName;

   fileView.SelectionStart = match.matchIndex;
   fileView.SelectionLength = matchString.Length;
   fileView.SelectionColor = Color.Red;


   fileView.SelectionStart = (match.matchIndex - 200);
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

  bool isBlacklistedItemInList(List<string> list, string str)
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

      ////readList("blacklist", blacklist,100);

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
    ctrl.Text = xmlReader.Value;
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

  private void writeLog(string msg)
  {
   log.Append(msg + "\n");
  }

  private void startSearch_Click(object sender, EventArgs e)
  {

  }
 }
}
