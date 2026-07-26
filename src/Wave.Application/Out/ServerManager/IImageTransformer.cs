namespace Wave.Application.Out.ServerManager;

public interface IImageTransformer
{
    public Task TransformToPngAsync(Stream input, Stream output, int width, int height, CancellationToken ct = default);
}
