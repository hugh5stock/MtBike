using MintCyclingService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.BicycleHandle
{
    /// <summary>
    /// 单车入库输入参数模型
    /// </summary>
    public class AddBicycleBase_PM
    {
        /// <summary>
        /// 维修人员Guid
        /// </summary>
        public Guid ServicePersonID { get; set; }

        /// <summary>
        /// 自行车编号
        /// </summary>

        public string BicycleNumber { get; set; }

         /// <summary>
         /// 0非助力模式，1助力模式
         /// </summary>
        public int BicycleType { get; set; }
 
    }

    /// <summary>
    /// 车锁入库或检测绑定输入参数 模型
    /// </summary>
    public class ComponentsBinding: MatchHelp_PM
    {
        /// <summary>
        /// 车辆编号
        /// </summary>
        public string BikeNumber { get; set; }

        /// <summary>
        /// 车辆类型
        /// </summary>
        public int BicycleType { get; set; }



        public Guid ServicePersonGuid { get; set; }
    }

    /// <summary>
    ///控制器和电池组硬件锁上传信息 输入参数模型
    /// </summary>
    public class MatchHelp_PM
    {
        //设备编号
        public string LockNumber { get; set; }

        //控制器序列号
        public string ControllerNumber { get; set; }

        //BMS保护板序列号
        public string BatteryNumber { get; set; }


        #region 控制器输入参数
      
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


        #endregion

        #region BMS输入参数
        //1– BMS信息
        public int Bms_Type { get; set; }

        //接收到控制器信息时锁产生的时间, 如:2017-3-31 22:07:43
        //public DateTime Bms_Date { get; set; }

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


        #endregion

    }


    public class Detecting
    {
        /// <summary>
        /// 车辆编号
        /// </summary>

        public string BicycleNumber { get; set; }
        /// <summary>
        /// 锁编号
        /// </summary>
        public string LockNumber { get; set; }

        /// <summary>
        /// 车架状态
        /// </summary>
        public int BicycleFrame { get; set; }
        /// <summary>
        /// 前叉状态
        /// </summary>
        public int FrontFork { get; set; }
        /// <summary>
        /// 车铃状态
        /// </summary>
        public  int BellStatus { get; set; }
        /// <summary>
        /// 刹车状态
        /// </summary>
        public int Brake { get; set; }

        /// <summary>
        /// 车把状态
        /// </summary>
        public int HandleBar { get; set; }
        /// <summary>
        /// 把套状态
        /// </summary>
        public int HandlebarGrip { get; set; }
        /// <summary>
        /// 内外胎状态
        /// </summary>
        public int Tires { get; set; }
        /// <summary>
        /// 车轮组状态
        /// </summary>
        public int Wheels { get; set; }
        /// <summary>
        /// 坐垫状态
        /// </summary>
        public int Cushion { get; set; }
        /// <summary>
        /// 泥巴状态
        /// </summary>
        public int Mud { get; set; }
        /// <summary>
        /// 脚撑状态
        /// </summary>
        public int KickStand { get; set; }
        /// <summary>
        /// 车筐状态
        /// </summary>
        public int Basket { get; set; }


        /// <summary>
        /// 控制盒状态
        /// </summary>
        public int ControlBox { get; set; }
        /// <summary>
        /// 尾灯状态
        /// </summary>
        public int TailLight { get; set; }
        /// <summary>
        /// 前灯状态
        /// </summary>
        public int HeadLight { get; set; }
        /// <summary>
        /// 二维码状态
        /// </summary>
        public int QRCode { get; set; }
        /// <summary>
        /// 车锁状态
        /// </summary>
        public int Lock { get; set; }
        /// <summary>
        /// 蓝牙开锁状态
        /// </summary>
        public int BluetoothStatus { get; set; }
        /// <summary>
        /// 二维码开锁状态
        /// </summary>
        public int QRCodeStatus { get; set; }
        /// <summary>
        /// 陀螺仪校准状态
        /// </summary>
        public int? GyroscopeStatus { get; set; }

        /// <summary>
        /// 控制器编号
        /// </summary>
        public string  ContorollerNumber { get; set; }

        /// <summary>
        /// 控制器状态
        /// </summary>
        public int? ContorollerStatus { get; set; }

        /// <summary>
        /// 控制器故障原因
        /// </summary>
        public string ContorollerExReason { get; set; }
        /// <summary>
        /// 用户guid
        /// </summary>
        public Guid UserGuid { get; set; }
  

    }

    public class Detec_PM:Paging_Model
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string BicycleNumber { get; set; }


    }
    public  class Detec_OM
    {
        /// <summary>
        /// 车编号
        /// </summary>

        public string BikeNumber { get; set; }
        /// <summary>
        /// 车辆类型
        /// </summary>
        public int  BikeType { get; set; }

        /// <summary>
        /// 锁编号
        /// </summary>
        public string LockNumber { get; set; }

        /// <summary>
        /// 车架状态
        /// </summary>
        public int? BicycleFrame { get; set; }
        /// <summary>
        /// 前叉状态
        /// </summary>
        public int? FrontFork { get; set; }
        /// <summary>
        /// 车铃状态
        /// </summary>
        public int? BellStatus { get; set; }
        /// <summary>
        /// 刹车状态
        /// </summary>
        public int? Brake { get; set; }

        /// <summary>
        /// 车把状态
        /// </summary>
        public int? HandleBar { get; set; }
        /// <summary>
        /// 把套状态
        /// </summary>
        public int? HandlebarGrip { get; set; }
        /// <summary>
        /// 内外胎状态
        /// </summary>
        public int? Tires { get; set; }
        /// <summary>
        /// 车轮组状态
        /// </summary>
        public int? Wheels { get; set; }
        /// <summary>
        /// 坐垫状态
        /// </summary>
        public int? Cushion { get; set; }
        /// <summary>
        /// 泥巴状态
        /// </summary>
        public int? Mud { get; set; }
        /// <summary>
        /// 脚撑状态
        /// </summary>
        public int? KickStand { get; set; }
        /// <summary>
        /// 车筐状态
        /// </summary>
        public int? Basket { get; set; }


        /// <summary>
        /// 控制盒状态
        /// </summary>
        public int? ControlBox { get; set; }
        /// <summary>
        /// 尾灯状态
        /// </summary>
        public int? TailLight { get; set; }
        /// <summary>
        /// 前灯
        /// </summary>
        public int? HeadLight { get; set; }
        /// <summary>
        /// 二维码状态
        /// </summary>
        public int? QRCode { get; set; }
        /// <summary>
        /// 车锁状态
        /// </summary>
        public int? Lock { get; set; }
        /// <summary>
        /// 控制器故障原因
        /// </summary>
        public string ContorollerExReason{ get; set; }
        /// <summary>
        /// 蓝牙状态
        /// </summary>
        public int? BluetoothStatus { get; set; }
        /// <summary>
        /// 二维码状态
        /// </summary>
        public int? QRCodeStatus { get; set; }
        /// <summary>
        /// 陀螺仪状态
        /// </summary>
        public int? GyroscopeStatus { get; set; }
        /// <summary>
        /// 控制器编码
        /// </summary>
        public string  ContorollerNumber { get; set; }
        /// <summary>
        /// 控制器状态
        /// </summary>
        public int? ContorollerStatus { get; set; }

        /// <summary>
        /// 人工检测状态
        /// </summary>
        public bool HumanCheck { get; set; }
        /// <summary>
        /// 锁状态
        /// </summary>
        public bool LockStatus { get; set; }
        /// <summary>
        /// 检测时间
        /// </summary>
        public DateTime DelecTime { get; set; }
    }




}
