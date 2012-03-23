
//Copyright 2012 Joshua Scoggins. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are
//permitted provided that the following conditions are met:
//
//   1. Redistributions of source code must retain the above copyright notice, this list of
//      conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright notice, this list
//      of conditions and the following disclaimer in the documentation and/or other materials
//      provided with the distribution.
//
//THIS SOFTWARE IS PROVIDED BY Joshua Scoggins ``AS IS'' AND ANY EXPRESS OR IMPLIED
//WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Joshua Scoggins OR
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//The views and conclusions contained in the software and documentation are those of the
//authors and should not be interpreted as representing official policies, either expressed
//or implied, of Joshua Scoggins. 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace Libraries.Filter
{
  public class DynamicForm : FilterDialogForm
  {
    private OpenFileDialog openFile;
    private SaveFileDialog saveFile;
    private Guid uid;
    private Hashtable storageCells;
    public Hashtable StorageCells { get { return storageCells; } }
    public Guid UniqueID { get { return uid; } }
    public DynamicForm()
    {
      uid = Guid.NewGuid();
      storageCells = new Hashtable();
      saveFile = new SaveFileDialog();
      saveFile.FileName = "";
      openFile = new OpenFileDialog();
      openFile.FileName = "";
      openFile.FileOk += new System.ComponentModel.CancelEventHandler(OpenImageHandler);

    }
    private void OpenImageHandler(object sender, System.ComponentModel.CancelEventArgs e)
    {
      try
      {
        //load an image
        string path = openFile.FileName;
        var image = new Bitmap(Image.FromFile(path));
        byte[][] newImage = new byte[image.Width][];
        for(int x = 0; x < image.Width; x++)
        {
          byte[] line = new byte[image.Height];
          for(int y = 0; y < image.Height; y++)
            line[y] = image.GetPixel(x,y).R; 
          newImage[x] = line;
        }
        storageCells["otherImage"] = newImage;
        shouldApply = true;
      }
      catch(Exception)
      {
        MessageBox.Show("Invalid File Type Given");
        shouldApply = false;
      }
    }
    public void ButtonOpensFileDialog(Button b, bool addControl)
    {
      b.Click += new EventHandler(OnClick);
      if(addControl)
      {
        Controls.Add(b);
      }
    }
    private void OnClick(object sender, EventArgs e)
    {
      var result = openFile.ShowDialog();
      shouldApply &= (result == DialogResult.OK || result == DialogResult.Yes);
    }
    protected override bool OnOk(object sender, EventArgs e)
    {
      if(!shouldApply)
      {
        MessageBox.Show("Insufficient (or Invalid) Information Provided for Filter to be able to continue");
        shouldApply = true;
        return false;
      }
      else
      {
        foreach(var o in Controls)
        {
          Control ctrl = (Control)o;
          if(ctrl is CheckBox)
          {
            CheckBox cc = (CheckBox)ctrl;
            storageCells[cc.Name] = cc.Checked;
          }
          else if(!(ctrl is Label))
          {
            if(storageCells.ContainsKey(ctrl.Name))
              storageCells[ctrl.Name] = ctrl.Text; 
          }
        }
        shouldApply = true;
        return true;
      }
    }
  }
}
