using Qorpent.IO.Http;

namespace qorpent.v2.security.management {
    public static class UpdateUserProcessorExtensions {
        public static UpdateResult DefineUser(this IUpdateUserProcessor processor, WebContext request) {
            var updateinfo = UserUpdateInfoSerializer.ExtractFromParameters(RequestParameters.Create(request));
            return processor.DefineUser(request.User.Identity, updateinfo, null, true);
        }
    }
}