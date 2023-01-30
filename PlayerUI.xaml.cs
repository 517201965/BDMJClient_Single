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
using System.IO.Pipes;

namespace BDMJClient_Single
{
    /// <summary>
    /// Interaction logic for PlayerUI.xaml
    /// </summary>
    public partial class PlayerUI : UserControl
    {
        ObsNotify_Model_Pai Shoupai_List = new ObsNotify_Model_Pai();
        ObsNotify_Model_Pai Huase_list = new ObsNotify_Model_Pai();

        public string strZuowei = string.Empty;
        private int iZuowei = 0;
        public PlayerUI()
        {
            InitializeComponent();
            Shoupai_ListView.ItemsSource = Shoupai_List.Members;
            Huase_ListView.ItemsSource = Huase_list.Members;
        }

        public void InitialUI(string ServerInfo)
        {
            string[] ServerInfos = ServerInfo.Split(';');
            //string strZuowei = this.Zuowei.Text;

            //百搭
            string baidapath = string.Format("D:\\JayShen\\20 - Work\\10 - Project\\05 - Majiang\\Software\\BDMJClient_Single\\image\\{0}.png", ServerInfos[CmdNo.Baida].Replace("【百搭麻将】", ""));
            Baida.Source = Image_Common.ReadFromPath(baidapath);

            #region 手牌信息
            //手牌
            int iShoupai = 0;
            switch(strZuowei)
            {
                case "东": iShoupai = CmdNo.Shoupai_Dong; iZuowei = ZUOWEI.Dong; break;
                case "南": iShoupai = CmdNo.Shoupai_Nan; iZuowei = ZUOWEI.Nan; break;
                case "西": iShoupai = CmdNo.Shoupai_Xi; iZuowei = ZUOWEI.Xi; break;
                case "北": iShoupai = CmdNo.Shoupai_Bei; iZuowei = ZUOWEI.Bei; break;
                default: iShoupai = CmdNo.Shoupai_Dong; break;
            }
            string ShoupaiInfo = ServerInfos[iShoupai];
            ShoupaiInfo = ShoupaiInfo.Replace(string.Format("【{0}家手牌】", strZuowei), "");
            ShoupaiInfo = ShoupaiInfo.Replace("【吃碰杠】", "");
            ShoupaiInfo = ShoupaiInfo.Replace("【花】", "");
            ShoupaiInfo = ShoupaiInfo.Replace("【庄】", "");
            string[] ShoupaiInfos = ShoupaiInfo.Split(',');
            for (int i = CmdNo.Shoupai_First; i <= CmdNo.Shoupai_Last; i++)
            {
                if(ShoupaiInfos[i] != "99")
                {
                    Model_Pai pai = new Model_Pai();
                    pai.index = i;
                    pai.Pai = ShoupaiInfos[i];
                    if (i == CmdNo.Shoupai_Last)
                    {
                        pai.margin = new Thickness(25, 0, -5, 0);
                    }
                    Shoupai_List.Members.Add(pai);
                }

            }
            //花色
            for (int i = CmdNo.Hua_First; i <= CmdNo.Hua_Last; i++)
            {
                if (ShoupaiInfos[i] != "99")
                {
                    Model_Pai huase = new Model_Pai();
                    huase.Pai = ShoupaiInfos[i];
                    Huase_list.Members.Add(huase);
                }
            }
            //庄
            if (ShoupaiInfos[CmdNo.Zhuang] == "是")
            {
                Zhuang.Visibility = Visibility.Visible;
                Global.isFinished[iZuowei] = false;
            }
            else
            {
                Zhuang.Visibility = Visibility.Hidden;
                Global.isFinished[iZuowei] = true;
            }
            #endregion

            #region 指令
            string CommandInfo = ServerInfos[CmdNo.ServerCommand];
            CommandInfo = CommandInfo.Replace("【等待指令】","");
            string[] CommandInfos = CommandInfo.Split(',');
            if (CommandInfos.Length == 3)
            {
                if (CommandInfos[1] == strZuowei)
                {
                    EnableCommandButton(CommandInfos[0], CommandInfos[2]);
                }
                else
                {
                    EnableCommandButton("空", CommandInfos[2]);
                }
            }
            else
            {
                EnableCommandButton("空","等");
            }
            #endregion
        }

