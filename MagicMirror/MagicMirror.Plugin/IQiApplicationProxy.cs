namespace MagicMirror.Plugin
{
    /// <summary><see cref="MagicMirror"/>で使用可能な通信エンドポイントを表します。</summary>
    public interface IQiApplicationProxy
    {
        //TODO: これSessionに比べて公開する意義があまり無いように思うので対応を検討して頂きたく。

        void Run();
        void Stop();

    }
}
