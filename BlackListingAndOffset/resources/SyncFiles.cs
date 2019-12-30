using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;
using System.IO;
using System.Collections;

namespace ZOT.resources
{
    /// <summary>
    /// Singleton pensado para manejar de forma sencilla la sincronizacion de ficheros 
    /// </summary>
    class SyncFiles
    {
        private readonly static SyncFiles _instance = new SyncFiles();

        private SessionOptions sessionOptions;
        private string ZOTpath;
        private readonly string credentials_path = Path.Combine(Environment.CurrentDirectory, @"Data\", "ZOTCredentials.txt");

        private SyncFiles()
        {
        }
        public void Init()
        { 
            //Seguridad de mierda, pero evita que la contraseña este escrita directamente en el source. Si algun día se ponen en serio con la seguridad,
            //que lo dudo, considera usar una seccion protegida de de web.config por ejemplo
            string[] credentials;

            try {
                using (StreamReader reader = new StreamReader(credentials_path))
                {
                    credentials = reader.ReadLine().Split(':');
                }
                sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = "172.16.28.215",
                    UserName = credentials[1],
                    Password = credentials[2],
                };
            }
            catch (FileNotFoundException)
            {
                ZOTUtiles.ShowError("No se encuentra el fichero con las credenciales del servidor");
            }
        }
        /// <summary>
        /// Se establece una conexion entre uno de los ficheros y el ftp
        /// </summary>
        /// <param name="localPath">Uri del fichero local</param>
        /// <param name="remotePath">Uri del fichero remoto</param>
        public void NewConnection(ISynchronizable fileObject)
        {
            using (Session SFTPsession = new Session())
            {
                // Connect
                SFTPsession.Open(sessionOptions);
                RemoteDirectoryInfo dir = SFTPsession.ListDirectory(fileObject.remoteDir);

                string fileName = fileObject.LastVersion(dir);
                
                if (fileName != null)
                {
                    
                }


            }
        }
        void Pull(string filename, string remoteDir, string localDir)
        {

        }
        void Push(string filename, string ftp_path)
        {

        }
        void Rebase(string filename, string ftp_path)
        {

        }


    }

    /// <summary>
    /// Cada tipo de archivo tiene distintos formatos tanto de datos como de nominación, esta interfaz define las distintas estrategias
    /// que se van a usar para cada uno, mientras la clase SyncFiles controla controla los aspectos comunes de la comunicación
    /// </summary>
    interface ISynchronizable
    {
        /// <summary>
        /// La ruta al directorio en el que se encuentra el archivo local
        /// </summary>
        string localDir { get; }
        /// <summary>
        /// La ruta al directorio en el que se encuentra el archivo en el ftp
        /// </summary>
        string remoteDir { get; }

        /// <summary>
        /// Determina la estrategia a seguir para saber cual es la ultima versión del fichero, o si la ultima version ya esta en local. En algunos casos podría
        /// ser necesario descargar varios archivos, esto se puede lograr retornando wildcards en vez de el nombre de un solo fichero.
        /// </summary>
        /// <param name="dir">Una coleccion con datos de todos los archivos que hay en el directorio seleccionado</param>
        /// <returns></returns>
        string LastVersion(RemoteDirectoryInfo dir);

        /// <summary>
        /// Determina la estrategia a seguir una vez descargado el fichero en la carpeta temp
        /// </summary>
        void AfterDownload();

        /// <summary>
        /// Determina la estrategia a seguir una vez subido el archivo en caso de haberla
        /// </summary>
        void AfterUpload();
    }

}


