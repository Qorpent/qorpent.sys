namespace Qorpent.Model {
    public interface IWithIntId
    {
        /// <summary>
        /// PK ID in database terms
        /// </summary>
        int Id { get; set; }
    }
}