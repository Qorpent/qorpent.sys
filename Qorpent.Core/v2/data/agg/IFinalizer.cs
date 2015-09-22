namespace qorpent.v2.data.agg {
    public interface IFinalizer {
        void Execute(AggregateNode node);
    }
}