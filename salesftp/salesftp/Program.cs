using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace salesftp
{
    class Program
    {
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
                int conecto = 1;

                if (conecto == 1)
                {
                    //Conexión establecida
                    Console.Write("\n Connecting success. \n");

                    //Realizamos la conexión
                    Console.Write("\n Getting data from server... \n");
                }
                else {
                    //fallo al conectar
                    Console.Write("\n Error, trying again...");
                }
            }
            catch (Exception ex) {
                Console.Write("\n An error was handle:  " + ex.Message);
            }
            Console.WriteLine("\n\n Closing application");
            Thread.Sleep(5000);

        }
    }
}
