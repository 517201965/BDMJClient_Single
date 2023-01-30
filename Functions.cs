using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDMJClient_Single
{
    class Functions
    {
    }
    public static class ZUOWEI
    {
        static public int Kong = 4;
        static public int Dong = 0;
        static public int Nan = 1;
        static public int Xi = 2;
        static public int Bei = 3;
        static public string[] FengXiang = new string[5] { "东", "南", "西", "北", "空" };
    }

    public static class Global
    {
        static public string[] Command = new string[4] { string.Empty, string.Empty, string.Empty, string.Empty };
        static public bool[] isFinished = new bool[4] { false, false, false, false };
    }
}
