﻿/******************************************************************************
 * SunnyUI 开源控件库、工具类库、扩展类库、多页面开发框架。
 * CopyRight (C) 2012-2025 ShenYongHua(沈永华).
 * QQ群：56829229 QQ：17612584 EMail：SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 * 如果您使用此代码，请保留此说明。
 ******************************************************************************
 * 文件名称: UIDatetimePicker.cs
 * 文件说明: 日期时间选择框
 * 当前版本: V3.1
 * 创建日期: 2020-01-01
 *
 * 2020-01-01: V2.2.0 增加文件说明
 * 2020-07-06: V2.2.6 重写下拉窗体，缩短创建时间
 * 2020-08-07: V2.2.7 可编辑输入，日期范围控制以防止出错
 * 2020-09-16: V2.2.7 更改滚轮选择时间的方向
 * 2021-04-15: V3.0.3 增加ShowToday显示今日属性
 * 2024-06-09: V3.6.6 下拉框可选放大倍数为2
 * 2024-07-13: V3.6.7 修改选择日期在下拉框中显示方式
 * 2024-08-28: V3.7.0 修复格式化字符串包含/时显示错误
 * 2024-11-10: V3.7.2 增加StyleDropDown属性，手动修改Style时设置此属性以修改下拉框主题
 * 2025-09-20: V3.8.8 添加MaxDate与MinDate属性
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultProperty("Value")]
    [DefaultEvent("ValueChanged")]
    [Description("日期时间选择框控件")]
    public sealed class UIDatetimePicker : UIDropControl, IToolTip
    {
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // UIDatetimePicker
            // 
            Name = "UIDatetimePicker";
            SymbolDropDown = 61555;
            SymbolNormal = 61555;
            ButtonClick += UIDatetimePicker_ButtonClick;
            ResumeLayout(false);
            PerformLayout();
        }

        [Browsable(false)]
        public override string[] FormTranslatorProperties => ["DateFormat"];

        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            item?.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// 需要额外设置ToolTip的控件
        /// </summary>
        /// <returns>控件</returns>
        public Control ExToolTipControl()
        {
            return edit;
        }

        [DefaultValue(false)]
        [Description("日期输入时，是否可空显示"), Category("SunnyUI")]
        public bool CanEmpty { get; set; }

        [DefaultValue(false)]
        [Description("日期输入时，显示今日按钮"), Category("SunnyUI")]
        public bool ShowToday { get; set; }

        public UIDatetimePicker()
        {
            InitializeComponent();
            Value = DateTime.Now;
            Text = Value.ToString(DateFormat);
            Width = 200;
            EditorLostFocus += UIDatePicker_LostFocus;
            TextChanged += UIDatePicker_TextChanged;
            MaxLength = 19;

            CreateInstance();
        }

        private void UIDatePicker_TextChanged(object sender, EventArgs e)
        {
            if (Text.Length == MaxLength && !DropSetted)
            {
                try
                {
                    DateTime dt = Text.ToDateTime(DateFormat);
                    if (Value != dt) Value = dt;
                }
                catch
                {
                    Value = DateTime.Now.Date;
                }
            }
        }

        private void UIDatePicker_LostFocus(object sender, EventArgs e)
        {
            if (Text.IsNullOrEmpty())
            {
                if (CanEmpty) return;
            }

            try
            {
                DateTime dt = Text.ToDateTime(DateFormat);
                if (Value != dt) Value = dt;
            }
            catch
            {
                Value = DateTime.Now.Date;
            }
        }

        public delegate void OnDateTimeChanged(object sender, DateTime value);


        public event OnDateTimeChanged ValueChanged;

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender">控件</param>
        /// <param name="value">值</param>
        protected override void ItemForm_ValueChanged(object sender, object value)
        {
            Value = (DateTime)value;
            Invalidate();
        }

        private readonly UIDateTimeItem item = new UIDateTimeItem();

        /// <summary>
        /// 创建对象
        /// </summary>
        protected override void CreateInstance()
        {
            ItemForm = new UIDropDown(item);
        }

        private bool DropSetted = false;
        [Description("选中日期时间"), Category("SunnyUI")]
        public DateTime Value
        {
            get => item.Date;
            set
            {
                if (value < new DateTime(1900, 1, 1))
                    value = new DateTime(1900, 1, 1);

                DropSetted = true;
                Text = value.ToString(dateFormat, CultureInfo.InvariantCulture);
                DropSetted = false;

                if (item.Date != value)
                {
                    item.Date = value;
                }

                ValueChanged?.Invoke(this, Value);
            }
        }

        [DefaultValue(1)]
        [Description("弹窗放大倍数，可以1或者2"), Category("SunnyUI")]
        public int SizeMultiple { get => item.SizeMultiple; set => item.SizeMultiple = value; }

        private void UIDatetimePicker_ButtonClick(object sender, EventArgs e)
        {
            item.Date = Value;
            item.ShowToday = ShowToday;
            item.PrimaryColor = RectColor;
            item.Translate();
            item.SetDPIScale();
            item.SetStyleColor(UIStyles.ActiveStyleColor);
            item.max = MaxDate;
            item.min = MinDate;
            if (StyleDropDown != UIStyle.Inherited) item.Style = StyleDropDown;
            Size size = SizeMultiple == 1 ? new Size(452, 200) : new Size(904, 400);
            ItemForm.Show(this, size);
        }

        private string dateFormat = "yyyy-MM-dd HH:mm:ss";

        [Description("日期格式化掩码"), Category("SunnyUI")]
        [DefaultValue("yyyy-MM-dd HH:mm:ss")]
        public string DateFormat
        {
            get => dateFormat;
            set
            {
                dateFormat = value;
                Text = Value.ToString(dateFormat);
                MaxLength = dateFormat.Length;
            }
        }

        private DateTime max = DateTime.MaxValue;
        private DateTime min = DateTime.MinValue;

        internal static DateTime EffectiveMaxDate(DateTime maxDate)
        {
            DateTime maxSupportedDate = DateTimePicker.MaximumDateTime;
            if (maxDate > maxSupportedDate)
            {
                return maxSupportedDate;
            }
            return maxDate;
        }

        internal static DateTime EffectiveMinDate(DateTime minDate)
        {
            DateTime minSupportedDate = DateTimePicker.MinimumDateTime;
            if (minDate < minSupportedDate)
            {
                return minSupportedDate;
            }
            return minDate;
        }

        [DefaultValue(typeof(DateTime), "9998/12/31")]
        [Description("最大日期"), Category("SunnyUI")]
        public DateTime MaxDate
        {
            get
            {
                return EffectiveMaxDate(max);
            }
            set
            {
                if (value != max)
                {
                    if (value < EffectiveMinDate(min))
                    {
                        value = EffectiveMinDate(min);
                    }

                    // If trying to set the maximum greater than MaxDateTime, throw.
                    if (value > MaximumDateTime)
                    {
                        value = MaximumDateTime;
                    }

                    max = value;

                    //If Value (which was once valid) is suddenly greater than the max (since we just set it)
                    //then adjust this...
                    if (Value > max)
                    {
                        Value = max;
                    }
                }
            }
        }

        [DefaultValue(typeof(DateTime), "1753/1/1")]
        [Description("最小日期"), Category("SunnyUI")]
        public DateTime MinDate
        {
            get
            {
                return EffectiveMinDate(min);
            }
            set
            {
                if (value != min)
                {
                    if (value > EffectiveMaxDate(max))
                    {
                        value = EffectiveMaxDate(max);
                    }

                    // If trying to set the minimum less than MinimumDateTime, throw.
                    if (value < MinimumDateTime)
                    {
                        value = MinimumDateTime;
                    }

                    min = value;

                    //If Value (which was once valid) is suddenly less than the min (since we just set it)
                    //then adjust this...
                    if (Value < min)
                    {
                        Value = min;
                    }
                }
            }
        }

        internal static DateTime MaximumDateTime
        {
            get
            {
                DateTime maxSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MaxSupportedDateTime;
                if (maxSupportedDateTime.Year > MaxDateTime.Year)
                {
                    return MaxDateTime;
                }
                return maxSupportedDateTime;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DateTime MinDateTime = new DateTime(1753, 1, 1);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DateTime MaxDateTime = new DateTime(9998, 12, 31);

        internal static DateTime MinimumDateTime
        {
            get
            {
                DateTime minSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime;
                if (minSupportedDateTime.Year < 1753)
                {
                    return new DateTime(1753, 1, 1);
                }
                return minSupportedDateTime;
            }
        }
    }
}
