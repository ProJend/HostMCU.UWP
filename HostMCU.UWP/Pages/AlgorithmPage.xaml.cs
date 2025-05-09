using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace HostMCU.UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AlgorithmPage : Page
    {
        // 示例数据：参考曲线（匀速）和实际曲线（波动）
        private List<(float time, float speed)> referenceData = new List<(float, float)>();
        private List<(float time, float speed)> actualData = new List<(float, float)>();
        private readonly float maxSpeed = 100f; // 最大速度（km/s）
        private readonly float maxTime = 3f;    // 最大时间（s）

        public AlgorithmPage()
        {
            this.InitializeComponent();
            GenerateDemoData();
        }

        private void GenerateDemoData()
        {
            // 参考数据（匀速50%）
            for (float t = 0; t <= maxTime; t += 0.1f)
            {
                referenceData.Add((t, 50f));
            }

            // 实际数据（带波动）
            var rand = new Random();
            for (float t = 0; t <= maxTime; t += 0.1f)
            {
                float fluctuation = (float)(rand.NextDouble() * 20 - 10);
                actualData.Add((t, Math.Clamp(50 + fluctuation, 0, 100)));
            }
        }

        private void OnChartDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            // 1. 绘制白色背景
            //ds.Clear(Colors.White);

            // 2. 绘制坐标轴和刻度
            DrawAxes(ds);

            // 3. 绘制参考曲线（蓝色实线）
            DrawCurve(ds, referenceData, Colors.DarkRed, 2);

            // 4. 绘制实际曲线（红色虚线）
            DrawCurve(ds, actualData, Colors.Red, 2, new float[] { 5, 2 });

            // 5. 添加图例
            DrawLegend(ds);
        }

        private void DrawAxes(CanvasDrawingSession ds)
        {
            const float margin = 50;
            float width = (float)ChartCanvas.Width - 2 * margin;
            float height = (float)ChartCanvas.Height - 2 * margin;

            // 坐标轴
            ds.DrawLine(margin, margin + height, margin + width, margin + height, Colors.White, 2); // X轴
            ds.DrawLine(margin, margin, margin, margin + height, Colors.White, 2); // Y轴

            // Y轴刻度保持不变（0%, 50%, 100%）
            for (int percent = 0; percent <= 100; percent += 50)
            {
                float y = margin + height - (percent / 100f) * height;
                ds.DrawLine(margin - 5, y, margin, y, Colors.White, 1);
                ds.DrawText(
                    $"{percent}%",
                    new Vector2(margin - 10, y - 10),
                    Colors.White,
                    new CanvasTextFormat()
                    {
                        HorizontalAlignment = CanvasHorizontalAlignment.Right,
                        VerticalAlignment = CanvasVerticalAlignment.Center,
                        FontSize = 12
                    });
            }

            // 修改后的X轴刻度（每0.5秒一个刻度）
            for (float seconds = 0; seconds <= maxTime; seconds += 0.5f)
            {
                float x = margin + (seconds / maxTime) * width;

                // 主刻度（整数秒）
                if (seconds % 1 == 0)
                {
                    // 长刻度线
                    ds.DrawLine(x, margin + height, x, margin + height + 8, Colors.White, 1.5f);
                    // 完整数字标签
                    ds.DrawText(
                        $"{seconds}s",
                        new Vector2(x, margin + height + 12),
                        Colors.White,
                        new CanvasTextFormat()
                        {
                            HorizontalAlignment = CanvasHorizontalAlignment.Center,
                            FontSize = 12
                        });
                }
                // 次刻度（0.5秒）
                else
                {
                    // 短刻度线（与虚线样式同步）
                    ds.DrawLine(x, margin + height, x, margin + height + 5, Colors.White, 1);

                    // 可选：添加小圆点标记
                    ds.FillCircle(x, margin + height + 2.5f, 1.5f, Colors.White);
                }
            }

            // 轴标签位置微调
            ds.DrawText("速度 (km/s)", new Vector2(margin - 40, margin - 45), Colors.White);
            ds.DrawText("时间 (s)", new Vector2(margin + width - 30, margin + height - 40), Colors.White);
        }

        private void DrawCurve(CanvasDrawingSession ds,
                             List<(float time, float speed)> data,
                             Color color,
                             float strokeWidth,
                             float[] dashPattern = null)
        {
            if (data.Count < 2) return;

            const float margin = 50;
            float width = (float)ChartCanvas.Width - 2 * margin;
            float height = (float)ChartCanvas.Height - 2 * margin;

            // 坐标转换函数
            Vector2 ToCanvasPoint(float time, float speed)
            {
                float x = margin + (time / maxTime) * width;
                float y = margin + height - (speed / maxSpeed) * height;
                return new Vector2(x, y);
            }

            // 创建路径
            using (var pathBuilder = new CanvasPathBuilder(ds.Device))
            {
                pathBuilder.BeginFigure(ToCanvasPoint(data[0].time, data[0].speed));

                for (int i = 1; i < data.Count; i++)
                {
                    pathBuilder.AddLine(ToCanvasPoint(data[i].time, data[i].speed));
                }

                pathBuilder.EndFigure(CanvasFigureLoop.Open);

                var geometry = CanvasGeometry.CreatePath(pathBuilder);

                // 绘制样式
                var strokeStyle = dashPattern != null
                    ? new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash, CustomDashStyle = dashPattern }
                    : null;

                ds.DrawGeometry(geometry, color, strokeWidth, strokeStyle);
            }
        }

        private void DrawLegend(CanvasDrawingSession ds)
        {
            const float legendX = 650;
            const float legendY = 60;

            // 图例背景
            //ds.FillRectangle(legendX - 10, legendY - 20, 150, 60, Colors.WhiteSmoke);

            // 参考速度图例
            ds.DrawLine(legendX, legendY, legendX + 40, legendY, Colors.DarkRed, 2);
            ds.DrawText("参考速度", legendX + 50, legendY - 10, Colors.DarkRed);

            // 实际速度图例
            ds.DrawLine(legendX, legendY + 30, legendX + 40, legendY + 30, Colors.Red, 2,
                new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash, CustomDashStyle = new float[] { 5, 2 } });
            ds.DrawText("实际速度", legendX + 50, legendY + 20, Colors.Red);
        }
    }
}
