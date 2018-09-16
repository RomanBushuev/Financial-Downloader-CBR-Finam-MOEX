using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Core.Mir.Enumerations
{
    [Flags]
    public enum FinTypeDetailedLevel
    {
        [Description("Неклассифицированные")]
        Default = 1 << 0,
        /// <summary>
        /// Акция обыкновенная
        /// </summary>
        [Description("Акция обыкновенная")]
        OrdinaryStock = 1 << 1,
        /// <summary>
        /// Корпоративная облигация
        /// </summary>
        [Description("Корпоративная облигация")]
        CorporateBond = 1 << 2,
        /// <summary>
        /// ETF
        /// </summary>
        [Description("ETF")]
        ETF = 1 << 3,
        /// <summary>
        /// Акция привилегированная
        /// </summary>
        [Description("Акция привилегированная")]
        PreferredStock = 1 << 4,
        /// <summary>
        /// ОФЗ
        /// </summary>
        [Description("ОФЗ")]
        FederalLoanBond = 1 << 5,
        /// <summary>
        /// Пай закрытого ПИФа
        /// </summary>
        [Description("Пай закрытого ПИФа")]
        PieOfClosedMutualFund = 1 << 6,
        /// <summary>
        /// Пай интервального ПИФа
        /// </summary>
        [Description("Пай интервального ПИФа")]
        PieOfIntervalMutualFund = 1 << 7,
        /// <summary>
        /// Региональная облигация
        /// </summary>
        [Description("Региональная облигация")]
        RegionalBond = 1 << 8,
        /// <summary>
        /// Пай открытого ПИФа
        /// </summary>
        [Description("Пай открытого ПИФа")]
        PieOfOpenedMutualFund = 1 << 9,
        /// <summary>
        /// Облигация МФО
        /// </summary>
        [Description("Облигация МФО")]
        MfoBond = 1 << 10,
        /// <summary>
        /// Биржевая облигация
        /// </summary>
        [Description("Биржевая облигация")]
        ExchangeTradedBond = 1 << 11,
        /// <summary>
        /// Ипотечный сертификат
        /// </summary>
        [Description("Ипотечные сертификаты")]
        MortgageCertificate = 1 << 12,
        /// <summary>
        /// Муниципальная облигация
        /// </summary>
        [Description("Муниципальная облигация")]
        MunicipalBond = 1 << 13,
        /// <summary>
        /// Клиринговый сертификат участия
        /// </summary>
        [Description("Клиринговый сертификат участия")]
        ClearingParticipationCertificate = 1 << 14,
        /// <summary>
        /// Депозитарная расписка
        /// </summary>
        [Description("Депозитарная расписка")]
        DepositaryReceipt = 1 << 15
    }
}
