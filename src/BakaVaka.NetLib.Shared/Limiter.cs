namespace BakaVaka.NetLib.Shared;
/// <summary>
/// Полностью копируем идею с лимитами от майков)))
/// </summary>
public abstract class Quota {
    public abstract bool TryPeakOne();
    public abstract void ReleaseResource();
    public static Quota NoLimit() => new Unlimited();
    public static Quota Limit(Int64 quota) => new Limited(quota);
    private sealed class Unlimited : Quota {
        public override void ReleaseResource() { }
        public override Boolean TryPeakOne() => true;
    }
    private sealed class Limited : Quota {
        private readonly Int64 _quota;
        private Int64 _count;
        public Limited(Int64 quota) {
            _quota = quota;
            _count = 0;
        }

        public override void ReleaseResource() => Interlocked.Decrement(ref _count);
        public override Boolean TryPeakOne() {
            var temp = _count;
            while( temp < _quota && temp != Int64.MaxValue ) {
                var current = Interlocked.CompareExchange(ref _count, temp+1, temp);
                if( current == temp ) {
                    return true;
                }
                temp = current;

            }
            return false;
        }
    }
}
