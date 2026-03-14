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
  static string xmlFilePath = "settings.xml";

  const int maxThreads = 3;

  StringBuilder log = new StringBuilder();

  List<string> blacklist = new List<string>();

  public Form1()
  {
   InitializeComponent();

   readSettings();

   var mainThread = new Thread(new ThreadStart(() => searchFiles()));
   mainThread.Start();
  }

  class Match
  {
   public string fileContents;
   public string fileName;
   public int matchIndex;

   public Match(string fileName, string fileContents, int matchIndex)
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

  private void searchFiles()
  {


   try
   {
    showProcessText("Starting");

    clearMatches();

    string[] allFiles = Directory.GetFiles(armaPath.Text, "*.pbo", SearchOption.AllDirectories);

    List<string> files = new List<string>();

    foreach (string file in allFiles)
    {
     if (!(file.ToLower().Contains("!Workshop".ToLower())))
      files.Add(file);
    }

    

    //numFilesRead.Text = "Found " + files.Length.ToString() + " files";

    //searchString();
    // filesProcessed.Text = "";

    numFiles = files.Count;


    for (int fi = 53; fi < 55; fi += maxThreads)
    {

     List<Thread> threads = new List<Thread>();

     List<List<Match>> foundMatchesList = new List<List<Match>>();

     for (int fb = 0; fb < maxThreads; fb++)
     {

      // "functions_f.pbo"

      //MessageBox.Show(">>>> " + files[fb]);

      int nextIndex = fi + fb;

      if (nextIndex >= files.Count)
       break;

      string filename = files[nextIndex];

      if (subStringInList2(blacklist, filename))
       continue;

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

      //MessageBox.Show();
      foreach (Match match in matches)
      {
       //  matchesList.Items.Add(match.fileName);

       addToMatches(match.fileName);

       allMatches.Add(match);
      }

      workerI++;
     }

     // Thread.Sleep(1);
    }




    // Thread.Sleep(2000);
    showProcessText("Done");


    /*
    foreach (Match match in foundMatches)
    {
     matchesList.Items.Add(match.fileName);
     allMatches.Add(match);
    }*/

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



  private void searchString(string pboName, List<Match> foundMatches)
  {
   if (true)
   {
    try
    {
     //MessageBox.Show(pboName);

     var info = new System.IO.FileInfo(pboName);

     if (info.Length > (200 * 1000000))
      return;


     PboFile pbo = new BisUtils.PBO.PboFile(pboName, PboFileOption.Read);

     //MessageBox.Show("ok!");

     var entries = pbo.GetDataEntries();

     //filesProcessed.Text = entries.Count().ToString();

     int num = 1;

     int numSqf = 0;

     int index = 0;
     foreach (var entry in entries)
     {

      if (Path.GetExtension(entry.EntryName) != ".sqf") continue;

      numSqf++;

      //var t = Encoding.UTF8.GetString(entry.EntryData);
      //MessageBox.Show(t);

      //curEntry.Text = index.ToString();
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

        foundMatches.Add(new Match(entry.EntryName, Encoding.UTF8.GetString(data), textLength - matchString.Length));

        break;
       }
      }
      //Console.WriteLine(t);

      num--;
      if (num < 0)
       break;

      //Thread.Sleep(10);
     }

     pbo.Dispose();

     if(numSqf == 0)
      writeLog("PBO " + pboName + " does not have any SQF");

    }
    catch (Exception e)
    {
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

  bool stringInList(List<string> list, string str)
  {
   return list.Any(toCheck => toCheck.Equals(str, StringComparison.OrdinalIgnoreCase));
  }
  bool subStringInList(List<string> list, string str)
  {
   return list.Any(tocheck => tocheck.ToLower().Contains(str.ToLower()));
  }

  bool subStringInList2(List<string> list, string str)
  {
   return list.Any(toCheck => str.ToLower().Contains(toCheck.ToLower()));
  }

  void readSettings()
  {
   try
   {
    xmlReader = XmlReader.Create(xmlFilePath, new XmlReaderSettings());

    while (xmlReader.Read())
    {
     if (xmlReader.IsStartElement())
     {

      readSetting("armaPath", armaPath);

      ////readList("blacklist", blacklist,100);

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


    if (!Directory.Exists(armaPath.Text))
    throw new Exception("Invalid arma path: '" + armaPath.Text + "'");

   }
   catch (Exception e)
   {
    MessageBox.Show("Setting error " + e.ToString());
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
   File.WriteAllText("log.txt", log.ToString());
  }

  private void writeLog(string msg)
  {
   log.Append(msg + "\n");
  }
 }
}
