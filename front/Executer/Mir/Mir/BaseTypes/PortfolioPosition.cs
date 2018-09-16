using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Mir.Enumerations;
using System.Collections;

namespace Core.Mir.BaseTypes
{
    public abstract class PortfolioPosition : IEqualityComparer, IEqualityComparer<PortfolioPosition>
    {
        private string _ident;
        private FinType _finType;
        private decimal _amount;

        public PortfolioPosition(string ident,
            FinType finType,
            decimal amount = 0.0m)
        {
            _ident = ident;
            _finType = finType;
            _amount = amount;
        }

        public bool Equals(object x, object y)
        {
            PortfolioPosition first = (PortfolioPosition)x;
            PortfolioPosition second = (PortfolioPosition)y;
            if (first._ident == second._ident &&
                first._finType == second._finType)
                return true;
            return false;
        }

        public int GetHashCode(object obj)
        {
            PortfolioPosition position = (PortfolioPosition)obj;
            return position._ident.GetHashCode() ^ position._finType.GetHashCode();
        }

        public string Ident { get { return _ident; } set { _ident = value; } }
        public FinType FinType { get { return _finType; } set { _finType = value; } }
        public decimal Amount { get { return _amount; } set { _amount = value; } }

        public bool Equals(PortfolioPosition other)
        {
            if (this.Ident == other.Ident &&
                this.FinType == other.FinType)
                return true;
            return false;
        }

        public bool Equals(PortfolioPosition x, PortfolioPosition y)
        {
            if (x._ident == y._ident &&
                    x._finType == y._finType)
                return true;
            return false;
        }

        public int GetHashCode(PortfolioPosition obj)
        {
            return obj._ident.GetHashCode() ^ obj._finType.GetHashCode();
        }
    }

    public class BalancePosition : PortfolioPosition, IEqualityComparer, IEquatable<BalancePosition>
    {
        public BalancePosition(string ident,
            FinType finType,
            decimal amount = decimal.Zero)
            :base(ident, finType, amount)
        {

        }

        public bool Equals(BalancePosition other)
        {
            return base.Equals(other);
        }
    }

    public class PortfolioPositionCompare :IEqualityComparer<PortfolioPosition>
    {

        public bool Equals(PortfolioPosition x, PortfolioPosition y)
        {
            if (x.Ident == y.Ident &&
                    x.FinType == y.FinType)
                return true;
            return false;
        }

        public int GetHashCode(PortfolioPosition obj)
        {
            return obj.Ident.GetHashCode() ^ obj.FinType.GetHashCode();
        }
    }
}
