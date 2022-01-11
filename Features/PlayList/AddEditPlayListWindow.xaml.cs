using MusikPlayer.ViewModel;
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
using System.Windows.Shapes;

namespace MusikPlayer.Features.PlayList
{
    /// <summary>
    /// Interaktionslogik für AddEditPlayListWindow.xaml
    /// </summary>
    public partial class AddEditPlayListWindow : Window
    {
        public AddEditPlayListWindow(bool isEdit = false, PlayListListItem playListListItem = null)
        {
            this.DataContext = new PlayListViewModel(this, isEdit, playListListItem);
            InitializeComponent();
        }
    }
}
