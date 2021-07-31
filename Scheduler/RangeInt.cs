using System;

namespace Scheduler
{
    public readonly struct RangeInt
    {
        public short CurrentIndex { get; init; }
        public short Lower { get; init; }
        public short Upper { get; init; }

        public RangeInt(short lowerBound, short upperBound, Int16 current = 0)
        {
            Lower = lowerBound;
            Upper = upperBound;
            CurrentIndex = current;
        }

        public static implicit operator short(RangeInt value) => value.CurrentIndex;

        public static RangeInt operator ++(RangeInt r) => r.CurrentIndex < r.Upper ?
            new RangeInt(r.Lower, r.Upper, (short)(r.CurrentIndex + 1)) : r;
        public static RangeInt operator --(RangeInt r) => r.CurrentIndex > r.Lower ?
            new RangeInt(r.Lower, r.Upper, (short)(r.CurrentIndex - 1)) : r;

        public static bool operator ==(RangeInt left, RangeInt right) => left.Equals(right);

        public static bool operator !=(RangeInt left, RangeInt right) => !left.Equals(right);

        public readonly override bool Equals(object obj)
        {
            if (obj is RangeInt rangeInt)
            {
                return this.CurrentIndex == rangeInt.CurrentIndex;
            }
            else if (obj is int value)
            {
                return this.CurrentIndex == value;
            }

            return false;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
