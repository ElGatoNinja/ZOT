using System;
using System.Windows.Forms;

public abstract class FileReader
{
    public static string lastDir = "c:\\";
    protected string[] _path;
    public FileReader(string filter,string title, bool Multi = false)
    {
        _path = new string[2];
        #if !DEBUG
            using(OpenFileDialog explorer = new OpenFileDialog())
            {
                explorer.InitialDirectory = lastDir;
                explorer.Filter = filter;
                explorer.Title = title;
                explorer.Multiselect = Multi;
                if (explorer.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                lastDir = explorer.FileName; //Esto no es correcto
                _path = explorer.FileNames;
            }
        #endif

    }
}