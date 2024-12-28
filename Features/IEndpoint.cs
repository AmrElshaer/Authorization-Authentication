namespace Authorization.Features;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}