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
        ObsNotify_Model_Pai CPG1_List = new ObsNotify_Model_Pai();
        ObsNotify_Model_Pai CPG2_List = new ObsNotify_Model_Pai();
        ObsNotify_Model_Pai CPG3_List = new ObsNotify_Model_Pai();
        ObsNotify_Model_Pai CPG4_List = new ObsNotify_Model_Pai();
        ObsNotify_Model_Pai Chupai_list = new ObsNotify_Model_Pai();

        public string strZuowei = string.Empty;
        private int iZuowei = 0;
        public PlayerUI()
        {
            InitializeComponent();
            Shoupai_ListView.ItemsSource = Shoupai_List.Members;
            Huase_ListView.ItemsSource = Huase_list.Members;
            CPG1_ListView.ItemsSource = CPG1_List.Members;
            CPG2_ListView.ItemsSource = CPG2_List.Members;
            CPG3_ListView.ItemsSource = CPG3_List.Members;
            CPG4_ListView.ItemsSource = CPG4_List.Members;
            Chupai_ListView.ItemsSource = Chupai_list.Members;
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
                Model_Pai pai = new Model_Pai();
                pai.index = i;
                pai.Pai = ShoupaiInfos[i];
                if (i == CmdNo.Shoupai_Last)
                    pai.margin = new Thickness(25, 0, -5, 0);
                
                Shoupai_List.Members.Add(pai);
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

            #region 指令判断
            string CommandInfo = ServerInfos[CmdNo.ServerCommand];
            CommandInfo = CommandInfo.Replace("【等待指令】", "");
            string[] CommandInfos = CommandInfo.Split(',');
            if (CommandInfos.Length == 3) isCPGZ = false; //指令长度等于3，杠胡出阶段
            else isCPGZ = true; //指令长度大于3，吃碰杠抓阶段

            //显示对应的指令按钮
            int iCmd = isCPGZ ? 2 * iZuowei + 2 : 2;
            bool isMyTurn = CommandInfos[iCmd-1] == strZuowei;
            string cmd = isMyTurn ? CommandInfos[0] : "空";
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => EnableCommandButton(cmd, CommandInfos[iCmd])));
            if (CommandInfos[iCmd] == "过")
                Global.isFinished[iZuowei] = true;

            if (!isMyTurn) //杠胡出阶段，非我的回合
            {
                Global.isFinished[iZuowei] = true;
            }
            #endregion

            #region 杠胡出阶段
            //摸牌
            int iShoupai = 0;
            switch (strZuowei) //确定当前座位
            {
                case "东": iShoupai = CmdNo.Shoupai_Dong; iZuowei = ZUOWEI.Dong; break;
                case "南": iShoupai = CmdNo.Shoupai_Nan; iZuowei = ZUOWEI.Nan; break;
                case "西": iShoupai = CmdNo.Shoupai_Xi; iZuowei = ZUOWEI.Xi; break;
                case "北": iShoupai = CmdNo.Shoupai_Bei; iZuowei = ZUOWEI.Bei; break;
                default: iShoupai = CmdNo.Shoupai_Dong; break;
            }
            string ShoupaiInfo = ServerInfos[iShoupai];
            UpdateShoupaiInfo(ShoupaiInfo);
            #endregion
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

        void UpdateShoupaiInfo(string ShoupaiInfo)
        {
            ShoupaiInfo = ShoupaiInfo.Replace(string.Format("【{0}家手牌】", strZuowei), "");
            ShoupaiInfo = ShoupaiInfo.Replace("【吃碰杠】", "");
            ShoupaiInfo = ShoupaiInfo.Replace("【花】", "");
            ShoupaiInfo = ShoupaiInfo.Replace("【庄】", "");
            ShoupaiInfo = ShoupaiInfo.Replace("【出牌】", "");
            string[] ShoupaiInfos = ShoupaiInfo.Split(','); //手牌信息

            #region 手牌信息 屏蔽方法1
            //int iLength_LocalShoupai = Shoupai_List.Members.Count; //当前UI内的麻将牌数量
            //int iLength_ServerShoupai = 0; //服务器数据内的麻将牌数量，剔除掉99
            //for (int i = CmdNo.Shoupai_First; i <= CmdNo.Shoupai_Last; i++)
            //{
            //    if (ShoupaiInfos[i] != "99")
            //        iLength_ServerShoupai++;
            //}
            //int iLength_Diff = iLength_ServerShoupai - iLength_LocalShoupai; //麻将牌差数
            //if (iLength_Diff <= 1) //等于0则更新手牌，等于1则需要更新摸牌
            //{
            //    foreach (Model_Pai pai in Shoupai_List.Members) //更新手牌
            //    {
            //        int i = pai.index;
            //        if (pai.Pai != ShoupaiInfos[CmdNo.Shoupai_First + i]) //如果UI手牌不等于服务器手牌
            //        {
            //            int iPai = Convert.ToInt32(pai.Pai);
            //            int iShoupaiInfo = Convert.ToInt32(ShoupaiInfos[CmdNo.Shoupai_First + i]);
            //            if ((iPai > 50) && (iPai < 60)) //判断是否 UI手牌为花，是则替换为服务器手牌
            //            {
            //               pai.Pai = ShoupaiInfos[CmdNo.Shoupai_First + i];
            //            }
            //            else if (iShoupaiInfo == 99)
            //            {

            //            } //其他情况，说明程序有问题
            //            else
            //            {

            //            }
            //        }
            //    }

            //    if (iLength_Diff == 1) //更新摸牌信息
            //    {
            //        Model_Pai pai = new Model_Pai();
            //        pai.Pai = ShoupaiInfos[CmdNo.Shoupai_First + iLength_ServerShoupai - 1];
            //        pai.index = iLength_ServerShoupai - 1;
            //        pai.margin = new Thickness(25, 0, -5, 0);
            //        Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => Shoupai_List.Members.Add(pai)));
            //    }

            //}
            //else //其他则说明UI与服务器不匹配，发生了吃碰杠的行为，则更新手牌
            //{
            //    foreach (Model_Pai pai in Shoupai_List.Members) //更新手牌
            //    {
            //        int i = pai.index;
            //        if (pai.Pai != ShoupaiInfos[CmdNo.Shoupai_First + i]) //如果UI手牌不等于服务器手牌
            //        {
            //            int iPai = Convert.ToInt32(pai.Pai);
            //            int iShoupaiInfo = Convert.ToInt32(ShoupaiInfos[CmdNo.Shoupai_First + i]);
            //            if ((iPai > 50) && (iPai < 60)) //判断是否 UI手牌为花，是则替换为服务器手牌
            //            {
            //                pai.Pai = ShoupaiInfos[CmdNo.Shoupai_First + i];
            //            }
            //            else if (iShoupaiInfo == 99)
            //            {
            //                pai.Pai = ShoupaiInfos[CmdNo.Shoupai_First + i];
            //            } //其他情况，说明程序有问题
            //            else
            //            {

            //            }
            //        }
            //    }
            //}
            #endregion

            #region 手牌信息
            foreach (Model_Pai pai in Shoupai_List.Members) //更新手牌
            {
                int i = pai.index;
                if (pai.Pai != ShoupaiInfos[CmdNo.Shoupai_First + i]) //如果UI手牌不等于服务器手牌
                {
                    pai.Pai = ShoupaiInfos[CmdNo.Shoupai_First + i];
                }
            }
            #endregion

            #region 吃碰杠信息
            string[] CPG1Info = new string[4];
            for (int i = CmdNo.CPG1_First; i <= CmdNo.CPG1_Last; i++)
                CPG1Info[i - CmdNo.CPG1_First] = ShoupaiInfos[i];
            UpdateListInfo(CPG1Info, CPG1_List);

            string[] CPG2Info = new string[4];
            for (int i = CmdNo.CPG2_First; i <= CmdNo.CPG2_Last; i++)
                CPG2Info[i - CmdNo.CPG2_First] = ShoupaiInfos[i];
            UpdateListInfo(CPG2Info, CPG2_List);

            string[] CPG3Info = new string[4];
            for (int i = CmdNo.CPG3_First; i <= CmdNo.CPG3_Last; i++)
                CPG3Info[i - CmdNo.CPG3_First] = ShoupaiInfos[i];
            UpdateListInfo(CPG3Info, CPG3_List);

            string[] CPG4Info = new string[4];
            for (int i = CmdNo.CPG4_First; i <= CmdNo.CPG4_Last; i++)
                CPG4Info[i - CmdNo.CPG4_First] = ShoupaiInfos[i];
            UpdateListInfo(CPG4Info, CPG4_List);
            #endregion

            #region 花色信息
            string[] HuaseInfo = new string[8];
            for (int i = CmdNo.Hua_First; i <= CmdNo.Hua_Last; i++)
                HuaseInfo[i - CmdNo.Hua_First] = ShoupaiInfos[i];
            UpdateListInfo(HuaseInfo, Huase_list);
            #endregion

            #region 出牌信息
            int iLength_Chupai = ShoupaiInfos.Length - CmdNo.Chupai_First;
            string[] ChupaiInfo = new string[iLength_Chupai];
            for (int i = CmdNo.Chupai_First; i < CmdNo.Chupai_First + iLength_Chupai; i++)
                ChupaiInfo[i - CmdNo.Chupai_First] = ShoupaiInfos[i];
            UpdateListInfo(ChupaiInfo, Chupai_list);
            #endregion
        }

        void UpdateListInfo(string[] infos, ObsNotify_Model_Pai list)
        {
            int iLength_infos = 0;
            int iLength_List = list.Members.Count;
            for (int i = 0; i < infos.Length; i++) //非99的麻将牌数量
            {
                if ((infos[i] != "99") && (infos[i].Length == 2))
                    iLength_infos++;
            }
            if (iLength_infos == 0) return; //数量为0，则返回
            if (iLength_infos == iLength_List) return; //数量与UI相同，则返回

            int iLength_Diff = iLength_infos - iLength_List;
            if (iLength_Diff < 0)
            {
                iLength_Diff = iLength_infos;
                iLength_List = 0;
            }

            for (int i = iLength_List; i < iLength_List + iLength_Diff; i++)
            {
                Model_Pai pai = new Model_Pai();
                pai.Pai = infos[i];
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => list.Members.Add(pai)));
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
            int index1 = 0;
            int index2 = 0;
            int index3 = 0;
            int index4 = 0;
            if (btnName == "btnHu")//胡
            {

            }
            else if(btnName == "btnZhua")//抓
            {

            }
            else if (btnName == "btnGang")//杠
            {
                bool isZiGang = btnZhua.Visibility == Visibility.Hidden; //是自杠还是抓杠
                if ((iItemsCount == 3) && (isZiGang == false))
                {
                    index1 = items[0].index;
                    index2 = items[1].index;
                    index3 = items[2].index;
                    clientCmd = string.Format("{0};杠;{1},{2},{3}", strZuowei, index1, index2, index3);
                    EnableCommandButton("空", "等");
                }
                else if ((iItemsCount == 4) && (isZiGang == true))
                {
                    index1 = items[0].index;
                    index2 = items[1].index;
                    index3 = items[2].index;
                    index4 = items[3].index;
                    clientCmd = string.Format("{0};杠;{1},{2},{3},{4}", strZuowei, index1, index2, index3, index4);
                    EnableCommandButton("空", "等");
                }
            }
            else if (btnName == "btnChi")//吃
            {
                if (iItemsCount != 2) return;
                index1 = items[0].index;
                index2 = items[1].index;
                clientCmd = string.Format("{0};吃;{1},{2}", strZuowei, index1, index2);
                EnableCommandButton("空", "等");
            }
            else if (btnName == "btnPeng")//碰
            {
                if (iItemsCount != 2) return;
                index1 = items[0].index;
                index2 = items[1].index;
                clientCmd = string.Format("{0};碰;{1},{2}", strZuowei, index1, index2);
                EnableCommandButton("空", "等");
            }
            else if (btnName == "btnChu")//出
            {
                if (iItemsCount != 1) return;
                index = items[0].index;
                clientCmd = string.Format("{0};出;{1}", strZuowei, index);
                items[0].Pai = "99";
            }
            else if (btnName == "btnGuo")//过
            {
                clientCmd = string.Format("{0};过;0", strZuowei);
                EnableCommandButton("空", "等");
            }

            if (string.IsNullOrEmpty(clientCmd) == false)
            {
                Global.Command[iZuowei] = clientCmd;
            }
        }

        void UpdateMembersIndex (int index)
        {
            foreach(Model_Pai pai in Shoupai_List.Members)
            {
                if (pai.index > index)
                    pai.index = pai.index - 1;
                pai.margin = new Thickness(-5, 0, -5, 0);
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

                if (_Pai == "99") this.width = 0;
                else this.width = 72;

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

        private double _width = 72;
        public double width
        {
            get 
            {
                return _width; 
            }
            set
            {
                _width = value;
                OnPropertyChanged("width");
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
