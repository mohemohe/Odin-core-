using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Livet;
using Livet.Commands;
using Microsoft.Win32;
using Odin.Models;

namespace Odin.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        /* コマンド、プロパティの定義にはそれぞれ
         *
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
         *
         * を使用してください。
         *
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         *
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

        /* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

        /* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         *
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         *
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         *
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

        /* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         *
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */

        public void Initialize()
        {
            StatusBarProxy.StatusBarText = "ready";
        }

        #region FilePath変更通知プロパティ

        private string _FilePath;

        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                if (_FilePath == value)
                {
                    return;
                }
                _FilePath = value;
                RaisePropertyChanged();
            }
        }

        #endregion FilePath変更通知プロパティ

        #region BackgroundImage変更通知プロパティ
        private BitmapImage _BackgroundImage;

        public BitmapImage BackgroundImage
        {
            get
            { return _BackgroundImage; }
            set
            { 
                if (_BackgroundImage == value)
                    return;
                _BackgroundImage = value;
                RaisePropertyChanged();
            }
        }
        #endregion BackgroundImage変更通知プロパティ

        #region Text変更通知プロパティ

        private string _Text;

        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text == value)
                {
                    return;
                }
                _Text = value;
                RaisePropertyChanged();

                long length;
                if (value != null)
                {
                    var task = Models.Text.GetUTF8TextLength(value);
                    task.Wait();
                    length = (task.Result/8);
                }
                else
                {
                    length = 0;
                }
                TextLength = length.ToString();
                if (Core._image != null)
                {
                    IsEnableButton = length <= (Core._image.Width * Core._image.Height) / 8;
                }
            }
        }

        #endregion Text変更通知プロパティ

        #region TextLength変更通知プロパティ
        private string _TextLength = "";

        public string TextLength
        {
            get
            { return _TextLength; }
            set
            {
                //if (_TextLength == value)
                //{
                //    return;
                //}
                if (Core._image != null)
                {
                    _TextLength = value + " / " + (Core._image.Width*Core._image.Height)/8 + "  byte";
                }
                RaisePropertyChanged();
            }
        }
        #endregion TextLength変更通知プロパティ

        #region IsEnableButton変更通知プロパティ
        private bool _IsEnableButton = true;

        public bool IsEnableButton
        {
            get
            { return _IsEnableButton; }
            set
            { 
                if (_IsEnableButton == value)
                    return;
                _IsEnableButton = value;
                RaisePropertyChanged();
            }
        }
        #endregion IsEnableButton変更通知プロパティ

        #region OpenCommand

        private ViewModelCommand _OpenCommand;

        public ViewModelCommand OpenCommand
        {
            get
            {
                if (_OpenCommand == null)
                {
                    _OpenCommand = new ViewModelCommand(Open);
                }
                return _OpenCommand;
            }
        }

        public async void Open()
        {
            IsEnableButton = false;

            //TODO: フルMVVMではないので書き直すべき
            var ofd = new OpenFileDialog
            {
                Filter = "Image(*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files(*.*)|*.*",
                Title = "Open Image",
                RestoreDirectory = true
            };

            var result = ofd.ShowDialog();
            if (result != null && result.Value)
            {
                Text = null;

                FilePath = ofd.FileName;
                BackgroundImage = new BitmapImage(new Uri(FilePath));

                StatusBarProxy.StatusBarText = "loading...";
                Text = await Core.Read(FilePath);
                StatusBarProxy.StatusBarText = "loaded: " + Path.GetFileName(FilePath);
            }

            IsEnableButton = true;
        }

        #endregion OpenCommand

        #region WriteCommand

        private ViewModelCommand _WriteCommand;

        public ViewModelCommand WriteCommand
        {
            get
            {
                if (_WriteCommand == null)
                {
                    _WriteCommand = new ViewModelCommand(Write);
                }
                return _WriteCommand;
            }
        }

        public async void Write()
        {
            IsEnableButton = false;

            StatusBarProxy.StatusBarText = "writing...";
            var fileName = await Core.Write(Text);
            if (!String.IsNullOrEmpty(fileName))
            {
                StatusBarProxy.StatusBarText = "wrote! : " + fileName;
            }
            else
            {
                StatusBarProxy.StatusBarText = "error";
            }

            IsEnableButton = true;
        }

        #endregion WriteCommand
    }
}