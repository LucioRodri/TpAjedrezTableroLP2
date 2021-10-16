﻿using System;
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
            int[] arrayPiezas = CrearPiezas();

            int ContTableros = 0;
            int CasillasMax = 0;
            int CasillasMaxAux = 0;
            int casillasAtacadas = 0;
            int[] PosPiezaParcial = new int[2]; 
            int[,] PosPiezas = new int[5,2];

            //TORRES
            int[] PosTorre1 = new int[2];
            PosTorre1[0] = 0; PosTorre1[1] = 0;
            int[] PosTorre2 = new int[2];
            PosTorre2[0] = 7; PosTorre2[1] = 7;
            casillasAtacadas += colocarTorres(PosTorre1, PosTorre2, TableroOriginal);

            int[] PosReina = new int[2];
             
            int auxK = 0; //para el segundo for interior

            //---------------------------Aca empieza el while principal del programa --------------------------------------------
            do
            {
                //Determinamos el orden aleatorio que se van a probar las piezas
                int[] arrayAux = OrdenAleatorio(arrayPiezas); //para no modificar el array original
                //Determinamos la posicion de la Reina de forma aleatoria
                Random rnd = new Random();
                int fila = rnd.Next(4, 6);
                int columna = rnd.Next(4, 6);
                SetPosicion(Program.Piezas.Reina, fila, columna, TableroOriginal); //cuando implementemos el forms, esto queda determinado por el usuario

                atacarCasillas(fila, columna, Piezas.Reina, TableroOriginal);
                CopiarTablero(TableroOriginal, TableroAux); //cada vez que buscamos un tablero, lo reinciamos

                //Triple for n^3 -> Primer FOR para las piezas y los otros dos para recorrer el tablero con varios métodos de poda. 
                for (int i = 0; i < 5; i++)//FOR para cada pieza xq son 5 piezas
                {
                    CasillasMax = 0;
                    int aux = 1;
                    for (int j = 0; j < N; j++)
                    {
                        if (arrayPiezas[i] == (int)Piezas.AB || arrayPiezas[i] == (int)Piezas.AN)//Pregunta si la pieza es un alfil
                        {
                            aux = 2; //incrementamos el for de a 2 
                            if ((arrayPiezas[i] == (int)Piezas.AB && j % 2 != 0) || (arrayPiezas[i] == (int)Piezas.AN && j % 2 == 0))
                                auxK = 1;
                        }
                        for (int k = auxK; k < N; k = +aux)
                        {
                            //aca deberiamos verificar que si la pieza es un alfil, que no este en la diagonal de la reina, pq sino las posiciones atacadas nunca serían las maximas
                            PosPiezaParcial[0] = j; PosPiezaParcial[1] = k;
                            CasillasMaxAux = atacarCasillas(j, k, (Piezas)arrayPiezas[i], TableroAux);
                            if (CasillasMaxAux > CasillasMax)
                            {
                                CasillasMax = CasillasMaxAux;
                                PosPiezaParcial[0] = j; PosPiezaParcial[1] = k;
                            }
                        }
                    }
                    PosPiezas[i,0] = PosPiezaParcial[0];
                    PosPiezas[i, 1] = PosPiezaParcial[1];
                    SetPosicion((Piezas)arrayPiezas[i], PosPiezaParcial[0], PosPiezaParcial[1], TableroAux);
                    casillasAtacadas += atacarCasillas(PosPiezaParcial[0], PosPiezaParcial[1], (Piezas)arrayPiezas[i], TableroAux);
                    pintarCasillas(PosPiezaParcial[0], PosPiezaParcial[1], (Piezas)arrayPiezas[i], TableroAux);
                }
                casillasAtacadas += CasillasMax;
                if(casillasAtacadas == 64)
                {
                    //llamar funcion atacar casillas fatales
                    ImprimirTablero(TableroAux);
                    ContTableros++;
                }
            } while (ContTableros < Tableros);

        }

        public static int atacarCasillas(int fila, int columna, Piezas pieza, int[,] tablero)
        {
            int contadorCasillas = 0;
            if (tablero[fila, columna] == 0)
                contadorCasillas = 1; //si pongo la pieza en una posicion vacia
            switch (pieza)
            {
                case Piezas.Rey:
                    if(fila != 0 && fila != 7 && columna != 0 && columna != 7)
                    {
                        if (tablero[fila + 1, columna - 1] == 0)
                            contadorCasillas++;
                        if (tablero[fila - 1, columna + 1] == 0)
                            contadorCasillas++;
                        if (tablero[fila + 1, columna + 1] == 0)
                            contadorCasillas++;
                        if (tablero[fila - 1, columna - 1] == 0)
                            contadorCasillas++;
                        if (tablero[fila, columna - 1] == 0)
                            contadorCasillas++;
                        if (tablero[fila, columna + 1] == 0)
                            contadorCasillas++;
                        if (tablero[fila + 1, columna] == 0)
                            contadorCasillas++;
                        if (tablero[fila - 1, columna] == 0)
                            contadorCasillas++;
                    }
                    else //caso en los bordes 
                    {
                        if (tablero[fila + 1, columna - 1] == 0 && DentroTablero(fila, columna))
                            contadorCasillas++;
                        if (tablero[fila - 1, columna + 1] == 0 && DentroTablero(fila, columna))
                            contadorCasillas++;
                        if (tablero[fila + 1, columna + 1] == 0 && DentroTablero(fila, columna))
                            contadorCasillas++;
                        if (tablero[fila - 1, columna - 1] == 0 && DentroTablero(fila, columna))
                            contadorCasillas++;
                        if (tablero[fila, columna - 1] == 0 && DentroTablero(fila, columna))
                            contadorCasillas++;
                        if (tablero[fila, columna + 1] == 0 && DentroTablero(fila, columna))
                            contadorCasillas++;
                        if (tablero[fila + 1, columna] == 0 && DentroTablero(fila, columna))
                            contadorCasillas++;
                        if (tablero[fila - 1, columna] == 0 && DentroTablero(fila, columna))
                            contadorCasillas++;
                    }
                    break;
                case Piezas.AB:
                case Piezas.AN:
                    int i = 1;
                    int direcciones = 0;
                    while (direcciones != 4)
                    {
                        if (direcciones == 0)
                        {
                            if (!DentroTablero(fila - i, columna - i))//arriba a la izquierda
                                direcciones++;
                            else if (tablero[fila - i, columna - i] == 0)
                            {
                                contadorCasillas++;
                                i++;
                            }
                        }
                        if (direcciones == 1)
                        {
                            i = 1;
                            if (!DentroTablero(fila - i, columna + i)) //arriba a la derecha
                                direcciones++;
                            else if (tablero[fila - i, columna + i] == 0)
                            {
                                contadorCasillas++;
                                i++;
                            }
                        }
                        if (direcciones == 2)
                        {
                            i = 1;
                            if (!DentroTablero(fila + i, columna + i)) //abajo a la derecha
                                direcciones++;
                            else if (tablero[fila + i, columna + i] == 0)
                            {
                                contadorCasillas++;
                                i++;
                            }
                                
                        }
                        if (direcciones == 3) 
                        {
                            i = 1;
                            if (!DentroTablero(fila + i, columna - i)) //arriba a la derecha
                                direcciones++;
                            else if (tablero[fila + i, columna - i] == 0)
                            {
                                contadorCasillas++;
                                i++;
                            }
                        }
                    }
                    break;

                case Piezas.C1:
                case Piezas.C2:
                    if (DentroTablero(fila - 2, columna - 1) && tablero[fila - 2,columna - 1] == 0)
                        contadorCasillas++;
                    if (tablero[fila - 2,columna + 1] == 0 && DentroTablero(fila - 2, columna + 1))
                        contadorCasillas++;
                    if (tablero[fila + 2,columna - 1] == 0 && DentroTablero(fila + 2, columna - 1))
                        contadorCasillas++;
                    if (tablero[fila + 2,columna + 1] == 0 && DentroTablero(fila + 2, columna + 1))
                        contadorCasillas++;
                    if (tablero[fila - 1,columna - 2] == 0 && DentroTablero(fila - 1, columna - 2))
                        contadorCasillas++;
                    if (tablero[fila + 1,columna - 2] == 0 && DentroTablero(fila + 1, columna - 2))
                        contadorCasillas++;
                    if (tablero[fila - 1, columna + 2] == 0 && DentroTablero(fila - 1, columna + 2))
                        contadorCasillas++;
                    if (tablero[fila + 1,  columna + 2] == 0 && DentroTablero(fila + 1, columna + 2))
                        contadorCasillas++;                   
                    break;   
                    
                case Piezas.Reina:

                    break;
            }
            return contadorCasillas;
        }

        public static void pintarCasillas(int fila, int columna, Piezas pieza, int[,] tablero)
        {

        }
        public static int[] OrdenAleatorio(int[] Piezas)
        {
            Random rand = new Random();

            for (int i = 0; i < Piezas.Length - 1; i++)
            {
                int j = rand.Next(i, Piezas.Length);
                int aux = Piezas[i];
                Piezas[i] = Piezas[j];
                Piezas[j] = aux;
            }
            return Piezas;
        }
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
        //Completar
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