using System;
using System.Threading;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Serialization;

namespace pksp.kb.web
{
    public abstract class ActionHandler<TContext> : RequestHandlerBase where TContext: ActionContext,new()
    {
        [Inject] protected ISerializerFactory SerializerFactory { get; set; }

        public override void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel)
        {
            var ctx = new TContext
            {
                Server = server,
                WebContext = context,
                Cancel = cancel,
                Parameters = RequestParameters.Create(context)
            };
            ctx.Setup();
            try
            {
                var result = RunAction(ctx);
                if (result is TContext)
                {
                    ctx = (TContext) result;
                }
                else
                {
                    ctx.Result = ctx.Result ?? result;
                }
            }
            catch (Exception e)
            {
                ctx.Error = e;
            }
            WriteOut(ctx);
        }

        protected virtual void WriteOut(ActionContext ctx)
        {
            var obj = ctx.Error ?? ctx.Result;
            if (null != ctx.Error && ctx.State==200)
            {
                ctx.State = 500;
            }
            if (ctx.MimeType.Contains("json") )
            {
                var s = obj as string;
                if(null==s || !(s.StartsWith("{") || s.StartsWith("[")))
                {
                    obj = obj.stringify(SerializeMode.OnlyNotNull, pretty: true, jsonmode: ctx.RenderMode);
                }
                
            }else if (ctx.MimeType.Contains("xml"))
            {
                obj = SerializerFactory.GetSerializer(SerializationFormat.Xml).Serialize(obj, usermode: ctx.RenderMode);
            }
            ctx.WebContext.Finish(obj,ctx.MimeType,ctx.State);
        }

        protected abstract object RunAction(TContext ctx);
    }

    public abstract class ActionHandler : ActionHandler<ActionContext> { }
}