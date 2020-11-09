using HotChocolate.Execution.Configuration;
using MagicMedia.GraphQL;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.GraphQL.Face;
using MagicMedia.GraphQL.SearchFacets;
using Microsoft.Extensions.DependencyInjection;

namespace MagicMedia
{
    public static class GrapQLServiceCollectionExtensions
    {
        public static IRequestExecutorBuilder AddMagicMediaGrapQL(
            this IRequestExecutorBuilder builder)
        {
            builder
                .AddQueryType(d => d.Name("Query"))
                .AddType<MediaQueries>()
                .AddType<FaceQueries>()
                .AddType<PersonQueries>()
                .AddType<SearchFacetQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                .AddType<FaceMutations>()
                .AddType<MediaMutations>()
                .AddType<MediaType>()
                .AddType<FaceType>()
                .AddType<ThumbnailType>()
                .AddType<SearchFacetType>()
                .AddDataLoader<CameraByIdDataLoader>()
                .AddDataLoader<ThumbnailByMediaIdDataLoader>()
                .AddDataLoader<MediaByIdDataLoader>()
                .AddSubscriptionType(d => d.Name("Subscription"))
                .AddType<MediaSubscriptions>()
                .AddInMemorySubscriptions()
                .AddAuthorization();

            return builder;
        }
    }
}