        public void UpdateUI(string ServerInfo)
        {
            string[] ServerInfos = ServerInfo.Split(';');
            bool isCPGZ = false;

            string CommandInfo = ServerInfos[CmdNo.ServerCommand];
            CommandInfo = CommandInfo.Replace("【等待指令】", "");
            string[] CommandInfos = CommandInfo.Split(',');
            if (CommandInfos.Length == 3) isCPGZ = false;
            else isCPGZ = true;

            int iCmd = isCPGZ ? 2 * iZuowei + 2 : 2;
            bool isMyTurn = CommandInfos[iCmd-1] == strZuowei;
            string cmd = isMyTurn ? CommandInfos[0] : "空";
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => EnableCommandButton(cmd, CommandInfos[iCmd])));
            if (CommandInfos[iCmd] == "过")
                Global.isFinished[iZuowei] = true;

            if (isCPGZ) return;
            if (!isMyTurn)
            {
                Global.isFinished[iZuowei] = true;
                return;
            }

            //摸牌
            int iShoupai = 0;
            switch (strZuowei)
            {
                case "东": iShoupai = CmdNo.Shoupai_Dong; iZuowei = ZUOWEI.Dong; break;
                case "南": iShoupai = CmdNo.Shoupai_Nan; iZuowei = ZUOWEI.Nan; break;
                case "西": iShoupai = CmdNo.Shoupai_Xi; iZuowei = ZUOWEI.Xi; break;
                case "北": iShoupai = CmdNo.Shoupai_Bei; iZuowei = ZUOWEI.Bei; break;
                default: iShoupai = CmdNo.Shoupai_Dong; break;
            }
            string ShoupaiInfo = ServerInfos[iShoupai];
            ShoupaiInfo = ShoupaiInfo.Replace(string.Format("【{0}家手牌】", strZuowei), "");
            ShoupaiInfo = ShoupaiInfo.Replace("【吃碰杠】", "");
            ShoupaiInfo = ShoupaiInfo.Replace("【花】", "");
            ShoupaiInfo = ShoupaiInfo.Replace("【庄】", "");
            ShoupaiInfo = ShoupaiInfo.Replace("【出牌】", "");
            string[] ShoupaiInfos = ShoupaiInfo.Split(',');
            foreach(Model_Pai pai in Shoupai_List.Members)
            {
                int i = pai.index;
                if(pai.Pai !=  ShoupaiInfos[CmdNo.Shoupai_First + i])
                {
                    int iPai = Convert.ToInt32(pai.Pai);
                    int iShoupaiInfo = Convert.ToInt32(ShoupaiInfos[CmdNo.Shoupai_First + i]);
                    if ((iPai > 50) && (iPai < 60)) 
                    {
                        Model_Pai huase = new Model_Pai();
                        huase.Pai = pai.Pai; 
                        Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => Huase_list.Members.Add(huase)));
                        pai.Pai = ShoupaiInfos[CmdNo.Shoupai_First + i];
                    }
                    else if(iShoupaiInfo == 99)
                    {

                    }
                    else
                    {

                    }
                }
            }
            int iLength_LocalShoupai = Shoupai_List.Members.Count;
            int iLength_ServerShoupai = 0;
            for (int i = CmdNo.Shoupai_First; i <= CmdNo.Shoupai_Last; i++)
            {
                if (ShoupaiInfos[i] != "99")
                {
                    iLength_ServerShoupai++;
                }
                else
                    break;
            }
            if ((iLength_ServerShoupai - iLength_LocalShoupai) == 1)
            {
                Model_Pai pai = new Model_Pai();
                pai.Pai = ShoupaiInfos[CmdNo.Shoupai_First + iLength_ServerShoupai - 1];
                pai.index = iLength_ServerShoupai - 1;
                pai.margin = new Thickness(25, 0, -5, 0);
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => Shoupai_List.Members.Add(pai)));
            }
            else
            {

            }
        }

        void EnableCommandButton(string cmd,string status)
        {
            if (status != "等")
            {
                btnHu.Visibility = Visibility.Hidden;
                btnZhua.Visibility = Visibility.Hidden;
                btnGang.Visibility = Visibility.Hidden;
                btnChi.Visibility = Visibility.Hidden;
                btnPeng.Visibility = Visibility.Hidden;
                btnChu.Visibility = Visibility.Hidden;
                btnGuo.Visibility = Visibility.Hidden;
            }
            else
            {
                btnHu.Visibility = cmd.Contains("胡") ? Visibility.Visible : Visibility.Hidden;
                btnZhua.Visibility = cmd.Contains("抓") ? Visibility.Visible : Visibility.Hidden;
                btnGang.Visibility = cmd.Contains("杠") ? Visibility.Visible : Visibility.Hidden;
                btnChi.Visibility = cmd.Contains("吃") ? Visibility.Visible : Visibility.Hidden;
                btnPeng.Visibility = cmd.Contains("碰") ? Visibility.Visible : Visibility.Hidden;
                btnChu.Visibility = cmd.Contains("出") ? Visibility.Visible : Visibility.Hidden;
                btnGuo.Visibility = cmd.Contains("过") ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void btnCommand_Click(object sender, RoutedEventArgs e)
        {
            string btnName = (sender as Button).Name;
            string clientCmd = string.Empty;
            int iItemsCount = Shoupai_ListView.SelectedItems.Count;
            if ((iItemsCount == 0) && (btnName != "btnGuo"))
                return;

            Model_Pai[] items = new Model_Pai[iItemsCount];
            for (int i = 0; i < iItemsCount; i++) 
            {
                items[i] = Shoupai_ListView.SelectedItems[i] as Model_Pai;
            }

            //string strZuowei = this.Zuowei.Text;
            int index = 0;
            if (btnName == "btnHu")//胡
            {

            }
            else if(btnName == "btnZhua")//抓
            {

            }
            else if (btnName == "btnGang")//杠
            {

            }
            else if (btnName == "btnChi")//吃
            {

            }
            else if (btnName == "btnPeng")//碰
            {

            }
            else if (btnName == "btnChu")//出
            {
                if (iItemsCount != 1) return;
                index = items[0].index;
                clientCmd = string.Format("{0};出;{1}", strZuowei, index);
                Shoupai_List.Members.Remove(items[0]);
                UpdateMembersIndex(index);
            }
            else if (btnName == "btnGuo")//过
            {
                clientCmd = string.Format("{0};过;0", strZuowei);
                EnableCommandButton("空", "等");
            }

            if (string.IsNullOrEmpty(clientCmd) == false)
            {
                //streamClient.WriteString(clientCmd);
                Global.Command[iZuowei] = clientCmd;
            }
        }

        void UpdateMembersIndex (int index)
        {
            foreach(Model_Pai pai in Shoupai_List.Members)
            {
                if (pai.index > index)
                    pai.index = pai.index - 1;
            }
        }
    }

    public class Model_Pai : INotifyPropertyChanged
    {
        public int index = 0;
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

    public static class CmdNo
    {
        public static int PaikuSum = 0;
        public static int PaikuInfo = 1;
        public static int TouziResult = 2;
        public static int Baida = 3;
        public static int LastChuPai = 4;
        public static int Shoupai_Dong = 5;
        public static int Shoupai_Nan = 6;
        public static int Shoupai_Xi = 7;
        public static int Shoupai_Bei = 8;
        public static int ServerCommand = 9;

        public static int Shoupai_First = 0;
        public static int Shoupai_Last = 13;
        public static int CPG1_First = 14;
        public static int CPG1_Last = 17;
        public static int CPG2_First = 18;
        public static int CPG2_Last = 21;
        public static int CPG3_First = 22;
        public static int CPG3_Last = 25;
        public static int CPG4_First = 26;
        public static int CPG4_Last = 29;
        public static int Hua_First = 30;
        public static int Hua_Last = 37;
        public static int Zhuang = 38;
        public static int Chupai_First = 39;
    }
}
