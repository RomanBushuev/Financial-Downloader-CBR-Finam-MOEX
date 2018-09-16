using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Mir.Enumerations
{
    public enum FinType
    {
        /// <summary>
        /// Неопределенный инструмент
        /// </summary>
        Default = 1 << 0,
        /// <summary>
        /// Акция
        /// </summary>
        Equity = 1 << 1,
        /// <summary>
        /// Облигация
        /// </summary>
        Bond = 1 << 2,
        /// <summary>
        /// Фонд
        /// </summary>
        Fund = 1 << 3,
        /// <summary>
        /// Сертификат
        /// </summary>
        Certificate = 1 << 4,
        /// <summary>
        /// Депозитарная расписка
        /// </summary>
        DepositaryReceipt = 1 << 5,
        /// <summary>
        /// Процентная кривая
        /// </summary>
        PercentCurve = 1 << 6,
        /// <summary>
        /// FX_RATE
        /// </summary>
        FxRate = 1 <<7,
    }
}
