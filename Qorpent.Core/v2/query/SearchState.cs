namespace qorpent.v2.query {
    public enum SearchState {
        None = 0,
        Wait =1,
        Finish =2,
        Frozen = 4,
        Data = 8,
        Error =16,
        
    }
}