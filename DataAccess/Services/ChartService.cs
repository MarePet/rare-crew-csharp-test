using Application.DTOs;
using Application.DTOs.Response;
using Application.Enums;
using Application.IServices;
using SkiaSharp;

namespace DataAccess.Services
{
    public class ChartService(ITimeEntryService timeEntryService) : IChartService
    {
        readonly ITimeEntryService timeEntryService = timeEntryService;
        public async Task<Result<string?>> GeneratePieChartAsync()
        {
            try
            {
                var timeEntiresResponse = await timeEntryService.GetTimeEntriesAsync();

                if(timeEntiresResponse == null || timeEntiresResponse.Data == null || !timeEntiresResponse.IsSuccess)
                {
                    return Result<string?>.Failure(errorCode: ErrorCodeEnum.InternalServerError,
                        errorMessage: "Failed to retrieve time entries.");
                }

                var validEntries = timeEntiresResponse.Data.Where(e => e.TotalHours.HasValue && e.TotalHours.Value > 0).ToList();
                if (!validEntries.Any())
                {
                    return Result<string?>.Failure(errorCode: ErrorCodeEnum.BadRequest,
                        errorMessage: "No valid time entries with hours found.");
                }

                var base64Image = GeneratePieChartImage(validEntries);
                return Result<string?>.Success(base64Image);
            }
            catch
            {
                return Result<string?>.Failure(errorCode: ErrorCodeEnum.InternalServerError,
                errorMessage: "Internal server error!");
            }
        }

        private string GeneratePieChartImage(IList<EmployeeTimeEntryResponseDTO> entries)
        {
            const int width = 600;
            const int height = 400;
            const int centerX = 200;
            const int centerY = height / 2;
            const int radius = 120;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            var totalHours = entries.Sum(e => e.TotalHours ?? 0);
            var colors = GenerateRandomColors(entries.Count);

            var rect = new SKRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius);

            float startAngle = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries.ElementAt(i);
                var sweepAngle = (float)((entry.TotalHours ?? 0) / totalHours * 360);
                var color = colors[i % colors.Length];

                using var paint = new SKPaint
                {
                    Color = color,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };

                using var path = new SKPath();
                path.MoveTo(centerX, centerY);
                path.ArcTo(rect, startAngle, sweepAngle, false);
                path.Close();

                canvas.DrawPath(path, paint);

                startAngle += sweepAngle;
            }

            DrawLegend(canvas, entries, colors, totalHours);

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return Convert.ToBase64String(data.ToArray());
        }

        private void DrawLegend(SKCanvas canvas, IList<EmployeeTimeEntryResponseDTO> entries, SKColor[] colors, double totalHours)
        {
            const int legendX = 400;
            const int legendY = 50;
            const int legendItemHeight = 25;
            const int colorBoxSize = 15;

            using var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true
            };

            using var font = new SKFont(SKTypeface.FromFamilyName("Arial"), 12); 

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries.ElementAt(i);
                var color = colors[i % colors.Length];
                var percentage = (entry.TotalHours ?? 0) / totalHours * 100;
                var yPos = legendY + i * legendItemHeight;

                // Draw color box
                using var colorPaint = new SKPaint
                {
                    Color = color,
                    Style = SKPaintStyle.Fill
                };

                canvas.DrawRect(legendX, yPos, colorBoxSize, colorBoxSize, colorPaint);

                // Draw legend text
                var legendText = $"{entry.Name ?? "Unknown"}: {entry.TotalHours:F1}h ({percentage:F1}%)";
                canvas.DrawText(legendText, legendX + colorBoxSize + 8, yPos + 10, SKTextAlign.Left, font, textPaint);
            }
        }

        private SKColor[] GenerateRandomColors(int count)
        {
            var colors = new SKColor[count];
            for (int i = 0; i < count; i++)
            {
                var random = new Random();
                byte r = (byte)random.Next(50, 256);
                byte g = (byte)random.Next(50, 256);
                byte b = (byte)random.Next(50, 256);
                colors[i] = new SKColor(r, g, b);
            }
            return colors;
        }
    }
}

