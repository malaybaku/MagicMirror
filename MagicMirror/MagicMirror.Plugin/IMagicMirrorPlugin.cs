using System;
using System.Windows.Controls;

namespace MagicMirror.Plugin
{
    /// <summary>MagicMirrorに追加登録可能な機能を定義します。</summary>
    public interface IMagicMirrorPlugin : IDisposable
    {
        /// <summary>タブに表示する名前を取得します。</summary>
        string Name { get; }

        /// <summary>日本語による機能の要約文を取得します。</summary>
        string JapaneseDescription { get; }

        /// <summary>英語による機能の要約文を取得します。</summary>
        string EnglishDescription { get; }

        /// <summary>機能を表す一意なGuidを取得します。</summary>
        Guid Guid { get; } 

        /// <summary>
        /// GUIとしてタブに表示する要素を表します。
        /// [NOTE]見た目の統一感の都合があるのでなるべくMetroRadianceベースで作って下しあ
        /// </summary>
        UserControl GuiContent { get; }

        /// <summary>常駐型プラグインの場合trueにすることで、初期化の直後に有効化処理が行われます。</summary>
        bool EnableAfterInitialize { get; }

        /// <summary>アプリケーション開始時に初期化を行います。</summary>
        /// <param name="connectionService">セッションの取得元です。</param>
        void Initialize(IQiConnectionServiceProxy connectionService);

        /// <summary>プラグインを有効化します。</summary>
        void Enable();

        /// <summary>更新処理です。アプリケーション側から60fps以下で可能な限り頻繁に呼び出されます。</summary>
        void Update();

        /// <summary>プラグインを無効化します。</summary>
        void Disable();


    }
}
