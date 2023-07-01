namespace BakaVaka.NetLib.Shared.Types;

public class OneOf<TLeft, TRight> : IOneOf<TLeft, TRight> {
    private TLeft? _left;
    private TRight? _right;
    private OneOf() {
        State = OneOfState.None;
    }
    public OneOfState State { get; private init; }
    public TResult Match<TResult>(IOneOf<TLeft, TRight>.MatchLeftDelegate<TResult> onLeft, IOneOf<TLeft, TRight>.MatchRightDelegate<TResult> onRight) => throw new NotImplementedException();
    public Task<TResult> MatchAsync<TResult>(IOneOf<TLeft, TRight>.MatchRightAsyncDelegate<TResult> onLeft, IOneOf<TLeft, TRight>.MatchRightAsyncDelegate<TResult> onRight) => throw new NotImplementedException();

    public static OneOf<TLeft, TRight> Right(TRight right) => new Right_(right);
    public static OneOf<TLeft, TRight> Left(TLeft left) => new Left_(left);


    private class Right_ : OneOf<TLeft, TRight> {
        public Right_(TRight right) {
            State = OneOfState.Right;
            _right = right;
        }
    }
    private class Left_ : OneOf<TLeft, TRight> {
        public Left_(TLeft left) {
            State = OneOfState.Left;
            _left = left;
        }
    }
}