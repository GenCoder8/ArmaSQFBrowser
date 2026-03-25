//#define QUICK_TEST

using BisUtils.PBO;
using BisUtils.PBO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ArmaSQFBrowser
{
 public class SqfSearch
 {

  public class Match
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

  int numFiles = 0;
  int numFilesDone = 0;

  List<string> noSqfFiles;

  const int maxThreads = 5;

  public void searchFiles()
  {
   MainForm.form.setSearchButton(false);

   if (MainForm.form.matchString.Length == 0)
    throw new Exception("Search string empty");

   try
   {

    var watch = System.Diagnostics.Stopwatch.StartNew();

    noSqfFiles = new List<string>();

    numFiles = 0;
    numFilesDone = 0;

    MainForm.form.allMatches = new List<SqfSearch.Match>();

    MainForm.form.pbos = new Dictionary<string, PboFile>();

    MainForm.form.showProcessText("Starting");

    MainForm.form.clearMatches();

    string[] allFiles = Directory.GetFiles(MainForm.form.armaPath.Text, "*.pbo", SearchOption.AllDirectories);

    List<string> files = new List<string>();

    foreach (string file in allFiles)
    {
     var info = new System.IO.FileInfo(file);

     if (!(file.ToLower().Contains("!Workshop".ToLower())))
      if (!MainForm.form.isBlacklistedItemInList(MainForm.form.blacklist, file))
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

      PboFile pboFile = new BisUtils.PBO.PboFile(filename, PboFileOption.Read);

      MainForm.form.pbos.Add(filename, pboFile);

      Thread worker = new Thread(new ThreadStart(() => searchString(pboFile, filename, list)));
      threads.Add(worker);

      worker.Start();
     }

     int workerI = 0;
     foreach (Thread worker in threads)
     {
      worker.Join();

      numFilesDone++;

      MainForm.form.showProcessText("Processing files... " + numFilesDone.ToString() + " / " + numFiles.ToString());

      var matches = foundMatchesList[workerI];

      foreach (Match match in matches)
      {
       MainForm.form.addToMatches(match.fileName);

       MainForm.form.allMatches.Add(match);
      }

      workerI++;
     }

     // Thread.Sleep(1);
    }


    watch.Stop();

    // Thread.Sleep(2000);
    MainForm.form.showProcessText("Done. Time taken: " + (watch.ElapsedMilliseconds / 1000.0).ToString() + " seconds");

    if (noSqfFiles.Count > 0)
    {
     MainForm.form.writeLog("List of no SQF files:");

     foreach (string file in noSqfFiles)
     {
      MainForm.form.writeLog(file);
     }
    }
   }
   catch (Exception e)
   {
    MessageBox.Show("Reading fail: " + e.ToString());
   }

   MainForm.form.setSearchButton(true);
  }



  private void searchString(PboFile pboFile, string filename, List<Match> foundMatches)
  {
   if (true)
   {
    try
    {

     //PboFile pbo = new BisUtils.PBO.PboFile(pboFile, PboFileOption.Read);

     var entries = pboFile.GetDataEntries();

     string pboName = filename;

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


      var data = entry.EntryData;

      int textLength = 0;
      for (int i = 0; i < data.Length; i++)
      {
       byte b = data[i];

       char c = Convert.ToChar(b);

       read += c;

       if (c != '\n')
        textLength++;

       if (read.Length > MainForm.form.matchString.Length)
        read = read.Remove(0, 1);

       if (MainForm.form.matchString.Equals(read, StringComparison.OrdinalIgnoreCase))
       {
        int mi = textLength - MainForm.form.matchString.Length;
        if (mi < 0) mi = 0;

        foundMatches.Add(new Match(entry.EntryName, Encoding.UTF8.GetString(data), mi, pboName));

        break;
       }
      }

      numLoopsLeft--;
      if (numLoopsLeft < 0)
       break;

     }

    /// pbo.Dispose();

     if (numLoopsLeft < 0)
      MainForm.form.writeLog("Max iters reached on '" + pboName + "'");

     if (numSqf == 0)
     {
      MainForm.form.writeLog("PBO " + pboName + " does not have any SQF");
      noSqfFiles.Add(Path.GetFileNameWithoutExtension(pboName));
     }
    }
    catch (Exception e)
    {
     MainForm.form.writeLog("Failed to process '" + filename + "'");
     MessageBox.Show("Failed to process '" + filename + "' " + e.ToString());
    }

   }


  }

 }
}
