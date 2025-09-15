﻿using System.Threading;
using System.Windows.Forms;

namespace Sunny.UI
{
    public class UIFormService
    {
        protected Thread thread;
        public bool IsRun => thread != null && thread.ThreadState == ThreadState.Running;
    }

    public static class UIFormServiceHelper
    {
        private static UIWaitFormService WaitFormService;
        private static UIProcessIndicatorFormService ProcessFormService;
        private static UIStatusFormService StatusFormService;

        static UIFormServiceHelper()
        {
            WaitFormService = new UIWaitFormService();
            ProcessFormService = new UIProcessIndicatorFormService();
            StatusFormService = new UIStatusFormService();
        }

        /// <summary>
        /// 显示等待提示窗
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="size"></param>
        /// <param name="showRect"></param>
        /// <param name="centerScreen"></param>
        public static void ShowProcessForm(this Form owner, int size = 200, bool showRect = true, bool centerScreen = true)
        {
            if (ProcessFormService.IsRun) return;
            ProcessFormServiceClose = false;
            if (centerScreen)
                ProcessFormService.CreateForm(size, showRect);
            else
                ProcessFormService.CreateForm(owner, size, showRect);
        }

        internal static bool ProcessFormServiceClose;

        /// <summary>
        /// 隐藏等待提示窗
        /// </summary>
        public static void HideProcessForm(this Form owner)
        {
            ProcessFormServiceClose = true;
        }

        /// <summary>
        /// 显示等待提示窗
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="desc">描述文字</param>
        /// <param name="centerScreen">居中</param>
        /// <param name="closeable">可关闭的</param>
        public static void ShowWaitForm(this Form owner, string desc = "系统正在处理中，请稍候...", bool centerScreen = true, bool closeable = false)
        {
            if (WaitFormService.IsRun) return;
            WaitFormServiceShow = true;
            if (centerScreen)
                WaitFormService.CreateForm(desc, closeable);
            else
                WaitFormService.CreateForm(owner, desc, closeable);
        }

        internal static bool WaitFormServiceShow;

        /// <summary>
        /// 隐藏等待提示窗
        /// </summary>
        public static void HideWaitForm(this Form owner)
        {
            WaitFormServiceShow = false;
        }

        /// <summary>
        /// 等待提示窗是否显示
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static bool WaitFormIsShow(this Form owner) => WaitFormServiceShow;

        /// <summary>
        /// 设置等待提示窗描述文字
        /// </summary>
        /// <param name="desc">描述文字</param>
        public static void SetWaitFormDescription(this Form owner, string desc)
        {
            if (!WaitFormService.IsRun) return;
            WaitFormService.SetDescription(desc);
        }

        /// <summary>
        /// 显示进度提示窗
        /// </summary>
        /// <param name="desc">描述文字</param>
        /// <param name="maximum">最大进度值</param>
        /// <param name="decimalCount">显示进度条小数个数</param>
        /// <param name="centerScreen">居中</param>
        /// <param name="closeable">可关闭的</param>
        public static void ShowStatusForm(this Form owner, int maximum = 100, string desc = "系统正在处理中，请稍候...", int decimalCount = 1, bool centerScreen = true, bool closeable = false)
        {
            if (StatusFormService.IsRun) return;
            StatusFormServiceShow = true;
            if (centerScreen)
                StatusFormService.CreateForm(maximum, desc, decimalCount, closeable);
            else
                StatusFormService.CreateForm(owner, maximum, desc, decimalCount, closeable);
        }

        internal static bool StatusFormServiceShow;

        /// <summary>
        /// 隐藏进度提示窗
        /// </summary>
        public static void HideStatusForm(this Form owner)
        {
            StatusFormServiceShow = false;
        }

        /// <summary>
        /// 等待提示窗是否显示
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static bool StatusFormIsShow(this Form owner) => StatusFormServiceShow;

        /// <summary>
        /// 设置进度提示窗步进值加1
        /// </summary>
        public static void SetStatusFormStepIt(this Form owner, int step = 1)
        {
            if (!StatusFormService.IsRun) return;
            StatusFormService.SetFormStepIt(step);
        }

