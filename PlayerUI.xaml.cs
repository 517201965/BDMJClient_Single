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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BDMJClient_Single
{
    /// <summary>
    /// Interaction logic for PlayerUI.xaml
    /// </summary>
    public partial class PlayerUI : UserControl
    {
        ObsNotify_Model_Pai Shoupai_List = new ObsNotify_Model_Pai();
        public PlayerUI()
        {
            InitializeComponent();
            Shoupai_ListView.ItemsSource = Shoupai_List.Members;

            Model_Pai pai1 = new Model_Pai();
            pai1.Pai = "05";
            Shoupai_List.Members.Add(pai1);
            Model_Pai pai2 = new Model_Pai();
            pai2.Pai = "06";
            Shoupai_List.Members.Add(pai2);
            Model_Pai pai3 = new Model_Pai();
            pai3.Pai = "07";
            Shoupai_List.Members.Add(pai3);
            Model_Pai pai4 = new Model_Pai();
            pai4.Pai = "09";
            Thickness m = new Thickness(-5, 0, -5, 0);
            m.Left = 20;
            pai4.margin = m;
            Shoupai_List.Members.Add(pai4);
        }
    }

    public class Model_Pai : INotifyPropertyChanged
    {
        private BitmapImage _PNG;
        public BitmapImage PNG
        {
            get { return _PNG; }
            set
            {
                _PNG = value;
                OnPropertyChanged("PNG");
            }
        }

        private string _Pai;
        public string Pai
        {
            get { return _Pai; }
            set
            {
                _Pai = value;
                string path = "D:\\JayShen\\20 - Work\\10 - Project\\05 - Majiang\\Software\\BDMJClient_Single\\image\\";
                path += value + ".png";
                this.PNG = Image_Common.ReadFromPath(path);
                OnPropertyChanged("Pai");
            }
        }

        private Thickness _margin = new Thickness(-5, 0, -5, 0);
        public Thickness margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                OnPropertyChanged("margin");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        //public void OnPropertyChanged(PropertyChangedEventArgs e)
        //{
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, e);
        //}
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class ObsNotify_Model_Pai : INotifyPropertyChanged
    {
        private ObservableCollection<Model_Pai> _Members = new ObservableCollection<Model_Pai>();
        public ObservableCollection<Model_Pai> Members
        {
            get
            {
                return this._Members;
            }
            set
            {
                if (this._Members != value)
                {
                    this._Members = value;
                    OnPropertyChanged("Members");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
