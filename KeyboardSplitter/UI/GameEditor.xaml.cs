using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KeyboardSplitter.UI
{
    /// <summary>
    /// Interaction logic for GameEditor.xaml
    /// </summary>
    public partial class GameEditor : CustomWindow
    {
        public GameEditor()
        {
            this.InitializeComponent();
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            var a = 1;
            base.OnLoaded(sender, e);
            var b = 2;
        }
    }
}
