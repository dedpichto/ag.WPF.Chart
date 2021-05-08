using ag.WPF.Chart.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Examples.UserControls
{
    /// <summary>
    /// Interaction logic for HLCStyle.xaml
    /// </summary>
    public partial class HLCStyle : UserControl
    {
        public ObservableCollection<ISeries> Series { get; } = new ObservableCollection<ISeries>();
        public List<string> CustomXTexts { get; } = new List<string>();

        public HLCStyle()
        {
            InitializeComponent();
        }

        public void SaveAsImage(string imageFilePath)
        {
            AgChart.SaveAsImage(imageFilePath);
        }
    }
}
