using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TpAjedrezLP2
{
    //TODO: Verificar que no esten repetido
    // - Guardar en una matriz el orden de piezas creado aleatoriamente. Una vez que se
    //   encuentra un nuevo orden, lo comparamos con la matriz ára ver que no se repita. 
    public class Program
    {
        public const int N = 8;
        public const int T = 10;
        public const int P = 5;
        public const int Tableros = 10;
        public enum Piezas
        {
            //El 0: Casilla Vacia
            //   1: Casilla Atacada
            Ra = 2, T1, T2, Ry, AN, AB, C1, C2, X //la x es para casillas fatales
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
            int[] PosPiezaParcialAux = new int[2];
            int[,] PosPiezas = new int[8,2];
            int[,] OrdenesTableros = new int[T,P];//T: Tableros  //P: PosicionesRand

            //TORRES
            int[] PosTorre1 = new int[2];
            PosTorre1[0] = 0; PosTorre1[1] = 0;
            int[] PosTorre2 = new int[2];
            PosTorre2[0] = 7; PosTorre2[1] = 7;
            casillasAtacadas += colocarTorres(PosTorre1, PosTorre2, TableroOriginal);

            int[] PosReina = new int[2];
             
            int auxK; //para el segundo for interior
            int[] arrayAux;

            //---------------------------Aca empieza el while principal del programa --------------------------------------------
            do
            {
                TableroAux = (int[,])TableroOriginal.Clone();
                casillasAtacadas = 28; //las casillas que atacan las torres
                //Determinamos el orden aleatorio que se van a probar las piezas
                bool var = true;
                
                while (var)
                {
                     arrayAux = OrdenAleatorio(arrayPiezas);
                    if (!TableroRepetido(OrdenesTableros, T, P, arrayAux))
                        var = false;
                }
                //para no modificar el array original
                
                //Determinamos la posicion de la Reina de forma aleatoria
                Random rnd = new Random();
                int fila = rnd.Next(3, 5);
                int columna = rnd.Next(3, 5);
                casillasAtacadas += atacarCasillas(fila, columna, Piezas.Ra, TableroAux);
                SetPosicion(Program.Piezas.Ra, fila, columna, TableroAux); //cuando implementemos el forms, esto queda determinado por el usuario
                PosReina[0] = fila;
                PosReina[1] = columna;

                //CopiarTablero(TableroOriginal, TableroAux); //cada vez que buscamos un tablero, lo reinciamos
                //la funcion de CopiarTablero() no esta funcionando
                //Triple for n^3 -> Primer FOR para las piezas y los otros dos para recorrer el tablero con varios métodos de poda. 
                for (int i = 0; i < 5; i++)//FOR para cada pieza xq son 5 piezas
                {
                    CasillasMax = 0;
                    int aux = 1;
                    for (int j = 0; j < N; j++)
                    {
                        auxK = 0;
                        if (arrayPiezas[i] == (int)Piezas.AB || arrayPiezas[i] == (int)Piezas.AN)//Pregunta si la pieza es un alfil
                        {
                            aux = 2; //incrementamos el for de a 2 
                            if ((arrayPiezas[i] == (int)Piezas.AB && j % 2 == 0) || (arrayPiezas[i] == (int)Piezas.AN && j % 2 != 0))
                                auxK = 1;
                        }
                        for (int k = auxK; k < N; k = k + aux)
                        {
                        
                            if (ValidarPosicion(j, k, TableroAux, (Piezas)arrayPiezas[i]))
                            {
                                //aca deberiamos verificar que si la pieza es un alfil, que no este en la diagonal de la reina, pq sino las posiciones atacadas nunca serían las maximas
                                CasillasMaxAux = atacarCasillas(j, k, (Piezas)arrayPiezas[i], TableroAux);
                                if (CasillasMaxAux >= CasillasMax)
                                {
                                    CasillasMax = CasillasMaxAux;
                                    PosPiezaParcial[0] = j; PosPiezaParcial[1] = k;
                                }                             
                            }
                        }
                    }
                    PosPiezas[i,0] = PosPiezaParcial[0];
                    PosPiezas[i, 1] = PosPiezaParcial[1];
                    SetPosicion((Piezas)arrayPiezas[i], PosPiezaParcial[0], PosPiezaParcial[1], TableroAux);
                    casillasAtacadas += CasillasMax;
                    pintarCasillas(PosPiezaParcial[0], PosPiezaParcial[1], (Piezas)arrayPiezas[i], TableroAux,0,1);

                }
                //casillasAtacadas += CasillasMax;
                if(casillasAtacadas == 64)
                {
                    //TODO: Fijarse xq sale error ACA
                    //GuardarPosicion(OrdenesTableros,ContTableros, P, arrayAux); 
                    Console.Write("TABLERO N° {0}\n", ContTableros + 1);
                    ImprimirTablero(TableroAux);
                    int[] arrayPiezasFatales = new int[8];//llamar funcion atacar casillas fatales
                    arrayPiezas.CopyTo(arrayPiezasFatales,0);
                    arrayPiezasFatales[5] = (int)Piezas.Ra;
                    arrayPiezasFatales[6] = (int)Piezas.T1;
                    arrayPiezasFatales[7] = (int)Piezas.T2;
                    PosPiezas[5,0] = PosReina[0];
                    PosPiezas[5, 1] = PosReina[1];
                    PosPiezas[6, 0] = PosTorre1[0];
                    PosPiezas[6, 1] = PosTorre1[1];
                    PosPiezas[7, 0] = PosTorre2[0];
                    PosPiezas[7, 1] = PosTorre2[1];
                    casillasFatales(arrayPiezasFatales, PosPiezas, TableroAux);
                    ImprimirTablero(TableroAux);
                    ContTableros++;
                }
            } while (ContTableros < Tableros);

        }

        //----------------------------------------------------FUNCiONES------------------------------------------------------------------------
        public static void casillasFatales(int[] arrayPiezas, int[,]Posiciones, int[,]tablero)//las posiciones estan en el mismo orden que las piezas
        {
            
            for(int i = 0; i < N; i++)
            {
                int k = 1, j = 1;
                switch ((Piezas)arrayPiezas[i])
                {
                    case Piezas.T1:                    
                    case Piezas.T2:
                        fatalHorizontalVertical(Posiciones[i, 0], Posiciones[i, 1], tablero);
                        break;

                    case Piezas.Ry:
                        pintarCasillas(Posiciones[i, 0], Posiciones[i, 1],Piezas.Ry, tablero, 1,(int)Piezas.X);
                        break;

                    case Piezas.C1:
                    case Piezas.C2:
                        pintarCasillas(Posiciones[i, 0], Posiciones[i, 1], Piezas.C1, tablero, 1, (int)Piezas.X);
                        break;

                    case Piezas.AN:
                    case Piezas.AB:
                        fatalDiagonal(Posiciones[i, 0], Posiciones[i, 1], tablero);
                        break;
                    case Piezas.Ra:
                        fatalHorizontalVertical(Posiciones[i, 0], Posiciones[i, 1], tablero);
                        fatalDiagonal(Posiciones[i, 0], Posiciones[i, 1], tablero);
                        break;
                }
            }
        }
        public static void fatalHorizontalVertical(int fila, int columna, int[,] tablero)
        {
            int j = 1;
            if (DentroTablero(fila, columna + j))
                {
                while (tablero[fila, columna + j] == 1 || tablero[fila, columna + j] == (int)Piezas.X)
                {
                    tablero[fila, columna + j] = (int)Piezas.X;
                    j++;
                    if (columna + j >= N)
                        break;
                }
            }
            j = 1;
            if (DentroTablero(fila, columna - j))
            {
                while (tablero[fila, columna - j] == 1 || tablero[fila, columna - j] == (int)Piezas.X)
                {
                    tablero[fila, columna - j] = (int)Piezas.X;
                    j++;
                    if (columna - j < 0)
                        break;
                }
            }
            j = 1;
            if (DentroTablero(fila + j, columna))
            {
                while (tablero[fila + j, columna] == 1 || tablero[fila + j, columna] == (int)Piezas.X)
                {
                    tablero[fila + j, columna] = (int)Piezas.X;
                    j++;
                    if (fila + j >= N)
                        break;
                }
            }
            j = 1;
            if (DentroTablero(fila - j, columna))
            {
                while (tablero[fila - j, columna] == 1 || tablero[fila - j, columna] == (int)Piezas.X)
                {
                    tablero[fila - j, columna] = (int)Piezas.X;
                    j++;
                    if (fila - j < 0)
                        break;
                }
            }


        }
        public static void fatalDiagonal(int fila, int columna, int[,] tablero)
        {
            int k = 1;
            if (DentroTablero(fila + k, columna + k)) //abajo a la derecha
            {
                while (tablero[fila + k, columna + k] == 1 || tablero[fila + k, columna + k] == (int)Piezas.X)
                {
                    tablero[fila + k, columna + k] = (int)Piezas.X;
                    k++;
                    if (fila + k >= N || columna + k >= N)
                        break;
                }
            }
            k = 1;
            if (DentroTablero(fila + k, columna - k)) //abajo a la izquierda
            {
                while (tablero[fila + k, columna - k] == 1 || tablero[fila + k, columna - k] == (int)Piezas.X)
                {
                    tablero[fila + k, columna - k] = (int)Piezas.X;
                    k++;
                    if (fila + k >= N || columna - k < 0)
                        break;
                }
            }
            k = 1;
            if (DentroTablero(fila - k, columna + k))
            {
                while (tablero[fila - k, columna + k] == 1 || tablero[fila - k, columna + k] == (int)Piezas.X)
                {
                    tablero[fila - k, columna + k] = (int)Piezas.X;
                    k++;
                    if (fila - k < 0 || columna + k >= N)
                        break;
                }
            }
            k = 1;
            if (DentroTablero(fila - k, columna - k))
            {
                while (tablero[fila - k, columna - k] == 1 || tablero[fila - k, columna - k] == (int)Piezas.X)
                {
                    tablero[fila - k, columna - k] = (int)Piezas.X;
                    k++;
                    if (fila - k < 0 || columna - k < 0)
                        break;
                }
            }
        }
        public static int atacarCasillas(int fila, int columna, Piezas pieza, int[,] tablero)
        {
            int contadorCasillas = 0;
            int i ;
            if (tablero[fila, columna] == 0)
                contadorCasillas = 1; //si pongo la pieza en una posicion vacia
            switch (pieza)
            {
                case Piezas.Ry:
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
                        if(DentroTablero(fila + 1, columna - 1))
                        {if (tablero[fila + 1, columna - 1] == 0) 
                                contadorCasillas++;
                        }
                        if (DentroTablero(fila - 1, columna + 1))
                        { if (tablero[fila - 1, columna + 1] == 0)
                                contadorCasillas++;
                        }
                        if ( DentroTablero(fila + 1, columna + 1))
                        {
                            if(tablero[fila + 1, columna + 1] == 0)
                                contadorCasillas++;
                        }
                        if(DentroTablero(fila - 1, columna - 1))
                        {
                            if (tablero[fila - 1, columna - 1] == 0)
                                contadorCasillas++;
                        }
                        if(DentroTablero(fila, columna - 1))
                        {
                            if (tablero[fila, columna - 1] == 0)
                                contadorCasillas++;
                        }
                        if(DentroTablero(fila, columna + 1))
                        {
                            if (tablero[fila, columna + 1] == 0 )
                                contadorCasillas++;
                        }
                        if(DentroTablero(fila + 1, columna))
                        {
                            if (tablero[fila + 1, columna] == 0)
                                contadorCasillas++;
                        }
                       if(DentroTablero(fila - 1, columna))
                        {
                            if (tablero[fila - 1, columna] == 0 )
                                contadorCasillas++;
                        }
                     
                    }
                    break;
                case Piezas.AB:
                case Piezas.AN:
                    int d0 = 1, d1 = 1, d2 = 1, d3 = 1;
                    int direcciones = 0;
                    while (direcciones != 4)
                    {
                        if (direcciones == 0)
                        {
                            if (!DentroTablero(fila - d0, columna - d0))//arriba a la izquierda
                                direcciones++;
                            else if (tablero[fila - d0, columna - d0] == 0)
                            {
                                contadorCasillas++;
                                d0++;
                            }
                            else
                                d0++;
                        }
                        if (direcciones == 1)
                        {
                            if (!DentroTablero(fila - d1, columna + d1)) //arriba a la derecha
                                direcciones++;
                            else if (tablero[fila - d1, columna + d1] == 0)
                            {
                                contadorCasillas++;
                                d1++;
                            }
                            else
                                d1++;
                        }
                        if (direcciones == 2)
                        {
                            if (!DentroTablero(fila + d2, columna + d2)) //abajo a la derecha
                                direcciones++;
                            else if (tablero[fila + d2, columna + d2] == 0)
                            {
                                contadorCasillas++;
                                d2++;
                            }
                            else
                                d2++;
                                
                        }
                        if (direcciones == 3) 
                        {
                            if (!DentroTablero(fila + d3, columna - d3)) //abajo a la izquierda
                                direcciones++;
                            else if (tablero[fila + d3, columna - d3] == 0)
                            {
                                contadorCasillas++;
                                d3++;
                            }
                            else
                                d3++;
                        }
                    }
                    break;

                case Piezas.C1:
                case Piezas.C2:
                    if (DentroTablero(fila - 2, columna - 1))
                    {
                        if (tablero[fila - 2, columna - 1] == 0)
                            contadorCasillas++;
                    }
                    if (DentroTablero(fila - 2, columna + 1))
                    {
                        if (tablero[fila - 2, columna + 1] == 0)
                            contadorCasillas++;
                    }
                    if (DentroTablero(fila + 2, columna - 1))
                    {
                        if (tablero[fila + 2, columna - 1] == 0)
                            contadorCasillas++;
                    }
                    if (DentroTablero(fila + 2, columna + 1))
                    {
                        if (tablero[fila + 2, columna + 1] == 0)
                            contadorCasillas++;
                    }
                    if (DentroTablero(fila - 1, columna - 2))
                    {
                        if (tablero[fila - 1, columna - 2] == 0)
                            contadorCasillas++;
                    }
                    if (DentroTablero(fila + 1, columna - 2))
                    {
                        if (tablero[fila + 1, columna - 2] == 0)
                            contadorCasillas++;
                    }
                    if (DentroTablero(fila - 1, columna + 2))
                    {
                        if (tablero[fila - 1, columna + 2] == 0)
                            contadorCasillas++;
                    }
                    if (DentroTablero(fila + 1, columna + 2))
                    {
                        if (tablero[fila + 1, columna + 2] == 0)
                            contadorCasillas++;
                    }
                    break;   
                    
                case Piezas.Ra:
                    i = 1;
                    while (i<5) //ya ataco todas las casillas posibles para la reina,//esta condicion se puede reemplazar para un caso mas general con la condicion del while de la funcion pintar casillas
                    {
                        switch (i){
                            case 1:
                            case 2:
                            case 3:
                                if (tablero[fila + i, columna + i] == 0)
                                {
                                    tablero[fila + i, columna + i] = 1;
                                    contadorCasillas++;
                                }
                                if (tablero[fila + i, columna - i] == 0)
                                {
                                    tablero[fila + i, columna - i] = 1;
                                    contadorCasillas++;
                                }
                                if (tablero[fila - i, columna + i] == 0)
                                {
                                    tablero[fila - i, columna + i] = 1;
                                    contadorCasillas++;
                                }
                                if (tablero[fila - i, columna - i] == 0)
                                {
                                    tablero[fila - i, columna - i] = 1;
                                    contadorCasillas++;
                                }
                                if (tablero[fila, columna + i] == 0)
                                {
                                    tablero[fila, columna + i] = 1;
                                    contadorCasillas++;
                                }
                                if (tablero[fila, columna - i] == 0)
                                {
                                    tablero[fila, columna - i] = 1;
                                    contadorCasillas++;
                                }
                                if (tablero[fila + i, columna] == 0)
                                {
                                    tablero[fila + i, columna] = 1;
                                    contadorCasillas++;
                                }
                                if (tablero[fila - i, columna] == 0)
                                {
                                    tablero[fila - i, columna] = 1;
                                    contadorCasillas++;
                                }
                                break;
                            case 4:
                                if (DentroTablero(fila + i, columna + i))
                                {
                                    if (tablero[fila + i, columna + i] == 0)
                                    {
                                        tablero[fila + i, columna + i] = 1;
                                        contadorCasillas++;
                                    }
                                }
                                if (DentroTablero(fila + i, columna - i))
                                {
                                    if (tablero[fila + i, columna - i] == 0)
                                    {
                                        tablero[fila + i, columna - i] = 1;
                                        contadorCasillas++;
                                    }
                                }
                                if (DentroTablero(fila - i, columna + i))
                                {
                                    if (tablero[fila - i, columna + i] == 0)
                                    {
                                        tablero[fila - i, columna + i] = 1;
                                        contadorCasillas++;
                                    }
                                }
                                if (DentroTablero(fila - i, columna - i))
                                {
                                    if (tablero[fila - i, columna - i] == 0)
                                    {
                                        tablero[fila - i, columna - i] = 1;
                                        contadorCasillas++;
                                    }
                                }
                                if (DentroTablero(fila, columna + i))
                                {
                                    if (tablero[fila, columna + i] == 0)
                                    {
                                        tablero[fila, columna + i] = 1;
                                        contadorCasillas++;
                                    }
                                }
                                if (DentroTablero(fila, columna - i))
                                {
                                    if (tablero[fila, columna - i] == 0)
                                    {
                                        tablero[fila, columna - i] = 1;
                                        contadorCasillas++;
                                    }
                                }
                                if (DentroTablero(fila + i, columna))
                                {
                                    if (tablero[fila + i, columna] == 0)
                                    {
                                        tablero[fila + i, columna] = 1;
                                        contadorCasillas++;
                                    }
                                }
                                if (DentroTablero(fila - i, columna))
                                {
                                    if (tablero[fila - i, columna] == 0)
                                    {
                                        tablero[fila - i, columna] = 1;
                                        contadorCasillas++;
                                    }
                                }
                                break;
                        } 
                        
                        i++;
                    }
                    break;
            }
            return contadorCasillas;
        }
        public static void pintarCasillas(int fila, int columna, Piezas pieza, int[,] tablero,  int condicion, int parametro)
        {
            int i = 0;
            switch (pieza)
            {
                case Piezas.Ry:
                    if (fila != 0 && fila != 7 && columna != 0 && columna != 7)
                    {
                        if (tablero[fila + 1, columna - 1] == condicion)
                            tablero[fila + 1, columna - 1] = parametro;
                        if (tablero[fila - 1, columna + 1] == condicion)
                            tablero[fila - 1, columna + 1] = parametro;
                        if (tablero[fila + 1, columna + 1] == condicion)
                            tablero[fila + 1, columna + 1] = parametro;
                        if (tablero[fila - 1, columna - 1] == condicion)
                            tablero[fila - 1, columna - 1] = parametro;
                        if (tablero[fila, columna - 1] == condicion)
                            tablero[fila, columna - 1] = parametro;
                        if (tablero[fila, columna + 1] == condicion)
                            tablero[fila, columna + 1] = parametro;
                        if (tablero[fila + 1, columna] == condicion)
                            tablero[fila + 1, columna] = parametro;
                        if (tablero[fila - 1, columna] == condicion)
                            tablero[fila - 1, columna] = parametro;
                    }
                    else //caso en los bordes 
                    {
                        if (DentroTablero(fila + 1, columna - 1))
                        {
                            if (tablero[fila + 1, columna - 1] == condicion)
                                tablero[fila + 1, columna - 1] = parametro;
                        }
                        if (DentroTablero(fila - 1, columna + 1))
                        {
                            if (tablero[fila - 1, columna + 1] == condicion)
                                tablero[fila - 1, columna + 1] = parametro;
                        }
                        if (DentroTablero(fila + 1, columna + 1))
                        {
                            if (tablero[fila + 1, columna + 1] == condicion)
                                tablero[fila + 1, columna + 1] = parametro;
                        }
                        if (DentroTablero(fila - 1, columna - 1))
                        {
                            if (tablero[fila - 1, columna - 1] == condicion)
                                tablero[fila - 1, columna - 1] = parametro;
                        }
                        if (DentroTablero(fila, columna - 1))
                        {
                            if (tablero[fila, columna - 1] == condicion)
                                tablero[fila, columna - 1] = parametro;
                        }
                        if (DentroTablero(fila, columna + 1))
                        {
                            if (tablero[fila, columna + 1] == condicion)
                                tablero[fila, columna + 1] = parametro;
                        }
                        if (DentroTablero(fila + 1, columna))
                        {
                            if (tablero[fila + 1, columna] == condicion)
                                tablero[fila + 1, columna] = parametro;
                        }
                        if (DentroTablero(fila - 1, columna))
                        {
                            if (tablero[fila - 1, columna] == condicion)
                                tablero[fila - 1, columna] = parametro;
                        }

                    }
                    break;
                case Piezas.AB:
                case Piezas.AN:
                    int d0 = 1,d1 = 1, d2 = 1, d3=1;
                    int direcciones = 0;
                    while (direcciones != 4)
                    {
                        if (direcciones == 0)
                        {
                            if (!DentroTablero(fila - d0, columna - d0))//arriba a la izquierda
                                direcciones++;
                            else if (tablero[fila - d0, columna - d0] == 0)
                            {
                                tablero[fila - d0, columna - d0] = 1;
                                d0++;
                            }
                            else
                                d0++;
                        }
                        if (direcciones == 1)
                        {
                            if (!DentroTablero(fila - d1, columna + d1)) //arriba a la derecha
                                direcciones++;
                            else if (tablero[fila - d1, columna + d1] == 0)
                            {
                                tablero[fila - d1, columna + d1] = 1;
                                d1++;
                            }
                            else
                                d1++;
                        }
                        if (direcciones == 2)
                        {
                            if (!DentroTablero(fila + d2, columna + d2)) //abajo a la derecha
                                direcciones++;
                            else if (tablero[fila + d2, columna + d2] == 0)
                            {
                                tablero[fila + d2, columna + d2] = 1;
                                d2++;
                            }
                            else
                                d2++;
                        }
                        if (direcciones == 3)
                        {
                            if (!DentroTablero(fila + d3, columna - d3)) //arriba a la derecha
                                direcciones++;
                            else if (tablero[fila + d3, columna - d3] == 0)
                            {
                                tablero[fila + d3, columna - d3] = 1;
                                d3++;
                            }
                            else
                                d3++;
                        }
                    }
                    break;

                case Piezas.C1:
                case Piezas.C2:
                    if (DentroTablero(fila - 2, columna - 1))
                    {
                        if (tablero[fila - 2, columna - 1] == condicion)
                            tablero[fila - 2, columna - 1] = parametro;
                    }
                    if (DentroTablero(fila - 2, columna + 1))
                    {
                        if (tablero[fila - 2, columna + 1] == condicion)
                            tablero[fila - 2, columna + 1] = parametro;
                    }
                    if (DentroTablero(fila + 2, columna - 1))
                    {
                        if (tablero[fila + 2, columna - 1] == condicion)
                            tablero[fila + 2, columna - 1] = parametro;
                    }
                    if (DentroTablero(fila + 2, columna + 1))
                    {
                        if (tablero[fila + 2, columna + 1] == condicion)
                            tablero[fila + 2, columna + 1] = parametro;
                    }
                    if (DentroTablero(fila - 1, columna - 2))
                    {
                        if (tablero[fila - 1, columna - 2] == condicion)
                            tablero[fila - 1, columna - 2] = parametro;
                    }
                    if (DentroTablero(fila + 1, columna - 2))
                    {
                        if (tablero[fila + 1, columna - 2] == condicion)
                            tablero[fila + 1, columna - 2] = parametro;
                    }
                    if (DentroTablero(fila - 1, columna + 2))
                    {
                        if (tablero[fila - 1, columna + 2] == condicion)
                            tablero[fila - 1, columna + 2] = parametro;
                    }
                    if (DentroTablero(fila + 1, columna + 2))
                    {
                        if (tablero[fila + 1, columna + 2] == condicion)
                            tablero[fila + 1, columna + 2] = parametro;
                    }
                    break;
            }           
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
                tablero[posicionTorre2[0] - i, posicionTorre2[1]] = 1; //pinto como atacada toda la ultima columna
                tablero[posicionTorre2[0], posicionTorre2[1] - i] = 1; //pinto como atacada toda la ultima fila
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
            int[] aux = new int[5];
            aux[0] = (int)Piezas.Ry;
            aux[1] = (int)Piezas.C1;
            aux[2] = (int)Piezas.C2;
            aux[3] = (int)Piezas.AB;
            aux[4] = (int)Piezas.AN;
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
            if (tablero[fila, columna] == (int)Piezas.Ry && (pieza == Piezas.C1 || pieza == Piezas.C2))
                return true;
            //Si hay un caballo y quiero poner un rey
            if (tablero[fila, columna] == (int)Piezas.C1 && pieza == Piezas.Ry)
                return true;
            if (tablero[fila, columna] == (int)Piezas.C2 && pieza == Piezas.Ry)
                return true;
            return false;
        }
        public static void ImprimirTablero(int[,] tablero)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if(tablero[i,j] <= 1)
                        Console.Write(tablero[i, j] + "\t ");
                    else
                        Console.Write((Piezas)tablero[i, j] + "\t ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }
        public static void CopiarTablero(int[,] tableroFuente, int[,] tableroDestino)
        {
            tableroDestino = (int[,])tableroFuente.Clone();
        }

        public static bool TableroRepetido(int[,] Ordenes, int fil, int col, int[]OrdenPiezas)
        {
            int i = 0, j = 0;
            while (i<fil)
            {
                if (Ordenes[i, j] == OrdenPiezas[j])
                {
                    j++;
                    if(j==col)
                    {
                        return true;
                    }
                }
                else
                {
                    i++;
                }
            }
            return true;
        }
        public static void GuardarPosicion(int[,] Ordenes,int pos, int col, int[] OrdenPiezas)
        {
            for (int k = 0; k < col; k++)
            {
                Ordenes[pos, k] = OrdenPiezas[k];
            }
        }
    }
}
// 1 2 3 4
// 1 2 3 4
// 1 2 3 4
// 1 2 3 4
