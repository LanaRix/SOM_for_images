using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesRecovery3.Model.Algorithm
{
    class ChooseFile
    {
        public ChooseFile() {}

        public string ChooseImage()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg|.jpg";
            dlg.Filter = "Images (.jpg)|*.bmp;*jpg";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                return dlg.FileName;
            }
            return "Выберете файл";
        }

        public string ChooseDatFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Binary (.dat)|*.dat";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                return dlg.FileName;
            }
            return "";
        }
    }
}
