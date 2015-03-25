using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace juego_memorize
{
    public partial class Form1 : Form
    {
        #region variables
        private Button[] botones = new Button[16];
        private Button botonAnt = new Button();
        private PictureBox[] miniatura = new PictureBox[16];
        private int parejas = 0;
        private int contador = 1;
        private int[] arr = new int[16];
        private Random r = new Random();
        private String[] rutas = null;
        #endregion

        public Form1()
        {
            InitializeComponent();
            this.botonAnt.Tag = -1;//seteo por defecto del boton anterior
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            rutas = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "imagenes"));
            this.crearBotones();
            this.crearMiniatura();

        }
        //click de botones
        private void boton_click(object sender, EventArgs e)
        {
            Button miboton = sender as Button;
            miboton.BackgroundImage = miniatura[(int)miboton.Tag].Image;//sete de imagen a botn
            verificacion(this.botonAnt, miboton);
            this.botonAnt = miboton;//guarda el boton actual como el anterior

        }
        //boton de reseteo (Nuevo Juego)
        private void button1_Click(object sender, EventArgs e)
        {

            this.arr = new int[16];
            this.crearMiniatura();

            foreach (Control c in this.Controls)
            {
                if (c is Button)
                {
                    c.BackgroundImage = null;
                    c.Enabled = true;
                }
            }

        }

        #region Crea botones
        private void crearBotones()
        {
            int x0 = 80;
            int y0 = 80;
            int x = x0;
            int y = y0;
            int w = 100;
            for (int i = 0; i < 16; i++)
            {
                this.botones[i] = new Button();
                this.botones[i].Location = new Point(x, y);
                this.botones[i].Size = new Size(w, w);
                this.botones[i].BackColor = Color.Red;
                this.botones[i].Tag = i;
                this.botones[i].Click += new EventHandler(this.boton_click);
                this.Controls.Add(this.botones[i]);

                x = x + w;
                if ((i + 1) % 4 == 0)
                {
                    x = x0;
                    y = y + w;
                }
            }
        }
        #endregion

        #region crea miniaturas
        private void crearMiniatura()
        {

            this.genera_random_no_repetido();//invoco metodo que genera numeros randomicos sin repetirlos
            int j = 0;
            //recorre el arreglo con los numeros desordenados
            foreach (int e in this.arr)
            {
                this.miniatura[j] = new PictureBox();
                this.miniatura[j].Location = new Point(0, 0);
                this.miniatura[j].Size = new Size(20, 20);
                this.miniatura[j].SizeMode = PictureBoxSizeMode.StretchImage;
                this.miniatura[j].Tag = Path.GetFileName(rutas[e]); ;
                this.miniatura[j].Image = Image.FromFile(rutas[e]);
                j++;
            }
        }
        #endregion

        #region logica
        /**
         *Método de verificacion, si hubo un ganador 
         **/
        private void verificacion(Button anterior, Button actual)
        {

            if ((this.contador % 2 == 0) && (int)anterior.Tag != -1)
            {
                //validacion si son imagenes iguales en este caso por nombre img1 = img1  (solo tomo el numero)
                if (this.miniatura[(int)anterior.Tag].Tag.ToString().Substring(3, 1) == this.miniatura[(int)actual.Tag].Tag.ToString().Substring(3, 1))
                {
                    anterior.Enabled = false;//desactiva boton anterior
                    actual.Enabled = false;//desactiva boton actual
                    this.parejas++;
                }
                else//de lo contrario se muestra por medio segundo y se resetean las imagenes
                {
                    Thread a = new Thread(() =>//se utiliza un hilo para que no se trabe el programa
                    {
                        actual.Invoke(new Action(() =>//se aplica una llamada a un metodo cuando se presiona el boton actual
                        {
                            Thread.Sleep(500);//espera de .5 seg
                            anterior.BackgroundImage = null;//resetea imagen boton anterior
                            actual.BackgroundImage = null;//resetea imagen boton actual
                        }));

                    });
                    a.Start();//se inicia el hilo
                }
            }
            //si el contador de parejas es igual a la mitad de la cantidad de miniaturas ganas
            if (this.parejas == (this.miniatura.Length) / 2)
            {
                MessageBox.Show("Ganaste");
                this.parejas = 0;
                this.contador = 1;
                return;
            }
            this.contador++;
        }


        /**
         * Metodo que genera numeros aleatorios sin repetir 
         *
         **/
        private void genera_random_no_repetido()
        {

            this.arr[0] = 0;
            for (int i = 1; i < 16; i++)
            {
                int num;
                do
                {
                    num = r.Next(1, 16);
                } while (repetido(this.arr, num));
                this.arr[i] = num;

            }
        }

        /**
         *Método que verifica que los numeros que se ingresan al arreglo no esten repetidos
         *
         **/
        private static bool repetido(int[] a, int r)
        {
            for (int i = 1; i < 16; i++)
                if (r == a[i])
                    return true;
            return false;
        }
        #endregion

    }  
}
