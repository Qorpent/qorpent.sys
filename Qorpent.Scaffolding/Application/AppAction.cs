using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application {
    /// <summary>
    /// 
    /// </summary>
    public class AppAction : AppObject<AppAction> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cls"></param>
        /// <returns></returns>
        public override AppAction Setup(ApplicationModel model, IBSharpClass cls) {
            base.Setup(model, cls);
            Url = Resolve(AppSyntax.ActionUrlAttribute);
            ResultIsArray = false;
            ArgumentsReference = Resolve(AppSyntax.ActionArgumentsAttribute);
            ResultReference = Resolve(AppSyntax.ActionResultAttribute);
            if (ResultReference.EndsWith("*")) {
                ResultIsArray = true;
                ResultReference = ResultReference.Replace("*", "");
            }
            SupportAsync = Resolve(AppSyntax.ActionSupportAsyncAttribute).ToBool();
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Bind() {
            if (!string.IsNullOrWhiteSpace(ArgumentsReference)) {
                Arguments= Model.Resolve<AppStruct>(ArgumentsReference);
                if (null == Arguments) {
                    Model.RegisterError(new BSharpError {
                        Level = ErrorLevel.Error,
                        Message = "Неразрешимая ссылка на аргументы в действии",
                        Class = Class
                    });
                }
            }
            if (!string.IsNullOrWhiteSpace(ResultReference)) {
                Result = Model.Resolve<AppStruct>(ResultReference);
                if (null == Result) {
                    Model.RegisterError(new BSharpError {
                        Level = ErrorLevel.Error,
                        Message = "Неразрешимая ссылка на результат в действии",
                        Class = Class
                    });
                }
            }
        }

        /// <summary>
        /// Адрес вызова
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AppStruct Arguments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ArgumentsReference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ResultIsArray { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AppStruct Result { get; set; }

        /// <summary>
        /// Структура результата вызова акшена 
        /// </summary>
        public string ResultReference { get; set; }

        /// <summary>
        /// Поддерживает ли ассинхронный вызов
        /// </summary>
        public bool SupportAsync { get; set; }
    }
}