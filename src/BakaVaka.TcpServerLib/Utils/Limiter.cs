namespace BakaVaka.TcpServerLib.Utils;
public abstract class Limiter {
    public abstract bool TryEnter();
    public abstract void Leave();

    public static Limiter SetUnilmit() => new UnlimitQuota();
    public static Limiter SetLimited(long quota) {
        return quota < 0 ? throw new ArgumentException("Quota should be positiva value") : (Limiter)new LimitedQuota(quota);
    }

    // в случае если мы не хотим лимитировать что-то
    private class UnlimitQuota : Limiter {
        public override void Leave() { }
        public override bool TryEnter() {
            return true;
        }
    }
    private class LimitedQuota : Limiter {
        private long _quota;
        private long _current;
        public LimitedQuota(long quota) {
            _quota = quota;
        }

        public override void Leave() => Interlocked.Decrement(ref _current);

        public override bool TryEnter() {
            var temp = _current;
            // пытаемся в цикле увеличивать значение
            while( temp < _quota && temp != long.MaxValue ) {
                var current = Interlocked.CompareExchange(ref _current, temp+1, temp);
                // другой поток не делал своих грязных дел, так что можно переставать увеличивать значение
                if( temp == current ) {
                    return true;
                }
                // другой поток уже увеличил текущее значение (
                temp = current;
            }
            return false;
        }
    }
}
