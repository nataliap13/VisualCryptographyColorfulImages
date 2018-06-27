using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.Util;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace POD_lab9_kryptografia_wizualna
{
    class Program
    {
        private static string[] group = { "__XX", "_X_X", "_XX_", "X__X", "X_X_", "XX__" };
        private static Random rand = new Random();
        private static char empty = '_';
        private static char full = 'X';
        private static char new_line = (char)13;
        static void Main(string[] args)
        {
            Console.Write("Autor: Natalia Popielarz");
            int choice = 1;
            do
            {
                Console.WriteLine("\n\n\nKryptografia Wizualna kolorowa");
                Console.WriteLine("1. Zaszyfruj wiele sekretow - liste plikow umiesc w pliku files.txt");
                Console.WriteLine("2. Zaszyfruj sekret z pliku secret.png");
                Console.WriteLine("3. Odszyfruj wiele sekretow - liste plikow umiesc w pliku files2.txt");
                Console.WriteLine("4. Odszyfruj sekret z plikow secretpart.png");
                Console.WriteLine("5. Pomoc");
                Console.WriteLine("0. Wyjscie");

                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 0:
                            { break; }
                        case 1:
                            {
                                try
                                {
                                    var source = Read_file_txt("files.txt");
                                    var filename = Parse_message_txt(source);
                                    Console.Write("Podaj liczbe udzialow do utworzenia: ");
                                    if (int.TryParse(Console.ReadLine(), out choice) && choice > 0)
                                    {
                                        for (int i = 0; i < filename.Length; i++)
                                        {
                                            UI_encrypt_secret_img(filename[i], choice);
                                        }
                                    }
                                    else { Console.WriteLine("Niepoprawna wartość."); choice = 1; }
                                }
                                catch (Exception e)
                                { Console.WriteLine(e.Message); }
                                break;
                            }
                        case 2:
                            {
                                Console.Write("Podaj liczbe udzialow do utworzenia: ");
                                if (int.TryParse(Console.ReadLine(), out choice) && choice > 0)
                                {
                                    UI_encrypt_secret_img("secret.png", choice);
                                }
                                else { Console.WriteLine("Niepoprawna wartość."); choice = 1; }
                                break;
                            }
                        case 3:
                            {
                                try
                                {
                                    var source = Read_file_txt("files2.txt");
                                    var filename = Parse_message_txt(source);
                                    Console.Write("Podaj liczbe udzialow do zlozenia: ");
                                    if (int.TryParse(Console.ReadLine(), out choice) && choice > 0)
                                    {
                                        for (int i = 0; i < filename.Length; i++)
                                        {
                                            UI_decrypt_secret_txt(filename[i], choice);
                                        }
                                    }
                                    else { Console.WriteLine("Niepoprawna wartość."); choice = 1; }
                                }
                                catch (Exception e)
                                { Console.WriteLine(e.Message); }
                                break;
                            }
                        case 4:
                            {
                                Console.Write("Podaj liczbe udzialow do zlozenia: ");
                                if (int.TryParse(Console.ReadLine(), out choice) && choice > 0)
                                {
                                    UI_decrypt_secret_txt("secret.png", choice);
                                }
                                else { Console.WriteLine("Niepoprawna wartość."); choice = 1; }
                                break;
                            }
                        case 5:
                            {
                                UI_help();
                                break;
                            }
                    }
                }
                else
                {
                    choice = 1;
                }
            } while (choice != 0);
        }
        public static void UI_help()
        {
            Console.WriteLine("\nPOMOC\n");
            Console.WriteLine("Jesli chcesz zaszyfrować wiele obrazków, stwórz plik files.txt.");
            Console.WriteLine("W srodku umiesc nazwy wszystkich plikow, ktore chcesz zaszyfrowac w udzialy.");
            Console.WriteLine("\nJesli chcesz odszyfrować wiele obrazków, stwórz plik files2.txt.");
            Console.WriteLine("W srodku umiesc nazwy plikow pierwotnych tzn.");
            Console.WriteLine("Jesli posiadasz udzialy o nazwie np. abcpart1.png i abcpart2.png");
            Console.WriteLine("W pliku files2.txt umiesc napis abc.png");
        }
        public static void UI_encrypt_secret_img(string input_filename, int number_of_parts)
        {
            //wczytujemy obraz w ktorym bedziemy szyfrowac wiadomosc
            Image<Bgr, Byte> img = null;
            try
            {
                img = new Image<Bgr, Byte>(input_filename);
                input_filename = input_filename.Remove(input_filename.Length - 4);
                List<Image<Bgr, Byte>> parts = new List<Image<Bgr, byte>>();
                for (int i = 0; i < number_of_parts; i++)
                {
                    parts.Add(new Image<Bgr, byte>(new Size(img.Width, img.Height)));
                }

                for (int i = 0; i < img.Height; i++)
                {
                    for (int j = 0; j < img.Width; j++)
                    {
                        double sum_r = 0;
                        double sum_g = 0;
                        double sum_b = 0;

                        for (int k = 0; k < number_of_parts - 1; k++)
                        {
                            double red = rand.Next(0, 256);
                            double green = rand.Next(0, 256);
                            double blue = rand.Next(0, 256);
                            if (red == 256 || green == 256 || blue == 256)
                            { Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!"); }
                            sum_r += red;
                            sum_g += green;
                            sum_b += blue;
                            Bgr clr = new Bgr(blue, green, red);
                            parts[k][i, j] = clr;
                            /*
                             Bgr color = img[y, x];
                             img[y,x] = color;
                             */
                        }
                        sum_r %= 256;
                        sum_g %= 256;
                        sum_b %= 256;
                        if (sum_r > img[i, j].Red)
                        { sum_r -= 255; }
                        if (sum_g > img[i, j].Green)
                        { sum_g -= 255; }
                        if (sum_b > img[i, j].Blue)
                        { sum_b -= 255; }

                        Bgr color = new Bgr(img[i, j].Blue - sum_b, img[i, j].Green - sum_g, img[i, j].Red - sum_r);
                        parts[number_of_parts - 1][i, j] = color;
                    }
                }
                /*/////ukrywanie ostatniego udzialu zbyt podobnego do oryginalu
                for (int i = 0; i < img.Height; i++)
                {
                    for (int j = 0; j < img.Width; j++)
                    {
                        int rand_part = rand.Next(0, number_of_parts - 1);
                        Bgr temp = parts[rand_part][i, j];
                        parts[rand_part][i, j] = parts[number_of_parts - 1][i, j];
                        parts[number_of_parts-1][i, j] = temp;
                    }
                }
                /*///////zapisywanie obrazow
                for (int i = 0; i < number_of_parts; i++)
                {
                    parts[i].Save(input_filename + " part" + i + ".png");
                }
                Console.WriteLine("Szyfrowanie sekretu zakonczone.");
            }
            catch (Exception e)
            { Console.WriteLine(e.Message); }
        }
        public static void UI_decrypt_secret_txt(string input_filename, int number_of_parts)
        {
            //img = new Image<Bgr, Byte>(input_filename);
            input_filename = input_filename.Remove(input_filename.Length - 4);
            List<Image<Bgr, Byte>> parts = new List<Image<Bgr, byte>>();
            for (int i = 0; i < number_of_parts; i++)
            {
                parts.Add(new Image<Bgr, byte>(input_filename + " part" + i + ".png"));
            }
            Image<Bgr, Byte> img = new Image<Bgr, byte>(new Size(parts[0].Width, parts[0].Height));

            for (int i = 0; i < parts[0].Height; i++)
            {
                for (int j = 0; j < parts[0].Width; j++)
                {
                    double sum_r = 0;
                    double sum_g = 0;
                    double sum_b = 0;

                    for (int k = 0; k < number_of_parts; k++)
                    {
                        sum_r += parts[k][i, j].Red;
                        sum_g += parts[k][i, j].Green;
                        sum_b += parts[k][i, j].Blue;
                    }
                    sum_r %= 256;
                    sum_g %= 256;
                    sum_b %= 256;

                    img[i, j] = new Bgr(sum_b, sum_g, sum_r);
                }
            }
            img.Save(input_filename + " decrypted.png");
            try
            {
                Console.WriteLine("\nOdszyfrowywanie zakonczone.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static string Read_file_txt(string filename)
        {
            try
            {
                StreamReader sr = new StreamReader(filename);
                string source = sr.ReadToEnd();
                sr.Close();
                return source;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }
        public static string[] Parse_message_txt(string message)
        {
            var rows = message.Split('\n');
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = rows[i].Trim(new_line);
            }
            return rows;
        }
    }
}
