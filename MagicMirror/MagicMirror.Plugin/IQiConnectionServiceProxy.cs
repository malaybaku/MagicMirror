using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Baku.MagicMirror.Plugin
{
    public interface IQiConnectionServiceProxy : INotifyPropertyChanged
    {
        /// <summary>接続済みのセッション一覧を取得します。</summary>
        ReadOnlyObservableCollection<IQiSessionProxy> Sessions { get; }

        /// <summary>接続済みセッションから選択された複数のアクティブセッション一覧を取得します。</summary>
        ReadOnlyObservableCollection<IQiSessionProxy> SelectedSessions { get; }

        /// <summary>
        /// 接続済みセッションから選択された単一のアクティブセッション(ない場合はnull)を取得します。
        /// 変更されると<see cref="INotifyPropertyChanged.PropertyChanged"/>が発生します。
        /// </summary>
        IQiSessionProxy CurrentSession { get; }
    }

    /// <summary>アプリケーションから提供される外部への接続サービス提供元を定義します。</summary>
    public interface _IQiConnectionServiceProxy
    {
        /// <summary>接続済みセッションの一覧を取得します。</summary>
        IReadOnlyList<IQiSessionProxy> Sessions { get; }

        /// <summary>明示的に選択されている場合はカレントのセッションを取得します。</summary>
        IQiSessionProxy CurrentSession { get; }

        /// <summary>カレントセッションが変更されたときに発生します。</summary>
        event EventHandler<QiSessionChangedEventArgs> CurrentSessionChanged;

        /// <summary>セッションが切断されたときに発生します。</summary>
        event EventHandler<QiSessionEventArgs> SessionDisconnected;
    }

    public class QiSessionEventArgs : EventArgs
    {
        internal QiSessionEventArgs(IQiSessionProxy session)
        {
            Session = session;
        }

        public IQiSessionProxy Session { get; }

    }

    public class QiSessionChangedEventArgs : EventArgs
    {
        internal QiSessionChangedEventArgs(IQiSessionProxy currentSession, IQiSessionProxy previousSession)
        {
            Current = currentSession;
            Previous = previousSession;
        }

        public IQiSessionProxy Current { get; }

        public IQiSessionProxy Previous { get; }
    }

}
