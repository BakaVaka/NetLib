namespace BakaVaka.NetLib.Shared.Types;
public enum OneOfState {
    None,
    Left,
    Right,
}

public interface IOneOf<TLeft, TRight> {
    public delegate TResult MatchLeftDelegate<TResult>(TLeft left);
    public delegate TResult MatchRightDelegate<TResult>(TRight right);
    public delegate Task<TResult> MatchLeftAsyncDelegate<TResult>(TLeft left, CancellationToken cancellationToken = default);
    public delegate Task<TResult> MatchRightAsyncDelegate<TResult>(TRight right, CancellationToken cancellationToken = default);
    public OneOfState State { get; }
    public TResult Match<TResult>(MatchLeftDelegate<TResult> onLeft, MatchRightDelegate<TResult>  onRight);
    public Task<TResult> MatchAsync <TResult>(MatchRightAsyncDelegate<TResult> onLeft, MatchRightAsyncDelegate<TResult> onRight);

}
