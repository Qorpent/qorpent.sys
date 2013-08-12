using System.Collections.Generic;

namespace Qorpent.Security.Cryptography {
    /// <summary>
    ///     An entity
    /// </summary>
    public class CryptoProviderEntity {
        /// <summary>
        ///     File type of this action
        /// </summary>
        public CryptoProviderFileType FileType { get; private set; }

        /// <summary>
        ///     Privacy
        /// </summary>
        public CryptoProviderEntityPrivacy Privacy { get; private set; }

        /// <summary>
        ///     Entity body
        /// </summary>
        public string EntityBody { get; private set; }

        /// <summary>
        ///     Metadata
        /// </summary>
        public IDictionary<string, string> EntityMetadata { get; set; }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="entityBody">Entity body</param>
        public CryptoProviderEntity(string entityBody) : this (entityBody, CryptoProviderFileType.Pem) {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityBody">Entity body</param>
        /// <param name="fileType">type of file</param>
        public CryptoProviderEntity(string entityBody, CryptoProviderFileType fileType) : this(entityBody, fileType, CryptoProviderEntityPrivacy.Public) {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityBody">Entity body</param>
        /// <param name="fileType">type of file</param>
        /// <param name="privacy">file's privacy settings</param>
        public CryptoProviderEntity(string entityBody, CryptoProviderFileType fileType, CryptoProviderEntityPrivacy privacy) {
            Privacy = privacy;
            FileType = fileType;
            EntityBody = entityBody;
            EntityMetadata = new Dictionary<string, string>();
        }
    }
}