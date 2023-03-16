using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Win32;
namespace WpfOsztalyzas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fajlNev = "naplo.txt";
        ObservableCollection<Osztalyzat> jegyek = new ObservableCollection<Osztalyzat>();

        public MainWindow()
        {
            InitializeComponent();
            OpenFileDialog ofd = new OpenFileDialog();

            if ((bool)ofd.ShowDialog()! && ofd.FileName.EndsWith(".csv"))
            {
                fajlNev = ofd.FileName;
            }
            using (StreamReader sr = new StreamReader(fajlNev))
            {
                while (!sr.EndOfStream)
                {
                    string[] currentSplit = sr.ReadLine()!.Split(";");
                    jegyek.Add(new Osztalyzat(currentSplit[0],
                        currentSplit[1],
                        currentSplit[2],
                        Convert.ToInt32(currentSplit[^1])));
                }
            }
            dgJegyek.ItemsSource = jegyek;
            FilePath_txt.Text = fajlNev;
            Grades_txt.Text = $"Jegyek száma: {jegyek.Count()}, Jegyek átlaga: {jegyek.Average(x => x.Jegy):.0}";
        }

        private void btnRogzit_Click(object sender, RoutedEventArgs e)
        {

            string[] splittedName = txtNev.Text.Split(" ");
            if (splittedName.Count() == 1)
            {
                MessageBox.Show("A névnek minimum 2 szóból kell állnia", "NameError", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Array.FindAll(splittedName, x => x.Count() < 3).ToArray().Count() > 0)
            {
                MessageBox.Show("A névnek szavanként minimum 3 karakterből kell állnia",
                    "ShortNameError",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            if (DateTime.Compare(datDatum.SelectedDate!.Value, DateTime.Now) > 0)
            {
                MessageBox.Show("Nem lehet jövőbeli dátum!",
                    "DateError",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }


            string csvSor = txtNev.Text+";"+datDatum.Text+";"+cboTantargy.Text+";"+sliJegy.Value;
            StreamWriter sw = new StreamWriter(fajlNev, append: true);
            sw.WriteLine(csvSor);
            sw.Close();
            jegyek.Add(new Osztalyzat(txtNev.Text,datDatum.Text,cboTantargy.Text,Convert.ToInt32(sliJegy.Value)));
            Grades_txt.Text = $"Jegyek száma: {jegyek.Count()}, Jegyek átlaga: {jegyek.Average(x => x.Jegy):.0}";

        }

        private void btnBetolt_Click(object sender, RoutedEventArgs e)
        {
            jegyek.Clear();
            StreamReader sr = new StreamReader(fajlNev);
            while (!sr.EndOfStream)
            {
                string[] mezok = sr.ReadLine()!.Split(";");
                Osztalyzat ujJegy = new Osztalyzat(mezok[0], mezok[1], mezok[2], int.Parse(mezok[3]));
                jegyek.Add(ujJegy);
            }
            sr.Close();

            dgJegyek.ItemsSource = jegyek;
            Grades_txt.Text = $"Jegyek száma: {jegyek.Count()}, Jegyek átlaga: {jegyek.Average(x => x.Jegy):.0}";
        }
        private void nevCsere(object? sender, RoutedEventArgs e)
        {
            foreach (Osztalyzat osztalyzat in jegyek)
            {
                osztalyzat.ForditottNev();
            }
            dgJegyek.Items.Refresh();
        }

        private void sliJegy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblJegy.Content = sliJegy.Value;
        }
    }
}

