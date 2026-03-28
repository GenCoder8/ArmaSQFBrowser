using BisUtils.PBO.Entries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaSQFBrowser
{
 internal class SqfCodeInjector
 {
  string codeInsertPrefix = "//codeMod" + System.Environment.NewLine;
  int maxInjCodeLength = 512;

  public bool isInjected(PboDataEntry entry)
  {
   string text = System.Text.Encoding.UTF8.GetString(entry.EntryData);

   if (text.Length < codeInsertPrefix.Length) return false;

   return text.Substring(0, codeInsertPrefix.Length).Equals(codeInsertPrefix);
  }

  public void inject(PboDataEntry entry, string code)
  {
   if (isInjected(entry))
    throw new Exception("Code already injected");

   string orgCode = System.Text.Encoding.UTF8.GetString(entry.EntryData);

   string injectedCode = codeInsertPrefix + code;

   int emptyNeeded = maxInjCodeLength - injectedCode.Length;

   if (emptyNeeded < 0)
    throw new Exception("Code too long");

   string final = injectedCode.PadRight(maxInjCodeLength, ' ') + orgCode;

   entry.EntryData = System.Text.Encoding.UTF8.GetBytes(final);


   //MessageBox.Show(System.Text.Encoding.UTF8.GetString(entry.EntryData));
  }

  public void removeCode(PboDataEntry entry)
  {
   if (!isInjected(entry))
    throw new Exception("Code not injected");

   string modedCode = System.Text.Encoding.UTF8.GetString(entry.EntryData);

   int injLength = modedCode.Length - maxInjCodeLength;

   if (injLength < 0)
    throw new Exception("Sub string weird");

   string orgCode = modedCode.Substring(maxInjCodeLength, injLength);

   byte[] bytes = System.Text.Encoding.UTF8.GetBytes(orgCode);

   entry.EntryData = bytes;

  }

 }
}
