using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace Racebaan_Scherm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            Data.Initialize();
            Data.newRace += onFinished;
            Data.NextRace();
            InitializeComponent();
        }

        public void DriversChanged(object o, DriversChangedEventArgs e)
        {
            this.Screen.Dispatcher.BeginInvoke(
                DispatcherPriority.Render,
                new Action(() =>
                {
                    this.Screen.Source = null;
                    this.Screen.Source = Visualize.drawTrack(Data.CurrentRace.Track);
                })
            );
        }

        public void onFinished(object o, EventArgs e)
        {
            make_images.clear();
            Data.CurrentRace.DriversChanged += DriversChanged;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

       




    }
}
