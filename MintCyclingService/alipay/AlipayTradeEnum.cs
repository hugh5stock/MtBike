namespace MintCyclingService.alipay
{
    /// <summary>
    /// 交易状态值
    /// </summary>
    public enum AlipayTradeEnum
    {
        /// <summary>
        /// 0充值失败
        /// </summary>
        Recharge_Faile,

        /// <summary>
        /// 1充值成功
        /// </summary>
        TRADE_SUCCESS,

        /// <summary>
        /// 2交易创建，等待买家付款--WAIT_BUYER_PAY
        /// </summary>
        WAIT_BUYER_PAY,

        /// <summary>
        /// 3.未付款交易超时关闭，或支付完成后全额退款--TRADE_CLOSED；
        /// </summary>
        TRADE_CLOSED,

        /// <summary>
        /// 4.交易结束，不可退款--TRADE_FINISHED
        /// </summary>
        TRADE_FINISHED,
    }
}