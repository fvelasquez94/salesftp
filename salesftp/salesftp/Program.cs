using LINQtoCSV;
using Postal;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace salesftp

{
    
    class Program
    {
        private DLI_PROEntities sigmadb = new DLI_PROEntities();

        static void Main(string[] args)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(28, 0); Console.WriteLine("Welcome To Limena FTP Service");
            Console.SetCursorPosition(33, 1); Console.WriteLine("Sales Information");
            Console.ResetColor();
            Console.WriteLine();
            try
            {

                Console.Write("\n Connecting to server... \n");
                int conecto = 0;
                try //comprobamos si el servidor está devolviendo datos
                {
                    using (var context = new DLI_PROEntities())
                    {
                        var lst = (from a in context.SigmaFR select a).FirstOrDefault();
                    }

                    conecto = 1;

                }
                catch {

                    conecto = 0;
                }
                
                if (conecto == 1)
                {
                    //Conexión establecida
                    Console.Write("\n Successful connection. \n");

                    //Realizamos solicitud de datos
                    Console.Write("\n Getting data from server... \n");

                    List<SigmaFR> lista_datos = new List<SigmaFR>();

                    using (var context = new DLI_PROEntities())
                    {
                        lista_datos = (from b in context.SigmaFR select b).ToList();
                    }

                    if (lista_datos.Count > 0)
                    {
                        //Generamos el archivo csv
                        try {
                            //Propiedades
                            CsvFileDescription outputFileDescription = new CsvFileDescription
                            {
                                SeparatorChar = ',', // delimitamos por comas
                                FirstLineHasColumnNames = true // aquí se asigna si llevará header o no
                            };
                            //objeto
                            Console.Write("\n Creating .csv file... \n");
                            CsvContext cc = new CsvContext();
                            //Salida

                            cc.Write(lista_datos, "Sigma_Weekly_Sales_" + DateTime.Now.AddDays(-6).ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("yyyyMMdd") + ".csv", outputFileDescription);
  
                            Console.Write("\n File created successfully... \n");


                            //Ocupamos un metodo de subida para SFTP, no FTP... Usando el framework SSH.NET
                            FileUploadSFTP("12.195.70.242", 5001, "bar-s", "Lim3nA_B@r-$", "Sigma_Weekly_Sales_" + DateTime.Now.AddDays(-6).ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");


                            Console.Write("\n Sending notifications... \n");

                            try{

                                //Enviamos el correo de notificación
                                var viewsPath = Path.GetFullPath(@"Views\Emails");

                                var engines = new ViewEngineCollection();
                                engines.Add(new FileSystemRazorViewEngine(viewsPath));

                                var service = new EmailService(engines);

                                dynamic email = new Email("ftp_notification");

                                service.Send(email);
                                Console.Write("\n Email sent successfully. \n");

                            } catch (Exception e) {
                                Console.Write("\n Email can't send. " + e.Message + " \n");
                            }

                        }
                        catch (Exception ex) {
                            Console.Write("\n An error was handle:  " + ex.Message);

                            // Enviamos el correo de notificación
                            var viewsPath = Path.GetFullPath(@"Views\Emails");

                            var engines = new ViewEngineCollection();
                            engines.Add(new FileSystemRazorViewEngine(viewsPath));

                            var service = new EmailService(engines);

                            dynamic email = new Email("ftp_notification_error");

                            service.Send(email);
                        }

                    }
                    else {
                        //Realizamos solicitud de datos
                        Console.Write("\n No data was found... \n");
                    }

                 

                }
                else {
                    //fallo al conectar
                    Console.Write("\n Error to connect with database, try again...");
                }
            }
            catch (Exception ex) {
                Console.Write("\n An error was handle:  " + ex.Message);
            }
            Console.WriteLine("\n\n Closing application");
            Environment.Exit(0);
        }

        
        public static void FileUploadSFTP(string host, int port, string username, string password, string file)
        {

            var uploadFile = @file;

            using (var client = new SftpClient(host, port, username, password))
            {
               
                client.Connect();

                if (client.IsConnected)
                {
                    Console.Write("\n Connected to the ftp client. \n");
                    client.ChangeDirectory("/FillRate");

                    using (var fileStream = new FileStream(uploadFile, FileMode.Open))
                        {
                            Console.WriteLine("\n Uploading {0} ({1:N0}) bytes. \n)", uploadFile, fileStream.Length);
                            client.BufferSize = 4 * 1024; // bypass Payload error large files
                            client.UploadFile(fileStream, Path.GetFileName(uploadFile));

              
                        }

                        Console.Write("\n Upload File Complete. \n");


                }
                else
                {
                    Console.Write("\n Error, could not connect with ftp client. \n");
                }
                client.Disconnect();
                client.Dispose();
          
            }


        }

    }
}
