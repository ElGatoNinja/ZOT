using System;
using System.Windows.Forms;

public abstract class FileReader
{
    public static string lastDir = "c:\\";
    protected string path;
    public FileReader(string filter,string title)
    {
        using(OpenFileDialog explorer = new OpenFileDialog())
        {
            explorer.InitialDirectory = lastDir;
            explorer.Filter = filter;
            explorer.Title = title;
            if (explorer.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            lastDir = explorer.FileName; //Esto no es correcto
            path = explorer.FileName;
        }
    }
}