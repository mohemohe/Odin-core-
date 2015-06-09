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
        }

        #region FilePath変更通知プロパティ

        private string _FilePath;

        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                if (_FilePath == value)
                    return;
                _FilePath = value;
                RaisePropertyChanged();
            }
        }

        #endregion FilePath変更通知プロパティ

        #region Text変更通知プロパティ

        private string _Text;

        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text == value)
                    return;
                _Text = value;
                RaisePropertyChanged();
            }
        }

        #endregion Text変更通知プロパティ

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

        public void Open()
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image(*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files(*.*)|*.*";
            ofd.Title = "Open Image";
            ofd.RestoreDirectory = true;

            var result = ofd.ShowDialog();
            if (result != null && result.Value)
            {
                FilePath = ofd.FileName;
                Text = Core.Read(FilePath);
            }
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

        public void Write()
        {
            Core.Write(Text);
        }

        #endregion WriteCommand
    }
}