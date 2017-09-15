using KeyboardSplitter.Models;
using SplitterCore.Emulation;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyboardSplitter.Controls
{
    /// <summary>
    /// Interaction logic for GameList.xaml
    /// </summary>
    public partial class GameList : UserControl
    {
        public GameList()
        {
            this.InitializeComponent();
        }

        private void listView_Loaded(object sender, RoutedEventArgs e)
        {
            var splitter = Helpers.SplitterHelper.TryFindSplitter();
            if (splitter == null)
            {
                return;
            }


            List<GameData> gamesData = new List<GameData>();
            foreach (var slot in splitter.EmulationManager.Slots)
            {
                gamesData.Add(new GameData(null, slot.Keyboard.FriendlyName));
            }

            this.listView.ItemsSource = gamesData;
        }
    }
}
