using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using Qorpent.Applications;
using Qorpent.Wiki;

namespace Zeta.Extreme.MongoDB.Integration.WikiStorage {
	/// <summary>
	///     Класс сериализации Wiki страниц в MongoDB
	/// </summary>
	public class MongoWikiSerializer {
	    private ToWikiPageSerializer _toWikiPageSerializer;

        /// <summary>
        /// 
        /// </summary>
	    public ToWikiPageSerializer ToWikiPage {
	        get { return _toWikiPageSerializer ?? (_toWikiPageSerializer = new ToWikiPageSerializer()); }
            set { _toWikiPageSerializer = value; }
	    } 

        /// <summary>
        /// 
        /// </summary>
        public class ToWikiPageSerializer {

            /// <summary>
            /// 
            /// </summary>
            /// <param name="document"></param>
            /// <returns></returns>
            public WikiPage FromMain(BsonDocument document) {
                BsonValue locker;
                document.TryGetValue("locker", out locker);

                var result = new WikiPage {
                    Code = document["_id"].AsString,
                    Text = document["text"].AsString,
                    Owner = document["owner"].AsString,
                    Editor = document["editor"].AsString,
                    Title = document["title"].AsString,
                    Published = document["ver"].ToUniversalTime(),
                    Locker = (locker != null) ? (locker.AsString) : (""),
                    Version = "Current",
                    Existed = true
                };

                foreach (var e in document) {
                    if (IsProperty(e.Name)) {
                        result.Propeties[e.Name] = e.Value.AsString;
                    }
                }

                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="document"></param>
            /// <returns></returns>
            public WikiPage FromHistory(BsonDocument document) {
                var result = new WikiPage {
                    Code = document["code"].AsString,
                    Text = document["text"].AsString,
                    Owner = document["owner"].AsString,
                    Editor = document["editor"].AsString,
                    Title = document["title"].AsString,
                    Published = document["published"].ToUniversalTime(),
                    Version = document["version"].AsString,
                    Existed = true
                };

                foreach (var e in document) {
                    if (IsProperty(e.Name)) {
                        result.Propeties[e.Name] = e.Value.AsString;
                    }
                }
                
                return result;
            }

            /// <summary>
            ///     Проверяет, не является ли поле в документе мета-свойством
            /// </summary>
            /// <param name="fieldName">Имя поля</param>
            /// <returns>True — является, false — не явялется</returns>
            private static bool IsProperty(string fieldName) {
                if (fieldName == "_id") {
                    return false;
                }

                if (fieldName == "code") {
                    return false;
                }

                if (fieldName == "text") {
                    return false;
                }

                if (fieldName == "owner") {
                    return false;
                }

                if (fieldName == "editor") {
                    return false;
                }

                if (fieldName == "title") {
                    return false;
                }

                if (fieldName == "published") {
                    return false;
                }

                if (fieldName == "version") {
                    return false;
                }

                if (fieldName == "ver") {
                    return false;
                }

                if (fieldName == "locker") {
                    return false;
                }

                return true;
            }
        }

		/// <summary>
		///     Конвертирует описатель MongoFile в WikiBinary
		/// </summary>
		/// <param name="info"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public  WikiBinary ConvertToWikiBinary(MongoGridFSFileInfo info, byte[] data) {
			return new WikiBinary {
				Code = info.Id.AsString,
				Data = data,
				MimeType = info.ContentType,
				Size = info.Length,
				Title = info.Metadata["title"].AsString,
				Owner = info.Metadata["owner"].AsString,
				Editor = info.Metadata["editor"].AsString,
				LastWriteTime = info.Metadata["lastwrite"].ToLocalTime()
			};
		}

		/// <summary>
		/// Формирует запрос по кодам и версиям
		/// </summary>
		/// <param name="codes"></param>
        /// <param name="versions"></param>
		/// <returns></returns>
		public  IMongoQuery GetQueryFromCodes(string[] codes, string[] versions) {
			var query = new BsonDocument();
			var idcond = query["code"] = new BsonDocument();
		    var versionCond = query["version"] = new BsonDocument();
			idcond["$in"] = new BsonArray(codes);
            versionCond["$in"] = new BsonArray(versions);
			var qdoc = new QueryDocument(query);
			return qdoc;
		}

		/// <summary>
		///     Конвертирует BSON в страницу
		/// </summary>
		/// <param name="page">Страница Wiki</param>
		/// <returns>Конвертированный BSON документ</returns>
		public BsonDocument NewFormPage(WikiPage page) {
		    var document = new BsonDocument {
                {"_id", page.Code},
                {"text", page.Text},
                {"owner", Application.Current.Principal.CurrentUser.Identity.Name},
                {"editor", Application.Current.Principal.CurrentUser.Identity.Name},
                {"title", page.Title ?? ""},
                {"ver", DateTime.Now}
		    };

			foreach (var propety in page.Propeties) {
                document[propety.Key] = propety.Value ?? "";
			}

            return document;
		}

		/// <summary>
		///     Конвертирует страницу в команду обновления
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		public IMongoUpdate UpdateFromPage(WikiPage page) {
			var updateBuilder = new UpdateBuilder();

			if (!string.IsNullOrEmpty(page.Text)) {
				updateBuilder.Set("text", page.Text);
			}

			if (!string.IsNullOrEmpty(page.Title)) {
				updateBuilder.Set("title", page.Title);
			}

			updateBuilder.Set("editor", Application.Current.Principal.CurrentUser.Identity.Name);

			foreach (var propety in page.Propeties) {
				updateBuilder.Set(propety.Key, propety.Value ?? "");
			}

			return updateBuilder;
		}

        /// <summary>
        ///     Обновление версии и даты отправки страницы в БД
        /// </summary>
        /// <param name="page">Страница</param>
        public void UpdateWikiPageVersion(WikiPage page) {
            page.Version = Guid.NewGuid().ToString();
            page.Published = DateTime.Now;
        }

        /// <summary>
        ///     Копирует данные о версии и дате занесения в базу страницы Wiki
        /// </summary>
        /// <param name="page">Страница Wiki</param>
        /// <param name="document">Целевой документ</param>
        public void CopyWikiPageVersionToBson(WikiPage page, BsonDocument document) {
            document["version"] = page.Version;
            document["published"] = page.Published;
        }
	}
}