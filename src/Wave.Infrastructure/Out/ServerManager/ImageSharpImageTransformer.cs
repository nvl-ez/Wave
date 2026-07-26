using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Wave.Application.Out.ServerManager;

namespace Wave.Infrastructure.Out.ServerManager;

public class ImageSharpImageTransformer : IImageTransformer
{
    public async Task TransformToPngAsync(Stream input, Stream output, int width, int height, CancellationToken ct = default)
    {
        using Image image = await Image.LoadAsync(input, ct);
        image.Mutate(context => context.Resize(width, height));
        await image.SaveAsPngAsync(output, ct);
    }
}
