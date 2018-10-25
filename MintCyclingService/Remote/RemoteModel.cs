using MintCyclingService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Remote 
{
    /// <summary>
    /// 发送骑行模式控制指令
    /// </summary>
    public class HandleCyclingMode_PM
    {
        //锁的编号
        public string LockNumber { get; set; }

        // 运行模式: 0–普通模式;1–无助力模式[充电骑行模式];2–助力模式

        public string RunningMode { get; set; }
    }


    /// <summary>
    /// 发送查询控制器与BMS序列号
    /// </summary>
    public class SearchControllerBMS_PM
    {
        //锁的编号
        public string LockNumber { get; set; }

 
    }

    /// <summary>
    /// 接收查询控制器与BMS序列号
    /// </summary>
    public class ReceiveSearchControllerBMS_PM
    {
        //锁的编号
        public string LockNumber { get; set; }

        //应答码, 见应答码定义:0	    成功-1格式错误-2;设备不存在;-3加密数据验证失败

        public int ResCode { get; set; }

        
        //控制器序列号
        public string CtrlerSN { get; set; }
        //BMS 序列号

        public string BMSSN { get; set; }
        
    }


    /// <summary>
    /// 发送远程升级锁程序包暂时没用
    /// </summary>
    public class UpgradeBicLockProgram_PM
    {
        //锁的编号
        public string LockNumber { get; set; }

        //版本号
        public string Version { get; set; }

        //升级文件存储位置
        public string Url { get; set; }
    }

    //LockGuid: s.LockGuid, BicycleGuid: s.BicycleGuid, CustomerID: s.CustomerID
    /// <summary>
    /// 发送远程升级锁程序包
    /// </summary>
    public class RemoteUpgradeBicLockProgram_PM
    {
        //锁Guid
        public Guid LockGuid { get; set; }

        //车辆Guid
        public Guid BicycleGuid { get; set; }

        //客户编号
        public int CustomerID { get; set; }
  
    }

    /// <summary>
    /// 接受远程升级锁程序包完成后锁的应答
    /// </summary>
    public class ReceiveUpgradeFinish_PM
    {
        //锁的编号
        public string LockNumber { get; set; }

        //升级是否成功的状态，0表示升级失败，1表示升级成功
        public int Status { get; set; }

        //客户编号
        public int CustomerID { get; set; }

    }



    /// <summary>
    /// 控制器上报信息输入参数模型
    /// </summary>
    public class ControllerReport_PM
    {
        //锁的编号
        public string LockNumber { get; set; }

        //0–控制器状态
        public int Type { get; set; }

        //接收到控制器信息时锁产生的时间, 如:2017-3-31 22:07:43
        public DateTime Date { get; set; }
        //控制器计算的电池组电压, 单位: 0.1V
        public decimal Vol1 { get; set; }
        // 控制器计算的电池组电流, 单位: 0.1A
        public decimal Cur1 { get; set; }

        //当前整车速度, 单位: 0.1km/h
        public decimal Speed { get; set; }
        //踏频
        public decimal RPM { get; set; }

        //故障代码 0x00:无故障(0); 0x21:电流异常(33); 0x24:电机霍尔异常(36); 0x25:刹车故障(37); 0x26: 电池欠压(38)；0x27: I2C通讯故障(39)
        public int ErrCode { get; set; }

        //0: 无校准 1: 校准成功
        public int CalibrateStatus { get; set; }

        //1: 不受后台控制，即自行充放电运行模式
        //2: 受后台控制进行充放电切换运行模式
        public int RunningCtrl { get; set; }

        //0: 普通模式，既不助力也不充电
        //1: 助力模式
        //2: 欠压充电模式
        //3: 超速充电模式
        public int RunningStatus { get; set; }

        //本次骑行公里数，单位公里
        public decimal Kms { get; set; }

    }
 

    /// <summary>
    /// BMS上报信息输入参数模型
    /// </summary>
    public class BMSReport_PM
    {
        //锁的编号
        public string LockNumber { get; set; }

        //0–控制器状态
        public int Type { get; set; }

        //接收到控制器信息时锁产生的时间, 如:2017-3-31 22:07:43
        public DateTime Date { get; set; }

        //电池表面温度，单位为开尔文的温度数值，分辨率0.1K。
        public decimal Temp { get; set; }

        //电池剩余容量，单位: mAh
        public decimal Electricity { get; set; }


        //电池充放电次数
        public int ChargeDischargeCounts { get; set; }

        //历史上最大的充电间隔时间，单位:小时
        public decimal MaxChargeInterval { get; set; }

        //当前充电间隔, 单位: 小时
        public int CurChargeInterval { get; set; }

        //电池满充容量, 单位: mAh
        public decimal Capacity { get; set; }

        // 荷电态，数值范围 0~100
        public int Elec { get; set; }

        //BMS自身采集出的电池组电压, 单位: mV
        public decimal Vol2 { get; set; }


        //BMS自身采集出的电池组电流, 单位: mA
        public decimal Cur2 { get; set; }

    }



    /// <summary>
    ///控制器和电池组硬件锁上传信息输入参数模型
    /// </summary>
    public class HardWareHelp_PM
    {
        //锁的编号
        public string DeviceNo { get; set; }

        public int Type { get; set; }

        public DateTime Date { get; set; }

        public decimal Vol1 { get; set; }
        public decimal Cur1 { get; set; }

        public decimal Speed { get; set; }
        public decimal RPM { get; set; }

        public int ErrCode { get; set; }


        public int Status { get; set; }

        public decimal Temp { get; set; }

        public decimal Electricity { get; set; }

        public int ChargeDischargeCounts { get; set; }
        public decimal MaxChargeInterval { get; set; }

        public decimal CurChargeInterval { get; set; }

        public decimal Capacity { get; set; }

        public int Elec { get; set; }

        public decimal Vol2 { get; set; }
        public decimal Cur2 { get; set; }
    }


    /// <summary>
    ///锁自动上传信息    输入参数模型
    /// </summary>
    public class HardWareAuto_PM
    {
        //设备编号
        public string DeviceNo { get; set; }

        //经度
        public decimal? Longitude { get; set; }

        //纬度
        public decimal? Latitude { get; set; }

        //电压
        public decimal? Voltage { get; set; }


        /// <summary>
        /// 时间
        /// </summary>
        public string Timestamp { get; set; }



    }








}
