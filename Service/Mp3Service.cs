using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WPFMvvMTest.Model;

namespace WPFMvvMTest.Service
{
    class Mp3Service
    {
       
        public static MP3FileList Mp3Init()
        {
            MP3FileList fileList = new MP3FileList();
            //ToDO FIleList 읽어오기
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            string path;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog.SelectedPath;
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (var file in di.GetFiles())
                {
                    
                    fileList.Add(new MP3File { Url = file.FullName, FileName = file.Name });
                }
            }
            return fileList;
        }
    }
}
