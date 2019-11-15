using System;
using System.Windows.Forms;

public abstract class FileReader
{
    public static string lastDir = "c:\\";
    protected string path;
    public FileReader(string filter)
    {
        using(OpenFileDialog explorer = new OpenFileDialog())
        {
            explorer.InitialDirectory = lastDir; 
            explorer.Filter = filter;
            if(explorer.ShowDialog() == DialogResult.OK)
            {
                lastDir = explorer.FileName; //Esto no es correcto
                path = explorer.FileName;
            }
        }  
    }
}