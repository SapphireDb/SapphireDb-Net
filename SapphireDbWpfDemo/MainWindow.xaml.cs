using System;
using System.Collections.Generic;
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
using SapphireDb_Net;

namespace SapphireDbWpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SapphireDb _db;

        public MainWindow(SapphireDb db)
        {
            _db = db;
            InitializeComponent();

            List<Entry> data = new List<Entry>();
            ListDisplay.ItemsSource = data;

            db.Collection<Entry>("demo.entries").Values().Subscribe((values) =>
            {
                data.Clear();
                data.AddRange(values);
            });
        }
    }
}