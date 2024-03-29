﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class GrpcFileStorageExtensions
    {
        public static GrpcServiceEndpointConventionBuilder MapGrpcFileStorage(this IEndpointRouteBuilder builder)
        {
            return builder.MapGrpcService<GrpcFileStorage.FileStorageService>();
        }
    }
}