        /// <summary>
        /// 设置进度提示窗描述文字
        /// </summary>
        /// <param name="desc">描述文字</param>
        public static void SetStatusFormDescription(this Form owner, string desc)
        {
            if (!StatusFormService.IsRun) return;
            StatusFormService.SetFormDescription(desc);
        }
    }

    public class UIWaitFormService : UIFormService
    {
        private UIWaitForm form;

        public void CreateForm(string desc, bool closeable)
        {
            thread = new Thread(delegate ()
            {
                form = new UIWaitForm(desc);
                form.ShowInTaskbar = false;
                form.TopMost = true;
                form.ControlBox = closeable;
                form.Render();
                if (IsRun) Application.Run(form);
            });

            thread.Start();
        }

        public void CreateForm(Form owner, string desc, bool closeable)
        {
            thread = new Thread(delegate ()
            {
                form = new UIWaitForm(desc);
                form.ShowInTaskbar = false;
                form.TopMost = true;
                form.ControlBox = closeable;

                if (owner != null)
                {
                    form.StartPosition = FormStartPosition.Manual;
                    Form parent = owner.ParentForm ?? owner;
                    form.Left = parent.Left + (parent.Width - form.Width) / 2;
                    form.Top = parent.Top + (parent.Height - form.Height) / 2;
                }

                form.Render();
                if (IsRun) Application.Run(form);
            });

            thread.Start();
        }

        public void SetDescription(string desc)
        {
            try
            {
                form?.SetDescription(desc);
            }
            catch
            {
            }
        }
    }

    public class UIProcessIndicatorFormService : UIFormService
    {
        private UIProcessIndicatorForm form;

        public void CreateForm(int size = 200, bool showRect = true)
        {
            thread = new Thread(delegate ()
            {
                form = new UIProcessIndicatorForm();
                form.ShowRect = showRect;
                form.Size = new System.Drawing.Size(size, size);
                form.ShowInTaskbar = false;
                form.TopMost = true;
                form.Render();
                Application.Run(form);
            });

            thread.Start();
        }

        public void CreateForm(Form owner, int size = 200, bool showRect = true)
        {
            thread = new Thread(delegate ()
            {
                form = new UIProcessIndicatorForm();
                form.ShowRect = showRect;
                form.Size = new System.Drawing.Size(size, size);
                form.ShowInTaskbar = false;
                form.TopMost = true;

                if (owner != null)
                {
                    form.StartPosition = FormStartPosition.Manual;
                    Form parent = owner.ParentForm ?? owner;
                    form.Left = parent.Left + (parent.Width - form.Width) / 2;
                    form.Top = parent.Top + (parent.Height - form.Height) / 2;
                }

                form.Render();
                Application.Run(form);
            });

            thread.Start();
        }
    }

    public class UIStatusFormService : UIFormService
    {
        private UIStatusForm form;

        public void CreateForm(int max, string desc, int decimalCount = 1, bool closeable = false)
        {
            thread = new Thread(delegate ()
            {
                form = new UIStatusForm(max, desc, decimalCount);
                form.ShowInTaskbar = false;
                form.TopMost = true;
                form.ControlBox = closeable;
                form.Render();
                Application.Run(form);
            });

            thread.Start();
        }

        public void CreateForm(Form owner, int max, string desc, int decimalCount = 1, bool closeable = false)
        {
            thread = new Thread(delegate ()
            {
                form = new UIStatusForm(max, desc, decimalCount);
                form.ShowInTaskbar = false;
                form.TopMost = true;
                form.ControlBox = closeable;

                if (owner != null)
                {
                    form.StartPosition = FormStartPosition.Manual;
                    Form parent = owner.ParentForm ?? owner;
                    form.Left = parent.Left + (parent.Width - form.Width) / 2;
                    form.Top = parent.Top + (parent.Height - form.Height) / 2;
                }

                form.Render();
                Application.Run(form);
            });

            thread.Start();
        }

        public void SetFormDescription(string desc)
        {
            try
            {
                form?.SetDescription(desc);
            }
            catch
            {
            }
        }

        public void SetFormStepIt(int step = 1)
        {
            try
            {
                form?.StepIt(step);
            }
            catch
            {
            }
        }
    }
}