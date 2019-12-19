using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;
using System.IO;

namespace ZOT.resources
{
    /// <summary>
    /// Cualquier clase que la herede, obtiene un conjunto de operaciones utiles para mantener ficheros comunes en el sftp de la oficina.
    /// Con su uso se pretende evitar que haya que cambiar manualmente los ficheros de configuración, o incluso descargar manualmente ficheros utiles
    /// </summary>
    abstract class SynchronisedFile
    {
        public string localPath2File;
        public string remotePath2File;
        private SessionOptions sessionOptions;
        protected static string ZOTpath;
        private static string credentials_path = Path.Combine(Environment.CurrentDirectory, @"Data\", "ZOTCredentials.txt");

        public SynchronisedFile(string localPath, string remotePath)
        {
            localPath2File = localPath;
            remotePath2File = remotePath;
            //Seguridad de mierda, pero evita que la contraseña este escrita directamente en el source. Si algun día se ponen en serio con la seguridad,
            //que lo dudo, considera usar una seccion protegida de de web.config por ejemplo
            string[] credentials;
            using (StreamReader reader = new StreamReader(credentials_path))
            {
                credentials = reader.ReadLine().Split(':');
                ZOTpath = "ftp:/" + credentials[0];
            }
            sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = "172.16.28.215",
                UserName = credentials[1],
                Password = credentials[2],
            };
        }

        public void ListDir(string sftp_path)
        {

            using (Session SFTPsession = new Session())
            {
                // Connect
                SFTPsession.Open(sessionOptions);

                RemoteDirectoryInfo dir = SFTPsession.ListDirectory(sftp_path);

                // Print results
                foreach (var file in dir.Files)
                {
                    Console.WriteLine(file);
                }
            }

        }
        void Pull(string ftp_path)
        {

        }
        void Push(string filename, string ftp_path)
        {

        }
        void Rebase(string filename, string ftp_path)
        {

        }
    }
}
