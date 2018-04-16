using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                    Console.Write("\n Connecting success. \n");

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
                            cc.Write(lista_datos, "Sigmadata_" + DateTime.Now.ToString("yyyyMMdd") + ".csv", outputFileDescription);
                            Console.Write("\n File created successfully... \n");

                            Console.Write("\n Uploading file... \n");

                            // Obtenemos el objeto con el cual nos comunicaremos al servidor
                            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.contoso.com/test.htm");
                            request.Method = WebRequestMethods.Ftp.UploadFile;

                            // Credenciales
                            request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");

                            // Copiamos el archivo a un objeto stream
                            StreamReader sourceStream = new StreamReader("Sigmadata_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
                            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                            sourceStream.Close();
                            request.ContentLength = fileContents.Length;

                            Stream requestStream = request.GetRequestStream();
                            requestStream.Write(fileContents, 0, fileContents.Length);
                            requestStream.Close();

                            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                            Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                            response.Close();

                        }
                        catch (Exception ex) {
                            Console.Write("\n An error was handle:  " + ex.Message);
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
            Thread.Sleep(6000);

        }
    }
}
