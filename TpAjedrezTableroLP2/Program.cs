using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TpAjedrezLP2
{
    public class Program
    {
        public const int N = 8;
        public const int Tableros = 10;
        public enum Piezas
        {
            //El 0: Casilla Vacia
            //   1: Casilla Atacada
            Rey = 3, Reina, AB, AN, T1, T2, C1, C2
        }

        static void Main()
        {
            // ----------------------------------------TABLEROS Y VARIABLES -----------------------------------------------
            int[,] TableroOriginal = CrearTablero();
            int[,] TableroAux = CrearTablero();
            int[] Piezas = CrearPiezas();

            int ContTableros = 0;
            int CasillasMax = 0;
            int casillasAtacadas = 0;
            int[] PosPiezaParcial = new int[2]; // [fila, columna], va a ser la posición donde se ponga el alfil blanco
            int[,] PosPiezas = new int[5, 2];
            int[,] OrdenesUsados = new int[124, 8];
            int contOrdenes = 0;

            //TORRES
            int[] PosTorre1 = new int[2];
            PosTorre1[0] = 0; PosTorre1[1] = 0;
            int[] PosTorre2 = new int[2];
            PosTorre2[0] = 7; PosTorre2[1] = 7;
            casillasAtacadas += colocarTorres(PosTorre1, PosTorre2, TableroOriginal);

            int[] PosReina = new int[2];

            //---------------------------Aca empieza el while principal del programa --------------------------------------------
            do
            {
                //TODO: aca deberiamos llamar para que se cree un orden de las piezas
                Random rnd = new Random();
                int fila = rnd.Next(4, 6);
                int columna = rnd.Next(4, 6);
                SetPosicion(Program.Piezas.Reina, fila, columna, TableroOriginal);
            } while (ContTableros < Tableros);

        }

        /*public static int crearOrdenPiezas(this int[] Piezas, int[,] OrdenesUsados,  int contadorOrdenes)
        {
            while(true)
            {
                /*
                 * codigo de orden random
                
                if (verificarOrden(Piezas,  OrdenesUsados, contadorOrdenes) == true)
                    break;
            }
            return contadorOrdenes++;
        } 

        public static bool verificarOrden(int[,] Piezas, int[,] Ordenes, int cantidad)
        {
            int i;
            for(i = 0; i < cantidad; i++)
            {

            }
            if (i == cantidad)
                return false;
        }*/

        public static int colocarTorres(int[] posicionTorre1, int[] posicionTorre2, int[,] tablero)
        {
            tablero[posicionTorre1[0], posicionTorre1[1]] = (int)Piezas.T1;
            tablero[posicionTorre2[0], posicionTorre2[1]] = (int)Piezas.T2;
            for (int i = 1; i < N; i++)
            {
                tablero[posicionTorre1[0] + i, posicionTorre1[1]] = 1; //pinto como atacada toda la primer columna
                tablero[posicionTorre1[0], posicionTorre1[1] + i] = 1; //pinto como atacada toda la primer fila
                tablero[posicionTorre2[0] - i, posicionTorre1[1]] = 1; //pinto como atacada toda la ultima columna
                tablero[posicionTorre1[0], posicionTorre1[1] - i] = 1; //pinto como atacada toda la ultima fila
            }
            return 28;
        }
        public static int[,] CrearTablero()
        {
            int[,] Tablero = new int[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Tablero[i, j] = 0;
                }
            }
            return Tablero;
        }
        public static int[] CrearPiezas()
        {//Nos fijamos como podemos hacer para que a la hora de imprimir el tablero coloquemos Sus iniciales en el numero de la pieza
            int[] aux = new int[8];
            aux[0] = (int)Piezas.Rey;
            aux[1] = (int)Piezas.Reina;
            aux[2] = (int)Piezas.AB;
            aux[3] = (int)Piezas.AN;
            aux[4] = (int)Piezas.T1;
            aux[5] = (int)Piezas.T2;
            aux[6] = (int)Piezas.C1;
            aux[7] = (int)Piezas.C2;
            return aux;

        }
        public static void SetPosicion(Piezas pieza, int fila, int columna, int[,] tablero)
        {
            tablero[fila, columna] = (int)pieza;
        }
        public static bool DentroTablero(int fila, int columna)
        {
            if (fila >= N || fila < 0 || columna >= N || columna < 0)
                return false;
            else
                return true;
        }
        public static bool ValidarPosicion(int fila, int columna, int[,] tablero, Piezas pieza)
        {
            if (tablero[fila, columna] == 0 || tablero[fila, columna] == 1)
                return true;
            //Si hay un rey y quiero poner un caballo
            if (tablero[fila, columna] == (int)Piezas.Rey && (pieza == Piezas.C1 || pieza == Piezas.C2))
                return true;
            //Si hay un caballo y quiero poner un rey
            if (tablero[fila, columna] == (int)Piezas.C1 && pieza == Piezas.Rey)
                return true;
            if (tablero[fila, columna] == (int)Piezas.C2 && pieza == Piezas.Rey)
                return true;
            return false;
        }
        public static void ImprimirTablero(int[,] tablero)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Console.Write(tablero[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        public static void CopiarTablero(int[,] tableroFuente, int[,] tableroDestino)
        {
            tableroFuente.CopyTo(tableroDestino, 0);
        }

    }
}
// 
//
//
//