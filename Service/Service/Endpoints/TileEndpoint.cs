using Mapster.Common.MemoryMappedTypes;
using Mapster.Rendering;
using SixLabors.ImageSharp;

namespace Mapster.Service.Endpoints;

internal class TileEndpoint : IDisposable
{
    private readonly DataFile _mapData;
    private bool _disposedValue;

    public TileEndpoint(string mapDataFilePath)
    {
        _mapData = new DataFile(mapDataFilePath);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public static void LogPerformance(double time)
    {
        string path = "../../performance.txt";

        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(time);
            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(time);
            }
        }
    }

    public static void Register(WebApplication app)
    {
        // Map HTTP GET requests to this
        // Set up the request as parameterized on 'boundingBox', 'width' and 'height'
        app.MapGet("/render", HandleTileRequest);
        async Task HandleTileRequest(HttpContext context, double minLat, double minLon, double maxLat, double maxLon, int? size, TileEndpoint tileEndpoint)
        {
            if (size == null)
            {
                size = 800;
            }

            var pixelBb = new TileRenderer.BoundingBox
            {
                MinX = float.MaxValue,
                MinY = float.MaxValue,
                MaxX = float.MinValue,
                MaxY = float.MinValue
            };

            long lStart = DateTime.Now.Ticks;
            var shapes = new PriorityQueue<BaseShape, int>();
            tileEndpoint._mapData.ForeachFeature(
                new BoundingBox(
                    new Coordinate(minLat, minLon),
                    new Coordinate(maxLat, maxLon)
                ),
                featureData =>
                {
                    featureData.Tessellate(ref pixelBb, ref shapes);
                    return true;
                }
            );
            long lFinish = DateTime.Now.Ticks;
            double performance_ms = TimeSpan.FromTicks(lFinish - lStart).TotalMilliseconds;
            LogPerformance(performance_ms);
            Console.WriteLine("Tesselate running time: " + performance_ms + " ms");

            context.Response.ContentType = "image/png";
            await tileEndpoint.RenderPng(context.Response.BodyWriter.AsStream(), pixelBb, shapes, size.Value, size.Value);
        }
    }

    private async Task RenderPng(Stream outputStream, TileRenderer.BoundingBox boundingBox, PriorityQueue<BaseShape, int> shapes, int width, int height)
    {

        var canvas = await Task.Run(() =>
        {
            return shapes.Render(boundingBox, width, height);
        }).ConfigureAwait(continueOnCapturedContext: false);

        await canvas.SaveAsPngAsync(outputStream).ConfigureAwait(continueOnCapturedContext: false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _mapData.Dispose();
            }

            _disposedValue = true;
        }
    }
}
