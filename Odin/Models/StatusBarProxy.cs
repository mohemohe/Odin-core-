using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace Odin.Models
{
    public delegate void UpdateStatusBarText(string text);

    public static class StatusBarProxy
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        public static event UpdateStatusBarText StatusBarTextOnUpdate;

        private static string _StatusBarText;
        public static string StatusBarText
        {
            get
            {
                return _StatusBarText;
            }

            set
            {
                _StatusBarText = value;

                if (StatusBarTextOnUpdate != null)
                {
                    StatusBarTextOnUpdate(value);
                }
            }
        }
    }
}
